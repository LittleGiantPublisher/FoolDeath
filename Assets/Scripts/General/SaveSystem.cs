using System;
using System.IO;
using UnityEngine;
using SaveCompatibility;
using Porting;

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
            Debug.Log($"DATA LOADED {SaveSystem.data.coins} {SaveSystem.data.rounds} {SaveSystem.data.points}");
        }

        public static void Save()
        {
            LocalPlayerPrefs.SetInt("coins", SaveSystem.data.coins);
            LocalPlayerPrefs.SetInt("rounds", SaveSystem.data.rounds);
            LocalPlayerPrefs.SetInt("points", SaveSystem.data.points);
            LocalPlayerPrefs.Save();
            Debug.Log($"DATA SAVED {SaveSystem.data.coins} {SaveSystem.data.rounds} {SaveSystem.data.points}");
        }

        public static SaveData data;
    }
}