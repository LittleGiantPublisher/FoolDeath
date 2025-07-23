using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

namespace F.Cards
{
    public class CrossDecksController : MonoBehaviour
    {
        [SerializeField] public  List<DeckVisual> decks;
        [SerializeField] private float minDistance = 100f;
        [SerializeField] private float fadeDuration = 0.2f;

        [SerializeField] private SoundEffectSO coinUseSFX;


        public event Action OnThreeDecksFilled;
        public event Action OnFourDecksFilled;
        public event Action OnAllDecksFilled;
        public event Action OnRubyCoin;

        private bool isThreeCards = false;
        private bool isfourCards = false;

        private DeckVisual nearestDeck;
        private CardVisual currentCard;
        public HashSet<DeckVisual> unavailableDecks = new HashSet<DeckVisual>();
        public  HashSet<DeckVisual> tarotOccupiedDecks = new HashSet<DeckVisual>();
        public HashSet<DeckVisual> coinAddedDecks = new HashSet<DeckVisual>();
        
        [SerializeField] private SoundEffectSO sfxDeath;

        private void Start()
        {
            isThreeCards = false;
            isfourCards = false;

            foreach (var deck in decks)
            {
                SetDeckAlpha(deck, 0f);
            }
        }

        private void Update()
        {
            if (currentCard == null) return;

            float closestDistance = float.MaxValue;
            DeckVisual closestDeck = null;

            foreach (var deck in decks)
            {
                if (currentCard is TarotCardVisual && unavailableDecks.Contains(deck)) continue;
                if (currentCard is CoinCardVisual && (!tarotOccupiedDecks.Contains(deck) || coinAddedDecks.Contains(deck))) continue;

                Vector3 cardPosition = new Vector3(currentCard.transform.position.x, currentCard.transform.position.y, 0);
                Vector3 deckPosition = new Vector3(deck.transform.position.x, deck.transform.position.y, 0);

                float distance = Vector3.Distance(cardPosition, deckPosition);
                if (distance < closestDistance && distance <= minDistance)
                {
                    closestDistance = distance;
                    closestDeck = deck;
                }
            }

            if (closestDeck != nearestDeck)
            {
                if (nearestDeck != null)
                {
                    SetDeckAlpha(nearestDeck, 0f);
                }

                nearestDeck = closestDeck;

                if (nearestDeck != null)
                {
                    SetDeckAlpha(nearestDeck, 0.7f);
                }
            }
        }

        private void SetDeckAlpha(DeckVisual deck, float alpha)
        {
            var deckImage = deck.GetComponentInChildren<UnityEngine.UI.Image>();
            if (deckImage != null)
            {
                deckImage.DOFade(alpha, fadeDuration);
            }
        }

        public void OnCardBeginDrag(CardVisual card)
        {
            currentCard = card;
        }
        
        public void DistributeCardsRandomly(List<TarotCardVisual> tarotCards)
        {
            if (tarotCards == null || tarotCards.Count < 4)
            {
                Debug.LogError("Not enough cards to distribute.");
                return;
            }

            var eligibleDecks = decks.Skip(1).ToList();

            var randomDecks = eligibleDecks.OrderBy(_ => UnityEngine.Random.value).Take(4).ToList();

            for (int i = 0; i < 4; i++)
            {
                DeckVisual targetDeck = randomDecks[i];
                TarotCardVisual tarotCard = tarotCards[i];

                tarotCard.parentCard.deck.SwapCardHand(tarotCard.parentCard, targetDeck);
                tarotCard.SetInDeckState(true);
                tarotOccupiedDecks.Add(targetDeck);
                unavailableDecks.Add(targetDeck);

                PerformDeckAction(targetDeck, tarotCard.parentCard as TarotCard);
            }

            //CheckDeckFillMilestones();
        }
    
