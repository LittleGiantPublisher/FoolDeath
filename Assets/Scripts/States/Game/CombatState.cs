using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using System.Collections.Generic;
using System.Linq;
using F.Cards;
using TMPro;

namespace F.State
{
    public class CombatState : GameState
    {
        [SerializeField]
        private CardScriptable deathCard;

        [SerializeField]
        private TMP_Text cardPoolSizeText;

        private bool isInitialized = false;
        private bool isDelayStart = true;
        private bool tutorialStarted = false;
        private bool canEscape = false;
        

        private void Initialize()
        {
            CombatStateStatus.PhaseChangedEvent += OnPhaseChanged;
            CombatStateStatus.RoundChangedEvent += OnRoundChanged;
            CombatStateStatus.KarmaChangedEvent += KarmaChanged;
            CombatStateStatus.MoneyChangedEvent += MoneyChanged;
            CombatStateStatus.GamePointsChangedEvent += GamePointsChanged;

            if (crossDecksController != null)
            {
                crossDecksController.OnThreeDecksFilled += OnThreeDecksFilled;
                crossDecksController.OnFourDecksFilled += OnFourDecksFilled;
                crossDecksController.OnAllDecksFilled += OnAllDecksFilled;
                crossDecksController.OnRubyCoin += OnRubyCoin;
            }

            CombatStateStatus.CurrentPhase = CombatPhase.Neutral;
            CombatStateStatus.Round = 1;
            CombatStateStatus.PhaseStarted = false;
            CombatStateStatus.Karma = 0;
            CombatStateStatus.MinKarma = -50;
            CombatStateStatus.MaxKarma = 50;

            CombatStateStatus.Money = 0;
            CombatStateStatus.TotalCoins = 0;
            CombatStateStatus.GamePoints = 0;


            isInitialized = true;
            isDelayStart = true;
            tutorialStarted = false;
            canEscape = false;
        }

       private IEnumerator SetupInitialCards()
        {

            List<CardScriptable> randomCards = tarotGenerator.GetRandom(4, tarotGenerator.cardPool, new List<CardScriptable> { deathCard });

            List<DeckVisual> eligibleDecks = crossDecksController.decks.Skip(1).ToList();

            eligibleDecks = eligibleDecks.OrderBy(_ => UnityEngine.Random.value).Take(4).ToList();

            DeckVisual deathHand = base.deathHand.GetComponentInChildren<DeckVisual>();
                

            for (int i = 0; i < randomCards.Count; i++)
            {
                DeckVisual targetDeck = eligibleDecks[i];
                CardScriptable cardSpec = randomCards[i];

                deathHand.AddCard(cardSpec);
                UpdateCardPoolSize();
                yield return new WaitForSeconds(0.00001f);

                if (deathHand.cards.Count > 0)
                {
                    Card cardToSwap = deathHand.cards.Last();
                    if(cardToSwap is TarotCard card){
                        card.deck.SwapCardHand(card, targetDeck);
                        card.cardVisual.SetInDeckState(true);

                        crossDecksController.tarotOccupiedDecks.Add(targetDeck);
                        crossDecksController.unavailableDecks.Add(targetDeck);
                        crossDecksController.PerformDeckAction(targetDeck, cardToSwap as TarotCard);
                    }
                }

            }

            AddDeathCardToHand();
        }


        private void AddDeathCardToHand()
        {
            StartCoroutine(AddSpecificCardsToHand(new List<CardScriptable> { deathCard }));
        }

        private void OnThreeDecksFilled()
        {
            CombatStateStatus.AdvancePhase();
        }

        private void OnFourDecksFilled()
        {
            CombatStateStatus.AdvancePhase();
        }

        private void OnAllDecksFilled()
        {
            if(tutorialStarted){
                CombatStateStatus.AdvancePhase();
                StartCoroutine(WaitToCoinStore());
            }
            else{
                StartCoroutine(WaitToShowCD2());
            }   
        }
        
