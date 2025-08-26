// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
#if MICROSOFT_GAME_CORE
//using Microsoft.GameCore.Utilities;
#endif
using UnityEngine;
using UnityEngine.UI;

#if MICROSOFT_GAME_CORE
using XGamingRuntime;

public class ErrorEventArgs : System.EventArgs
{
    public string ErrorCode { get; private set; }
    public string ErrorMessage { get; private set; }

    public ErrorEventArgs(string errorCode, string errorMessage)
    {
        this.ErrorCode = errorCode;
        this.ErrorMessage = errorMessage;
    }
}


public class MS_Platform : IPlatform
{

    public static string scid = "";

    /// <summary>
    /// Used to display gamertag in Sign-In sample
    /// </summary>
    public Text gamertagLabel;
    public static bool userInitialized;
    private static bool _initialized;
    private static Dictionary<int, string> _hresultToFriendlyErrorLookup;

    private Microsoft.Xbox.XGameSaveWrapper _gameSaveHelper; 

    private const int _100PercentAchievementProgress = 100;
    private const string _GameSaveContainerName = "x_game_save_default_container";
    private const string _GameSaveBlobName = "x_game_save_default_blob";

    XUserAddOptions UserAddOptions = XUserAddOptions.AddDefaultUserSilently;

    public byte[] CurrentSaveData;

    public bool EndedGetUser = false;

    bool inSave;
    bool gameSaved;
    bool inLoad;
    bool applyLoad;

    internal const Int32 S_OK = 0x00000000;

#region MS_UserManager


    public XUserHandle userHandle;
    public XUserLocalId m_localId;
    public ulong userXUID;

    Action<bool> userCallback;

    public enum UserOpResult
    {
        Success,
        NoDefaultUser,
        ResolveUserIssueRequired,
        UnclearedVetoes,

        UnknownError
    }

    private enum State
    {
        Initializing,
        GetContext,
        WaitForAddingUser,
        GetBasicInfo,
        InitializeNetwork,
        GrabAchievements,
        UserDisplayImage,
        ReturnMuteList,
        ReturnAvoidList,
        UserPermissionsCheck,
        WaitForNextTask,
        Error,
        Idle,
        End
    }

    public class UserData
    {
        public XUserHandle userHandle;
        public XUserLocalId m_localId;
        public ulong userXUID;
        public string userGamertag;
        public bool userIsGuest;
        public XblPermissionCheckResult canPlayMultiplayer;
        public ulong[] avoidList;
        public ulong[] muteList;
        public byte[] imageBuffer;
        public XblContextHandle m_context;
    }

    public delegate void AddUserCompletedDelegate(UserOpResult result);

    public List<UserData> UserDataList = new List<UserData>();

    public event EventHandler<XUserChangeEvent> UsersChanged;

    UserData m_CurrentUserData;
    AddUserCompletedDelegate m_CurrentCompletionDelegate;
    XRegistrationToken m_CallbackRegistrationToken;

    abstract class Callback
    {
        public abstract void Invoke();
    }

    class LoadCallbackData : Callback
    {
        Action<bool, byte[]> callback;
        bool success;
        byte[] data;

        public LoadCallbackData(Action<bool, byte[]> callback, bool success, byte[] data)
        {
            this.callback = callback;
            this.success = success;
            this.data = data;
        }

        public override void Invoke() => callback?.Invoke(success, data);
    }

    class ProfileRequestCallback : Callback
    {
        Action<bool> callback;
        bool success;

        public ProfileRequestCallback(Action<bool> callback, bool success)
        {
            this.callback = callback;
            this.success = success;
        }

        public override void Invoke() => callback?.Invoke(success);
    }

    class SaveCallbackData : Callback
    {
        Action<bool> callback;
        bool success;
        int saveDataIndex;

        public SaveCallbackData(Action<bool> callback, bool success)
        {
            this.callback = callback;
            this.success = success;
        }

