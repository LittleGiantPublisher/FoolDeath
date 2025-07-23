using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F.UI
{
    public class Menu : Panel
    {
        [SerializeField]
        internal protected List<ButtonExtension> entries;

        public UnityEvent onCancel;
        public IntEvent    onSelect;
        public IntEvent    onClick;
        public IntEvent    onHover;

        protected int      index;
        protected bool     isActive;
        private   EventSystem eventSystem;
        private   GameObject  selected;

        public override void Show()
        {
            base.Show();
            isActive = true;

            bool virtualCursor = CursorManager.Instance != null && CursorManager.Instance.IsUICursorActive;

            eventSystem = FindObjectOfType<EventSystem>();

            // only auto-select if NOT using the virtual cursor
            if (!virtualCursor && eventSystem != null && entries != null && entries.Count > 0)
            {
                var firstGO = SelectDefault();
                //eventSystem.SetSelectedGameObject(firstGO);
                //eventSystem.firstSelectedGameObject = firstGO;
                selected = firstGO;

                for (int i = 0; i < entries.Count; i++)
                    AddEventTrigger(entries[i].button, i);
            }
            else if (entries != null)
            {
                // still add hover triggers even if virtual cursor is active
                for (int i = 0; i < entries.Count; i++)
                    AddEventTrigger(entries[i].button, i);
            }

            // only resume highlight if NOT virtual
            if (!virtualCursor && entries.Count > 0 && entries[index].gameObject.activeSelf)
                entries[index].button.Select();

            // hook up select, click, cancel regardless
            for (int i = 0; i < entries.Count; i++)
            {
                int c = i;
                entries[c].onSelect.AddListener(() => OnSelect(c));
                entries[c].onClick.AddListener(() => OnClick(c));
                entries[c].onCancel.AddListener(OnCancel);
            }

            UpdateLayout();
        }

        public override void Hide()
        {
            base.Hide();
            isActive = false;

            foreach (var e in entries)
            {
                e.onSelect.RemoveAllListeners();
                e.onClick.RemoveAllListeners();
                e.onCancel.RemoveAllListeners();
            }
        }

        private void AddEventTrigger(Button btn, int idx)
        {
            var trigger = btn.gameObject.GetComponent<EventTrigger>()
                          ?? btn.gameObject.AddComponent<EventTrigger>();

            trigger.triggers.RemoveAll(entry =>
                entry.eventID == EventTriggerType.PointerEnter ||
                entry.eventID == EventTriggerType.PointerExit);

            var enter = new EventTrigger.Entry { eventID = EventTriggerType.PointerEnter };
            enter.callback.AddListener((_) =>
            {
                OnButtonHover(btn);
                OnHover(idx);
            });
            trigger.triggers.Add(enter);

            var exit = new EventTrigger.Entry { eventID = EventTriggerType.PointerExit };
            exit.callback.AddListener((_) => OnButtonExit(btn));
            trigger.triggers.Add(exit);
        }

        public void OnButtonHover(Button btn)
        {
            var es = EventSystem.current;
            if (es != null && es.currentSelectedGameObject != btn.gameObject)
            {
                es.SetSelectedGameObject(btn.gameObject);
                selected = btn.gameObject;
            }
        }

        public void OnButtonExit(Button btn)
        {

            var es = EventSystem.current;
            if (es != null && es.currentSelectedGameObject == btn.gameObject)
            {
                es.SetSelectedGameObject(null);
                selected = null;
            }
        }

        protected void OnCancel()
        {
            onCancel?.Invoke();
        }

        protected virtual void OnSelect(int idx)
        {
            index = idx;
            onSelect?.Invoke(idx);
            onHover?.Invoke(idx);
        }

        protected virtual void OnHover(int idx)
        {
            onHover?.Invoke(idx);
        }

        protected virtual void OnClick(int idx)
        {
            onClick?.Invoke(idx);
        }
    }
}
