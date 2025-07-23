using UnityEngine;
using UnityEngine.EventSystems;

namespace F.Cards
{
    public class CoinCard : Card
    {
        public override void OnPointerUp(PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            
        }

        public override void OnPointerEnter(PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            mouseOverSFX.Play(null);
        }


        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);
        }

        public override void OnBeginDrag(PointerEventData eventData)
        {
            base.OnBeginDrag(eventData);
        }

        public override void OnEndDrag(PointerEventData eventData)
        {
            base.OnEndDrag(eventData);
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
        }

        public override void OnDrag(PointerEventData eventData)
        {
            base.OnDrag(eventData);

        }

        [SerializeField]
		private SoundEffectSO mouseOverSFX;

        [SerializeField]
		public SoundEffectSO moveSFX;
    }
}
