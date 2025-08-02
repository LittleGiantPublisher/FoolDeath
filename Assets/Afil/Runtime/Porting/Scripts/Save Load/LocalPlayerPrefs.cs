using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Text;

namespace SaveCompatibility
{
    /// <summary>
    /// Classe substituta do PlayerPrefs da Unity, salva os valores na memória e possui função de serializar e deserializar
    /// </summary>
    public class LocalPlayerPrefs : MonoBehaviour
    {

        public static LocalPlayerPrefs instance;
        static public bool initialized = false;

        public SaveContainer ViewData;
        
        public static SaveContainer saveContainer;

        private List<SaveData> backupData;

        public static System.Action OnSaveCallback;
        private static bool unsavedChanges;
        public static byte[] loadedBytes;

        private void Awake()
        {
            if(instance == null)
            {
                instance = this;
                saveContainer = new SaveContainer();
                backupData = new List<SaveData>();
                ViewData = saveContainer;
                DontDestroyOnLoad(this);
            }
            else
            {
                Destroy(this);
            }
            
        }

        
        public static void UpdateLoadenbytes()
        {
#if UNITY_GAMECORE || UNITY_STANDALONE || MICROSOFT_GAME_CORE
            loadedBytes = Serialize();
#endif
        }

        static public int[] GetVectorInt(string key, int[] defaultValue)
        {
            return(int[])saveContainer.Get(key, defaultValue);
        }

        static public int GetInt(string key, int defaultValue = 0)
        {
            
            return saveContainer !=null? (int)saveContainer.Get(key, defaultValue):defaultValue;
        }

        static public string GetString(string key, string defaultValue = "")
        {
            return (string)saveContainer.Get(key, defaultValue);
        }

        static public float GetFloat(string key, float defaultValue = 0)
        {
            return (float)saveContainer.Get(key, defaultValue);
        }
        static public bool GetBool(string key, bool defaultValue = false)
        {
            return (bool)saveContainer.Get(key,defaultValue);
        }

        static public bool[] GetBool(string key, bool[] defaultValue)
        {
            return (bool[])saveContainer.Get(key, defaultValue);
        }

        static public void SetIntVector(string key, int[] value)
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!saveContainer.ContainsKey(key))
            {
                saveContainer.Add(key, value);
                unsavedChanges = true;
                return;
            }

            saveContainer.UpdateValue(key, value);
            unsavedChanges = true;
        }
        static public void SetInt(string key, int value = 0)
        {

            if (string.IsNullOrEmpty(key))
                return;

            if (!saveContainer.ContainsKey(key))
            {
                saveContainer.Add(key, value);
                unsavedChanges = true;
                return;
            }

            saveContainer.UpdateValue(key, value);
            unsavedChanges = true;
        }

        static public void SetString(string key, string value = "")
        {
            if (string.IsNullOrEmpty(key))
                return;

            if (!saveContainer.ContainsKey(key))
            {
                saveContainer.Add(key, value);
                unsavedChanges = true;
                return;
            }

            saveContainer.UpdateValue(key,value);
			unsavedChanges = true;
		}

        static public void SetFloat(string key, float value = 0)
        {
            if (!saveContainer.ContainsKey(key))
            {
                saveContainer.Add(key, value);
                unsavedChanges = true;
                return;
            }

            saveContainer.UpdateValue(key, value);
            unsavedChanges = true;

        }

        static public void SetBool(string key, bool value)
        {
            if (!saveContainer.ContainsKey(key))
            {
                saveContainer.Add(key, value);
                unsavedChanges = true;
                return;
            }

            saveContainer.UpdateValue(key, value);
            unsavedChanges = true;
        }
        static public void SetBool(string key, bool[] value)
        {
            if (!saveContainer.ContainsKey(key))
            {
                saveContainer.Add(key, value);
                unsavedChanges = true;
                return;
            }

            saveContainer.UpdateValue(key, value);
            unsavedChanges = true;
        }

        static public bool HasKey(string key)
        {
            return saveContainer.ContainsKey(key);
        }

        static public void DeleteKey(string key)
        {
            saveContainer.Remove(key);
        }

        static public void DeleteAll()
        {
			saveContainer.ClearAll();
		}

        private void Update()
        {
            
        }

        static public void SetDefault()
        {


        }

        static public void Save()
        {
            Debug.LogError("Chamando Save");
            Porting.PlatformManager.instance.StartCoroutine(Porting.PlatformManager.instance.WaitingSaveFinish());
        }

        static public byte[] Serialize()
        {
            return saveContainer.SerializeSaveData(); 
        }

     
        static public void Deserialize(byte[] data)
        {
            Debug.LogError($"Entrou no deserialize:  {data.Length}");
            if (data.Length <= 0)
                return;

            System.IO.MemoryStream stream = new System.IO.MemoryStream(data);
            System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            object containerData = binaryFormatter.Deserialize(stream);
            if (containerData is SaveData[])
            {
                saveContainer.Deserialize(containerData as SaveData[]);
            }
        }
        static public void DeserializeManual()
        {
          
            Debug.LogError("[DESERIALIZE MANUAL] CHAMOU O DESCERIALIZE MANUAL");
            if (loadedBytes == null)
                return;

            Deserialize(loadedBytes);

        }
        public static void ManualySave()
		{
			SaveSaveFile();
		}

		private static void SaveSaveFile()
		{
			if (!unsavedChanges)
				return;

			unsavedChanges = false;
			OnSaveCallback?.Invoke();
		}


        [System.Serializable]
        public class SaveData
        {
            public string key;
            public object data;
        }

        [System.Serializable]
        public class SaveContainer
        {
            public List<SaveData> datas;
            bool checkbackup = true;
            public SaveContainer()
            {
                datas = new List<SaveData>();
            }

            public bool ContainsKey(string key)
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    if(datas[i].key == key)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void CallBackup()
            {
                if (checkbackup)
                {

                }
            }

            public void UpdateValue(string key, object value)
            {
                for (int i = 0; i < datas.Count; i++)
                {
                    if (datas[i].key == key)
                    {
                        datas[i].data = value;
                        instance.backupData[i].data = value;
                    }
                }
            }
            public object Get(string key, object defaulValue)
            {

                for (int i=0; i<datas.Count;i++)
                {
                    if (datas[i].key == key)
                    {
                        return datas[i].data;
                    }
                }

                return defaulValue;
            }
            public void Add(string key, object value)
            {
                if (!ContainsKey(key))
                {
                    SaveData newData = new SaveData();
                    newData.key = key;
                    newData.data = value;

                    datas.Add(newData);
                    instance.backupData.Add(newData);
                }
            }
            public void Remove(string key)
            {
                foreach (SaveData data in datas)
                {
                    if (data.key == key)
                    {
                        datas.Remove(data);
                        instance.backupData.Remove(data);
                    }
                }
            }

            public void ClearAll()
            {
                //Debug.LogError("ENTROU NO CLEAR ALL");
                datas.Clear();

            }
            public byte[] SerializeSaveData()
            {
                System.IO.MemoryStream stream = new System.IO.MemoryStream();
                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                SaveData[] SerializedDatas = datas.ToArray();
                binaryFormatter.Serialize(stream, SerializedDatas);

                return stream.ToArray();
            }

            public void Deserialize(SaveData[] loadedData)
            {
                for(int i=0; i< loadedData.Length; i++)
                {
                    if (datas.FindAll(x => x.key == loadedData[i].key).Count > 0)
                        continue;
                    datas.Add(loadedData[i]);
                    instance.backupData.Add(loadedData[i]);

                }
                checkbackup = false;
            }
    }
    }
}