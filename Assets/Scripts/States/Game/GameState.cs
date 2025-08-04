
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace F.State
{
    public abstract class GameState : State
    {
        protected InputPlayer input => this.owner.input;
//
        protected F.UI.Panel balancePanel => this.owner.balancePanel;

        protected F.UI.Panel deckHand => this.owner.deckHand;

        protected F.UI.Panel battleHand => this.owner.battleHand;

        protected F.UI.Panel deathHand => this.owner.deathHand;
        
        protected F.UI.Panel pauseBG => this.owner.pauseBG;
        
        protected F.UI.Panel mainBackground => this.owner.mainBackground;
        
        protected F.UI.Menu storeMenu => this.owner.storeMenu;

        protected F.UI.Menu languageMenu => this.owner.languageMenu;
        
        protected F.UI.Menu pauseMenu => this.owner.pauseMenu;

        protected F.UI.Menu optionsMenu => this.owner.optionsMenu;

        protected F.UI.Menu resultsMenu => this.owner.resultsMenu;

        protected F.UI.Menu messagePanel => this.owner.messagePanel;

        protected F.UI.Panel screenCover => this.owner.screenCover;

        protected F.UI.Panel karmaPanel => this.owner.karmaPanel;

        protected F.UI.Panel crossPanel => this.owner.crossPanel;

        protected F.UI.Panel HUD => this.owner.HUD;

        protected F.UI.Panel coinPanel => this.owner.coinPanel;
        
        protected F.UI.Panel curtainRight => this.owner.curtainRight;

        protected F.UI.Panel curtainLeft => this.owner.curtainLeft;

        protected F.KarmaController karmaController => this.owner.karmaController;
        
        protected F.Cards.CrossDecksController crossDecksController => this.owner.crossDecksController;

        protected F.UI.Menu pauseMenuButton => this.owner.pauseMenuButton;

        protected F.Cards.CardGenerator tarotGenerator => this.owner.tarotGenerator;

        protected AudioClip gameScreenMusic => this.owner.gameScreenMusic;
//      
        protected F.SoundEffectSO cameraMoveDown => this.owner.cameraMoveDown;

        protected F.SoundEffectSO cameraMoveUp => this.owner.cameraMoveUp;

        protected TMP_Text gamePoints => this.owner.gamePoints;

        protected TMP_Text gamePoints2 => this.owner.gamePoints2;

        protected TMP_Text round => this.owner.round;

        protected TMP_Text round2 => this.owner.round2;

        protected TMP_Text money => this.owner.money;

        protected TMP_Text moneyStore => this.owner.moneyStore;

        protected TMP_Text totalCoins => this.owner.totalCoins;

        protected F.UI.Panel moneyStorePanel => this.owner.moneyStorePanel;

        protected F.SoundEffectSO sfxButtonClick => this.owner.sfxButtonClick;


        protected F.Cards.CoinScriptable coinTest => this.owner.coinTest;
            
        private void Awake()
        {
            this.owner = base.GetComponentInParent<GameController>();
        }

        protected void Save()
        {
            SaveSystem.Save();
        }

        protected GameController owner;
    }
}