        public override void Invoke()
        {
            callback?.Invoke(success);
        }
    }

    List<Callback> callbacks = new List<Callback>();

#endregion

    private void InitializeHresultToFriendlyErrorLookup()
    {
        if (_hresultToFriendlyErrorLookup == null)
        {
            return;
        }

        _hresultToFriendlyErrorLookup.Add(-2143330041, "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? " +
            "You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, " +
            "have you published it in Partner Center?");

        _hresultToFriendlyErrorLookup.Add(-1994108656, "E_GAMEUSER_NO_PACKAGE_IDENTITY: Are you trying to call GDK APIs from the Unity editor?" +
            " To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once your" +
            "game is launched.");

        _hresultToFriendlyErrorLookup.Add(-1994129152, "E_GAMERUNTIME_NOT_INITIALIZED: Are you trying to call GDK APIs from the Unity editor?" +
            " To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once your" +
            "game is launched.");

        _hresultToFriendlyErrorLookup.Add(-2015559675, "AM_E_XAST_UNEXPECTED: Have you added the Windows 10 PC platform on the Xbox Settings page " +
            "in Partner Center? Learn more: aka.ms/sandboxtroubleshootingguide");
    }

    public void OnPlatformAwake()
    {
        //#if !UNITY_EDITOR
       // scid = GetScid();
//#endif
       // Debug.Log($"@Debug - MS_Platform - OnPlatformAwake - scid: {scid}");        
    }

    private string GetScid()
    {
#if MICROSOFT_GAME_CORE
        
        return "00000000-0000-0000-0000-000079962b79";
#endif
    }

    // FlowManager _FM;

    public void OnPlatformStart()
    {
        //   _FM = FlowManager.Instance;
        // _FM.StartFlow(FlowManager.currentStep.GDK_Init);
        // _FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 0, "GDK Initialization");
        //#if UNITY_EDITOR
        //      Debug.Log("@Debug - MS_Platform - OnPlatformStart - Editor -> return;");
        //    return;
        //#endif

        Porting.PlatformManager.enterButtonParam = 1;

        Debug.Log($"@Debug - MS_Platform - OnPlatformStart - scid: {scid}");
        _hresultToFriendlyErrorLookup = new Dictionary<int, string>();
        InitializeHresultToFriendlyErrorLookup();

        if (!Succeeded(SDK.XGameRuntimeInitialize(), "Initialize gaming runtime"))
        {
#if UNITY_EDITOR
            //Debug.LogError("You may need to update your config file for the editor. GDK -> PC -> Update Editor Game Config will copy your current game config to the Unity.exe location to enable GDK features when playing in-editor.");
#endif
         //   _FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 0 , "SDK.XGameRuntimeInitialize() Failed");
            return;
        }
        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 0, "GDK Initialization XGameRuntimeInitialize");

        _gameSaveHelper = new Microsoft.Xbox.XGameSaveWrapper();

        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 0, "GDK Initialization XGameRuntimeInitialize XGameSaveWrapper");
        
    }


    public void OnPlatformUpdate()
    {

#if MICROSOFT_GAME_CORE || UNITY_GAMECORE
        SDK.XTaskQueueDispatch();

        callbacks.ForEach(x => x.Invoke());
        callbacks.Clear();
#endif
    }

    public void RequestUserSignIn(int padIndex, Action<bool> callback)
    {
        AddUser(padIndex,callback);
    }

    void AddUser(int x, Action<bool> callBack)
    {
        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 1, "MS_Platform AddUser");
        userCallback = callBack;

        if(x == 1)
            UserAddOptions = XUserAddOptions.None;
        else
            UserAddOptions = XUserAddOptions.AddDefaultUserSilently;

        if(UserAddOptions == XUserAddOptions.AddDefaultUserSilently)
        {
            AddUserSilently();
        } else
        {
            AddUserWithUI(AddUserComplete);
        }
    }    

    public void AddUserSilently()
    {
        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 1, "MS_Platform AddUserSilently");
