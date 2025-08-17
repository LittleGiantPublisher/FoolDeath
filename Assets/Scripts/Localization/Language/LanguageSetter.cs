using UnityEngine;
using UnityEngine.Events;
using SaveCompatibility;
using UnityEngine.SocialPlatforms;

namespace F
{
    public class LanguageSetter : MonoBehaviour
    {
        public static LanguageSetter Instance { get; private set; }

        public static UnityEvent OnLanguageChanged = new UnityEvent();
        public static string ChangedEvent = "LanguageSetter.ChangedEvent";

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            LocalizationSystem.language = (LocalizationSystem.Language)LocalPlayerPrefs.GetInt("Language", -1);
            if ((int)LocalizationSystem.language == -1)
            {
                DefaultLanguage();
            }
            OnLanguageChanged.Invoke();
        }

        public void SetEN()
        {
            LocalizationSystem.language = LocalizationSystem.Language.English;
            LocalPlayerPrefs.SetInt("Language", (int)LocalizationSystem.language);
            OnLanguageChanged.Invoke();
            LocalPlayerPrefs.Save();
        }

        public void SetBR()
        {
            LocalizationSystem.language = LocalizationSystem.Language.BrazilPortuguese;
            LocalPlayerPrefs.SetInt("Language", (int)LocalizationSystem.language);
            OnLanguageChanged.Invoke();
            LocalPlayerPrefs.Save();
        }

        void DefaultLanguage()
        {
            
#if UNITY_PS4 && !UNITY_EDITOR
            switch (UnityEngine.PS4.Utility.systemLanguage)
            {
                case UnityEngine.PS4.Utility.SystemLanguage.ENGLISH_US:
                    LocalizationSystem.language = LocalizationSystem.Language.English;
                    break;
                case UnityEngine.PS4.Utility.SystemLanguage.PORTUGUESE_BR:
                    LocalizationSystem.language = LocalizationSystem.Language.BrazilPortuguese;
                    break;

                default:
                    LocalizationSystem.language = LocalizationSystem.Language.English;
                    break;
            }
#else
            switch (Application.systemLanguage)
            {
                case SystemLanguage.English:
                    LocalizationSystem.language = LocalizationSystem.Language.English;
                    break;
                case SystemLanguage.Portuguese:
                    LocalizationSystem.language = LocalizationSystem.Language.BrazilPortuguese;
                    break;

                default:
                    LocalizationSystem.language = LocalizationSystem.Language.English;
                    break;
            }
        }
#endif
    }
}
