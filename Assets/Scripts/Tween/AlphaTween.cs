using System;
using UnityEngine;
using DG.Tweening;

namespace F.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class AlphaTween : UITweener
    {
        private void Awake()
        {
            this.canvasGroup = base.GetComponent<CanvasGroup>();
        }

        public override void ShowIt()
        {
            if (this.canvasGroup != null)
            {
                this.canvasGroup.DOComplete(); // Cancel any ongoing animations
                this.canvasGroup.DOFade(1f, this.duration).SetUpdate(true); // SetUpdate(true) to ignore time scale
            }
        }

        public override void HideIt()
        {
            if (this.canvasGroup != null)
            {
                this.canvasGroup.DOComplete(); // Cancel any ongoing animations
                this.canvasGroup.DOFade(minimumAlpha, this.duration).SetUpdate(true); // SetUpdate(true) to ignore time scale
            }
        }

        public override void SetUp()
        {
            if (this.canvasGroup != null)
            {
                this.canvasGroup.alpha = 1;
            }
        }

        public override void SetOff()
        {
            if (this.canvasGroup != null)
            {
                this.canvasGroup.alpha = minimumAlpha;
            }
        }

        [SerializeField]
        private float minimumAlpha = 0f;

        private CanvasGroup canvasGroup;
    }
}
