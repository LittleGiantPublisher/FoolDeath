using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.SocialPlatforms;
using UnityEngine.InputSystem;

namespace F.State
{
    public class PressAnyState : TitleScreenState
    {
        public void OnClick(int i)
        {
        }

        public override void Enter()
        {
#if UNITY_GAMECORE && !UNITY_EDITOR
    anyKeyText.text = LocalizationSystem.GetLocalizedValue("PRESS_ANY_XBOX");
#elif (UNITY_PS5 || UNITY_PS4) && !UNITY_EDITOR
    anyKeyText.text = LocalizationSystem.GetLocalizedValue("PRESS_ANY_PS");
#elif UNITY_SWITCH && !UNITY_EDITOR
    anyKeyText.text = LocalizationSystem.GetLocalizedValue("PRESS_ANY_SWITCH");
#else
    anyKeyText.text = LocalizationSystem.GetLocalizedValue("PRESS_ANY_MS");
#endif
            base.StartCoroutine(this.WaitToShowMenuCR());
            ControllerManager.current.inputPlayerActions.UI.Confirm.started += TransitionToMain;
        }

        public override void Exit()
        {
            base.anyKeyMenu.Hide();
            ControllerManager.current.inputPlayerActions.UI.Confirm.started -= TransitionToMain;
            //base.mainMenu.onClick.RemoveListener(new UnityAction<int>(this.OnClick));
        }

        private IEnumerator WaitToShowMenuCR()
        {
            yield return new WaitForSeconds(transitionTime);
            base.characterPanel.Show();
            base.logoPanel.Show();
            base.anyKeyMenu.Show();

            //base.mainMenu.onClick.AddListener(new UnityAction<int>(this.OnClick));
        }

        private void TransitionToMain(InputAction.CallbackContext context)
        {
            Debug.Log("TransitionToMain");
            this.owner.ChangeState<TitleMainMenuState>();
        }
    }
}