using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace F 
{
    public class CSVLoader
    {
        // Fields
        private TextAsset csvFile;
        private char lineSeperator = '\n';
        private char surround = '"';
        private string[] fieldSeperator = new string[] { "\",\"" };

        // Methods
        // Loads the CSV file from Resources folder
        public void LoadCSV()
        {
            this.csvFile = Resources.Load<TextAsset>("localization");
        }

        // Extracts the values associated with a given attribute ID from the loaded CSV file
        public Dictionary<string, string> GetDictionaryValues(string attributeID)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            string[] array = this.csvFile.text.Split(new char[] { this.lineSeperator });

            int num = -1;
            string[] array2 = array[0].Split(this.fieldSeperator, StringSplitOptions.None);
            for (int i = 0; i < array2.Length; i++)
            {
                if (array2[i].Contains(attributeID))
                {
                    num = i;
                    break;
                }
            }

            Regex regex = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            for (int j = 1; j < array.Length; j++)
            {
                string input = array[j];
                string[] array3 = regex.Split(input);
                for (int k = 0; k < array3.Length; k++)
                {
                    array3[k] = array3[k].TrimStart(new char[] { ' ', this.surround });
                    array3[k] = array3[k].TrimEnd(new char[] { this.surround });
                }
                if (array3.Length > num)
                {
                    string key = array3[0];
                    if (!dictionary.ContainsKey(key))
                    {
                        string value = array3[num];
                        dictionary.Add(key, value);
                        Debug.Log($"Key: {key}, Value: {value}");
                    }
                }
            }
            return dictionary;
        }
    }
}
