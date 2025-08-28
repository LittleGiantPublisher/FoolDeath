using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Collections.Generic;
using System.Collections;
using SaveCompatibility;

namespace PlatformSupport
{
    public enum DataResult
    {
        Success,
        OK,
        NOT_OK,
        Busy,
        UserCanceled,
        FailedNoFreeSpace,
        FailedCorrupted,
        FailedTempered,
        FailedGenericError,
        DoesntExists,
        GenericError,
        NULL
    }
}

namespace Porting
{
    public class PlatformManager : MonoBehaviour
    {

        public static UNLOCK_CHECK CUR_PATCH;

    	public static bool notSpaceAvailable;

        public static Dictionary<string, int> GetAchievementID = new Dictionary<string, int>();
        
        public static Action ForcePause;
        public bool[] hasUnlockedAchievement;
        public bool[] hasUnlockedAchievementOnPlat;
        public static PlatformManager instance { get; private set; }
        public static IPlatform currentPlatform { get; private set; }
        public static int enterButtonParam;
        static public bool isRequesting = false;
        static public bool isOnRechargeTime;


#if UNITY_GAMECORE
        public static Unity.GameCore.XUserLocalId localId;
#endif
#if MICROSOFT_GAME_CORE
        public static XGamingRuntime.XUserLocalId localId;
#endif

        public class PlatformEvent : EventArgs
        {
            public bool success;

            public PlatformEvent(bool success)
            {
                this.success = success;
            }
        }
        public static string UserNickName = "";

        static public Action OnUpdateProgress;
        static public event EventHandler<PlatformEvent> OnGameSaveEnd;
        static public event EventHandler<(PlatformEvent, byte[])> OnGameLoadEnd;
        static public event EventHandler<PlatformEvent> OnUserSignInEnd;
        static public event EventHandler<PlatformEvent> OnUserSignOutEnd;
        static public event EventHandler<PlatformEvent> OnActivityStatusChange;

        [HideInInspector]
        public bool initializeFinished;
        public static bool isLaunchingActivity;
        public static bool resaivingActivity;
        //SoundManager sound;
        bool activityStatusChangeRequest;
        bool hasRecievedValue;
        bool saveFinished = true;
        bool loadedData;

       


        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            const string prefabPath = "PlatformManager";

            if (instance)
                return;

            PlatformManager platformManagerPrefab = Resources.Load<PlatformManager>(prefabPath);

            if (platformManagerPrefab == null)
                throw new Exception($"[Platform Manager] Platform manager prefab not found at {prefabPath}");

            LoadAllAchievements();

            instance = Instantiate(platformManagerPrefab);
            instance.gameObject.SetActive(true);
            DontDestroyOnLoad(instance.gameObject);
            ////Debug.LogError($"\n [Platform Manager] = {instance.name}");
            //SaveCompatibility.LocalSaveCompatibility.LocalPlayerPrefs.OnSaveCallback = instance.RequestGameSave;
            instance.hasUnlockedAchievement = new bool[50];
            instance.hasUnlockedAchievementOnPlat = new bool[50];

#if UNITY_SWITCH && !UNITY_EDITOR
            currentPlatform = new Switch_Platform();
#elif UNITY_PS4 && !UNITY_EDITOR 
            currentPlatform = new PS_Platform();
#elif UNITY_PS5 && !UNITY_EDITOR
            currentPlatform = new PS5_Platform();
#elif UNITY_GAMECORE && !UNITY_EDITOR
            currentPlatform = new XB_Platform();
#elif MICROSOFT_GAME_CORE  && !UNITY_EDITOR
            currentPlatform = new MS_Platform();
#else
            currentPlatform = new Windows_Platform();
#endif

#if MICROSOFT_GDK_SUPPORT
    Debug.LogError("MICROSOFT_GDK_SUPPORT");
#endif
            ////Debug.LogError("\n[Current Platform] = " + currentPlatform);

            //instance.userDataStore = instance.gameObject.AddComponent<Rewired.Data.UserDataStore_SaveCompatibility.LocalPlayerPrefs>();
            //instance.userDataStore.Load();
            instance.StartCoroutine(instance.InitPlatform());
        }

