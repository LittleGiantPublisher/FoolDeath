using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;

namespace F.State
{
    public class TitleMainMenuState : TitleScreenState
    {
        public void OnClick(int i)
        {
            switch (i)
            {
                case 0:
                    base.StartCoroutine(this.TransitionToLoadoutSelect());
                    break;
                case 1:
                    this.owner.ChangeState<LanguageState>();
                    break;
                case 2:
                    this.owner.ChangeState<OptionsMenuState>();
                    break;
                case 3:
                    ExitGame.QuitGame();
                    break;
            }

            sfxButtonClick.Play(null);
        }

        public override void Enter()
        {   
            base.StartCoroutine(this.WaitToShowMenuCR());
        }

        public override void Exit()
        {
            base.mainMenu.Hide();
            base.recordsPanel.Hide();
            base.mainMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);
            base.characterPanel.Show();
            base.logoPanel.Show();
            base.mainMenu.Show();
            base.recordsPanel.Show();
            base.mainMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
        }

        private IEnumerator TransitionToLoadoutSelect()
        {
            yield return new WaitForSeconds(0.2f);
           this.owner.ChangeState<WaitLoadToPlay>();
        }
    }
}