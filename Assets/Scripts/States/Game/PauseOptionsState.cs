using System;
using System.Collections;
using F.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace F.State
{
	public class PauseOptionsState : GameState
	{
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
                    break; 
                case 4:
                    this.owner.ChangeState<PauseLanguageState>();
                    break;   
                case 5:
                    this.owner.ChangeState<PauseState>();
                    break;       
            }

            sfxButtonClick.Play(null);
            base.pauseMenu.GetComponent<Menu>().UpdateLayout();

		}

		public void OnCancel()
		{
			this.owner.ChangeState<PauseState>();
		}

        private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            OnCancel(); 
        }

		public override void Enter()
		{
            base.pauseBG.Show();
            base.StartCoroutine(this.WaitToShowMenuCR());

        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);
            base.optionsMenu.GetComponent<OptionsSetter>().Refresh();
            base.optionsMenu.Show();
			base.optionsMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
			base.optionsMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
            input.UI.Escape.canceled += this.OnEscapeCanceled;
		}

		public override void Exit()
		{
			base.optionsMenu.Hide();
			base.optionsMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
			base.optionsMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            input.UI.Escape.canceled -= this.OnEscapeCanceled;
        }
	}
}
