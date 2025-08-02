using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AfilLogoManager : MonoBehaviour
{
    public TitleScreenApresentation ScreenApresentation;
    AsyncOperation asyncLoad;

    public void InitLoading()
    {
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        yield return new WaitForSeconds(1f);
        while (!Porting.PlatformManager.instance.initializeFinished)
            yield return null;
        yield return Porting.PlatformManager.instance.StartCoroutine(Porting.PlatformManager.instance.WaitingLoadFinish());
        
        while(!ScreenApresentation.apresentationIsFinish)
            yield return null;
        Debug.Log("Finish wait");
        Porting.PlatformManager.instance.UpdateActivity("activityStart");
        //           while (!asyncLoad.isDone)
        //          yield return null;
        //     asyncLoad.allowSceneActivation = true;
        SceneManager.LoadScene("LoadControlleManager");
    }
}