using System;
using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.EventSystems;
using Unity.Collections;
using UnityEngine.UI;
using Unity.VisualScripting;
using F.UI;
using TMPro;

namespace F.Cards
{
    public class CardVisual : MonoBehaviour
    {
        private bool initialize = false;

        [Header("Card")]
        public Card parentCard;
        private Transform cardTransform;
        private Vector3 rotationDelta;
        private int savedIndex;
        Vector3 movementDelta;
        private Canvas canvas;

        [Header("References")]
        public Transform visualShadow;
        private float shadowOffset = 20;
        private Vector2 shadowDistance;
        public ShadowView shadowCanvas;
        [SerializeField] private Transform shakeParent;
        [SerializeField] private Transform tiltParent;
        [SerializeField] public Image cardImage;
        [SerializeField] public Image cardSuit;
        
        [Header("Follow Parameters")]
        [SerializeField] private float followSpeed = 30;

        [Header("Rotation Parameters")]
        [SerializeField] private float rotationAmount = 20;
        [SerializeField] private float rotationSpeed = 20;
        [SerializeField] private float autoTiltAmount = 30;
        [SerializeField] private float manualTiltAmount = 20;
        [SerializeField] private float maxManualTilt = 1;
        [SerializeField] private float tiltSpeed = 20;

        [Header("Scale Parameters")]
        [SerializeField] private bool scaleAnimations = true;
        [SerializeField] private float scaleOnHover = 1.15f;
        [SerializeField] private float scaleOnSelect = 1.25f;
        [SerializeField] private float scaleTransition = .15f;
        [SerializeField] private Ease scaleEase = Ease.OutBack;

        [Header("Select Parameters")]
        [SerializeField] private float selectPunchAmount = 20;

        [Header("Hover Parameters")]
        [SerializeField] private float hoverPunchAngle = 5;
        [SerializeField] private float hoverTransition = .15f;

        [Header("Swap Parameters")]
        [SerializeField] private bool swapAnimations = true;
        [SerializeField] private float swapRotationAngle = 30;
        [SerializeField] private float swapTransition = .15f;
        [SerializeField] private int swapVibrato = 5;

        [Header("Curve")]
        [SerializeField] private CurveParameters curve;

        [Header("Outline")]
        [SerializeField] private GameObject selectionOutline; 

        private float curveYOffset;
        private float curveRotationOffset;
        private Coroutine pressCoroutine;

        public bool isStatic = false;

        public void SetInDeckState(bool isStatic)
        {

            this.isStatic = isStatic;
        }

        private void Start()
        {
            shadowDistance = visualShadow.localPosition;
        }

        //called by Card.c
        public void AttachSpecs(Sprite bgImg){
            this.cardImage.sprite = bgImg;
            this.visualShadow.GetComponent<Image>().sprite = bgImg;

        }

        public virtual void ReInitialize(){
            canvas.overrideSorting = false;

        }

        public virtual void Initialize(Card target, int index = 0)
        {
            // Declarations
            parentCard = target;
            cardTransform = target.transform;
            canvas = GetComponent<Canvas>();
            shadowCanvas = visualShadow.GetComponent<ShadowView>();

            // Event Listening
            parentCard.PointerEnterEvent.AddListener(PointerEnter);
            parentCard.PointerExitEvent.AddListener(PointerExit);
            parentCard.BeginDragEvent.AddListener(BeginDrag);
            parentCard.EndDragEvent.AddListener(EndDrag);
            parentCard.PointerDownEvent.AddListener(PointerDown);
            parentCard.PointerUpEvent.AddListener(PointerUp);
            parentCard.SelectEvent.AddListener(Select);

            canvas.overrideSorting = false;
            shadowCanvas.SetActive(false);

            // Initialization
            initialize = true;

            SetOutline(false);
        }

        public void SetOutline(bool state){
            if (selectionOutline != null)
                selectionOutline.SetActive(state);
        }

        public void UpdateIndex(int length)
        {
            transform.SetSiblingIndex(parentCard.transform.parent.GetSiblingIndex());
        }

        void Update()
        {
            if (!gameObject.activeSelf || !initialize || parentCard == null) return;

            HandPositioning();
            SmoothFollow();
            FollowRotation();
            CardTilt();
        }

        private void HandPositioning()
        {
            curveYOffset = (curve.positioning.Evaluate(parentCard.NormalizedPosition()) * curve.positioningInfluence) * parentCard.SiblingAmount();
            curveYOffset = parentCard.SiblingAmount() < 5 ? 0 : curveYOffset;
            curveRotationOffset = curve.rotation.Evaluate(parentCard.NormalizedPosition());
        }

        private void SmoothFollow()
        {
            Vector3 verticalOffset = (Vector3.up * (parentCard.isDragging ? 0 : curveYOffset));
            transform.position = Vector3.Lerp(transform.position, cardTransform.position + verticalOffset, followSpeed * Time.unscaledDeltaTime);
        }

