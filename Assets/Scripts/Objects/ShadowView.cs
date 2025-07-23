using UnityEngine;
using UnityEngine.UI;

namespace F.UI
{
    [RequireComponent(typeof(Canvas))]
    public class ShadowView : MonoBehaviour
    {
        public Transform parentObject;
        public bool isActive = false;
        public float distanceY = 0.1f;
        public float distanceYMult = 2f;
        public float positionXMult = 0.5f;
        public float positionYMult = 0.25f;
        private Canvas shadowCanvas;
        public Image shadowImage;
        private Transform myTransform;
        private Camera mainCamera;

        private void Awake()
        {
            myTransform = transform;
            shadowCanvas = GetComponent<Canvas>();
            shadowCanvas.overrideSorting = false;
            shadowImage = GetComponent<Image>();
            mainCamera = Camera.main;

            if (parentObject != null)
            {
                SetParentObject(parentObject);
            }
        }

        private void Update()
        {

            if (parentObject == null) return;


            Vector3 parentPosition = parentObject.position;
            Vector3 screenPos = mainCamera.WorldToViewportPoint(parentPosition);
            float adjustedX = (screenPos.x - 0.5f) * 2 * positionXMult;
            float adjustedY = (screenPos.y - 0.5f) * 2 * positionYMult;

            if (isActive)
            {
                shadowCanvas.overrideSorting = false;
                myTransform.position = new Vector3(parentPosition.x - adjustedX, parentPosition.y - distanceY * distanceYMult - adjustedY, parentPosition.z);
                myTransform.rotation = Quaternion.Euler(0, 0, parentObject.rotation.eulerAngles.z);
            }
            else
            {
                shadowCanvas.overrideSorting = false;
                myTransform.position = new Vector3(parentPosition.x, parentPosition.y - distanceY, parentPosition.z);
                myTransform.rotation = Quaternion.Euler(0, 0, parentObject.rotation.eulerAngles.z); 
            }
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }

        public void SetParentObject(Transform parent)
        {
            parentObject = parent;
            Image parentImage = parentObject.GetComponentInChildren<Image>();

            if (parentImage != null && shadowImage != null)
            {
                shadowImage.sprite = parentImage.sprite;
                shadowImage.material = parentImage.material;
                shadowImage.type = parentImage.type;
                shadowImage.preserveAspect = parentImage.preserveAspect;
                shadowImage.fillCenter = parentImage.fillCenter;
                shadowImage.fillAmount = parentImage.fillAmount;
                shadowImage.fillClockwise = parentImage.fillClockwise;
                shadowImage.fillMethod = parentImage.fillMethod;
                shadowImage.fillOrigin = parentImage.fillOrigin;
            }
        }
    }
}
