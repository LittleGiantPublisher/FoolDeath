using System;
using UnityEngine;
using DG.Tweening;

namespace F.UI{
    [RequireComponent(typeof(RectTransform))]
    public class MoveTween : UITweener
    {
        private void Awake()
        {
            switch (this.entranceFrom)
            {
                case MoveTween.Direction.Up: 
                    this.moveVector = new Vector3(0f, this.moveAmount, 0f);
                    break;
                case MoveTween.Direction.Down:
                    this.moveVector = new Vector3(0f, -1f * this.moveAmount, 0f);
                    break;
                case MoveTween.Direction.Left:
                    this.moveVector = new Vector3(-1f * this.moveAmount, 0f, 0f);
                    break;
                case MoveTween.Direction.Right:
                    this.moveVector = new Vector3(this.moveAmount, 0f, 0f);
                    break;
            }
            
            GetRect();
            if (this.rectTransform == null){
                Debug.LogError("RectTransform is not assigned.");
                return;
            }

            this.startPosition = this.rectTransform.anchoredPosition;
            //SetOff();
        }

        public override void ShowIt()
        {
            GetRect();
            if (this.rectTransform == null){
                Debug.LogError("RectTransform is not assigned.");
                return;
            }

            //this.rectTransform.DOComplete(); // Complete any ongoing animation
            this.rectTransform.DOAnchorPos(this.startPosition, this.duration)
                .SetEase(this.easeType)
                .SetUpdate(true); // Ignore time scale
        }

        public override void HideIt()
        {
            GetRect();
            if (this.rectTransform == null){
                Debug.LogError("RectTransform is not assigned.");
                return;
            }

            //this.rectTransform.DOComplete(); // Complete any ongoing animation
            this.rectTransform.DOAnchorPos(this.startPosition + this.moveVector, this.duration)
                .SetUpdate(true); // Ignore time scale
        }

        public override void SetUp()
        {
            GetRect();
            if (this.rectTransform == null){
                Debug.LogError("RectTransform is not assigned.");
                return;
            }

            this.rectTransform.DOAnchorPos(this.startPosition, 0f)
                .SetUpdate(true); // Ignore time scale
        }

        public override void SetOff()
        {
            GetRect();
            if (this.rectTransform == null){
                Debug.LogError("RectTransform is not assigned.");
                return;
            }

            this.rectTransform.DOAnchorPos(this.startPosition + this.moveVector, 0f)
                .SetUpdate(true); // Ignore time scale
        }

        private void GetRect(){
            if (this.rectTransform == null){
                this.rectTransform = GetComponent<RectTransform>();
            }
            
        }

        [SerializeField]
        private Ease easeType = Ease.Linear;

        public MoveTween.Direction entranceFrom;
        public float moveAmount;
        private Vector3 moveVector;
        private Vector3 startPosition;
        private RectTransform rectTransform;

        public enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
    }
}