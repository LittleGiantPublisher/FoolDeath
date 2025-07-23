using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace F.UI
{
    public class TweenLoop : MonoBehaviour
    {
        private void OnEnable()
        {
            StartTweenCoroutines();
        }

        private void OnDisable()
        {
            StopTweenCoroutines();
        }

        private void OnDestroy()
        {
            StopTweenCoroutines();
            DOTween.KillAll();
        }

        private void OnApplicationQuit()
        {
            DOTween.KillAll();
        }

        private void StartTweenCoroutines()
        {
            foreach (var tweener in tweeners)
            {
                if (tweener != null)
                {
                    Coroutine coroutine = StartCoroutine(TweenRoutine(tweener));
                    activeCoroutines.Add(coroutine);
                }
            }
        }

        private void StopTweenCoroutines()
        {
            foreach (var coroutine in activeCoroutines)
            {
                if (coroutine != null)
                {
                    StopCoroutine(coroutine);
                }
            }
            activeCoroutines.Clear();
        }

        private IEnumerator TweenRoutine(UITweener tweener)
        {
            while (true)
            {
                if (tweener != null)
                {
                    tweener.ShowIt();
                }

                yield return new WaitForSeconds(tweener.duration);

                if (tweener != null)
                {
                    tweener.HideIt();
                }

                yield return new WaitForSeconds(tweener.duration);
            }
        }

        [SerializeField]
        private List<UITweener> tweeners = new List<UITweener>();

        private List<Coroutine> activeCoroutines = new List<Coroutine>();
    }
}
