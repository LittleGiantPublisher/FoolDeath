using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;


public class Windows_Platform : IPlatform
{

    string filePath;
    public bool CheckAchievement(string name)
    {
        return true;
    }

    public void ClearPresence()
    {
        
    }

    public int GetAchievementStats(string name)
    {
        return 0;
    }

    public string GetFileName()
    {
        return "";
    }

    public string GetUserGamertag()
    {
        return "";
    }

    public bool HasUserConnected()
    {
        return true;
    }

    public void LoadGameData(Action<bool, byte[]> callback)
    {
        if (File.Exists(filePath))
        {
            Debug.LogError("Loading game data... WINDOWS");
            BinaryFormatter formatter = new BinaryFormatter();

            FileStream stream = new FileStream(filePath, FileMode.OpenOrCreate);

            byte[] bytes = (byte[])formatter.Deserialize(stream);
            stream.Close();

            if (bytes == null || bytes.Length > 0 )
            {
                SaveCompatibility.LocalPlayerPrefs.loadedBytes = bytes;
            }          

            callback?.Invoke(true, bytes);
        }
        else
        {
            callback?.Invoke(false, null);
        }
    }

    public void OnPlatformAwake()
    {
        
    }

    public void OnPlatformStart()
    {
        filePath = Application.persistentDataPath + "/save.dat";
        Porting.PlatformManager.instance.initializeFinished = true;

        Application.focusChanged += Application_focusChanged;
        Porting.PlatformManager.UserNickName = "PC User";
        Porting.PlatformManager.enterButtonParam = 1;
    }

    

    public void OnPlatformUpdate()
    {
        
    }

    public void RequestUserSignIn(int padIndex, Action<bool> callback)
    {
        callback?.Invoke(true);
    }

    public void RequestUserSignOut(int padIndex, Action<bool> callback)
    {
        callback?.Invoke(false);
    }

    public void SaveGameData(byte[] rawData, Action<bool> callback)
    {
        
        BinaryFormatter formatter = new BinaryFormatter();
        
        FileStream stream = new FileStream(filePath, FileMode.Create);

        formatter.Serialize(stream, rawData);
        stream.Close();
        
        callback?.Invoke(true);
    }

    public void SetAchievementStats(string name, int amount, int maxAmount, Action<bool> callback)
    {
        
    }

    public void SetPresence(string id, params string[] extraInfo)
    {
        
    }

    public float StreamingInstallProgress()
    {
        return 0;
    }

    public void UnlockAchievement(int trophyID, int progress, Action<bool> callback)
    {
        Debug.Log($" UnlockAchievement : {trophyID}");
        callback?.Invoke(false);
    }

    public void UpdateActivity(string status, Action<bool> callback)
    {
        
    }
    private void Application_focusChanged(bool yes)
    {
        if (!yes && UnityEngine.SceneManagement.SceneManager.GetActiveScene().name.Contains("Gameplay"))
            Porting.PlatformManager.ForcePause?.Invoke();
    }

}