#if UNITY_EDITOR
        //UserOpResult result1 = UserOpResult.Success;
      //  AddUserComplete(result1);
      //  return;
#endif
        m_CurrentUserData = new UserData();
        SDK.XUserAddAsync(UserAddOptions, (Int32 hresult, XUserHandle userHandle) =>
        {
            if (hresult == 0 && userHandle != null)
            {
                Debug.Log("AddUserSilently - 0 - AddUser complete " + hresult + " user handle " + userHandle.GetHashCode());

                // Call XUserGetId here to ensure all vetos (privacy consent, gametag banned, etc) have passed
                UserOpResult result = GetUserId(userHandle);

                if(result == UserOpResult.Success)
                {
                    AddUserComplete(result);
                }
                else if (result == UserOpResult.ResolveUserIssueRequired)
                {
                    ResolveSigninIssueWithUI(userHandle, m_CurrentCompletionDelegate);
                }
                else if (result != UserOpResult.Success)
                {
                    m_CurrentCompletionDelegate(result);
                }
            }
            /*
            else if (hresult == HR.E_GAMEUSER_NO_DEFAULT_USER)
            {
                m_CurrentCompletionDelegate(UserOpResult.NoDefaultUser);
            }
            */
            else
            {
                m_CurrentCompletionDelegate(UserOpResult.UnknownError);
            }
        });
    }

    public bool AddUserWithUI(AddUserCompletedDelegate completionDelegate)
    {
#if UNITY_EDITOR
        //UserOpResult result1 = UserOpResult.Success;
       // AddUserComplete(result1);
       // m_CurrentCompletionDelegate = completionDelegate;
       // m_CurrentCompletionDelegate(result1);
       // return true;
#endif
        m_CurrentUserData = new UserData();
        m_CurrentCompletionDelegate = completionDelegate;

        SDK.XUserAddAsync(XUserAddOptions.None, (Int32 hresult, XUserHandle userHandle) =>
        {
            if (hresult == 0 && userHandle != null)
            {
                Debug.Log("AddUserWithUI complete " + hresult + " user handle " + userHandle.GetHashCode());

                // Call XUserGetId here to ensure all vetos (privacy consent, gametag banned, etc) have passed                    
                UserOpResult result = GetUserId(userHandle);
                if (result == UserOpResult.Success)
                {
                    AddUserComplete(result);
                }
                else if (result == UserOpResult.ResolveUserIssueRequired)
                {
                    ResolveSigninIssueWithUI(userHandle, m_CurrentCompletionDelegate);
                }
                else if (result != UserOpResult.Success)
                {
                    m_CurrentCompletionDelegate(result);
                }
            }
            else if (userHandle != null)
            {
                // Failed to log in, try to resolve issue
                ResolveSigninIssueWithUI(userHandle, m_CurrentCompletionDelegate);
            }
            else
            {
                Debug.Log("Got empty user handle back from AddUserWithUI.");
                m_CurrentCompletionDelegate(UserOpResult.UnknownError);
            }
        });

        return true;
    }

    private void AddUserComplete(UserOpResult userHandle)
    {
        switch (userHandle)
        {
            case UserOpResult.Success:
                Debug.LogError("UserAdded");
                //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 1, "XB_Platform AddUserComplete Success");
                userInitialized = true;
                userCallback?.Invoke(true);
                CompletePostSignInInitialization();
                Porting.PlatformManager.instance.initializeFinished = true;
                break;

            case UserOpResult.NoDefaultUser:
                AddUserWithUI(AddUserComplete);
                break;

            case UserOpResult.UnknownError:
                //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 1, "XB_Platform UnknownError");
                Debug.LogError("Error adding user.");
                Porting.PlatformManager.instance.initializeFinished = false;
                userCallback?.Invoke(false);
                break;

            default:
                //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 1, "XB_Platform UnknownError");
                Debug.LogError("Error default adding user.");
                userCallback?.Invoke(false);
                break;
        }

    }

    private void CompletePostSignInInitialization()
    {
        Debug.LogError("CompletePostSignInInitialization");
        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 2, "Complete Post Sign In Initialization");
        //#if !UNITY_EDITOR
        string gamertag = string.Empty;
        if (
            Succeeded(SDK.XUserGetGamertag(this.userHandle, XUserGamertagComponent.UniqueModern, out gamertag), "Get gamertag."))
        {
          //  gamertagLabel.text = gamertag;
            Porting.PlatformManager.UserNickName = gamertag;

        }
        if (Succeeded(SDK.XBL.XblInitialize(scid), "Initialize Xbox Live"))
        {
            //Debug.LogError("Complete Post Sign In Initialization + Initialize Xbox Live");
            //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 2, "Complete Post Sign In Initialization + Initialize Xbox Live");
        }
        else
        {
            //Debug.LogError("Complete Post Sign In Initialization - NO Xbox Live");
            //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 2, "Complete Post Sign In Initialization - NO Xbox Live");
        }
        Debug.LogError("try Create Xbox Live context");
        Succeeded(SDK.XBL.XblContextCreateHandle(
                this.userHandle,
                out m_CurrentUserData.m_context
            ), "Create Xbox Live context");