        public void OnCardEndDrag()
        {
            int filledCount = unavailableDecks.Count;
            if(filledCount == 5) return;
            
            if (nearestDeck != null && currentCard != null)
            {
                if (currentCard is TarotCardVisual tarotCard)
                {
                    tarotCard.parentCard.deck.SwapCardHand(tarotCard.parentCard, nearestDeck);
                    tarotCard.SetInDeckState(true);
                    tarotOccupiedDecks.Add(nearestDeck); 
                    unavailableDecks.Add(nearestDeck); 
                    PerformDeckAction(nearestDeck, tarotCard.parentCard as TarotCard);
                    CheckDeckFillMilestones();
                }
                else if (currentCard is CoinCardVisual coinCard && tarotOccupiedDecks.Contains(nearestDeck) && !coinAddedDecks.Contains(nearestDeck))
                {
                    Card tarotCardInDeck = nearestDeck.GetComponentInChildren<Card>();
                    
                    if (tarotCardInDeck != null && tarotCardInDeck.cardVisual is TarotCardVisual tarotVisual)
                    {
                        

                        tarotVisual.cardSuit.sprite = coinCard.parentCard.spec.cardArt;
                        tarotVisual.cardSuit.color = new Color(1, 1, 1, 1);

                        //TODO spaghetti
                        if (coinCard.parentCard.spec is CoinScriptable coinSpec && coinSpec.id == 2)
                        {
                            tarotVisual.cardImage.sprite = ((TarotScriptable)tarotCardInDeck.spec).foolArt;
                        }
   
                        
                        StartCoroutine(tarotVisual.DamageFlash());

                        coinCard.SetInDeckState(true);
                        coinAddedDecks.Add(nearestDeck);

                        PerformCoinAction(nearestDeck, coinCard.parentCard as CoinCard);
                        
                        coinCard.parentCard.deck.RemoveCard(coinCard.parentCard);
                    }
                }

                SetDeckAlpha(nearestDeck, 0f);
                nearestDeck = null;
            }

            currentCard = null;
        }

        private void CheckDeckFillMilestones()
        {
            int filledCount = unavailableDecks.Count;

            if (filledCount == 3 && !isThreeCards)
            {
                isThreeCards = true;
                OnThreeDecksFilled?.Invoke();
            }
            else if (filledCount == 4 && !isfourCards)
            {
                isfourCards = true;
                OnFourDecksFilled?.Invoke();
            }
            else if (filledCount == decks.Count)
            {
                isThreeCards = false;
                isfourCards = false;
                OnAllDecksFilled?.Invoke();
            }
        }

        public void PerformDeckAction(DeckVisual deck, TarotCard card)
        {
            int deckIndex = decks.IndexOf(deck);
            if (card.spec is TarotScriptable tarotSpec)
            {
                int karmaChange = tarotSpec.value;
                if(card.isZero)karmaChange = 0;


                if (karmaChange == 13)
                {
                    ApplySpecialEffectForValue13();
                }
                
                    switch (deckIndex)
                    {
                        case 0:
                            CombatStateStatus.Karma += karmaChange * 1;
                            break;
                        case 1:
                            CombatStateStatus.Karma += karmaChange * 2;
                            break;
                        case 2:
                            CombatStateStatus.Karma += karmaChange * 3;
                            break;
                        case 3:
                            CombatStateStatus.Karma += karmaChange * -2;
                            break;
                        case 4:
                            CombatStateStatus.Karma += karmaChange * -3;
                            break;
                    }
                
            }
        }

