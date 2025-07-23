using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using F;  // for CursorManager

namespace F.Cards
{
    [RequireComponent(typeof(Selectable))]
    public class DeckNavigationHandler : MonoBehaviour, ISelectHandler, IDeselectHandler, IMoveHandler
    {
        private DeckVisual deckVisual;
        private Selectable selfSelectable;
        private PlayerInput controls;
        private EventSystem eventSystem;

        private Card draggedCard;
        private bool uiClickHeld;
        private bool draggingStarted;
        private int currentIndex = -1;
        private GameObject prevSelectedCardGO;
        private GameObject lastSelectedGO;

        private const float flashDuration = 0.3f;

        [HideInInspector] public bool IsUsingStickDrag => draggingStarted;

        private void Awake()
        {
            selfSelectable  = GetComponent<Selectable>();
            deckVisual      = GetComponent<DeckVisual>();
            controls        = new PlayerInput();
            eventSystem     = EventSystem.current;
        }

        private void OnEnable()
        {
            controls.Enable();
            controls.UI.Click.started   += OnUIClickStarted;
            controls.UI.Click.canceled  += OnUIClickCanceled;
            controls.UI.Submit.started  += OnUIClickStarted;
            controls.UI.Submit.canceled += OnUIClickCanceled;

            uiClickHeld     = false;
            draggedCard     = null;
            draggingStarted = false;
            lastSelectedGO  = eventSystem.currentSelectedGameObject;
        }

        private void OnDisable()
        {
            controls.UI.Click.started   -= OnUIClickStarted;
            controls.UI.Click.canceled  -= OnUIClickCanceled;
            controls.UI.Submit.started  -= OnUIClickStarted;
            controls.UI.Submit.canceled -= OnUIClickCanceled;
            controls.Disable();
        }

        private void Update()
        {
            //var selectedGO = eventSystem.currentSelectedGameObject;
            //Debug.Log($"[DeckNav] Update → currentSelected = {(selectedGO != null ? selectedGO.name : "null")}");


            // detect native selection change to send pointer-exit
            var now = eventSystem.currentSelectedGameObject;
            if (now != lastSelectedGO)
            {
                if (lastSelectedGO != null)
                {
                    var exitEvent = new PointerEventData(eventSystem);
                    ExecuteEvents.Execute<IPointerExitHandler>(lastSelectedGO, exitEvent,
                        (h, d) => h.OnPointerExit(exitEvent));
                }
                lastSelectedGO = now;
            }

            // continuous stick-based drag
            if (uiClickHeld && draggedCard != null)
            {
                Vector2 moveDelta = controls.Player.Move.ReadValue<Vector2>();
                Vector2 lookDelta = Gamepad.current?.rightStick.ReadValue() ?? Vector2.zero;
                Vector2 delta     = moveDelta + lookDelta;

                if (delta.sqrMagnitude > 0f)
                {
                    if (!draggingStarted)
                    {
                        // send BeginDrag once
                        var beginEvent = new PointerEventData(eventSystem);
                        ExecuteEvents.Execute<IBeginDragHandler>(
                            draggedCard.gameObject, 
                            beginEvent,
                            (handler, data) => handler.OnBeginDrag(beginEvent)
                        );
                        draggingStarted = true;

                        // send PointerExit to the previously hovered object
                        var exitEvent = new PointerEventData(eventSystem);
                        ExecuteEvents.Execute<IPointerExitHandler>(
                            lastSelectedGO, 
                            exitEvent,
                            (handler, data) => handler.OnPointerExit(exitEvent)
                        );

                    }

                    // move the card by converting Vector2 delta to Vector3
                    draggedCard.transform.position += (Vector3)delta
                        * CursorManager.Instance.cardCursorSpeed 
                        * Time.unscaledDeltaTime;

                    ClampCardToScreen(draggedCard);

                    // get the screen position of the card
                    Vector2 screenPos = Camera.main.WorldToScreenPoint(
                        draggedCard.transform.position
                    );

                    // update the UI cursor position
                    CursorManager.Instance.UpdateCursorPosition(screenPos);
                    // show the gamepad cursor graphic
                    CursorManager.Instance.ShowGamepadCursor();
                    // temporarily disable default navigation
                    eventSystem.sendNavigationEvents = false;

                    // perform a UI raycast at the cursor position
                    //var pointerData = new PointerEventData(eventSystem) { position = screenPos };
                    //var results = new List<RaycastResult>();
                    //eventSystem.RaycastAll(pointerData, results);

                    //if (results.Count > 0)
                    //{
                        // set the UI element under the cursor as the selected object
                    //    eventSystem.SetSelectedGameObject(results[0].gameObject);
                    //}
                }
            }
        }