//#endif
        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 3, "Initialize Game Saves");
        InitializeGameSaves();
    }

    private void InitializeGameSaves()
    {
//#if !UNITY_EDITOR
        _gameSaveHelper.InitializeAsync(this.userHandle, scid, XGameSaveInitializeCompleted);
//#endif
    }

    private void XGameSaveInitializeCompleted(int hresult)
    {
        if (!Succeeded(hresult, "Initialize game save provider"))
        {
           // _FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 3, "XGame Save Initialize Failed");
            return;
        } else
        {
            //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 3, "XGame Save Initialize Completed");
        }
    }

    private void GameSaveSaveCompleted(int hresult)
    {
        Succeeded(hresult, "Game save submit update complete");
    }



    public void RequestUserSignOut(int padIndex, Action<bool> callback)
    {
        bool contextIsNull = false;
        bool userHandleIsNull = false;

        

//#if UNITY_EDITOR
       // contextIsNull = userHandleIsNull = true;
      //  callbacks.Add(new ProfileRequestCallback(callback, true));
//#elif !UNITY_EDITOR
        Debug.Log($"@Debug - MS_Platform - RequestUserSignOut({padIndex}) - START ");
        if (null != m_CurrentUserData.m_context)
        {
            SDK.XBL.XblContextCloseHandle(m_CurrentUserData.m_context);
            m_CurrentUserData.m_context = null;
            contextIsNull = true;
            Debug.Log($"@Debug - MS_Platform - RequestUserSignOut({padIndex}) - null != _xblContextHandle ");
        }

        if (null != this.userHandle)
        {
            SDK.XUserCloseHandle(this.userHandle);
            this.userHandle = null;
            this.userXUID = 0;
            m_CurrentUserData.userGamertag = string.Empty;
            userHandleIsNull = true;
            Debug.Log($"@Debug - MS_Platform - RequestUserSignOut({padIndex}) - null != _userHandle | userHandle = null | userXUID = 0 | userGamertag = string.Empty");
        }
//#endif

        callbacks.Add(new ProfileRequestCallback(callback, (contextIsNull && userHandleIsNull)));
        callback?.Invoke((contextIsNull && userHandleIsNull));

        Debug.Log($"@Debug - MS_Platform - RequestUserSignOut({padIndex}) - Success: {(contextIsNull && userHandleIsNull)} END ");
        
    }

    public bool HasUserConnected()
    {
        if (this.userHandle != null) return true;

        return false;
    }

    public void SaveGameData(byte[] rawData, Action<bool> callback)
    {
        Porting.PlatformManager.instance.StartCoroutine(SaveRotine(rawData, callback));
        //FM_.UpdateCurrentInfoState(FlowManager.currentStep.Save, FlowManager.currentState.InBetween, 1, "MS_Platform SaveGameData Start");

      
    }
    IEnumerator SaveRotine(byte[] rawData, Action<bool> callback)
    {

        while (inLoad)
            yield return new WaitForEndOfFrame();
//#if !UNITY_EDITOR
        if (inSave || rawData.Length <= 0)
            yield break;

        inSave = true;
        gameSaved = false;

        XGameSaveContainerHandle m_containerContext;
        XGameSaveUpdateHandle m_updateContext;
        string _filename = GetFileName();

        SDK.XGameSaveInitializeProviderAsync(this.userHandle, scid, false, (Int32 hresult, XGameSaveProviderHandle gameSaveProviderHandle) =>
        {
            if (hresult == 0)
            {
                SDK.XGameSaveCreateContainer(gameSaveProviderHandle, "SaveContainer", out m_containerContext);
                SDK.XGameSaveCreateUpdate(m_containerContext, "SaveUpdate", out m_updateContext);

                SDK.XGameSaveSubmitBlobWrite(m_updateContext, GetFileName(), rawData);
                SDK.XGameSaveSubmitUpdate(m_updateContext);

                SDK.XGameSaveCloseUpdateHandle(m_updateContext);
                SDK.XGameSaveCloseContainer(m_containerContext);
                callbacks.Add(new SaveCallbackData(callback, true));
                gameSaved = true;
                inSave = false;
                Debug.Log("    Save     ");
            }
            else
            {
                callbacks.Add(new SaveCallbackData(callback, false));
                inSave = false;
                gameSaved = false;
            }
            SDK.XGameSaveCloseProvider(gameSaveProviderHandle);
            Debug.Log("    Finish Save     ");
        });
//#else
     //   callback?.Invoke(true);
//#endif // UNITY_EDITOR
    }
   

    public void LoadGameData(Action<bool, byte[]> callback)
    {

        Porting.PlatformManager.instance.StartCoroutine(LoadGameRotine(callback));
        
    }
    IEnumerator LoadGameRotine(Action<bool, byte[]> callback)
    {

        while (inSave)
            yield return new WaitForEndOfFrame();

        if (inLoad)
            yield break;

        inLoad = true;
        //Debug.LogError("Entrou no LoadGameData \n");

//#if !UNITY_EDITOR
        while (!userInitialized)
        {
           
            yield return new WaitForEndOfFrame();
        }
//#else
      //  yield return new WaitForEndOfFrame();
//#endif

        applyLoad = true;
//#if !UNITY_EDITOR
            //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.InBetween, 4, "Initial Load Game Data");

			XGameSaveContainerHandle m_containerContext;
			XGameSaveUpdateHandle m_updateContext;
			var blobNames = new string[1] { GetFileName() };
            try
            {
            Debug.Log("  Try  Load     " + scid);
            SDK.XGameSaveInitializeProviderAsync(this.userHandle, scid, false, (Int32 hresult, XGameSaveProviderHandle gameSaveProviderHandle) =>
				{
					if (hresult == 0)
					{
						Debug.Log("    Load     ");
						SDK.XGameSaveCreateContainer(gameSaveProviderHandle, "SaveContainer", out m_containerContext);
						SDK.XGameSaveCreateUpdate(m_containerContext, "SaveUpdate", out m_updateContext);

						SDK.XGameSaveReadBlobDataAsync(m_containerContext, blobNames, (Int32 hrsult, XGameSaveBlob[] blobs) =>
						{
							//Debug.LogError("LoadBlob Hresult = " + hrsult + " ");
							Debug.Log("BlobNames " + blobNames[0]);
							if (hrsult == 0)
							{
								Debug.Log("LoadBlob Passou ");
                                //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 4, "Initial Load was Successfull");
                                callbacks.Add(new LoadCallbackData(callback, true, blobs[0].Data));
                                SaveCompatibility.LocalPlayerPrefs.loadedBytes = blobs[0].Data;
                                applyLoad = true;
                                SDK.XGameSaveCloseUpdateHandle(m_updateContext);
                                SDK.XGameSaveCloseContainer(m_containerContext);
                                inLoad = false;
                            }
							else if (hrsult == -2138898424)
							{
								Debug.Log("LoadBlob Passou Sem Data");
                                //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Completed, 4, "Initial Load without data");
                                callbacks.Add(new LoadCallbackData(callback, true, new byte[] { }));
                                SaveCompatibility.LocalPlayerPrefs.loadedBytes = new byte[] { };
                                applyLoad = true;
                                SDK.XGameSaveCloseUpdateHandle(m_updateContext);
                                SDK.XGameSaveCloseContainer(m_containerContext);
                                inLoad = false;
                            }
							else
							{
                                SaveCompatibility.LocalPlayerPrefs.loadedBytes = new byte[] { };
                                applyLoad = false;
                                SDK.XGameSaveCloseUpdateHandle(m_updateContext);
								SDK.XGameSaveCloseContainer(m_containerContext);
								Debug.Log("Load Blob Não Passou ");
                                //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 4, "Initial Load Blob Failed");
                                callbacks.Add(new LoadCallbackData(callback, false, new byte[] { }));
                                inLoad = false;
                            }
						});
                        //inputUnlock = true;
                    }
                    else
                    {
                        SaveCompatibility.LocalPlayerPrefs.loadedBytes = new byte[] { };
                        applyLoad = false;
                        Debug.Log("Erro::  " + hresult);
                        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 4, "Initial Load Blob Failed");
                        callbacks.Add(new LoadCallbackData(callback, false, new byte[] { }));
                        inLoad = false;
                    }
					Debug.Log("    Load Finished     ");
                    
					SDK.XGameSaveCloseProvider(gameSaveProviderHandle);
                });
			}
            catch (Exception e)
            {


            Debug.LogError($"[LOADGAMEDATA ERROR] : {e}");
            callbacks.Add(new LoadCallbackData(callback, false, new byte[] { }));
            SaveCompatibility.LocalPlayerPrefs.loadedBytes = new byte[] { };
                inLoad = false;
            }

       
