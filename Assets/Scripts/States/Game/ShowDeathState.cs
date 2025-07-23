using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using F.UI;

namespace F.State
{
	public class ShowDeathState : GameState
	{
		
		public void OnClickContinue(int i)
		{
			this.owner.ChangeState<PreGameStart>();
            sfxButtonClick.Play(null);

		}

		public override void Enter()
		{
			base.messagePanel.Show();
			base.StartCoroutine(this.WaitToShowDeath());
			AudioManager.Instance.SetLowPassFilter(true);
		}

		public override void Exit()
		{
			input.UI.Escape.canceled -= this.OnEscapeCanceled;
			base.messagePanel.Hide();
			AudioManager.Instance.SetLowPassFilter(false);
		}

		private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            OnCancel(); 
        }


		public void OnCancel()
		{
			this.owner.ChangeState<PreGameStart>();
		}

		private IEnumerator WaitToShowDeath()
		{
			base.messagePanel.onClick.AddListener(new UnityAction<int>(this.OnClickContinue));
			base.messagePanel.onCancel.AddListener(new UnityAction(this.OnCancel));
			input.UI.Escape.canceled += this.OnEscapeCanceled;
			yield return new WaitForSecondsRealtime(transitionTime);
			yield break;
		}
	}
}
