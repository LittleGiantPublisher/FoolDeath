using System;
using System.Collections;
using UnityEngine.UI;
using F.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;
using Porting;

namespace F.State
{
    public class ResultState : GameState
    {
        [SerializeField] private Sprite victoryBackground;
        [SerializeField] private Sprite defeatBackground;
        [SerializeField] private Image backgroundImage;

        [SerializeField] private GameObject pointsImageObject;
        [SerializeField] private GameObject roundsImageObject;
        [SerializeField] private GameObject coinsImageObject;

        [SerializeField] private TMP_Text victoryText;
        [SerializeField] private TMP_Text defeatText;

        public void OnClick(int i)
        {
            switch (i)
            {
                case 0:
                    this.owner.ChangeState<WaitLoadToMenu>();
                    break;
            }

            sfxButtonClick.Play(null);
        }

        public void OnCancel()
        {
            this.owner.ChangeState<WaitLoadToMenu>();
        }

        private void OnEscapeCanceled(InputAction.CallbackContext context)
        {
            OnCancel();
        }

        public override void Enter()
        {
            AudioManager.Instance.SetLowPassFilter(true);
            base.StartCoroutine(this.WaitToShowMenuCR());

            base.pauseBG.Hide();

            SetActive(pointsImageObject, false);
            SetActive(roundsImageObject, false);
            SetActive(coinsImageObject, false);

            if (CombatStateStatus.GamePoints > 13000)
            {
                //SteamIntegration.UnlockAchievement("ACH_13K");

                backgroundImage.sprite = victoryBackground;
                victoryText.gameObject.SetActive(true);
                defeatText.gameObject.SetActive(false);
            }
            else
            {
                backgroundImage.sprite = defeatBackground;
                victoryText.gameObject.SetActive(false);
                defeatText.gameObject.SetActive(true);
            }

            SaveSystem.Load();
            if (CombatStateStatus.GamePoints > SaveSystem.data.points)
            {
                SaveSystem.data.points = CombatStateStatus.GamePoints;
                SetActive(pointsImageObject, true);
            }
            if (CombatStateStatus.Round > SaveSystem.data.rounds)
            {
                SaveSystem.data.rounds = CombatStateStatus.Round;
                SetActive(roundsImageObject, true);
            }
            if (CombatStateStatus.TotalCoins > SaveSystem.data.coins)
            {
                SaveSystem.data.coins = CombatStateStatus.TotalCoins;
                SetActive(coinsImageObject, true);
            }

            Save();
        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);

            base.resultsMenu.Show();
            base.resultsMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
            base.resultsMenu.onCancel.AddListener(new UnityAction(this.OnCancel));
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
            base.resultsMenu.Hide();
            base.resultsMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
            base.resultsMenu.onCancel.RemoveListener(new UnityAction(this.OnCancel));
            if (PlatformManager.enterButtonParam == 1)
            {
                input.UI.Cancel.canceled -= this.OnEscapeCanceled;
            }
            else
            {
                input.UI.Confirm.canceled -= this.OnEscapeCanceled;
            }
        }

        private void SetActive(GameObject obj, bool isActive)
        {
            if (obj != null)
            {
                obj.SetActive(isActive);
            }
        }
    }
}