        private static void LoadAllAchievements()
        {
            var data = Resources.Load<AchievementData>("AchievementData");
            if(data == null)
            {
#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayDialog("Faltando data", "AchievementData n�o foi encontrado, clique em 'Afil/Porting/Check Achievement Data'", "OK");
#endif
                Debug.LogError("AchievementData n�o encontrado, clique em 'Afil/Porting/Check Achievement Data'");
                return;
            }
            if (data.Achievements == null || data.Achievements.Length == 0)
            {
                Debug.LogError("Achievements n�o carregados no data, por favor cheque o AchiementData'");
                return;
            }
            foreach (var achi in data.Achievements)
            {
                if (!GetAchievementID.ContainsKey(achi.name))
                {
                    GetAchievementID.Add(achi.name, achi.id);
                }
            }
            Debug.Log("Achievements carregados!");
        }

        IEnumerator InitPlatform()
        {
           
            //yield return new WaitForSeconds(0.3f);
            currentPlatform.OnPlatformAwake();
            yield return new WaitForSeconds(1f);
            currentPlatform.OnPlatformStart();
            yield return new WaitForSeconds(1f);
            
            instance.RequestUserSignIn();
#if !UNITY_EDITOR

#if UNITY_GAMECORE || MICROSOFT_GAME_CORE
#if UNITY_GAMECORE
            while (!XB_Platform.userInitialized)
#else
            while (!MS_Platform.userInitialized)
#endif
            {


                yield return new WaitForEndOfFrame();
            }
#endif
#endif
            // //Debug.LogError("REGISTRANDO EVENTO SAVE DESATIVANDO COISA IMPORTANTE");
            OnGameSaveEnd += instance.SaveEnd;
            ////Debug.LogError("REGISTRANDO EVENTO LOAD DESATIVANDO COISA IMPORTANTE");
            OnGameLoadEnd += instance.LoadEnd;
            OnUserSignInEnd += instance.SignedIn;
            OnActivityStatusChange += instance.ActivityChanged;
        }

        private void SignedIn(object sender, PlatformEvent result)
        {
            ////Debug.LogError($"[UserSignedIn] : {result.success}");
        }

        private void Update()
        {
//#if !UNITY_EDITOR
            currentPlatform.OnPlatformUpdate();
//#endif
        }

        void ActivityChanged(object sender, PlatformEvent result)
        {
            activityStatusChangeRequest = false;
           // //Debug.LogError($"[ActivityChanged] : {result.success}");
        }

        void SaveEnd(object sender, PlatformEvent result)
        {
           // //Debug.LogError("[SaveEnd] : " + result.success);
            saveFinished = true;
        }

        void LoadEnd(object sender, (PlatformEvent e, byte[] dataToLoad) result)
        {

 #if !UNITY_GAMECORE && !MICROSOFT_GAME_CORE && !UNITY_SWITCH
            switch (result.e.success)
            {
                case true:
                    Debug.Log("[LOAD END]");
                    if (result.dataToLoad == null || result.dataToLoad.LongLength < 0)
                        SaveCompatibility.LocalPlayerPrefs.SetDefault();
                    break;
                case false:
                    SaveCompatibility.LocalPlayerPrefs.SetDefault();
                    break;
            }
#endif

            hasRecievedValue = true;
        }

        public void UnlockAchievement(string name, float progress = 1.0f)
        {
            progress = Mathf.Clamp(progress, 0f, 1f);
            if (progress <= 0) return;

            if (!GetAchievementID.ContainsKey(name))
                return;

            int trophyID = GetAchievementID[name];
            if (!hasUnlockedAchievement[trophyID])
            {
                currentPlatform.UnlockAchievement(trophyID, progress, null);
                // Debug.LogError($"Unlocked {trophyID} : {name}");
                if (progress >= 1f) hasUnlockedAchievement[trophyID] = true;
                SaveCompatibility.LocalPlayerPrefs.SetBool("UnlockList", hasUnlockedAchievement);
                SaveCompatibility.LocalPlayerPrefs.Save();
            }

        }
        public void UnlockAchievement(int trophyID, float progress = 1.0f)
        {
            progress = Mathf.Clamp(progress, 0f, 1f);
            if (progress <= 0) return;

            if (!GetAchievementID.ContainsValue(trophyID))
                return;

            if (!hasUnlockedAchievementOnPlat[trophyID])
            {
                currentPlatform.UnlockAchievement(trophyID, progress, null);
                ////Debug.LogError($"Unlocked {trophyID} : {name}");
                if (progress >= 1f) hasUnlockedAchievement[trophyID] = true;
                SaveCompatibility.LocalPlayerPrefs.SetBool("UnlockList", hasUnlockedAchievement);
                SaveCompatibility.LocalPlayerPrefs.Save();
            }
        }

