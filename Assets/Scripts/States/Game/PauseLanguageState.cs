using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace F.State
{
	public class PauseLanguageState : GameState
	{
		public void OnClick(int i)
		{
            switch (i)
            {
                case 0:
                    LanguageSetter.Instance.SetEN();
                    break;
                case 1:
                    LanguageSetter.Instance.SetBR();
                    break;

            }

			sfxButtonClick.Play(null);
			this.owner.ChangeState<PauseOptionsState>();
		}

		public void OnCancel()
		{
			sfxButtonClick.Play(null);
			this.owner.ChangeState<PauseOptionsState>();
		}

        private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            OnCancel(); 
        }

		public override void Enter()
		{
            base.StartCoroutine(this.WaitToShowMenuCR());
        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);
            base.languageMenu.Show();
			base.languageMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
			base.languageMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
            input.UI.Escape.canceled += this.OnEscapeCanceled;
		}
		public override void Exit()
		{
			base.languageMenu.Hide();
			base.languageMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
			base.languageMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            input.UI.Escape.canceled -= this.OnEscapeCanceled;
		}
	}
}