        private void ApplySpecialEffectForValue13()
        {
            foreach (var targetDeck in decks)
            {
                Card tarotCardInDeck = targetDeck.GetComponentInChildren<Card>();

                if (tarotCardInDeck != null && tarotCardInDeck.cardVisual is TarotCardVisual tarotVisual)
                {
                    if (tarotCardInDeck.spec is TarotScriptable deckTarotSpec && deckTarotSpec.value != 13)
                    {
                        tarotVisual.cardImage.sprite = deckTarotSpec.foolArt;
                        
                        if (tarotCardInDeck is TarotCard tarotCard)
                        {
                            tarotCard.isZero = true; 
                        }
                        
                        StartCoroutine(tarotVisual.DamageFlash());
                    }
                }
            }

            CombatStateStatus.Karma = 0;
            sfxDeath.Play(null);
        }

        
        private void PerformCoinAction(DeckVisual deck, CoinCard card)
        {

            if (card.spec is CoinScriptable coinSpec)
            {
                int deckIndex = decks.IndexOf(deck);

                if (tarotOccupiedDecks.Contains(deck))
                {
                    int idCoin = coinSpec.id;
                    //TODO less spagheti

                    coinUseSFX.Play(null);

                    switch (idCoin)
                    {
                        case 0:
                            Card tarotCardInDeck = deck.GetComponentInChildren<Card>();

                            if (tarotCardInDeck != null && tarotCardInDeck.spec is TarotScriptable tarotSpec)
                            {
                                int originalValue = tarotSpec.value;
                                if(tarotCardInDeck is TarotCard cardTarotZero){
                                    if(cardTarotZero.isZero)originalValue = 0;
                                }
                                int multiplier = GetMultiplierByDeckIndex(deckIndex);

                                CombatStateStatus.Karma -= originalValue * multiplier;

                                multiplier = -multiplier;

                                CombatStateStatus.Karma += originalValue * multiplier;
                            }
                            break;
                        case 1:
                            tarotCardInDeck = deck.GetComponentInChildren<Card>();

                            if (tarotCardInDeck != null && tarotCardInDeck.spec is TarotScriptable tarotSpecCase2)
                            {
                                int originalValue = tarotSpecCase2.value;
                                if(tarotCardInDeck is TarotCard cardTarotZero){
                                    if(cardTarotZero.isZero)originalValue = 0;
                                }

                                int multiplier = GetMultiplierByDeckIndex(deckIndex);

                                CombatStateStatus.Karma -= originalValue * multiplier;
                            }
                            RemoveCardsFromDeck(deck);
                            OnRubyCoin?.Invoke();
                            break;
                        case 2:
                            tarotCardInDeck = deck.GetComponentInChildren<Card>();

                            if (tarotCardInDeck != null && tarotCardInDeck.spec is TarotScriptable tarotSpecCase3)
                            {
                                int originalValue = tarotSpecCase3.value;


                                if(tarotCardInDeck is TarotCard cardTarotZero){
                                    if(cardTarotZero.isZero)originalValue = 0;
                                }

                                int multiplier = GetMultiplierByDeckIndex(deckIndex);

                                CombatStateStatus.Karma -= originalValue * multiplier;
                            }
                            break;
                    }
                }
            }
        }



        private int GetMultiplierByDeckIndex(int deckIndex)
        {
            switch (deckIndex)
            {
                case 0: return 1;
                case 1: return 2;
                case 2: return 3;
                case 3: return -2;
                case 4: return -3;
                default: return 1;
            }
        }
        
        private void RemoveCardsFromDeck(DeckVisual deckTarget){
            deckTarget.RemoveAllCards();
            unavailableDecks.Remove(deckTarget);
            tarotOccupiedDecks.Remove(deckTarget);
            coinAddedDecks.Remove(deckTarget);
            SetDeckAlpha(deckTarget, 0f);
        }

        public void RemoveAllCards(float timer)
        {
            StartCoroutine(RemoveAllCardsCoroutine(timer));
        }

        private IEnumerator RemoveAllCardsCoroutine(float timer)
        {
            yield return new WaitForSeconds(timer);

            foreach (var deck in decks)
            {
                deck.RemoveAllCards();
                unavailableDecks.Remove(deck);
                tarotOccupiedDecks.Remove(deck);
                coinAddedDecks.Remove(deck);
                SetDeckAlpha(deck, 0f);
            }
        }
    }
}
