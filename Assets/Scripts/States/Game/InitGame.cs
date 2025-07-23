using System;
using System.Collections;
using UnityEngine;

namespace F.State
{

	public class InitGame : GameState
	{
		public override void Enter()
		{
			base.StartCoroutine(this.WaitToShowCD());
			AudioManager.Instance.SetLowPassFilter(true);
			AudioManager.Instance.PlayMusic(base.gameScreenMusic);
			AudioManager.Instance.FadeInMusic(5f);
		}

		public override void Exit()
		{

		}

		private IEnumerator WaitToShowCD()
		{
			yield return new WaitForSecondsRealtime(transitionTime);
			if(base.screenCover != null)base.screenCover.Hide();
			base.curtainLeft.Hide();
            base.curtainRight.Hide();
			this.owner.ChangeState<ShowDeathState>();

		}

	}
}
