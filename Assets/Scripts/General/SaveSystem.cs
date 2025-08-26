using System;
using System.IO;
using UnityEngine;
using SaveCompatibility;
using Porting;
using UnityEngine.SocialPlatforms;

namespace F
{
    public static class SaveSystem
    {
        public static void Load()
        {
            SaveSystem.data = new SaveData();
            SaveSystem.data.coins = LocalPlayerPrefs.GetInt("coins", 0);
            SaveSystem.data.rounds = LocalPlayerPrefs.GetInt("rounds", 0);
            SaveSystem.data.points = LocalPlayerPrefs.GetInt("points", 0);

            // Load achievements
            SaveSystem.achievData = new AchievData();
            SaveSystem.achievData.deathCardUsed = LocalPlayerPrefs.GetInt("deathCardUsed", 0);
            SaveSystem.achievData.emeraldBought = LocalPlayerPrefs.GetInt("emeraldBought", 0);
            SaveSystem.achievData.saphireBought = LocalPlayerPrefs.GetInt("saphireBought", 0);
            SaveSystem.achievData.rubyBought = LocalPlayerPrefs.GetInt("rubyBought", 0);
            SaveSystem.achievData.emeraldUsed = LocalPlayerPrefs.GetInt("emeraldUsed", 0);
            SaveSystem.achievData.saphireUsed = LocalPlayerPrefs.GetInt("saphireUsed", 0);
            SaveSystem.achievData.rubyUsed = LocalPlayerPrefs.GetInt("rubyUsed", 0);

            Debug.Log($"DATA LOADED {SaveSystem.data.coins} {SaveSystem.data.rounds} {SaveSystem.data.points}");
        }

        public static void Save()
        {
            LocalPlayerPrefs.SetInt("coins", SaveSystem.data.coins);
            LocalPlayerPrefs.SetInt("rounds", SaveSystem.data.rounds);
            LocalPlayerPrefs.SetInt("points", SaveSystem.data.points);

            // Save Achievements
            LocalPlayerPrefs.SetInt("deathCardUsed", SaveSystem.achievData.deathCardUsed);
            LocalPlayerPrefs.SetInt("emeraldBought", SaveSystem.achievData.emeraldBought);
            LocalPlayerPrefs.SetInt("saphireBought", SaveSystem.achievData.saphireBought);
            LocalPlayerPrefs.SetInt("rubyBought", SaveSystem.achievData.rubyBought);
            LocalPlayerPrefs.SetInt("emeraldUsed", SaveSystem.achievData.emeraldUsed);
            LocalPlayerPrefs.SetInt("saphireUsed", SaveSystem.achievData.saphireUsed);
            LocalPlayerPrefs.SetInt("rubyUsed", SaveSystem.achievData.rubyUsed);

            // Call save
            LocalPlayerPrefs.Save();
        }

        public static SaveData data;
        public static AchievData achievData;

    }
}