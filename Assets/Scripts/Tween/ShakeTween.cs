using UnityEngine;
using DG.Tweening;

namespace F.UI
{
    public class ShakeTween : UITweener
    {
        private Tween shakePositionTween;
        private Tween shakeRotationTween;

        private void Awake(){
            SetOff();
        }

        public override void ShowIt()
        {
            if (base.transform != null)
            {
                base.transform.DOComplete(); // Cancel any ongoing animations
                StartShaking();
            }
        }

        public override void HideIt()
        {
            if (base.transform != null)
            {
                base.transform.DOComplete(); // Cancel any ongoing animations
                StopShaking();
            }
        }

        public override void SetUp()
        {
            ShowIt();
        }

        public override void SetOff()
        {
            HideIt();
        }

        private void StartShaking()
        {
            if (base.transform != null)
            {
                // Check to ensure shakeDuration is positive
                if (shakeDuration <= 0)
                {
                    shakeDuration = 1f; // Default to 1 second if shakeDuration is not valid
                }
                
                // Start shaking position indefinitely in 2D
                shakePositionTween = base.transform.DOShakePosition(shakeDuration, new Vector3(shakeStrength, shakeStrength, 0), vibrato, randomness)
                    .SetEase(easeType)
                    .SetUpdate(true)
                    .OnKill(() => shakePositionTween = null); // Reset the tween when it is killed

                

                // Start shaking rotation indefinitely
                shakeRotationTween = base.transform.DOShakeRotation(shakeDuration, new Vector3(0, 0, rotationShakeStrength), vibrato, randomness)
                    .SetEase(easeType)
                    .SetUpdate(true)
                    .OnKill(() => shakeRotationTween = null); // Reset the tween when it is killed
            }
        }

        private void StopShaking()
        {
            // Kill the position shake tween if it's active
            if (shakePositionTween != null && shakePositionTween.IsActive())
            {
                shakePositionTween.Kill();
            }

            // Kill the rotation shake tween if it's active
            if (shakeRotationTween != null && shakeRotationTween.IsActive())
            {
                shakeRotationTween.Kill();
            }
        }

        private void OnDestroy()
        {
            // Ensure all tweens are killed when the object is destroyed
            StopShaking();
        }

        [SerializeField]
        private float shakeStrength = 0.5f; // Reduced shake strength

        [SerializeField]
        private float rotationShakeStrength = 5f; // Reduced rotation shake strength

        [SerializeField]
        private int vibrato = 20; // Increased vibrato for smoother shake

        [SerializeField]
        private Ease easeType = Ease.InOutSine; // Smooth easing

        [SerializeField]
        private float randomness = 10f; // Reduced randomness for smoother shake

        [SerializeField]
        private float shakeDuration = 2f; // Increased duration for smoother shake
    }
}
