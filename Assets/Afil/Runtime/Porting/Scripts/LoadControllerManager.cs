using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadControllerManager : MonoBehaviour
{
    // Start is called before the first frame update
    IEnumerator Start()
    {
        SaveCompatibility.LocalPlayerPrefs.DeserializeManual();

        yield return new WaitForSeconds(0.1f);
        SceneManager.LoadScene(2);
    }


}