//#else
        inLoad = false;
        //_FM.UpdateCurrentInfoState(FlowManager.currentStep.GDK_Init, FlowManager.currentState.Failed, 4, "Initial Load Exception");
//#endif // UNITY_EDITOR

    }
    private void GameSaveLoadCompleted(int hresult, byte[] savedData)
    {
        //_FM.EndFlow(FlowManager.currentStep.GDK_Init);

        if (!Succeeded(hresult, "Loaded Blob")) { 
            Debug.LogError("@Debug - MS_Platform - GameSaveLoadCompleted - Failed to locate blob - Allow player to continue - ");
            userInitialized = true;
            return;
        }

        Debug.Log($"@Debug - MS_Platform - GameSaveLoadCompleted(Success, {savedData.Length})");
        CurrentSaveData = savedData;
    }

    public void SetPresence(string id, params string[] extraInfo)
    {
        throw new NotImplementedException();
    }

    public void ClearPresence()
    {
        throw new NotImplementedException();
    }

    //FlowManager FM_ = FlowManager.Instance;

    public void UnlockAchievement(int achievementID, int progress, Action<bool> callback)
    {
        //FM_..UpdateCurrentInfoState(FlowManager.currentStep.Achievement, FlowManager.currentState.Start, 1, "MS_Platform UnlockAchievement: " + achievementID);

//#if !UNITY_EDITOR
        ulong xuid;
        if (!Succeeded(SDK.XUserGetId(this.userHandle, out xuid), "Get Xbox user ID"))
        {
            Debug.LogError("Get Xbox user ID fail");
           //FM_..UpdateCurrentInfoState(FlowManager.currentStep.Achievement, FlowManager.currentState.Failed, 1, "MS_Platform UnlockAchievement: " + achievementID + " - Could not get XUserGetID");
            return;
        }
        userXUID = xuid;
       //FM_..UpdateCurrentInfoState(FlowManager.currentStep.Achievement, FlowManager.currentState.InBetween, 1, "MS_Platform UnlockAchievement: " + achievementID + " - Got XUserGetID");

        SDK.XBL.XblAchievementsUpdateAchievementAsync(m_CurrentUserData.m_context, this.userXUID, achievementID.ToString(), progress * 100, (result) => 
        {

            if(result == 0)
            {
                //success
                Debug.Log("MS_Platform - Achievement Success");
            } else
            {
                //failed
                Debug.LogError("MS_Platform - Achievement Failed : " +result + " == " + result.ToString("X8"));
            }

        });
//#endif
        //FM_..UpdateCurrentInfoState(FlowManager.currentStep.Achievement, FlowManager.currentState.Completed, 1);
        callback?.Invoke(true);
    }

    private void UnlockAchievementComplete(int hresult)
    {
        Succeeded(hresult, "Unlock achievement");

        if (Succeeded(hresult, "Unlock achievement"))
        {
           //FM_..UpdateCurrentInfoState(FlowManager.currentStep.Achievement, FlowManager.currentState.Completed, 1);

        }
    }
    /*
    private XGameUiShowPlayerProfileCardCompleted XGameUiShowPlayerProfileCardCompletedNow(Int32 hresult)
    {

        XGameUiShowPlayerProfileCardCompleted xGame =new XGameUiShowPlayerProfileCardCompleted XGameUiShowPlayerProfileCardCompleted(); 
        if (hresult == 0)
        {
            return xGame;
        }
    }
    */
    public void GetUserInfo()
    {
        Debug.Log("@Debug - MS_Platform - GetUserInfo");
        //SDK.XGameUiShowPlayerProfileCardAsync(this.userHandle, this.userXUID, XGameUiShowPlayerProfileCardCompletedNow);
    }


    public bool CheckAchievement(string name)
    {
#if !UNITY_EDITOR
		return false;
		//return SaveCompatibility.LocalSaveCompatibility.LocalPlayerPrefs.GetInt(name) != 0;
#else
        return false;
#endif
    }

    public int GetAchievementStats(string name)
    {
        return 0;
    }

    public void SetAchievementStats(string name, int amount, int maxAmount, Action<bool> callback)
    {
       //throw new NotImplementedException();
    }

    public void UpdateActivity(string status, Action<bool> callback)
    {
       // throw new NotImplementedException();
    }

    public float StreamingInstallProgress()
    {
        throw new NotImplementedException();
    }


