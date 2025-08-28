using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Porting;

namespace F.State
{
	public class LanguageState : TitleScreenState
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
			this.owner.ChangeState<TitleMainMenuState>();
		}

		public void OnCancel()
		{
			this.owner.ChangeState<TitleMainMenuState>();
		}

        private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            OnCancel(); 
        }

        public override void Enter()
        {
            base.StartCoroutine(this.WaitToShowMenuCR());
            base.logoPanel.Hide();
            base.characterPanel.Hide();
        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);
            base.languageMenu.Show();
			base.languageMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
			base.languageMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
            if (PlatformManager.enterButtonParam == 1)
            {
                input.UI.Cancel.canceled += this.OnEscapeCanceled;
            }
            else
            {
                input.UI.Confirm.canceled += this.OnEscapeCanceled;
            }
		}
		public override void Exit()
		{
			base.languageMenu.Hide();
			base.languageMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
			base.languageMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            if (PlatformManager.enterButtonParam == 1)
            {
                input.UI.Cancel.canceled -= this.OnEscapeCanceled;
            }
            else
            {
                input.UI.Confirm.canceled -= this.OnEscapeCanceled;
            }
		}
	}
}
