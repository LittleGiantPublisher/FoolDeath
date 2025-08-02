using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlatformSupport;
#if UNITY_PS4
using Sony.NP;
using UnityEngine.PS4;
#endif

namespace Porting
{
#if UNITY_PS4

	public class PS_Platform : IPlatform
	{
		public const string fileName = "PrimalSurvival";
		static PlatformSupport.PS4SaveData saveData;
		static bool userInitialized;
		bool savedataInitialized;
		static UnityEngine.PS4.PS4Input.LoggedInUser mainUser;
		int mainUserControllerSlot => mainUser.padHandle;
		static public int enterButtonParam { get; private set; } = 1;

		Sony.NP.Core.EmptyResponse trophyResponse;
		bool waitingTrophyResponse;

		List<Action> responseActions = new List<Action>();

		bool IPlatform.HasUserConnected()
		{
#if !UNITY_EDITOR
		return userInitialized; 
#else
			return false;
#endif // UNITY_EDITOR
		}

		void IPlatform.SaveGameData(byte[] rawData, Action<bool> callback)
		{
#if !UNITY_EDITOR
			if (saveData.isSaving)
			{
				//PlatformManager.instance.StartCoroutine(PlatformManager.instance.WaitingSaveFinish());
				callback?.Invoke(false);
				return;
			}
			//Debug.LogError($"[SAVE GAME DATA] rawData.Length: {rawData.Length}");
			PlatformManager.instance.StartCoroutine(saveData.CommitSave(rawData, GetFileName(), (result) =>
			{
				Debug.Log($"[PS4Platform] Save data result: {result.ToString()}");
				saveData.isSaving = false;

				if(result == DataResult.FailedNoFreeSpace)
                {
					callback?.Invoke(false);
					return;
				}

				callback?.Invoke(true);
			})); 
#else
			callback?.Invoke(true);
#endif // UNITY_EDITOR
		}

		void IPlatform.LoadGameData(Action<bool, byte[]> callback)
		{
#if !UNITY_EDITOR
			string filename = PlatformManager.currentPlatform.GetFileName();

			//Debug.LogError($"[PS4Platform] Load filename: {filename}");
			////Debug.LogError($"[PS4Platform] Load savedataInitialized: {savedataInitialized}");
			PlatformManager.instance.StartCoroutine(saveData.LoadRequest(null, filename,
			(result, rawData, fileName, so) =>
			{
				Debug.Log($"[PS4Platform] Load result: {result.ToString()}");
				switch (result)
				{
					case PlatformSupport.DataResult.Success:
						//Debug.LogError("[LOAD DATA] Success");
						SaveCompatibility.LocalPlayerPrefs.loadedBytes = rawData;
						callback?.Invoke(true, rawData);
						break;
					case PlatformSupport.DataResult.DoesntExists:
						//Debug.LogError("[LOAD DATA] DoesntExists");
						SaveCompatibility.LocalPlayerPrefs.loadedBytes = null;
						callback?.Invoke(true, new byte[] {});
						break;
					default:
						//Debug.LogError("[LOAD DATA] DEFAULT");
						SaveCompatibility.LocalPlayerPrefs.loadedBytes = null;
						callback?.Invoke(false, null);
						break;
				}
				saveData.isLoading = false;
			}));

#else
			callback?.Invoke(true, new byte[] { });
#endif // UNITY_EDITOR
		}

		public void OnPlatformAwake()
		{

		}

		void IPlatform.OnPlatformStart()
		{
#if !UNITY_EDITOR
			saveData = new PlatformSupport.PS4SaveData();
			//UnityEngine.PS4.RenderSettings.SetNativeJobsSubmissionMode(UnityEngine.PS4.RenderSettings.NativeJobsSubmissionMode.Concatenated);
			//Debug.LogError($"[SaveData]: {saveData}");

			InitToolkit initParams = new InitToolkit();
			initParams.contentRestrictions.Init();
			initParams.contentRestrictions.ApplyContentRestriction = false;

            initParams.SetPushNotificationsFlags(PushNotificationsFlags.None);
			InitResult initResult = Main.Initialize(initParams);

			Main.OnAsyncEvent += Main_OnAsyncEvent;


			if (initResult.Initialized == true)
				//Debug.LogError("INITIALIZED");

			Sony.PS4.Dialog.Main.Initialise();

			for (int i = 0; i < 4; i++)
            {
                UnityEngine.PS4.PS4Input.LoggedInUser user = UnityEngine.PS4.PS4Input.GetUsersDetails(i);
                if (user.status != 0 && user.primaryUser)
                {
                    mainUser = user;
                    break;
                }
            }

			Debug.Log($"Main user found:" +
				$"\nController ID: 0" +
				$"\nUser name: {mainUser.userName}" +
				$"\nUser ID: {mainUser.userId}");
			
			saveData.Init(mainUser.userId, PlatformManager.instance);
			savedataInitialized = true;
           

			// R4207
			enterButtonParam = UnityEngine.PS4.Utility.GetSystemServiceParam(UnityEngine.PS4.Utility.SystemServiceParamId.EnterButtonAssign);
			Debug.Log($"[PS4Platform] Enter button param: {enterButtonParam}");

			//UnityEngine.PS4.PS4Input.OnUserServiceEvent += OnUserService;
#endif // UNITY_EDITOR

			PlatformManager.enterButtonParam = enterButtonParam;
			//Debug.LogError("OnPlatformStart");
		}

