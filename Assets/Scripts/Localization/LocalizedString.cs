using System;
using UnityEngine;

namespace F 
{
    // Struct representing a localized string
    [Serializable]
    public struct LocalizedString
    {
        // Constructor accepting a key
        public LocalizedString(string key)
        {
            this.key = key;
        }

        // Implicit conversion operator from string to LocalizedString
        public static implicit operator LocalizedString(string key)
        {
            return new LocalizedString(key);
        }

        // Field representing the key of the localized string
        public string key;
    }
}