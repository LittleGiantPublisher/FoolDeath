using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IPlatform
{
	string GetUserGamertag();
	abstract string GetFileName();
	
	//Platform Needed Calls
	void OnPlatformAwake();
	void OnPlatformStart();
	void OnPlatformUpdate();
	//------------------------


	//Platform User Calls
	void RequestUserSignIn(int padIndex, Action<bool /*success*/> callback);
	void RequestUserSignOut(int padIndex, Action<bool /*success*/> callback);
	bool HasUserConnected();

	//------------------------------------------------------------------------

	//Platform Save - Load
	void SaveGameData(byte[] rawData, Action<bool /*success*/> callback);
	void LoadGameData(Action<bool /*success*/, byte[] /*RawData*/> callback);
	//------------------------------------------------------------------------


	//Platform Presence
	void SetPresence(string id, params string[] extraInfo);
	void ClearPresence();

	//--------------------


	//Platform Achievements
	void UnlockAchievement(int trophyID, Action<bool /*success*/> callback);
	bool CheckAchievement(string name);
	int GetAchievementStats(string name);
	void SetAchievementStats(string name, int amount, int maxAmount, Action<bool /*success*/> callback);
	//--------------------------------------------------------------------------------------------------


	//Platform Activity
	void UpdateActivity(string status, Action<bool> callback);

	//--------------------------------------------------------------------------------------------------

	//PlayGo
	float StreamingInstallProgress(); // Must return 1 if not used by current platform

}