        private void FollowRotation()
        {
            Vector3 movement = (transform.position - cardTransform.position);
            movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.unscaledDeltaTime);
            Vector3 movementRotation = (parentCard.isDragging ? movementDelta : movement) * rotationAmount;
            rotationDelta = Vector3.Lerp(rotationDelta, movementRotation, rotationSpeed * Time.unscaledDeltaTime);
            transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60));
        }

        private void CardTilt()
        {
            if(isStatic){
                tiltParent.eulerAngles = new Vector3(0, 0, 0);
                return;
            }

            savedIndex = parentCard.isDragging ? savedIndex : parentCard.ParentIndex();

            float sine = Mathf.Sin(Time.unscaledTime + savedIndex) * (parentCard.isHovering ? .2f : 1);
            float cosine = Mathf.Cos(Time.unscaledTime + savedIndex) * (parentCard.isHovering ? .2f : 1);

            Vector3 offset = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);

            offset.x = Mathf.Clamp(offset.x, -maxManualTilt, maxManualTilt);
            offset.y = Mathf.Clamp(offset.y, -maxManualTilt, maxManualTilt);
            offset.z = Mathf.Clamp(offset.z, -maxManualTilt, maxManualTilt);
            float tiltX = parentCard.isHovering ? ((offset.y * -1) * manualTiltAmount) : 0;
            float tiltY = parentCard.isHovering ? ((offset.x * 1) * manualTiltAmount) : 0;
            float tiltZ = parentCard.isDragging ? tiltParent.eulerAngles.z : (curveRotationOffset * (curve.rotationInfluence * (parentCard.SiblingAmount() + 1)));

            if (float.IsNaN(tiltZ))
            {
                tiltZ = 0f;
            }

            float lerpX = Mathf.LerpAngle(tiltParent.eulerAngles.x, tiltX + (sine * autoTiltAmount), tiltSpeed * Time.unscaledDeltaTime);
            float lerpY = Mathf.LerpAngle(tiltParent.eulerAngles.y, tiltY + (cosine * autoTiltAmount), tiltSpeed * Time.unscaledDeltaTime);
            float lerpZ = Mathf.LerpAngle(tiltParent.eulerAngles.z, tiltZ, tiltSpeed / 2 * Time.unscaledDeltaTime);

            tiltParent.eulerAngles = new Vector3(lerpX, lerpY, lerpZ);
        }


        public virtual void Select(Card card, bool state)
        {
            DOTween.Kill(2, true);
            float dir = state ? 1 : 0;
            shakeParent.DOPunchPosition(shakeParent.up * selectPunchAmount * dir, scaleTransition, 10, 1).SetUpdate(true);
            shakeParent.DOPunchRotation(Vector3.forward * (hoverPunchAngle / 2), hoverTransition, 20, 1).SetId(2).SetUpdate(true);

            if (scaleAnimations)
                transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase).SetUpdate(true);
        }

        public virtual void Swap(float dir = 1)
        {
            if (!swapAnimations)
                return;

            DOTween.Kill(2, true);
            shakeParent.DOPunchRotation((Vector3.forward * swapRotationAngle) * dir, swapTransition, swapVibrato, 1).SetId(3).SetUpdate(true);
        }

        public virtual void BeginDrag(Card card)
        {
            shadowCanvas.SetActive(true);
            if (scaleAnimations)
                transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase).SetUpdate(true);

            canvas.overrideSorting = true;
        }

        public virtual void EndDrag(Card card)
        {
            shadowCanvas.SetActive(false);
            canvas.overrideSorting = parentCard.selected;

            transform.DOScale(1, scaleTransition).SetEase(scaleEase).SetUpdate(true);
        }

        public virtual void PointerEnter(Card card)
        {
            
            if (scaleAnimations)
                transform.DOScale(scaleOnHover, scaleTransition).SetEase(scaleEase).SetUpdate(true);

            DOTween.Kill(2, true);
            shakeParent.DOPunchRotation(Vector3.forward * hoverPunchAngle, hoverTransition, 20, 1).SetId(2).SetUpdate(true);
        }

        public virtual void PointerExit(Card card)
        {
            if (!parentCard.wasDragged)
                transform.DOScale(1, scaleTransition).SetEase(scaleEase).SetUpdate(true);
        }

        public virtual void PointerUp(Card card, bool longPress)
        {
            if (scaleAnimations)
                transform.DOScale(longPress ? scaleOnHover : scaleOnSelect, scaleTransition).SetEase(scaleEase).SetUpdate(true);

            visualShadow.localPosition = shadowDistance;
            shadowCanvas.SetActive(false);

            if (longPress  || parentCard.wasDragged)
                return;
            
            canvas.overrideSorting = !parentCard.selected;


        }

        public virtual void PointerDown(Card card)
        {
            if (scaleAnimations)
                transform.DOScale(scaleOnSelect, scaleTransition).SetEase(scaleEase).SetUpdate(true);

            visualShadow.localPosition += (-Vector3.up * shadowOffset);
            shadowCanvas.SetActive(true);
        }

    }
}