#region GDK
/*
    private void OnResume(double secondsSuspended)
    {

    }
    private void OnSuspend()
    {
        UnityEngine.WindowsGames.WindowsGamesPLM.AmReadyToSuspendNow();
    }
*/
    bool verifyingUser = false;

    IEnumerator VerifyCurrentUser()
    {
        yield return new WaitForSecondsRealtime(5);

        if (!userInitialized)
        {
            Debug.Log("@Debug - MS_Platform - VerifyCurrentUser - user was not Initialized");
            verifyingUser = false;
            yield break;
        }

        var hresult = S_OK;
        XUserHandle userHandle;
        ulong NewUserHandle;

        hresult = SDK.XUserFindUserByLocalId(this.m_localId, out userHandle);
        if (HR.SUCCEEDED(hresult))
        {
            SDK.XUserGetId(userHandle, out NewUserHandle);
            Debug.Log($"@Debug - MS_Platform - VerifyCurrentUser - Succeded - User still the same {this.m_localId} == {NewUserHandle}");
        }
        else
        {
            Porting.PlatformManager.instance.RequestUserSignOut(1);
        }
        verifyingUser = false;
    }

    private void OnResource(bool isConstrained)
    {
        if (!isConstrained)
        {
            if (!verifyingUser)
            {
                verifyingUser = true;
                Porting.PlatformManager.instance.StartCoroutine(VerifyCurrentUser());
            }
        }
        else
        {
            //Xbox UI, windows, etc;
            //Call Pause if needed here.
        }
    }

    // Helper methods
    protected static bool Succeeded(int hresult, string operationFriendlyName)
    {
        bool succeeded = false;
        if (HR.SUCCEEDED(hresult))
        {
            succeeded = true;
        }
        else
        {
            string errorCode = hresult.ToString("X8");
            string errorMessage = string.Empty;
            if (_hresultToFriendlyErrorLookup.ContainsKey(hresult))
            {
                errorMessage = _hresultToFriendlyErrorLookup[hresult];
            }
            else
            {
                errorMessage = operationFriendlyName + " failed.";
            }
            string formattedErrorString = string.Format("{0} Error code: hr=0x{1}", errorMessage, errorCode);
            //Debug.LogError(formattedErrorString);
            /*
            if (Helpers.OnError != null)
            {
                Helpers.OnError(Helpers, new ErrorEventArgs(errorCode, errorMessage));
            }
            */
        }
        Debug.LogError(operationFriendlyName + " ::: " + hresult.ToString("X8"));
        return succeeded;
    }

    public string GetUserGamertag()
    {
        string gamertag = string.Empty;
//#if !UNITY_EDITOR
        if (Succeeded(SDK.XUserGetGamertag(this.userHandle, XUserGamertagComponent.UniqueModern, out gamertag), "Get gamertag."))
        {
            return gamertag;
        }
//#endif
        return "Gamertag_Default";
    }

    public string GetFileName()
    {
        return "SaveData";
    }

