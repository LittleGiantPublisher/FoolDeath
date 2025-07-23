using UnityEngine;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace F.UI{
    public abstract class UITweener : MonoBehaviour
    {
        private void Awake()
        {
            DOTween.Init();
        }

        public abstract void ShowIt();

        public abstract void HideIt();

        
        private void OnApplicationQuit(){
            DOTween.KillAll();
        }

        private void OnDestroy(){
            DOTween.KillAll();
        }

        
		private IEnumerator DelayShow()
		{
            yield return new WaitForSeconds(delay);
			ShowIt();
        }

        private IEnumerator DelayHide()
		{
            yield return new WaitForSeconds(delay);
			HideIt();
        }

        public void Show() {
            if (permission) {
                StartCoroutine(DelayShow());
            }
        }

        public void Hide() {
            if (permission) {
                StartCoroutine(DelayHide());
            }
        }

        public abstract void SetOff();

        public abstract void SetUp();

        public float duration = 0.1f;

        [SerializeField]
		private float delay = 0f;

        public bool permission = true;

    }
}    