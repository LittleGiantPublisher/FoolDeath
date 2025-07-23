using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace F.State
{
	public class WaitLoadToPlay : TitleScreenState
	{
		public override void Enter()
		{
			base.StartCoroutine(this.WaitToLoadCR());
			if(base.screenCover != null)base.screenCover.Show();
			base.curtainLeft.Show();
            base.curtainRight.Show();
			AudioManager.Instance.FadeOutMusic(2f);
		}

		private IEnumerator WaitToLoadCR()
		{
			var loadTask = SceneController.ChangeScene("Game", transitionTime);

			yield return new WaitForSeconds(transitionTime);

			while (!loadTask.IsCompleted)
			{
				yield return null; // Wait for the task to complete
			}
		}

	}
}