        public bool CheckAchievement(string name) => currentPlatform.CheckAchievement(name);

        public string GetUserGamertag() => currentPlatform.GetUserGamertag();

        public void RequestUserSignIn()
        {
            ////Debug.LogError("[Platform Manager] Request user sign in");

            if (loadedData)
            {
                UserSignInCallback(true);
                return;
            }
#if UNITY_EDITOR
            currentPlatform.RequestUserSignIn(0, UserSignInCallback);

#else
            currentPlatform.RequestUserSignIn(0, UserSignInCallback);
#endif
        }

        public void RequestUserSignOut(int user)
        {
            //Debug.Log("[Platform Manager] Request user sign out");
            SaveCompatibility.LocalPlayerPrefs.DeleteAll();
            loadedData = false;
            currentPlatform.RequestUserSignOut(user, UserSignOutCallback);
        }

        void RequestGameSave()
        {
           Debug.Log("[Platform Manager] Saving game data");
          
            byte[] data = SaveCompatibility.LocalPlayerPrefs.Serialize();
            Debug.Log($"[Platform Manager]  Game data size: {data.Length}");
            //string saveData = Convert.ToBase64String(data);
            //SaveCompatibility.LocalSaveCompatibility.LocalPlayerPrefs.SetString(GameData.SaveData.SAVEDATA_FILENAME + saveDataIndex.ToString(), saveData);
           
            currentPlatform.SaveGameData(data, SaveDataCallback);
        }

        void RequestGameLoad()
        {
            Debug.Log("[Platform Manager] Loading game data ");
            currentPlatform.LoadGameData(LoadDataCallback);
        }

        public bool HasUserConnected() => currentPlatform.HasUserConnected();

        void UserSignInCallback(bool success)
        {
            ////Debug.LogError($"\n [UserSignInCallback] : {success}");
            initializeFinished = true;
            OnUserSignInEnd?.Invoke(this, new PlatformEvent(success));
        }

        void UserSignOutCallback(bool success)
        {
            OnUserSignOutEnd?.Invoke(this, new PlatformEvent(success));
        }

        void SaveDataCallback(bool success)
        {
           Debug.LogError("[SaveDataCallback]= " + success);
           
            OnGameSaveEnd?.Invoke(this, new PlatformEvent(success));
        }

        void LoadDataCallback(bool success, byte[] dataToLoad)
        {
            Debug.LogError("\n [LoadDataCallback] : " + success);
            Debug.LogError($"[LoadDataCallback] dataToLoad.Length: {dataToLoad.Length}");
            loadedData = success;
            
            OnGameLoadEnd?.Invoke(this, (new PlatformEvent(success), dataToLoad));
        }

        public void ReturnToMenu()
        {
            //reset variables and back to menu
        }

        public void UpdateActivity(string status)
        {
            ////Debug.LogError("Entrou no UpdateActivity");
            currentPlatform.UpdateActivity(status, UpdateActivityCallback);
        }
        public void UpdateActivityCallback(bool success)
        {
            OnActivityStatusChange?.Invoke(this, new PlatformEvent(success));
        }

        public IEnumerator WaitingSaveFinish()
        {
            RequestGameSave();
            yield return new WaitForSeconds(3);

            while (!saveFinished)
            {
                yield return new WaitForSeconds(1);
            }

            if (saveFinished)
            {
                StopCoroutine(WaitingSaveFinish());
                Debug.Log("[REQUEST] Save Request finished");
                saveFinished = false;
            }
        }

        public IEnumerator WaitingLoadFinish()
        {
            RequestGameLoad();

            while (!hasRecievedValue)
            {
           
                yield return new WaitForSeconds(1);
            }
            hasRecievedValue = false;
            yield break;
        }

        public IEnumerator WaitingActivity(string status)
        {
            if (!activityStatusChangeRequest)
                UpdateActivity(status);

            while (!activityStatusChangeRequest)
            {
                yield return new WaitForSeconds(1);
            }
            activityStatusChangeRequest = false;
            yield break;
        }
    }

}
public enum UNLOCK_CHECK
{
    GAME_BASE,
    PATCH_1,
    PATCH_2,
    PATCH_3,
    PATCH_4,
}