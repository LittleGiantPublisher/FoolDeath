using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Collections;
using SaveCompatibility;

namespace Porting
{
#if UNITY_SWITCH
    public class Switch_Platform : IPlatform
    {
        long size;

        const string fileName = "Template";
        const string mountName = "SaveData";
        public string filePath = mountName + ":/";
        public int longOffset = 0;

        static bool initialize;
        public static nn.account.Uid userId;
        public nn.account.Nickname userName;
        static nn.account.UserHandle userHandle;

#region Save/Load
        void IPlatform.SaveGameData(byte[] rawData, Action<bool> callback)
        {
#if !UNITY_EDITOR
            nn.Result result;
            SwitchMount(out result);
            size = rawData.LongLength; Debug.Log("Set Size Sucess");

            SwitchCreateFile(ref result, rawData);

            SwitchWrite(ref result, rawData);

            SwitchCommit(ref result);

            nn.fs.FileSystem.Unmount(mountName); Debug.Log("Unmount Success");
            LocalPlayerPrefs.loadedBytes = rawData;
            Debug.LogError("Quantas vezes tem dado: " +rawData.Length);
            callback?.Invoke(true);
#else //UNITY_EDITOR
            callback?.Invoke(true);

#endif

        }

        public void LoadGameData(Action<bool, byte[]> callback)
        {
            byte[] data = new byte[] { };
#if !UNITY_EDITOR
            nn.Result result;
            SwitchMount(out result);
            if (!SwitchExistsData())
            {
                nn.fs.FileSystem.Unmount(mountName);
                callback.Invoke(false, data);
                return;
            }

            nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();
            SwitchOpen(ref result, ref fileHandle, nn.fs.OpenFileMode.Read);

            long fileSize = 32;
            result = nn.fs.File.GetSize(ref fileSize, fileHandle);
            result.abortUnlessSuccess();
            Debug.Log("[Switch Platform] Found file of size: " + fileSize);

            byte[] saveDataRaw = new byte[fileSize];
            result = nn.fs.File.Read(fileHandle, longOffset, saveDataRaw, fileSize);
            result.abortUnlessSuccess();
            Debug.Log("[Switch Platform] File readed.");

            nn.fs.File.Close(fileHandle);
            Debug.Log("[Switch Platform] File has been closed.");

            nn.fs.FileSystem.Unmount(mountName);
            Debug.Log("Load Finished");
            LocalPlayerPrefs.loadedBytes = saveDataRaw;
            callback?.Invoke(true, saveDataRaw);
#else // UNITY_EDITOR
            callback?.Invoke(true, data);
#endif


        }
#endregion

        public void InitializeSwitch()
        {
#if !UNITY_EDITOR
            nn.account.Account.Initialize();

            Debug.Log("Initialize account");
            userHandle = new nn.account.UserHandle();
            Debug.Log("Set UserHandle Success");

            if (!nn.account.Account.TryOpenPreselectedUser(ref userHandle))
                nn.Nn.Abort("Can�t Open Preselected User");

            nn.Result result = nn.account.Account.GetUserId(ref userId, userHandle);
            result.abortUnlessSuccess();

            Debug.Log("Get UserId Success");
            PlatformManager.instance.GetUserGamertag();

            Debug.Log($"User handle {userHandle}");
#endif
        }



            #region Switch Load And Save Functions
#if !UNITY_EDITOR
        void SwitchMount(out nn.Result _result)
        {
            _result = nn.fs.SaveData.Mount(mountName, userId);
            _result.abortUnlessSuccess();
            Debug.Log("Mount Sucess");
        }
#endif

        public void SwitchCommit(ref nn.Result result)
        {
#if !UNITY_EDITOR
            result = nn.fs.FileSystem.Commit(mountName);
            result.abortUnlessSuccess();
            Debug.Log("Commit Success");
#endif
        }


