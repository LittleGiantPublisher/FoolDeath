using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

namespace F.State
{
    public abstract class TitleScreenState : State
    {
        protected InputPlayer input => this.owner.input;
//
        protected F.UI.Panel logoPanel => this.owner.logoPanel;

        protected F.UI.Menu mainMenu => this.owner.mainMenu;

        protected F.UI.Menu languageMenu => this.owner.languageMenu;
        
        protected F.UI.Menu optionsMenu => this.owner.optionsMenu;
//
        protected F.UI.Panel screenCover => this.owner.screenCover;

        protected F.UI.Panel curtainRight => this.owner.curtainRight;

        protected F.UI.Panel curtainLeft => this.owner.curtainLeft;

        protected F.UI.Panel recordsPanel => this.owner.recordsPanel;

        protected AudioClip titleScreenMusic => this.owner.titleScreenMusic;
//             
        protected TMP_Text menuPoints => this.owner.menuPoints;

        protected TMP_Text menuRounds => this.owner.menuRounds;

        protected TMP_Text menuCoins => this.owner.menuCoins;

        protected F.SoundEffectSO sfxButtonClick => this.owner.sfxButtonClick;


        private void Awake()
        {
            this.owner = base.GetComponentInParent<TitleScreenController>();
        }

        protected void Save()
        {
            SaveSystem.Save();
        }

        protected TitleScreenController owner;
    }
}