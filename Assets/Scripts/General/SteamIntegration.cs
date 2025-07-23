using System;
using System.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace F
{
    public class SteamIntegration : MonoBehaviour
    {
        private void Awake()
        {
            if (SteamIntegration.Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                SteamIntegration.Instance = this;
                try
                {
                    SteamClient.Init(appID, true);
                    connectedToSteam = true;
                    return;
                }
                catch (Exception message)
                {
                    connectedToSteam = false;
                    Debug.Log(message);
                    return;
                }
            }
            if (SteamIntegration.Instance != null)
            {
                Destroy(base.gameObject);
            }
        }

        private void Update()
        {
            if (ConnectedToSteam)
            {
                SteamClient.RunCallbacks();
            }
        }

        private void OnApplicationQuit()
        {
            SteamClient.Shutdown();
        }

        public static void UnlockAchievement(string id)
        {
			if (!ConnectedToSteam) return;
            Achievement achievement = new Achievement(id);
            if (!achievement.State) achievement.Trigger(true);
        }

        public static void ClearAchievement(string id)
        {
			if (!ConnectedToSteam) return;
            Achievement achievement = new Achievement(id);
            achievement.Clear();
        }

        public async Task<Leaderboard> GetLeaderboardAsync(string leaderboardName)
        {
            Leaderboard result;
            try
            {
                result = (await SteamUserStats.FindOrCreateLeaderboardAsync(leaderboardName, LeaderboardSort.Descending, LeaderboardDisplay.Numeric)).Value;
            }
            catch (Exception message)
            {
                Debug.Log(message);
                result = default(Leaderboard);
            }
            return result;
        }

        public static void ClearAll()
        {
			if (!ConnectedToSteam) return;
            SteamIntegration.ClearAchievement("ACH_0");
            SteamIntegration.ClearAchievement("ACH_13k");
            SteamIntegration.ClearAchievement("ACH_6666");
            SteamIntegration.ClearAchievement("ACH_22");
        }

        private async void SubmitScore(int score)
        {
            try
            {
                SteamClient.Init(appID, true);
            }
            catch (Exception)
            {
            }
            LeaderboardUpdate? leaderboardUpdate = await (await this.GetLeaderboardAsync("Endless Mode")).SubmitScoreAsync(score, null);
            Debug.Log(string.Concat(new object[]
            {
                "Submitted: ",
                leaderboardUpdate.Value.Changed.ToString(),
                ", Score: ",
                leaderboardUpdate.Value.Score
            }));
        }

        private async void ReplaceScore(int score)
        {
            try
            {
                SteamClient.Init(appID, true);
            }
            catch (Exception)
            {
            }
            LeaderboardUpdate? leaderboardUpdate = await (await this.GetLeaderboardAsync("Endless Mode")).ReplaceScore(score, null);
            Debug.Log(string.Concat(new object[]
            {
                "Submitted: ",
                leaderboardUpdate.Value.Changed.ToString(),
                ", Score: ",
                leaderboardUpdate.Value.Score
            }));
        }

        [SerializeField]
        public string achievementID;

        private uint appID = 3381140;
        public static SteamIntegration Instance;

        private static bool connectedToSteam = false;

        public static bool ConnectedToSteam
        {
            get
            {
                return SteamClient.IsValid;
            }
            set
            {
                connectedToSteam = value;
            }
        }
    }

    #if UNITY_EDITOR
    [CustomEditor(typeof(F.SteamIntegration))]
    public class SteamIntegrationEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            F.SteamIntegration steamIntegration = (F.SteamIntegration)target;

            if (Application.isPlaying)
            {
                if (GUILayout.Button("Unlock Achievement"))
                {
                    SteamIntegration.UnlockAchievement(steamIntegration.achievementID);
                }

                if (GUILayout.Button("Clear Achievement"))
                {
                    SteamIntegration.ClearAchievement(steamIntegration.achievementID);
                }

                if (GUILayout.Button("Clear ALL"))
                {
                    SteamIntegration.ClearAll();
                }
            }
        }
    }
    #endif
}