#endregion




#region MS_UserManager

    //Resolve sign in issue - launches UI to sign in users
    void ResolveSigninIssueWithUI(XUserHandle userHandle, AddUserCompletedDelegate completionDelegate)
    {
        SDK.XUserResolveIssueWithUiUtf16Async(userHandle, null, (Int32 resolveHResult) =>
        {
            if (resolveHResult == 0)
            {
                GetUserId(userHandle);
            }
            else
            {
                // User has uncleared vetoes.  The game should decide how to handle this,
                // either by gracefully continuing or dropping user back to title screen to
                // with "Press 'A' or 'Enter' to continue" to select a new user.
                completionDelegate(UserOpResult.UnclearedVetoes);
            }
        });
    }

    // Get User ID
   UserOpResult GetUserId(XUserHandle userHandle)
    {
        ulong xuid;
        int hr = SDK.XUserGetId(userHandle, out xuid);

        int hr_localId = SDK.XUserGetLocalId(userHandle, out m_localId);

        if (HR.SUCCEEDED(hr_localId))
        {
            //Debug.LogError("[ADD USER ASYNC] LOCAL ID ADDED ");
        }
        else
        {
            //Debug.LogError($"[ADD USER ASYNC] LOCAL ID HRESULT:{hr_localId} ");
        }


        //Debug.LogError("@Debug - MS_Platform - GetUserId - hr: " + hr);
        if (hr == 0)
        {
            this.userHandle = userHandle;
            this.userXUID = xuid;
            return UserOpResult.Success;
        }
	/*
        else if (hr == HR.E_GAMEUSER_RESOLVE_USER_ISSUE_REQUIRED)
        {
            return UserOpResult.ResolveUserIssueRequired;
        }
	*/

        return UserOpResult.UnknownError;
    }

#endregion


}
#endif



