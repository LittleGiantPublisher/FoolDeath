using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisconnectedController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] GameObject PS4;
    [SerializeField] GameObject PS5;
    [SerializeField] GameObject XB1;
    [SerializeField] GameObject PC;


    bool needUpdate;
    private void Awake() {
        DontDestroyOnLoad(this);
    }
    void OnEnable()
    {
#if UNITY_PS5
        PS5.SetActive(true);
        PS4.SetActive(false);
        XB1.SetActive(false);
        PC.SetActive(false);
#elif UNITY_PS4
        PS4.SetActive(true);
        PS5.SetActive(false);
        XB1.SetActive(false);
        PC.SetActive(false);
#elif UNITY_GAMECORE
        XB1.SetActive(true);
        PS4.SetActive(false);
        PS5.SetActive(false);
        PC.SetActive(false);
#elif UNITY_STANDALONE || MICROSOFT_GAME_CORE
        PC.SetActive(true);
        PS4.SetActive(false);
        PS5.SetActive(false);
        XB1.SetActive(false);
#endif

        ControllerManager.OnControllerConnnect += HideDisconnectedApplet;
        ControllerManager.OnControllerDisconnnect += ShowDisconnectedApplet;

        //controllerDisconnectionText.text = LocalizationSystem.instance.LanguageGetString("ControllerDisconnected");
        //controllerReconnectionText.text = LocalizationSystem.instance.LanguageGetString("ControllerReconnection");
    }

    private void OnDisable()
    {
        ControllerManager.OnControllerConnnect -= HideDisconnectedApplet;
        ControllerManager.OnControllerDisconnnect -= ShowDisconnectedApplet;

    }

    private void Update()
    {
        if(canvasGroup.alpha>0 && needUpdate)
        {
            needUpdate = false;
            LoadTranslations();
        }
        else if(canvasGroup.alpha <= 0 && !needUpdate)
        {
            needUpdate = true;
        }
    }

    public void LoadTranslations()
    {
/*
        PS4_Title.text = TranslationManager.instance.LoadTranslationData("PS4ControllerDisconnection-Title", TranslationManager.instance.SelectedLanguage);
        PS4_Description.text = TranslationManager.instance.LoadTranslationData("PS4ControllerDisconnection-Description", TranslationManager.instance.SelectedLanguage);
        PS5_Title.text = TranslationManager.instance.LoadTranslationData("PS5ControllerDisconnection-Title", TranslationManager.instance.SelectedLanguage);
        PS5_Description.text = TranslationManager.instance.LoadTranslationData("PS5ControllerDisconnection-Description", TranslationManager.instance.SelectedLanguage);
        //xbox
        XB1_Title.text = TranslationManager.instance.LoadTranslationData("XB1ControllerDisconnection-Title", TranslationManager.instance.SelectedLanguage);
        XB1_Description.text = TranslationManager.instance.LoadTranslationData("XB1ControllerDisconnection-Description", TranslationManager.instance.SelectedLanguage);
        //pc
        PC_Title.text = TranslationManager.instance.LoadTranslationData("PCControllerDisconnection-Title", TranslationManager.instance.SelectedLanguage);
        PC_Description.text = TranslationManager.instance.LoadTranslationData("PCControllerDisconnection-Description", TranslationManager.instance.SelectedLanguage);
*/

    }

    void ShowDisconnectedApplet()
    {
        canvasGroup.alpha = 1;
        //GameObject.Find("GameManager")?.GetComponent<LevelManager>()?.Pause();

    }
    void HideDisconnectedApplet()
    {
        canvasGroup.alpha = 0;
    }
}
