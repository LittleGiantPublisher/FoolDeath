using System;
using System.IO;
using UnityEngine;

namespace F
{
    public static class SaveSystem
    {
        public static void Load()
        {
            string path = Application.persistentDataPath + "/gamedata.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                try
                {
                    SaveSystem.data = JsonUtility.FromJson<SaveData>(json);
                }
                catch (Exception)
                {
                    string path2 = Application.persistentDataPath + "/gamedata_backup.json";
                    if (File.Exists(path2))
                    {
                        string json2 = File.ReadAllText(path2);
                        try
                        {
                            SaveSystem.data = JsonUtility.FromJson<SaveData>(json2);
                        }
                        catch (Exception)
                        {
                            SaveSystem.data = new SaveData();
                        }
                    }
                    else
                    {
                        SaveSystem.data = new SaveData();
                    }
                }
            }
            else
            {
                SaveSystem.data = new SaveData();
            }
        }

        public static void Save()
        {
            string contents = JsonUtility.ToJson(SaveSystem.data);
            File.WriteAllText(Application.persistentDataPath + "/gamedata.json", contents);
            File.WriteAllText(Application.persistentDataPath + "/gamedata_backup.json", contents);
        }

        public static SaveData data;
    }
}