         private void OnDestroy()
        {
            CombatStateStatus.PhaseChangedEvent -= OnPhaseChanged;
            CombatStateStatus.RoundChangedEvent -= OnRoundChanged;
            CombatStateStatus.KarmaChangedEvent -= KarmaChanged;
            CombatStateStatus.MoneyChangedEvent -= MoneyChanged;
            CombatStateStatus.GamePointsChangedEvent -= GamePointsChanged;
            crossDecksController.OnRubyCoin -= OnRubyCoin;
            if (crossDecksController != null)
            {
                crossDecksController.OnThreeDecksFilled -= OnThreeDecksFilled;
                crossDecksController.OnFourDecksFilled -= OnFourDecksFilled;
                crossDecksController.OnAllDecksFilled -= OnAllDecksFilled;
            }
        }


        private IEnumerator WaitToShowCD2(){
            tutorialStarted = true;
            yield return new WaitForSeconds(transitionTime + 1.25f);
            crossDecksController.RemoveAllCards(0.3f);
            CombatStateStatus.AdvancePhase();
            tarotGenerator.Reset();
            UpdateCardPoolSize();
                
        }

        private IEnumerator WaitToCoinStore()
        {
            yield return new WaitForSeconds(1.8f);

            if (ChecksWinOrLose())
            {
                this.owner.ChangeState<CoinStoreState>();
            }
            else
            {
                this.owner.ChangeState<ResultState>();
            }
        }

        private bool ChecksWinOrLose()
        {
            //if(CombatStateStatus.Karma == 0)SteamIntegration.UnlockAchievement("ACH_0");
            return CombatStateStatus.Karma >= CombatStateStatus.MinKarma && CombatStateStatus.Karma <= CombatStateStatus.MaxKarma;
        }

        private void UpdateCardPoolSize()
        {
            if (tarotGenerator != null && cardPoolSizeText != null)
            {
                cardPoolSizeText.text = tarotGenerator.cardPool.Count.ToString();
            }
        }

        private void OnPhaseChanged(object sender, CombatPhase newPhase)
        {
            switch (newPhase)
            {
                case CombatPhase.Start:
                    isDelayStart = true;
                    return;
                case CombatPhase.Neutral:
                    isDelayStart = true;
                    return;
                case CombatPhase.Past:
                    StartCoroutine(AddCardToHand(3));
                    break;
                case CombatPhase.Present:
                    StartCoroutine(AddCardToHand(1));
                    break;
                case CombatPhase.Future:
                    StartCoroutine(AddCardToHand(1));
                    break;
            }
        }

        private void OnRoundChanged(object sender, int newRound)
        {
            round.text = newRound.ToString();
            round2.text = newRound.ToString();
        }

        private void KarmaChanged(object sender, int newKarma) { }

        private void MoneyChanged(object sender, int newMoney)
        {
            money.text = newMoney.ToString();
        }

        private void GamePointsChanged(object sender, int newPoints)
        {
            gamePoints.text = CombatStateStatus.GamePoints.ToString() + " / 13000";
            gamePoints2.text = newPoints.ToString();
            totalCoins.text = CombatStateStatus.TotalCoins.ToString();
        }

        private void OnRubyCoin()
        {
            StartCoroutine(AddCardToHand(1));
        }

        private IEnumerator AddSpecificCardsToHand(List<CardScriptable> cardsToAdd)
        {
            if (cardsToAdd == null || cardsToAdd.Count == 0)
            {
                Debug.LogError("No cards provided to add to hand.");
                yield break;
            }

            if (isDelayStart)
            {
                yield return new WaitForSeconds(0.5f);
                isDelayStart = false;
            }

            yield return new WaitForSeconds(0.3f);

            foreach (var cardSpec in cardsToAdd)
            {

                DeckVisual deathHand = base.deathHand.GetComponentInChildren<DeckVisual>();
                DeckVisual deckHand = base.deckHand.GetComponentInChildren<DeckVisual>();

                deathHand.AddCard(cardSpec);
                UpdateCardPoolSize();

                yield return new WaitForSeconds(0.00001f);

                if (deathHand.cards.Count > 0)
                {
                    Card cardToSwap = deathHand.cards.Last();

                    yield return new WaitForSeconds(0.1f);
                    deathHand.SwapCardHand(cardToSwap, deckHand);
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.3f);
            }
        }

