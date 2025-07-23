using System;
using System.Collections;
using F.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using F.Cards;

namespace F.State
{

	public class CoinStoreState : GameState
	{
        private bool isInitialized = false;

        private void Initialize()
        {
            CombatStateStatus.MoneyChangedEvent += MoneyChanged;
            isInitialized = true;
        }

        private void OnDestroy()
        {
            CombatStateStatus.MoneyChangedEvent -= MoneyChanged;
        }

        private void MoneyChanged(object sender, int newRound)
        {
            moneyStore.text = newRound.ToString();
        }

        public void OnClick(int i)
		{   
            switch (i)
            {
                case 0:
                    break;
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    this.owner.ChangeState<CombatState>();
                    break;   
     
            }

            sfxButtonClick.Play(null);
            base.optionsMenu.GetComponent<Menu>().UpdateLayout();

		}

        public void OnCancel()
		{
			this.owner.ChangeState<CombatState>();
		}

		public override void Enter()
		{
            base.pauseBG.Hide();

            if (!isInitialized)
            {
                Initialize();
            }
            base.mainBackground.Hide();
			cameraMoveUp.Play(null);
			//cameraMoveDown.Play(null);
			base.StartCoroutine(this.WaitToShow());

            CombatStateStatus.PhaseStarted = false;

            MoneyChecker();
            GamePointsChecker();
            
            CombatStateStatus.IncrementRound();

            if(CombatStateStatus.MaxKarma > 20){
                CombatStateStatus.MinKarma = CombatStateStatus.MinKarma + 5;
                CombatStateStatus.MaxKarma = CombatStateStatus.MaxKarma - 5;
            }
            else{
                CombatStateStatus.MinKarma = CombatStateStatus.MinKarma + 1;
                CombatStateStatus.MaxKarma = CombatStateStatus.MaxKarma - 1;
            }

		}

		public override void Exit()
		{
            base.storeMenu.Hide();
            base.mainBackground.Show();
            base.moneyStorePanel.Hide();
            base.storeMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
			base.storeMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));

            
		}

        private void MoneyChecker()
        {
            int absKarma = Mathf.Abs(CombatStateStatus.Karma);

            if (absKarma == 0)
            {
                CombatStateStatus.Money += 3;
                CombatStateStatus.TotalCoins += 3;
            }
            else if (absKarma <= CombatStateStatus.MaxKarma / 2)
            {
                CombatStateStatus.Money += 2;
                CombatStateStatus.TotalCoins += 2;
            }
            else
            {
                CombatStateStatus.Money += 1;
                CombatStateStatus.TotalCoins += 1;
            }
        }


        private void GamePointsChecker()
        {
            int absKarma = Mathf.Abs(CombatStateStatus.Karma); 

            if (absKarma == 0)
            {
                CombatStateStatus.GamePoints += 1500;
            }
            else if (absKarma == 1)
            {
                CombatStateStatus.GamePoints += 1200;
            }
            else if (absKarma == 2)
            {
                CombatStateStatus.GamePoints += 1000;
            }
            else if (absKarma == 3)
            {
                CombatStateStatus.GamePoints += 900;
            }
            else if (absKarma == 4)
            {
                CombatStateStatus.GamePoints += 850;
            }
            else if (absKarma == 5)
            {
                CombatStateStatus.GamePoints += 800;
            }
            else if (absKarma > 5 && absKarma <= 10)
            {
                CombatStateStatus.GamePoints += 500;
            }               
            else if (absKarma > 10 && absKarma < 25)
            {
                CombatStateStatus.GamePoints += 250;
            }
            else if (absKarma >= 25)
            {
                CombatStateStatus.GamePoints += 100;
            }


            if(CombatStateStatus.GamePoints >= 6666){
                SteamIntegration.UnlockAchievement("ACH_6666");
            }


        }
		private IEnumerator WaitToShow()
		{
			yield return new WaitForSecondsRealtime(transitionTime);
            base.storeMenu.Show();
            base.moneyStorePanel.Show();
            base.storeMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
			base.storeMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
            
            yield break;
		}


	}


    
}

