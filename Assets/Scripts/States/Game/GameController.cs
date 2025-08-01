
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace F.State
{
	public class GameController : StateMachine
	{
		private void Start()
		{
			this.ChangeState<InitGame>();
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

		public F.UI.Panel balancePanel;

        public F.UI.Panel deckHand;

        public F.UI.Panel coinHand; 

        public F.UI.Panel battleHand;

        public F.UI.Panel deathHand;

		public F.UI.Panel pauseBG;

		public F.UI.Panel mainBackground;

		public F.UI.Menu storeMenu;

		public F.UI.Menu languageMenu;

		public F.UI.Menu pauseMenu;

		public F.UI.Menu optionsMenu;

        public F.UI.Menu pauseMenuButton;

		public F.UI.Menu resultsMenu;

		public F.UI.Panel screenCover;

		public F.UI.Panel karmaPanel;

		public F.UI.Menu messagePanel;

		public F.UI.Panel crossPanel;
		
		public F.UI.Panel HUD;

		public F.UI.Panel coinPanel;

		public F.UI.Panel curtainRight;

		public F.UI.Panel curtainLeft;

		public F.KarmaController karmaController;

		public F.Cards.CrossDecksController crossDecksController;

		public F.Cards.CardGenerator tarotGenerator;

		public AudioClip gameScreenMusic;

		public SoundEffectSO cameraMoveUp;

		public SoundEffectSO cameraMoveDown;

        public TMP_Text gamePoints;

		public TMP_Text gamePoints2;

		public TMP_Text round;

		public TMP_Text round2;

		public TMP_Text money;

		public TMP_Text moneyStore;

		public TMP_Text totalCoins;

		public F.UI.Panel moneyStorePanel;

		public F.Cards.CoinScriptable coinTest;

		public F.SoundEffectSO sfxButtonClick;

	}
}
