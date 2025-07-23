using System;
using UnityEngine;
using DG.Tweening;

namespace F.UI{
    public class RotationTween : UITweener
    {
        private void Awake()
        {
            if (base.transform != null)
            {
                base.transform.localEulerAngles = startRotation;
            }
            SetOff();
        }

        public override void ShowIt()
        {
            if (base.transform != null)
            {
                //base.transform.DOComplete(); // Cancel any ongoing animations
                base.transform.DOLocalRotate(targetRotation, this.duration)
                    .SetEase(this.easeType).SetUpdate(true); // Animate rotation
            }
        }

        public override void HideIt()
        {
            if (base.transform != null)
            {
                //base.transform.DOComplete(); // Cancel any ongoing animations
                base.transform.DOLocalRotate(startRotation, this.duration)
                    .SetEase(this.easeType).SetUpdate(true); // Animate rotation
            }
        }

        public override void SetUp()
        {
            if(startZeroRotation){
                base.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else{
               this.ShowIt(); 
            }
            
        }

        public override void SetOff()
        {
            if(startZeroRotation){
                base.transform.localEulerAngles = new Vector3(0, 0, 0);
            }
            else{
               this.HideIt(); 
            }
            
        }

        [SerializeField]
        private bool startZeroRotation = true;

        [SerializeField]
        private Ease easeType = Ease.Linear;

        [SerializeField]
        private Vector3 startRotation = Vector3.zero;

        [SerializeField]
        private Vector3 targetRotation = Vector3.zero;
    }
}