        private void ClampCardToScreen(Card card)
        {
            var cam = Camera.main;
            Vector3 worldPos = card.transform.position;
            Vector3 vp = cam.WorldToViewportPoint(worldPos);
            vp.x = Mathf.Clamp01(vp.x);
            vp.y = Mathf.Clamp01(vp.y);
            Vector3 clamped = cam.ViewportToWorldPoint(vp);
            card.transform.position = new Vector3(clamped.x, clamped.y, worldPos.z);
        }

        private void OnUIClickStarted(InputAction.CallbackContext ctx)
        {
            if (ctx.control.device is Pointer || uiClickHeld || draggedCard != null) return;

            var go = eventSystem.currentSelectedGameObject;
            if (go == null) return;
            var card = go.GetComponent<Card>();
            if (card == null) return;

            uiClickHeld = true;
            draggedCard = card;
            draggingStarted = false;
            eventSystem.sendNavigationEvents = false;

            // send pointer-down to card
            var downEvent = new PointerEventData(eventSystem) { button = PointerEventData.InputButton.Left };
            ExecuteEvents.Execute<IPointerDownHandler>(card.gameObject, downEvent,
                (h, d) => h.OnPointerDown(downEvent));
            
            // send BeginDrag once
            var beginEvent = new PointerEventData(eventSystem);
            ExecuteEvents.Execute<IBeginDragHandler>(
                draggedCard.gameObject, 
                beginEvent,
                (h, d) => h.OnBeginDrag(beginEvent)
            );

            eventSystem.SetSelectedGameObject(draggedCard.gameObject);
            deckVisual.hoveredCard = draggedCard;
            prevSelectedCardGO = draggedCard.gameObject;

        }

        private void OnUIClickCanceled(InputAction.CallbackContext ctx)
        {


            if (ctx.control.device is Pointer || !uiClickHeld || draggedCard == null) return;

            uiClickHeld = false;
            
            var endEvent = new PointerEventData(eventSystem);
            ExecuteEvents.Execute<IEndDragHandler>(
                draggedCard.gameObject,
                endEvent,
                (h, d) => h.OnEndDrag(endEvent)
            );
            // reativa o raycaster interno do Card
            draggingStarted = false;
                
            if (deckVisual.cards.Contains(draggedCard))
            {
                eventSystem.SetSelectedGameObject(draggedCard.gameObject);
                deckVisual.hoveredCard = draggedCard;
                prevSelectedCardGO = draggedCard.gameObject;
            }
            else
            {
                draggedCard.cardVisual.SetOutline(false);
                draggedCard.selected = false;
                draggedCard.SelectEvent.Invoke(draggedCard, false);
                prevSelectedCardGO = null;
            }

            // re-enter previous hovered
            var enterEvent = new PointerEventData(eventSystem);
            ExecuteEvents.Execute<IPointerEnterHandler>(prevSelectedCardGO, enterEvent,
            (h, d) => h.OnPointerEnter(enterEvent));

            // send pointer-up to card
            var upEvent = new PointerEventData(eventSystem) { button = PointerEventData.InputButton.Left };
            ExecuteEvents.Execute<IPointerUpHandler>(draggedCard.gameObject, upEvent,
                (h, d) => h.OnPointerUp(upEvent));

            eventSystem.sendNavigationEvents = true;
            draggedCard = null;

            // hide UI/gamepad cursor on release
            CursorManager.Instance.HideCursors();
        }

