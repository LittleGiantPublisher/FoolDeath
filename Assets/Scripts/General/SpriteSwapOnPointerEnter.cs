using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace F.UI
{
	public class SpriteSwapOnPointerEnter : MonoBehaviour, IPointerEnterHandler, IEventSystemHandler, IPointerExitHandler
	{
		public void Awake()
		{
			this._originalSprite = this.targetImage.sprite;
		}

		public void OnPointerEnter(PointerEventData pointerEventData)
		{
			this.targetImage.sprite = this.mouseOverSprite;
		}

		public void OnPointerExit(PointerEventData pointerEventData)
		{
			this.targetImage.sprite = this._originalSprite;
		}

		[SerializeField]
		private Image targetImage;

		[SerializeField]
		private Sprite mouseOverSprite;

		private Sprite _originalSprite;
	}
}