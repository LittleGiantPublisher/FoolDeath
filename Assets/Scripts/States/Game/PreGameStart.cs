using System;
using System.Collections;
using UnityEngine;

namespace F.State
{

	public class PreGameStart : GameState
	{
		public override void Enter()
		{
            base.mainBackground.Show();
			cameraMoveDown.Play(null);
			base.StartCoroutine(this.WaitToShow());
		}

		public override void Exit()
		{

		}

		private IEnumerator WaitToShow()
		{
			yield return new WaitForSecondsRealtime(transitionTime);
			this.owner.ChangeState<CombatState>();
			yield break;
		}

	}
}

