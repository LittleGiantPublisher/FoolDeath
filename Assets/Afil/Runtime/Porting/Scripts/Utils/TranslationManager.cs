using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class TranslationManager : MonoBehaviour
{
    [SerializeField] private string nomeXml;
    public static TranslationManager instance;
    private string english, portuguese, russian, spanish, german;
    XmlDocument xmlDocument;
    TextAsset xml;
    private string selectedLanguage;

    public string English { get => english; set => english = value; }
    public string Portuguese { get => portuguese; set => portuguese = value; }
    public string Russian { get => russian; set => russian = value; }
    public string Spanish { get => spanish; set => spanish = value; }
    public string German { get => german; set => german = value; }
    
    public string SelectedLanguage { get => selectedLanguage; set => selectedLanguage = value; }

    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else if (instance != this)
            Destroy(gameObject);

        xml = Resources.Load<TextAsset>(nomeXml);
        xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xml.text);


        if (SaveCompatibility.LocalPlayerPrefs.HasKey("selectedLanguage"))
        {
            selectedLanguage = SaveCompatibility.LocalPlayerPrefs.GetString("selectedLanguage");
        }
        else
        {
            DefaultLanguage();
        }
    }

   
    // Update is called once per frame
    void Update()
    {
        
    }

    void DefaultLanguage()
    {

#if UNITY_PS4 && !UNITY_EDITOR
        switch (UnityEngine.PS4.Utility.systemLanguage)
        {
            case UnityEngine.PS4.Utility.SystemLanguage.ENGLISH_US:
                selectedLanguage = "english";
                break;
            case UnityEngine.PS4.Utility.SystemLanguage.PORTUGUESE_BR:
                selectedLanguage = "portuguese";
                break;
            case UnityEngine.PS4.Utility.SystemLanguage.SPANISH:
                selectedLanguage = "spanish";
                break;
            case UnityEngine.PS4.Utility.SystemLanguage.DUTCH:
                selectedLanguage = "german";
                break;
            case UnityEngine.PS4.Utility.SystemLanguage.GERMAN:
                selectedLanguage = "german";
                break;

            case UnityEngine.PS4.Utility.SystemLanguage.RUSSIAN:
                selectedLanguage = "russian";
                break;

            default:
                selectedLanguage = "english";
                break;
        }
#else
        switch (Application.systemLanguage)
        {
            case SystemLanguage.English:
                selectedLanguage = "english";
                break;
            case SystemLanguage.Portuguese:
                selectedLanguage = "portuguese";
                break;
            case SystemLanguage.Spanish:
                selectedLanguage = "spanish";
                break;
            case SystemLanguage.German:
                selectedLanguage = "german";
                break;
            case SystemLanguage.Dutch:
                selectedLanguage = "german";
                break;

            case SystemLanguage.Russian:
                selectedLanguage = "russian";
                break;

            default:
                selectedLanguage = "english";
                break;
        }
#endif


    }
    public string LoadTranslationData(string key, string language)
    {
        foreach (XmlNode translation in xmlDocument["translations"].ChildNodes)
        {
            string keyXml = translation.Attributes["name"].Value;

            if (keyXml == key)
            {
                english = translation["english"].InnerText;
                portuguese = translation["portuguese"].InnerText;
                russian = translation["russian"].InnerText;
                spanish = translation["spanish"].InnerText;
                german = translation["german"].InnerText;
                break;
            }
        }
        switch(language)
        {
            case "english":
                return english;

            case "portuguese":
                return portuguese;



            case "russian":
                return russian;

            case "spanish":
                return spanish;
            case "german":
                return german;


            default:
                return english;

        }
    }
}
