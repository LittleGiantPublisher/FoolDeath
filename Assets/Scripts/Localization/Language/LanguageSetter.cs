using UnityEngine;
using UnityEngine.Events;

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

            LocalizationSystem.language = (LocalizationSystem.Language)PlayerPrefs.GetInt("Language", 0);
            OnLanguageChanged.Invoke();
        }

        public void SetEN()
        {
            LocalizationSystem.language = LocalizationSystem.Language.English;
            PlayerPrefs.SetInt("Language", (int)LocalizationSystem.language);
            OnLanguageChanged.Invoke();
        }

        public void SetBR()
        {
            LocalizationSystem.language = LocalizationSystem.Language.BrazilPortuguese;
            PlayerPrefs.SetInt("Language", (int)LocalizationSystem.language);
            OnLanguageChanged.Invoke();
        }
    }
}