        public void OnSelect(BaseEventData eventData)
        {
            // hide any cursor when using explicit navigation
            if (uiClickHeld || CursorManager.Instance.IsUICursorActive) 
                return;

            var sorted = GetSortedCards();
            var current = EventSystem.current.currentSelectedGameObject?.name ?? "null";
            if (sorted.Count == 0) 
            {
                if (eventData is AxisEventData axis){
                    NavigationMode navMode = axis.moveDir switch
                    {
                        MoveDirection.Left  => NavigationMode.Left,
                        MoveDirection.Right => NavigationMode.Right,
                        MoveDirection.Up    => NavigationMode.Up,
                        MoveDirection.Down  => NavigationMode.Down,
                        _                   => NavigationMode.Left
                    };
                    StartCoroutine(FallbackToExplicit(navMode));
                }
                else
                {
                    // only set it if it isn’t already selected
                    var deckGO = selfSelectable.gameObject;
                    if (EventSystem.current.currentSelectedGameObject != deckGO)
                        eventSystem.SetSelectedGameObject(deckGO); 
                }
                return;
            }

            currentIndex = 0;
            StartCoroutine(SelectFirstCardNextFrame(eventData));
        }

        private IEnumerator SelectFirstCardNextFrame(BaseEventData eventData)
        {
            // wait one frame so cards list is populated
            yield return null;

            var cards = GetSortedCards();
            int count = cards.Count;
            if (count == 0)
            {
                yield break;
            }

            int indexToSelect = 0;
            if (eventData is AxisEventData axis)
            {
                switch (axis.moveDir)
                {
                    case MoveDirection.Left:
                        // select first
                        indexToSelect = count - 1;
                        break;
                    case MoveDirection.Right:
                        // select last
                        indexToSelect = 0;
                        break;
                    case MoveDirection.Up:
                    case MoveDirection.Down:
                        // select middle
                        indexToSelect = count / 2;
                        break;
                    default:
                        indexToSelect = 0;
                        break;
                }
            }

            ChangeSelection(indexToSelect);
            eventData.Use();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            if (uiClickHeld) return;

            if (prevSelectedCardGO != null)
            {
                var exitEvent = new PointerEventData(eventSystem);
                ExecuteEvents.Execute<IPointerExitHandler>(
                    prevSelectedCardGO, exitEvent,
                    (h, d) => h.OnPointerExit(exitEvent)
                );

                var prevCard = prevSelectedCardGO.GetComponent<Card>();
                if (prevCard != null)
                    prevCard.cardVisual.SetOutline(false);

                prevSelectedCardGO = null;
                deckVisual.hoveredCard = null;
            }
        }
        
        public void AddedCard()
        {
            // if this deck now contains exactly one card
            // and there's no current selection in the EventSystem
            var remaining = deckVisual.cards;
            if (remaining.Count == 1 && EventSystem.current.currentSelectedGameObject == this.gameObject)
            {
                ChangeSelection(0); 
            }
        }

                    
        public void RemoveCard(Card card){
            card.cardVisual.SetOutline(false);
            var remaining = deckVisual.cards;
            if (remaining.Count > 0)
            {
                ChangeSelection(Mathf.Clamp(currentIndex, 0, remaining.Count - 1)); 
            }
            else
            {
                eventSystem.SetSelectedGameObject(selfSelectable.gameObject);
            }
            
        }

        public void OnMove(AxisEventData eventData)
        {
            if (uiClickHeld || CursorManager.Instance.IsUICursorActive) 
                return;
            
            if (eventSystem.currentSelectedGameObject == null)
            {
                eventSystem.SetSelectedGameObject(selfSelectable.gameObject);
                return;
            }

            var dir = eventData.moveDir; 
            
            NavigationMode navMode = dir switch
            {
                MoveDirection.Left  => NavigationMode.Left,
                MoveDirection.Right => NavigationMode.Right,
                MoveDirection.Up    => NavigationMode.Up,
                MoveDirection.Down  => NavigationMode.Down,
                _                   => NavigationMode.Left
            };
    
            HandleNav(navMode);
            eventData.Use();
        }    

        private void HandleNav(NavigationMode dir)
        {
            var cards = GetSortedCards();
            int count = cards.Count;
            if (count == 0)
            {
                StartCoroutine(FallbackToExplicit(dir));
                return;
            }

            if (prevSelectedCardGO != null)
            {
                var sel = prevSelectedCardGO.GetComponent<Card>();
                currentIndex = sel != null ? cards.IndexOf(sel) : -1;
            }

            if (currentIndex < 0 || currentIndex >= count)
            {
                ChangeSelection(0);
                return;
            }

            if (dir == NavigationMode.Left && currentIndex > 0)
            {
                ChangeSelection(currentIndex - 1);
                return;
            }
            if (dir == NavigationMode.Right && currentIndex < count - 1)
            {
                ChangeSelection(currentIndex + 1);
                return;
            }
            StartCoroutine(FallbackToExplicit(dir));
        }

