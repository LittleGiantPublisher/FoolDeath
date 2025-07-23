using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace F 
{
    // Class responsible for managing localization
    public class LocalizationSystem
    {
        // Current language
        public static LocalizationSystem.Language language;

        // Dictionaries for each supported language
        private static Dictionary<string, string> localizedEN;
        private static Dictionary<string, string> localizedBR;

        // Flag indicating whether the localization system is initialized
        public static bool isInit;

        // CSV loader instance
        public static CSVLoader csvLoader;

        private static List<TextRegistration> registeredComponents = new List<TextRegistration>();


        // Method to initialize the localization system
        public static void Init()
        {
            LocalizationSystem.csvLoader = new CSVLoader();
            LocalizationSystem.csvLoader.LoadCSV();
            LocalizationSystem.UpdateDictionaries();
            LocalizationSystem.isInit = true;

            LanguageSetter.OnLanguageChanged.AddListener(UpdateAllRegisteredComponents);
        }

        // Method to update dictionaries with localized values
        public static void UpdateDictionaries()
        {
            LocalizationSystem.localizedEN = LocalizationSystem.csvLoader.GetDictionaryValues("en");
            LocalizationSystem.localizedBR = LocalizationSystem.csvLoader.GetDictionaryValues("br");

        }

        // Method to get the dictionary for the editor
        public static Dictionary<string, string> GetDictionaryForEditor()
        {
            if (!LocalizationSystem.isInit)
            {
                LocalizationSystem.Init();
            }
            return LocalizationSystem.localizedEN;
        }

        // Method to get the localized value for a given key and language
        public static string GetLocalizedValue(string key)
        {
            if (!LocalizationSystem.isInit)
            {
                LocalizationSystem.Init();
            }
            string result = key;
            switch (LocalizationSystem.language)
            {
                case LocalizationSystem.Language.English:
                    LocalizationSystem.localizedEN.TryGetValue(key, out result);
                    break;
                case LocalizationSystem.Language.BrazilPortuguese:
                    LocalizationSystem.localizedBR.TryGetValue(key, out result);
                    break;
  
            }
            if(result == null)result = key;
            return result;
        }

        // Enumeration representing supported languages
        public enum Language
        {
            English,
            BrazilPortuguese,
        }


        private struct TextRegistration
        {
            public TMP_Text TextComponent { get; }
            public System.Func<string> GetTextFunction { get; }

            public TextRegistration(TMP_Text textComponent, System.Func<string> getTextFunction)
            {
                TextComponent = textComponent;
                GetTextFunction = getTextFunction;
            }
        }

        public static void RegisterComponent(TMP_Text textComponent, System.Func<string> getTextFunction)
        {
            registeredComponents.RemoveAll(r => r.TextComponent == textComponent);

            var registration = new TextRegistration(textComponent, getTextFunction);
            registeredComponents.Add(registration);
            UpdateTextComponent(textComponent, getTextFunction);
        }

        public static void UnregisterComponent(TMP_Text textComponent)
        {
            registeredComponents.RemoveAll(r => r.TextComponent == textComponent);
        }

        public static void UpdateAllRegisteredComponents()
        {
            foreach (var registration in registeredComponents)
            {
                UpdateTextComponent(registration.TextComponent, registration.GetTextFunction);
            }
        }

        private static void UpdateTextComponent(TMP_Text textComponent, System.Func<string> getTextFunction)
        {
            if (textComponent != null)
            {
                textComponent.text = getTextFunction();
            }
        }


    }
    
}
