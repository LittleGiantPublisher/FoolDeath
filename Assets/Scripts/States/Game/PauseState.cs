
using System;
using System.Collections;
using F.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace F.State
{
	public class PauseState : GameState
	{
		public void OnClick(int i)
		{
            switch (i)
            {
                case 0:
                    this.owner.ChangeState<CombatState>();
                    break;
                case 1:
                    this.owner.ChangeState<PauseOptionsState>();
                    break;
                case 2:
                    this.owner.ChangeState<WaitLoadToMenu>();
                    break;
                
            }

            sfxButtonClick.Play(null);
            base.pauseMenu.GetComponent<Menu>().UpdateLayout();

		}

		public void OnCancel()
		{
			this.owner.ChangeState<CombatState>();
		}

        private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            OnCancel(); 
        }

		public override void Enter()
		{
            AudioManager.Instance.SetLowPassFilter(true);
            base.StartCoroutine(this.WaitToShowMenuCR());

        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);
            base.pauseBG.Show();
            base.pauseMenu.Show();
			base.pauseMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
			base.pauseMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
            input.UI.Escape.canceled += this.OnEscapeCanceled;
		}

		public override void Exit()
		{
			base.pauseMenu.Hide();
			base.pauseMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
			base.pauseMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            input.UI.Escape.canceled -= this.OnEscapeCanceled;
        }
	}
}