		void OnUserService(uint id, uint userID)
		{
			////Utils.ExtDebug.ExtLog("PSPlatform", $"User service event ID: {id}, user: {userID}");
		}

		void IPlatform.OnPlatformUpdate()
		{
#if !UNITY_EDITOR
			UpdatePauseGame();
			UpdateResponses();
			UpdateServices();
#endif // UNITY_EDITOR
		}

		private void Main_OnAsyncEvent(Sony.NP.NpCallbackEvent npEvent)
		{
#if !UNITY_EDITOR
			//switch (npEvent.ApiCalled)
			//{
			//	case Sony.NP.FunctionTypes.NotificationUserStateChange:
			//		if (npEvent.UserId.Id == mainUser.userId)
			//		{
			//			AddResponse(() =>
			//			{
			//				PlatformManager.instance.RequestUserSignOut(mainUser.padHandle);
			//				PlatformManager.instance.ReturnToMenu();
			//			});
			//		}
			//		break;
			//}
#endif // UNITY_EDITOR
		}

		void UpdatePauseGame()
		{
			//Force pause event
			if (Utility.isInBackgroundExecution || Utility.isSystemUiOverlaid)
			{
				Porting.PlatformManager.ForcePause?.Invoke();
            }


		}

		void UpdateResponses()
		{
			for (int i = 0; i < responseActions.Count; i++)
			{
				responseActions[i]?.Invoke();
			}

			responseActions.Clear();
		}

		void AddResponse(Action response) => responseActions.Add(response);

		void UpdateServices()
		{
			if (savedataInitialized && userInitialized)
			{
				saveData.UpdateState();
			}
			if (!savedataInitialized)
				return;

			Sony.NP.Main.Update();
			Sony.PS4.Dialog.Main.Update();
}

		void IPlatform.RequestUserSignIn(int padIndex, Action<bool> callback)
		{
#if !UNITY_EDITOR
			// Register trophy pack
			try
			{
				Sony.NP.Trophies.RegisterTrophyPackRequest packRequest = new Sony.NP.Trophies.RegisterTrophyPackRequest();
				packRequest.UserId = mainUser.userId;
				Sony.NP.Trophies.RegisterTrophyPack(packRequest, new Sony.NP.Core.EmptyResponse());
			}
			catch (Exception e)
			{
				//Debug.LogError($"[PS Platform] Register trophy pack error: {e.Message}");
				callback?.Invoke(false);
				return;
			}
			userInitialized = true;
			callback?.Invoke(true);
#else
			callback?.Invoke(true);
#endif // UNITY_EDITOR
		}

		void IPlatform.RequestUserSignOut(int padIndex, Action<bool> callback)
		{
			//userInitialized = false;
			//saveData.Finish();
		}

		bool IPlatform.CheckAchievement(string name)
		{
#if !UNITY_EDITOR
		return false;
		//return SaveCompatibility.LocalSaveCompatibility.LocalPlayerPrefs.GetInt(name) != 0;
#else
			return false;
#endif // UNITY_EDITOR
		}

		void IPlatform.UnlockAchievement(int trophyID, Action<bool> callback)
		{
#if !UNITY_EDITOR


		Sony.NP.Trophies.UnlockTrophyRequest trophyRequest = new Sony.NP.Trophies.UnlockTrophyRequest
		{
			TrophyId = trophyID,
			UserId = mainUser.userId,
			Async = true
		};

		// Trophy screenshot test
		waitingTrophyResponse = true;
		trophyResponse = new Sony.NP.Core.EmptyResponse();

		// PS4 Achievement screenshot timing test
		Sony.NP.Trophies.UnlockTrophy(trophyRequest, trophyResponse);
            
#endif // UNITY_EDITOR
		}

		string IPlatform.GetUserGamertag()
		{
			return mainUser.userName;
		}


		int IPlatform.GetAchievementStats(string name)
		{
			if (!userInitialized)
			{
				//Utils.ExtDebug.ExtLog("PS Platform", "User not initialized");
				return 0;
			}
			return SaveCompatibility.LocalPlayerPrefs.GetInt($"ACH_{name}");
		}

		void IPlatform.SetAchievementStats(string name, int amount, int maxAmount, Action<bool> callback)
		{
		}

		void IPlatform.SetPresence(string id, params string[] extraInfo)
		{
		}

		void IPlatform.ClearPresence()
		{

		}

		float IPlatform.StreamingInstallProgress()
		{
			return 1f;
		}

		public string GetFileName()
		{
			return fileName;
		}


        public void UpdateActivity(string status, Action<bool> callback)
        {
            
        }
    }
#endif // UNITY_PS4

}