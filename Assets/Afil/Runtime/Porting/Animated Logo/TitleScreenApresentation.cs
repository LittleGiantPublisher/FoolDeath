using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenApresentation : MonoBehaviour
{
    public CanvasGroup canvas;
    public List<TitleObeject> TitleImages;

    public bool apresentationIsFinish;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartTitleApresentation());
    }

    IEnumerator StartTitleApresentation()
    {
        for(int i=0; i< TitleImages.Count; i++)
        {
            if (!TitleImages[i].hasAnimation)
            {
                canvas.alpha = 0;
                TitleImages[i].titleObject.SetActive(true);
                while(canvas.alpha < 1)
                {
                    canvas.alpha += 0.02f;
                    yield return new WaitForEndOfFrame();
                }
                yield return new WaitForSeconds(TitleImages[i].timeForAnim);
                while (canvas.alpha > 0)
                {
                    canvas.alpha -= 0.02f;
                    yield return new WaitForEndOfFrame();
                }
                TitleImages[i].titleObject.SetActive(false);

            }
            else
            {
                canvas.alpha = 1;
                TitleImages[i].titleObject.SetActive(true);
                yield return new WaitForSeconds(TitleImages[i].timeForAnim);
            }
        }

        apresentationIsFinish = true;
    }
}

[System.Serializable]
public class TitleObeject
{
    public GameObject titleObject;
    public bool hasAnimation;
    public float timeForAnim = 2;

}
