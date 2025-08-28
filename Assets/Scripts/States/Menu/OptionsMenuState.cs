using System;
using System.Collections;
using F.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using Porting;
using SaveCompatibility;

namespace F.State
{
	public class OptionsMenuState : TitleScreenState
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
                    this.owner.ChangeState<TitleMainMenuState>();
                    break;       
            }

            sfxButtonClick.Play(null);

            base.optionsMenu.GetComponent<Menu>().UpdateLayout();

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
            base.optionsMenu.GetComponent<OptionsSetter>().Refresh();
			base.optionsMenu.Show();
			base.optionsMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
			base.optionsMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
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
			base.optionsMenu.Hide();
			base.optionsMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
			base.optionsMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            LocalPlayerPrefs.Save();
            
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
