using UnityEngine;
using System;
using System.Collections;

namespace F.State
{
    public class InitState : TitleScreenState
    {
        public override void Enter()
        {
            base.StartCoroutine(this.WaitToLoadCR());
            if(base.titleScreenMusic != null)AudioManager.Instance.PlayMusic(base.titleScreenMusic);
            if(base.titleScreenMusic != null)AudioManager.Instance.FadeInMusic(5f);
            AudioManager.Instance.SetLowPassFilter(false);
            SaveSystem.Load();
            
            base.menuPoints.text = SaveSystem.data.points.ToString();
            base.menuRounds.text = SaveSystem.data.rounds.ToString();
            base.menuCoins.text = SaveSystem.data.coins.ToString();     

        }

        private IEnumerator WaitToLoadCR()
        {
            yield return new WaitForSeconds(transitionTime);
            if(base.screenCover != null)base.screenCover.Hide();
            base.curtainLeft.Hide();
            base.curtainRight.Hide();
            if (ControllerManager.firstConnect)
            {
                this.owner.ChangeState<PressAnyState>();
                ControllerManager.firstConnect = false;
            }
            else this.owner.ChangeState<TitleMainMenuState>();
        }
    }
}