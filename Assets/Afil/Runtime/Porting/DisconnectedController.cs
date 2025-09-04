using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using F;

public class DisconnectedController : MonoBehaviour
{
    [SerializeField] CanvasGroup canvasGroup;
    [SerializeField] GameObject PS4;
    [SerializeField] GameObject PS5;
    [SerializeField] GameObject XB1;
    [SerializeField] GameObject PC;

    [SerializeField] private TextMeshProUGUI PS4_Title;
    [SerializeField] private TextMeshProUGUI PS4_Description;

    [SerializeField] private TextMeshProUGUI PS5_Title;
    [SerializeField] private TextMeshProUGUI PS5_Description;

    [SerializeField] private TextMeshProUGUI XB_Title;
    [SerializeField] private TextMeshProUGUI XB_Description;

    [SerializeField] private TextMeshProUGUI PC_Title;
    [SerializeField] private TextMeshProUGUI PC_Description;

    bool needUpdate;
    private void Awake() {
        DontDestroyOnLoad(this);
    }
    void OnEnable()
    {
#if UNITY_PS5 && !UNITY_EDITOR
        PS5.SetActive(true);
        PS4.SetActive(false);
        XB1.SetActive(false);
        PC.SetActive(false);
#elif UNITY_PS4 && !UNITY_EDITOR
        PS4.SetActive(true);
        PS5.SetActive(false);
        XB1.SetActive(false);
        PC.SetActive(false);
#elif UNITY_GAMECORE && !UNITY_EDITOR
        XB1.SetActive(true);
        PS4.SetActive(false);
        PS5.SetActive(false);
        PC.SetActive(false);
#elif UNITY_STANDALONE || MICROSOFT_GAME_CORE || UNITY_EDITOR
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
#if UNITY_PS5 && !UNITY_EDITOR
        PS5_Title.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_TITLE");
        PS5_Description.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_BODY_PS");
#elif UNITY_PS4 && !UNITY_EDITOR    
        PS4_Title.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_TITLE");
        PS4_Description.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_BODY_PS");
#elif UNITY_GAMECORE && !UNITY_EDITOR
        XB_Title.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_TITLE");
        XB_Description.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_BODY_XB");
#elif UNITY_STANDALONE || MICROSOFT_GAME_CORE || UNITY_EDITOR
        PC_Title.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_TITLE");
        PC_Description.text = LocalizationSystem.GetLocalizedValue("DC_CONTROLLER_BODY_PC");
#endif        
    }

    void ShowDisconnectedApplet()
    {
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        //GameObject.Find("GameManager")?.GetComponent<LevelManager>()?.Pause();

    }
    void HideDisconnectedApplet()
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
