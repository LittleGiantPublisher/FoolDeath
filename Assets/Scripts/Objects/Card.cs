
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;
using F.UI;

namespace F.Cards{
    public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler, IPointerDownHandler, IMoveHandler
    {
        
        [Header ("Specs")]
        [SerializeField] public CardScriptable spec;
        [SerializeField] public DeckVisual deck; 
                
        private Canvas canvas;
        private Image imageComponent;
        private CardsHolder visualHandler;
        private Vector3 offset;
        
        [Header("Movement")]
        [SerializeField] private float moveSpeedLimit = 50;

        [Header("Selection")]
        public bool selected;
        public float selectionOffset = 50;
        private float pointerDownTime;
        private float pointerUpTime;
        private bool ignorePointerPlatform = false;
        public bool ignorePointer = false;
        
        [Header("Visual")]
        [SerializeField] private GameObject cardVisualPrefab;
        [HideInInspector] public CardVisual cardVisual;


        [Header("States")]
        public bool isHovering;
        public bool isDragging;
        [HideInInspector] public bool wasDragged;

        [Header("Events")]
        [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
        [HideInInspector] public UnityEvent<Card> PointerExitEvent;
        [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
        [HideInInspector] public UnityEvent<Card> PointerDownEvent;
        [HideInInspector] public UnityEvent<Card> BeginDragEvent;
        [HideInInspector] public UnityEvent<Card> EndDragEvent;
        [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

        void Start()
        {
            canvas = GetComponentInParent<Canvas>();
            imageComponent = GetComponent<Image>();

            visualHandler = FindObjectOfType<CardsHolder>();
            cardVisual = Instantiate(cardVisualPrefab, visualHandler ? visualHandler.transform : canvas.transform).GetComponent<CardVisual>();
            cardVisual.Initialize(this);

            canvas.GetComponent<GraphicRaycaster>().enabled = false;
            imageComponent.raycastTarget = false;
            canvas.GetComponent<GraphicRaycaster>().enabled = true;
            imageComponent.raycastTarget = true;

            AttachVisuals(); //initializes

#if !UNITY_STANDALONE && !UNITY_EDITOR && !MICROSOFT_GAME_CORE
            ignorePointerPlatform = true;
            ignorePointer = true;
#endif
        }

        private void AttachVisuals(){
            if(cardVisual != null && spec != null){
                if(spec.cardArt != null){
                    cardVisual.AttachSpecs(spec.cardArt);
                }
            }
        }

        public void ReInitialize(){
            AttachVisuals();
            selected = false;
            canvas.overrideSorting = false;
            cardVisual.ReInitialize();
        }

        public void OnMove(AxisEventData eventData)
        {
            if (deck.deckNav != null)
            {
                deck.deckNav.OnMove(eventData);
                eventData.Use();
            }
        }

        void Update()
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);
            if (ControllerManager.current.currentPCInput != ControllerManager.INPUT_TYPE.KEYBOARD)
            {
                return;
            }

            if (isDragging)
            {
                if (!(deck != null && deck.deckNav != null && deck.deckNav.IsUsingStickDrag))
                {
                    Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
                    Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
                    Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, Vector2.Distance(transform.position, targetPosition) / Time.unscaledDeltaTime);
                    transform.Translate(velocity * Time.unscaledDeltaTime);
                }
            }
        }

        public virtual void OnBeginDrag(PointerEventData eventData)
        {
            if (ignorePointer) return;
            ignorePointer = ignorePointerPlatform;
            
            if (cardVisual.isStatic) return;

            BeginDragEvent.Invoke(this);
            if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.KEYBOARD)
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                offset = (Vector2)transform.position - mousePosition;
            }
            
            isDragging = true;
            canvas.GetComponent<GraphicRaycaster>().enabled = false;
            imageComponent.raycastTarget = false;

            wasDragged = true;
        }

        public virtual void OnDrag(PointerEventData eventData)
        {
        }

        public virtual void OnEndDrag(PointerEventData eventData)
        {
            if (ignorePointer) return;
            ignorePointer = ignorePointerPlatform;
            
            if (cardVisual.isStatic) return;

            EndDragEvent.Invoke(this);
            isDragging = false;
            canvas.GetComponent<GraphicRaycaster>().enabled = true;
            imageComponent.raycastTarget = true;

            StartCoroutine(FrameWait());

            IEnumerator FrameWait()
            {
                yield return new WaitForEndOfFrame();
                wasDragged = false;
            }
        }

        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (ignorePointer) return;
            ignorePointer = ignorePointerPlatform;

            PointerEnterEvent.Invoke(this);
            isHovering = true;
        }

        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (ignorePointer) return;
            ignorePointer = ignorePointerPlatform;

            PointerExitEvent.Invoke(this);
            isHovering = false;
        }


        public virtual void OnPointerDown(PointerEventData eventData)
        {
            
        }

        public virtual void OnPointerUp(PointerEventData eventData)
        {
            
        }

        public virtual void Deselect()
        {
            if (selected)
            {
                ReInitialize();
                SelectEvent.Invoke(this, false);
                if (selected)
                    transform.localPosition += (cardVisual.transform.up * 50);
                else
                    transform.localPosition = Vector3.zero;
            }
        }


        public virtual int SiblingAmount()
        {
            return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
        }

        public virtual int ParentIndex()
        {
            return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
        }

        public virtual float NormalizedPosition()
        {
            return transform.parent.CompareTag("Slot") ? ExtensionMethods.Remap((float)ParentIndex(), 0, (float)(transform.parent.parent.childCount - 1), 0, 1) : 0;
        }

        public virtual void OnDestroy()
        {
            if(cardVisual != null)
            Destroy(cardVisual.gameObject);
        }
    }
}