using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using F.UI;

namespace F.Cards{
    public class DeckVisual : MonoBehaviour
    {

        [SerializeField] private Card selectedCard;
        [SerializeReference] public Card hoveredCard;

        [SerializeField] private GameObject slotPrefab;
        private RectTransform rect;
        [SerializeField] public bool isGameplay = true; //represents the used hand in game, where stores the powerups
        [SerializeField] public bool isCurrency = true; //represents if it shows the possibility to puy or sell
        [SerializeField] public DeckNavigationHandler deckNav;

        [Header("Spawn Settings")]
        public List<Card> cards;

        [HideInInspector]public bool isSelected = false;
        bool isCrossing = false;
        [SerializeField] private bool tweenCardReturn = true;

        private void Start()
        {  
            deckNav = GetComponent<DeckNavigationHandler>();
            
            rect = GetComponent<RectTransform>();

            StartCoroutine(Frame());

        }

        public void AddCard(Card card)
        {

            Transform parentTransform = card.transform.parent;

            parentTransform.SetParent(transform);
            card.transform.localPosition = Vector3.zero;
            card.transform.localScale = Vector3.one;

            card.PointerEnterEvent.AddListener(CardPointerEnter);
            card.PointerExitEvent.AddListener(CardPointerExit);
            card.BeginDragEvent.AddListener(BeginDrag);
            card.EndDragEvent.AddListener(EndDrag);
            card.SelectEvent.AddListener(OnCardSelectChanged); 

            card.name = cards.Count.ToString();
            cards.Add(card);

            foreach (Card c in cards)
            {
                if (c.cardVisual != null)
                    c.cardVisual.UpdateIndex(transform.childCount);
            }

            // Changes text direction

            card.deck = this;

            card.ReInitialize();

            StartCoroutine(Frame());
        }

        public void AddCard(CardScriptable cardSpecs)
        {
            GameObject newSlot = Instantiate(slotPrefab, transform);
            Card newCard = newSlot.GetComponentInChildren<Card>();

            newCard.spec = cardSpecs;
        
            if (newCard != null)
            {
                newCard.PointerEnterEvent.AddListener(CardPointerEnter);
                newCard.PointerExitEvent.AddListener(CardPointerExit);
                newCard.BeginDragEvent.AddListener(BeginDrag);
                newCard.EndDragEvent.AddListener(EndDrag);
                newCard.SelectEvent.AddListener(OnCardSelectChanged); 

                newCard.name = cards.Count.ToString();
                cards.Add(newCard);
            }

            newCard.deck = this;

            StartCoroutine(Frame());
        }

        //TODO add powerup remove
         public void RemoveAllCards()
        {
            List<Card> cardsToRemove = new List<Card>(cards);
            foreach (Card card in cardsToRemove)
            {
                if (card != null)
                {
                    RemoveCard(card);
                }
            }
            cards.Clear();
        }

        public void RemoveCard(Card card)
        {
            if (cards.Contains(card))
            {
                cards.Remove(card);

                if (deckNav) deckNav.RemoveCard(card); 

                card.PointerEnterEvent.RemoveListener(CardPointerEnter);
                card.PointerExitEvent.RemoveListener(CardPointerExit);
                card.BeginDragEvent.RemoveListener(BeginDrag);
                card.EndDragEvent.RemoveListener(EndDrag);
                card.SelectEvent.RemoveListener(OnCardSelectChanged); 

                if (card.cardVisual is TarotCardVisual tarotCardVisual)
                {
                    StartCoroutine(tarotCardVisual.Dissapear());
                }
                else{
                    Destroy(card.transform.parent.gameObject);
                }

                  
            }
        }


        public void SwapCardHand(Card card, DeckVisual toHand){
            if (cards.Contains(card))
            {
                cards.Remove(card); //dont uses RemoveCard to get animation

                if (deckNav) deckNav.RemoveCard(card); 

                card.PointerEnterEvent.RemoveListener(CardPointerEnter);
                card.PointerExitEvent.RemoveListener(CardPointerExit);
                card.BeginDragEvent.RemoveListener(BeginDrag);
                card.EndDragEvent.RemoveListener(EndDrag);

                toHand.AddCard(card);
                
                UpdateAllIndices();
                toHand.UpdateAllIndices();

                if (card is TarotCard tarotCard)
                {
                    tarotCard.moveSFX.Play(null);
                }
                else if(card is CoinCard coinCard){
                    coinCard.moveSFX.Play(null);
                }
            }
        }

        private void UpdateAllIndices()
        {
            for (int i = 0; i < cards.Count; i++)
                cards[i].cardVisual?.UpdateIndex(transform.childCount);
        }

        private IEnumerator Frame()
            {
                yield return new WaitForSecondsRealtime(.1f);

                for (int i = 0; i < cards.Count; i++)
                {
                    if (cards[i].cardVisual != null)
                        cards[i].cardVisual.UpdateIndex(transform.childCount);
                }

                if(deckNav) deckNav.AddedCard();
            }

        private void BeginDrag(Card card)
        {
            isSelected = true;
            selectedCard = card;
        }


        void EndDrag(Card card)
        {
            if (selectedCard == null)
                return;
            
            
            selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0,selectedCard.selectionOffset,0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack).SetUpdate(true);

            rect.sizeDelta += Vector2.right;
            rect.sizeDelta -= Vector2.right;
            
            isSelected = false;
            selectedCard = null;

        }

        void CardPointerEnter(Card card)
        {
            hoveredCard = card;
        }

        void CardPointerExit(Card card)
        {
            hoveredCard = null;
        }
    
        void DuplicateCard(Card card)
        {
            if (card == null)
                return;

            CardScriptable cardSpecs = card.spec;

            AddCard(cardSpecs);
        }

        void Update()
        {
            if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.KEYBOARD && Input.GetMouseButtonDown(1))
            {
                foreach (Card card in cards)
                {
                    card.Deselect();
                }
            }

            if (selectedCard == null)
                return;

            if (isCrossing)
                return;

            for (int i = 0; i < cards.Count; i++)
            {

                if (selectedCard.transform.position.x > cards[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                    {
                        Swap(i);
                        break;
                    }
                }

                if (selectedCard.transform.position.x < cards[i].transform.position.x)
                {
                    if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                    {
                        Swap(i);
                        break;
                    }
                }
            }
        }

        void Swap(int index)
        {
            isCrossing = true;

            Transform focusedParent = selectedCard.transform.parent;
            Transform crossedParent = cards[index].transform.parent;

            cards[index].transform.SetParent(focusedParent);
            cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
            selectedCard.transform.SetParent(crossedParent);

            isCrossing = false;

            if (cards[index].cardVisual == null)
                return;

            bool swapIsRight = cards[index].ParentIndex() > selectedCard.ParentIndex();
            cards[index].cardVisual.Swap(swapIsRight ? -1 : 1);

            //Updated Visual Indexes
            foreach (Card card in cards)
            {
                card.cardVisual.UpdateIndex(transform.childCount);
            }
        }

        private void OnCardSelectChanged(Card card, bool isSelected)
        {
            if (isSelected)
            {
                foreach (Card c in cards)
                {
                    if (c != card && c.selected)
                    {
                        c.Deselect();
                    }
                }
            }
        }
        

    }
}