        private void SwitchOpen(ref nn.Result result, ref nn.fs.FileHandle fileHandle, nn.fs.OpenFileMode openFileMode)
        {
#if !UNITY_EDITOR
            result = nn.fs.File.Open(ref fileHandle, filePath + PlatformManager.currentPlatform.GetFileName(), openFileMode);
            result.abortUnlessSuccess();

            Debug.Log("[Switch Platform] File openned at path " + filePath + PlatformManager.currentPlatform.GetFileName());
#endif
        }

        private bool SwitchExistsData()
        {
#if !UNITY_EDITOR
            nn.fs.EntryType entryType = nn.fs.EntryType.File;
            nn.Result result = nn.fs.FileSystem.GetEntryType(ref entryType, filePath + PlatformManager.currentPlatform.GetFileName());
            bool exists = result.IsSuccess();

            Debug.Log(exists ? "[Switch Platform] Save data found." : "[Switch Platform] Save Data is missing.");
            return exists;
#else
            return true;
#endif
        }

        public void SwitchCreateFile(ref nn.Result result, byte[] data)
        {
#if !UNITY_EDITOR
            if (SwitchExistsData())
            {
                result = nn.fs.File.Delete(filePath + PlatformManager.currentPlatform.GetFileName());
                result.abortUnlessSuccess();
            }
            Debug.Log("Result Dont�t Exists");
            result = nn.fs.File.Create(filePath + PlatformManager.currentPlatform.GetFileName(), size); Debug.Log("Create File Sucess");
            result.abortUnlessSuccess();
#endif
        }


        void SwitchWrite(ref nn.Result result, byte[] data)
        {
#if !UNITY_EDITOR
            nn.fs.OpenFileMode openFile = nn.fs.OpenFileMode.Write;
            nn.fs.FileHandle fileHandle = new nn.fs.FileHandle();

            result = nn.fs.File.Open(ref fileHandle, filePath + PlatformManager.currentPlatform.GetFileName(), openFile);
            result.abortUnlessSuccess();
            Debug.Log("File Open Success");

            result = nn.fs.File.Write(fileHandle, longOffset, data, size, nn.fs.WriteOption.Flush);
            result.abortUnlessSuccess();
            Debug.Log("File Write Sucess");

            nn.fs.File.Close(fileHandle);
            Debug.Log("File Close Success");
#endif
        }


#endregion

        string IPlatform.GetFileName()
        {
            return fileName;
        }

        void IPlatform.OnPlatformStart()
        {
#if !UNITY_EDITOR
            if (!initialize)
            {
                InitializeSwitch();
                initialize = true;
            }
#endif

            PlatformManager.enterButtonParam = 0;
        }

        void IPlatform.OnPlatformUpdate()
        {
            
        }

        void IPlatform.RequestUserSignIn(int padIndex, Action<bool> callback)
        {
            callback?.Invoke(true);
        }

        void IPlatform.RequestUserSignOut(int padIndex, Action<bool> callback)
        {
            
        }

        bool IPlatform.HasUserConnected()
        {
            return true;
        }

        string IPlatform.GetUserGamertag()
        {
#if !UNITY_EDITOR
            nn.Result result;
            result = nn.account.Account.GetNickname(ref userName, userId);
            result.abortUnlessSuccess();
            return userName.ToString();
#endif
            return "";
        }

        void IPlatform.OnPlatformAwake() { }
        void IPlatform.SetPresence(string id, params string[] extraInfo) { }
        void IPlatform.ClearPresence() { }
        void IPlatform.UnlockAchievement(int Id, Action<bool> callback) { }
        bool IPlatform.CheckAchievement(string name) { return true; }
        int IPlatform.GetAchievementStats(string name) { return 0; }
        void IPlatform.SetAchievementStats(string name, int amount, int maxAmount, Action<bool> callback) { }
        float IPlatform.StreamingInstallProgress() { return 0; }
        void IPlatform.UpdateActivity(string status, Action<bool> callback) {  }
    }
#endif
}