using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Porting;

public class TitlteScreenManager : MonoBehaviour
{
    public TMP_Text pressButton_txt;
    public GameObject MainMenu;
    bool goToMenu;
    // Start is called before the first frame update
    void Start()
    {

        Debug.LogError("macacheira: "+ControllerManager.firstInput);
        if (ControllerManager.firstInput)
        {
            goToMenu = true;
            MainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!ControllerManager.firstInput && !ControllerManager.startFindInput && !goToMenu)
        {
            ControllerManager.startFindInput = true;
#if UNITY_PS5
        pressButton_txt.text = "press <sprite=3> \r\n to start";
#elif UNITY_PS4
        if (Porting.PlatformManager.enterButtonParam == 0) 
        { 
            pressButton_txt.text = "press <sprite=5> \r\n to start";
        }
        else 
        {
            pressButton_txt.text = "press <sprite=1> \r\n to start";
        }

#elif UNITY_GAMECORE
            pressButton_txt.text = "press <sprite=0> \r\n to start";
#elif UNITY_STANDALONE || MICROSOFT_GAME_CORE
            pressButton_txt.text = "press <sprite=0> or \r\nenter to start";
#endif
        }
        if (ControllerManager.firstInput)
         { 
            goToMenu = true; 
            MainMenu.SetActive(true);
            gameObject.SetActive(false);
        }
    }
}
