using System;
using UnityEngine;
using DG.Tweening;

namespace F.UI{
    public class ScaleTween : UITweener
    {
        private void Awake()
        {
            if (base.transform != null)
            {
                base.transform.localScale = startScale;
            }
            SetOff();
        }

        public override void ShowIt()
        {
            if (base.transform != null)
            {
                //base.transform.DOComplete(); // Cancel any ongoing animations
                base.transform.DOScale(targetScale, this.duration)
                    .SetEase(this.easeType).SetUpdate(true); // Animate scale
            }
        }

        public override void HideIt()
        {
            if (base.transform != null)
            {
                //base.transform.DOComplete(); // Cancel any ongoing animations
                base.transform.DOScale(startScale, this.duration)
                    .SetEase(this.easeType).SetUpdate(true); // Animate scale
            }
        }

        public override void SetUp()
        {
            this.ShowIt();
        }

        public override void SetOff()
        {
            this.HideIt();
        }

        [SerializeField]
        private Ease easeType = Ease.Linear;

        [SerializeField]
        private Vector3 startScale = Vector3.one;

        [SerializeField]
        private Vector3 targetScale = Vector3.one;
    }
}
