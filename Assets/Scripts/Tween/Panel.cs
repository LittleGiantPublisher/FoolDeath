using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using TMPro;

namespace F.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Panel : MonoBehaviour
    {
        public bool interactable
        {
            get => this.canvasGroup.interactable;
            set
            {
                this.canvasGroup.interactable = value;
                this.canvasGroup.blocksRaycasts = value;
            }
        }

        public void UpdateLayout()
        {
            var allLayouts = this.GetComponentsInChildren<LayoutGroup>(true);

            foreach (var layout in allLayouts)
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(layout.GetComponent<RectTransform>());
            }
        }

        protected virtual void Awake()
        {
            this.canvasGroup = base.GetComponent<CanvasGroup>();
            this.canvasGroup.interactable = true;
            this.canvasGroup.blocksRaycasts = false;
            base.StartCoroutine(this.DelayStartCR());
            SetTextPairs();
        }

        private void OnEnable()
        {
            InputSystem.onActionChange += OnInputAction;
            ControllerManager.OnInputChanged += OnInputChangedOrPressed;
            ControllerManager.OnAnyKeyPressed += OnInputChangedOrPressed;
        }

        private void OnDisable()
        {
            InputSystem.onActionChange -= OnInputAction;
            ControllerManager.OnInputChanged -= OnInputChangedOrPressed;
            ControllerManager.OnAnyKeyPressed -= OnInputChangedOrPressed;
        }

        private void OnInputChangedOrPressed()
        {         
            if (this.canvasGroup.blocksRaycasts && EventSystem.current.currentSelectedGameObject == null)
            {
                bool virtualCursor = CursorManager.Instance != null && CursorManager.Instance.IsUICursorActive;
                if (virtualCursor) return;
                SelectDefault();
            }        
        }

        private void OnInputAction(object action, InputActionChange change)
        {
            if (change == InputActionChange.ActionPerformed && this.canvasGroup.blocksRaycasts)
            {
                var inputAction = action as InputAction;
                if (inputAction != null && IsNavigationAction(inputAction) && (EventSystem.current.currentSelectedGameObject == null))
                {
                    SelectDefault();
                }
            }
        }

        private bool IsNavigationAction(InputAction inputAction)
        {
            return inputAction.name == "Navigate" ;
        } 

        public virtual void Show()
        {
            this.canvasGroup.interactable = true; 
            this.canvasGroup.blocksRaycasts = true;

            UITweener[] componentsInChildren = base.GetComponentsInChildren<UITweener>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].Show();
            }

            if (CursorManager.Instance != null && !CursorManager.Instance.IsAnyCursorActive)
            {
                SelectDefault();
            }
        }

        public virtual void Hide()
        {
            this.canvasGroup.interactable = false;
            this.canvasGroup.blocksRaycasts = false;

            UITweener[] componentsInChildren = base.GetComponentsInChildren<UITweener>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                componentsInChildren[i].Hide();
            }
        }

        public GameObject SelectDefault()
        {
            if (EventSystem.current.currentSelectedGameObject != null && !gamepadDefaultSelectable)
                return EventSystem.current.currentSelectedGameObject;
            
            if (gamepadDefaultSelectable != null)
            {
                gamepadDefaultSelectable.Select();
                return gamepadDefaultSelectable.gameObject;
            }
            if (this is Menu menu && menu.entries.Count > 0 && menu.entries[0] is ButtonExtension) 
            {
                var go = menu.entries[0].button.gameObject;
                EventSystem.current.SetSelectedGameObject(go);
                return go;
            }

            return null;
        }

        private IEnumerator DelayStartCR()
        {
            yield return null;
            UITweener[] componentsInChildren = base.GetComponentsInChildren<UITweener>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                if(dontStartHide){
					componentsInChildren[i].SetUp();
				}
				else{
					componentsInChildren[i].SetOff();
				}
            }

            yield break;
        }

        private void SetTextPairs()
        {
            foreach (var pair in stringTMPTextPairs)
            {
                if (pair.tmpText != null)
                {
                    LocalizationSystem.RegisterComponent(pair.tmpText, () => LocalizationSystem.GetLocalizedValue(pair.text));
                }
                else
                {
                    Debug.LogWarning("TMP_Text component is missing for a StringTMPTextPair.");
                }
            }
        }   

        [SerializeField]
		private bool dontStartHide = false;

        [Serializable]
        public class StringTMPTextPair
        {
            public string text;
            public TMP_Text tmpText;
        }

        [SerializeField]
        private Selectable gamepadDefaultSelectable;

        private CanvasGroup canvasGroup;

        [SerializeField]
        private List<StringTMPTextPair> stringTMPTextPairs;
    }
}