        private IEnumerator AddCardToHand(int qtd)
        {
            if (isDelayStart)
            {
                yield return new WaitForSeconds(0.5f);
                isDelayStart = false;
            }

            
            yield return new WaitForSeconds(0.3f);

            for (int i = 0; i < qtd; i++)
            {
                List<CardScriptable> generatedCard = base.tarotGenerator.GetRandom(1);

                if (generatedCard == null || generatedCard.Count == 0)
                {
                    break;
                }

                CardScriptable cardSpecs = generatedCard[0];

                DeckVisual deathHand = base.deathHand.GetComponentInChildren<DeckVisual>();
                DeckVisual deckHand = base.deckHand.GetComponentInChildren<DeckVisual>();

                deathHand.AddCard(cardSpecs);
                UpdateCardPoolSize();

                yield return new WaitForSeconds(0.00001f);

                if (deathHand.cards.Count > 0)
                {
                    Card cardToSwap = deathHand.cards.Last();

                    yield return new WaitForSeconds(0.1f);
                    deathHand.SwapCardHand(cardToSwap, deckHand);
                    yield return new WaitForSeconds(0.1f);
                }

                yield return new WaitForSeconds(0.3f);
            }
        }

        public override void Enter()
        {
                     
            UpdateCardPoolSize();

            if (!isInitialized)
            {
                Initialize();
            }

            round.text = CombatStateStatus.Round.ToString();
            round2.text = CombatStateStatus.Round.ToString();
            money.text = CombatStateStatus.Money.ToString();
            gamePoints.text = CombatStateStatus.GamePoints.ToString() + " / 13000";
            gamePoints2.text = CombatStateStatus.GamePoints.ToString();
            totalCoins.text = CombatStateStatus.TotalCoins.ToString();

            AudioManager.Instance.SetLowPassFilter(false);
            base.pauseBG.Hide();
            base.StartCoroutine(this.WaitToShowCD());
        }

        public override void Exit()
        {
            //base.pauseMenuButton.Hide();
            base.deckHand.Hide();
            base.deathHand.Hide();
            base.karmaPanel.Hide();
            base.crossPanel.Hide();
            base.HUD.Hide();
            base.coinPanel.Hide();
            base.pauseBG.Hide();
            //base.pauseMenuButton.onClick.RemoveListener(new UnityAction<int>(this.OnClickOptions));
            //base.pauseMenuButton.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            input.UI.Cancel.canceled -= this.OnEscapeCanceled;
        }

        public void OnClickOptions(int i)
        {
            this.owner.ChangeState<PauseState>();
            sfxButtonClick.Play(null);
        }

        public void OnCancel()
        {
            this.owner.ChangeState<PauseState>();
        }

        private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            if(canEscape)OnCancel();
        }

        private IEnumerator WaitToShowCD()
        {
            yield return new WaitForSecondsRealtime(transitionTime);

            if (isDelayStart)
            {
                yield return new WaitForSeconds(1.2f);
            }

            if (!CombatStateStatus.PhaseStarted)
            {
                if(tutorialStarted){
                    crossDecksController.RemoveAllCards(0.3f);
                }
                else{
                    crossDecksController.RemoveAllCards(0);
                }    
                CombatStateStatus.PhaseStarted = true;
                if(tutorialStarted)CombatStateStatus.AdvancePhase();
                tarotGenerator.Reset();
                UpdateCardPoolSize();
            }

            if(!tutorialStarted && !canEscape){
                canEscape = true;
                StartCoroutine(SetupInitialCards());
            }
            
            //Wbase.pauseMenuButton.Show();
            base.deckHand.Show();
            base.deathHand.Show();
            base.karmaPanel.Show();
            base.crossPanel.Show();
            base.HUD.Show();
            base.coinPanel.Show();
            base.pauseBG.Hide();
            //base.pauseMenuButton.onClick.AddListener(new UnityAction<int>(this.OnClickOptions));
            //base.pauseMenuButton.onCancel.AddListener(new UnityAction(this.OnCancel));
            input.UI.Cancel.canceled += this.OnEscapeCanceled;
        }
    }
}
