using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace F.State
{
	public class TitleScreenController : StateMachine
	{
		private void Start()
		{
			this.ChangeState<InitState>();
		}

		private void Awake(){
			input = new FDPlayerInput();
			input.Enable();
		}

		private void OnDisable()
		{
			input.Disable();
		}

		public FDPlayerInput input;

		public F.UI.Panel logoPanel;

		public F.UI.Menu mainMenu;

		public F.UI.Menu languageMenu;

		public F.UI.Menu optionsMenu;

		public F.UI.Panel curtainRight;

		public F.UI.Panel curtainLeft;

		public F.UI.Panel screenCover;

		public F.UI.Panel recordsPanel;

		public AudioClip titleScreenMusic;

		public F.SoundEffectSO sfxButtonClick;

        public TMP_Text menuPoints;

		public TMP_Text menuRounds;

		public TMP_Text menuCoins;
	}
}