        private void ChangeSelection(int newIndex)
        {
            

            var cards = GetSortedCards();
            if (cards.Count == 0) return;
            newIndex = Mathf.Clamp(newIndex, 0, cards.Count - 1);

            var card = cards[newIndex];
            var go   = card.gameObject;

            if (prevSelectedCardGO != null && prevSelectedCardGO != go)
            {
                var exitEvent = new PointerEventData(eventSystem);
                ExecuteEvents.Execute<IPointerExitHandler>(prevSelectedCardGO, exitEvent,
                    (h, d) => h.OnPointerExit(exitEvent));

                var prevCard = prevSelectedCardGO.GetComponent<Card>();
                if (prevCard != null)
                    prevCard.cardVisual.SetOutline(false);

            }

            currentIndex = newIndex;
            eventSystem.SetSelectedGameObject(go);
            deckVisual.hoveredCard = card;

            card.cardVisual.SetOutline(true);

            

            var enterEvent = new PointerEventData(eventSystem);
            ExecuteEvents.Execute<IPointerEnterHandler>(go, enterEvent,
                (h, d) => h.OnPointerEnter(enterEvent));

            prevSelectedCardGO = go;

            if (card.cardVisual is TarotCardVisual tarotVis)
            { 
                // start the flash coroutine on the visual
                tarotVis.StartCoroutine(tarotVis.DamageFlash(flashDuration));
            }
            else if(card.cardVisual is CoinCardVisual coinVis){
                // start the flash coroutine on the visual
                coinVis.StartCoroutine(coinVis.DamageFlash(flashDuration));
            }
        }

        private IEnumerator FallbackToExplicit(NavigationMode mode)
        {
            // Wait one frame for the EventSystem to settle
            yield return null;

            // Only proceed if using Explicit navigation mode
            var nav = selfSelectable.navigation;
            if (nav.mode != Navigation.Mode.Explicit)
                yield break;

            // Determine the next selectable based on the given direction
            Selectable next = mode switch
            {
                NavigationMode.Left  => nav.selectOnLeft,
                NavigationMode.Right => nav.selectOnRight,
                NavigationMode.Up    => nav.selectOnUp,
                NavigationMode.Down  => nav.selectOnDown,
                _                    => null
            };

            // Prepare an AxisEventData so SetSelectedGameObject can carry the move direction
            var axisEvent = new AxisEventData(eventSystem)
            {
                moveDir = mode switch
                {
                    NavigationMode.Left  => MoveDirection.Left,
                    NavigationMode.Right => MoveDirection.Right,
                    NavigationMode.Up    => MoveDirection.Up,
                    NavigationMode.Down  => MoveDirection.Down,
                    _                    => MoveDirection.Left
                }
            };

            if (next != null)
            {
                // If the next selectable is another DeckNavigationHandler 
                // and that deck is empty, clear selection to break out of infinite looping
                var nextDeckNav = next.GetComponent<DeckNavigationHandler>();
                if (nextDeckNav != null && nextDeckNav.deckVisual.cards.Count == 0)
                {
                    eventSystem.SetSelectedGameObject(null);
                    yield break;
                }

                // Otherwise, select the next object as usual
                eventSystem.SetSelectedGameObject(next.gameObject, axisEvent);
            }

            // Finally, fire a pointer-exit on the previous selection to remove its outline
            var nextGO = next?.gameObject;
            if (prevSelectedCardGO != null && prevSelectedCardGO != nextGO)
            {
                var exitEvent = new PointerEventData(eventSystem);
                ExecuteEvents.Execute<IPointerExitHandler>(prevSelectedCardGO, exitEvent,
                    (handler, data) => handler.OnPointerExit(exitEvent));

                var prevCard = prevSelectedCardGO.GetComponent<Card>();
                if (prevCard != null)
                    prevCard.cardVisual.SetOutline(false);
            }
        }


        // sort cards left-to-right by sibling index
        private List<Card> GetSortedCards() =>
            deckVisual.cards.OrderBy(c => c.transform.parent.GetSiblingIndex()).ToList();

        private enum NavigationMode { Left, Right, Up, Down }
    }
}
