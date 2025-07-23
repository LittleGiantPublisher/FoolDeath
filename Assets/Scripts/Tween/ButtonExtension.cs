using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F.UI
{
	[RequireComponent(typeof(Button))]
	public class ButtonExtension : MonoBehaviour, ISelectHandler, IEventSystemHandler, IDeselectHandler, IPointerEnterHandler, IPointerExitHandler, ICancelHandler
	{
		public Button button { get; private set; }

		private void Awake()
		{
			this.button = base.GetComponent<Button>();
			this.button.onClick.AddListener(new UnityAction(this.OnClick));
		}

		private void OnDestroy()
		{
			this.button.onClick.RemoveListener(new UnityAction(this.OnClick));
		}

		public void OnClick()
		{
			UnityEvent unityEvent = this.onClick;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		public void OnSelect(BaseEventData eventData)
		{
			UnityEvent unityEvent = this.onSelect;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		public void OnDeselect(BaseEventData eventData)
		{
			UnityEvent unityEvent = this.onDeselect;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		public void OnPointerEnter(PointerEventData eventData)
		{
			UnityEvent unityEvent = this.onPointerEnter;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		public void OnPointerExit(PointerEventData eventData)
		{
			UnityEvent unityEvent = this.onPointerExit;
			if (unityEvent == null)
			{
				return;
			}
			unityEvent.Invoke();
		}

		public void OnCancel(BaseEventData eventData)
		{
			this.onCancel.Invoke();
		}

		public UnityEvent onClick;
		public UnityEvent onSelect;
		public UnityEvent onDeselect;
		public UnityEvent onPointerEnter;
		public UnityEvent onPointerExit;
		public UnityEvent onCancel;
	}
}
