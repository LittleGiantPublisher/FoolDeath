using System;
using Porting;
using UnityEngine;

namespace F
{
    [Serializable]
    public class AchievData
    {
        public int deathCardUsed;

        public int emeraldBought;
        public int saphireBought;
        public int rubyBought;

        public int emeraldUsed;
        public int saphireUsed;
        public int rubyUsed;
    }

    public static class AchievementCalls
    {
        public static void DeathCardUsed()
        {
            SaveSystem.achievData.deathCardUsed++;

            PlatformManager.instance.UnlockAchievement("WhispersOfDeath", SaveSystem.achievData.deathCardUsed / 3);
            PlatformManager.instance.UnlockAchievement("EchoOfDeath", SaveSystem.achievData.deathCardUsed / 5);
            PlatformManager.instance.UnlockAchievement("RelentlessDeath", SaveSystem.achievData.deathCardUsed / 7);
            PlatformManager.instance.UnlockAchievement("Reaper", SaveSystem.achievData.deathCardUsed / 10);
            PlatformManager.instance.UnlockAchievement("LordOfEndings", SaveSystem.achievData.deathCardUsed / 13);
            /*
                BASE Whispers of Death - 3

                DLC1 Echo of Death - 5
                DLC2 Relentless Death - 7
                DLC3 Reaper - 10
                DLC4 Lord of Endings - 13
            */
        }

        public static void CoinCollected(int coinAmnt)
        {
            PlatformManager.instance.UnlockAchievement("ThreeOfPentacles", coinAmnt / 3);
            PlatformManager.instance.UnlockAchievement("FourOfPentacles", coinAmnt / 4);
            PlatformManager.instance.UnlockAchievement("FiveOfPentacles", coinAmnt / 5);
            /*
                BASE Three of Pentacles - 3
                BASE Four of Pentacles - 4
                BASE Five of Pentacles - 5

            */
        }

        public static void KarmaChanged(int karmaValue)
        {
            Debug.Log($"Achievement KarmaChanged: {karmaValue}");

            if (karmaValue >= 100) PlatformManager.instance.UnlockAchievement("Sunlight", 1);
            if (karmaValue <= -100) PlatformManager.instance.UnlockAchievement("TheTowerHasFallen", 1);

            /*
                BASE Sunlight - 100+
                BASE The Tower Has Fallen - -100
                
            */
        }

        public static void PointChanged(int pointsTotal)
        {
            Debug.Log($"Achievement PointChanged: {pointsTotal}");
            PlatformManager.instance.UnlockAchievement("Temperance", pointsTotal / 1000);
            PlatformManager.instance.UnlockAchievement("TheMagician", pointsTotal / 2000);
            PlatformManager.instance.UnlockAchievement("TheWorld", pointsTotal / 3000);
            /*
                BASE Temperance - 1000
                BASE The Magician - 2000
                BASE The World - 3000
            */
        }

        public static void RoundNext(int roundInd, int meterValue)
        {
            Debug.Log($"Achievement RoundNext: {roundInd} {meterValue}");

            PlatformManager.instance.UnlockAchievement("FirstArcane", roundInd / 2);
            PlatformManager.instance.UnlockAchievement("TheWheelTurns", roundInd / 3);
            PlatformManager.instance.UnlockAchievement("FourthKey", roundInd / 4);
            PlatformManager.instance.UnlockAchievement("FifthArcana", roundInd / 5);
            PlatformManager.instance.UnlockAchievement("SixthSeal", roundInd / 6);
            PlatformManager.instance.UnlockAchievement("SeventhGate", roundInd / 7);

            if (meterValue == 0) PlatformManager.instance.UnlockAchievement("PerfectBalance", 1);
            /*
                Round ind
                BASE First Arcane - 2
                BASE The Wheel Turns - 3
                dlc1 Fourth Key - 4
                DLC2 Fifth Arcana - 5
                DLC3 The Sixth Seal - 6
                DLC4 The Seventh gate - 7

                Karma
                BASE Perfect Balance - 0
            */
        }

        public static void EmeraldBought()
        {
            SaveSystem.achievData.emeraldBought++;
            Debug.Log($"Achievement EmeraldBought: {SaveSystem.achievData.emeraldBought}");

            if (SaveSystem.achievData.emeraldBought >= 1) PlatformManager.instance.UnlockAchievement("HeartOfEmerald", 1);
            /*
                BASE Heart of Emerald - 1
            */
        }

        public static void SaphireBought()
        {
            SaveSystem.achievData.saphireBought++;
            Debug.Log($"Achievement SaphireBought: {SaveSystem.achievData.saphireBought}");

            if (SaveSystem.achievData.saphireBought >= 1) PlatformManager.instance.UnlockAchievement("SapphireTears", 1);
            /*
                BASE Sapphire Tears - 1
            */
        }

        public static void RubyBought()
        {
            SaveSystem.achievData.rubyBought++;
            Debug.Log($"Achievement RubyBought: {SaveSystem.achievData.rubyBought}");

            if (SaveSystem.achievData.rubyBought >= 1) PlatformManager.instance.UnlockAchievement("RubyBlood", 1);
            /*
                BASE Ruby Blood - 1
            */
        }

        public static void EmeraldUsed()
        {
            SaveSystem.achievData.emeraldUsed++;
            Debug.Log($"Achievement EmeraldUsed: {SaveSystem.achievData.emeraldUsed}");

            PlatformManager.instance.UnlockAchievement("FirstEnchantmentEmerald", SaveSystem.achievData.emeraldUsed / 1);
            PlatformManager.instance.UnlockAchievement("TrinityOfEmeralds", SaveSystem.achievData.emeraldUsed / 3);
            PlatformManager.instance.UnlockAchievement("GreenPentacle", SaveSystem.achievData.emeraldUsed / 5);
            PlatformManager.instance.UnlockAchievement("EightFacetsEmerald", SaveSystem.achievData.emeraldUsed / 8);
            PlatformManager.instance.UnlockAchievement("SupremeJewelEmerald", SaveSystem.achievData.emeraldUsed / 10);
            /*
                BASE First Enchantment – Emerald - 1
                DLC1 Trinity of Emeralds - 3
                DLC2 Green Pentacle - 5
                DLC3 Eight Facets – Emerald - 8
                DLC4 Supreme Jewel – Emerald - 10
            */
        }

        public static void SaphireUsed()
        {
            SaveSystem.achievData.saphireUsed++;
            Debug.Log($"Achievement SaphireUsed: {SaveSystem.achievData.saphireUsed}");

            PlatformManager.instance.UnlockAchievement("FirstEnchantmentSapphire", SaveSystem.achievData.saphireUsed / 1);
            PlatformManager.instance.UnlockAchievement("TrinityOfSapphires", SaveSystem.achievData.saphireUsed / 3);
            PlatformManager.instance.UnlockAchievement("BluePentacle", SaveSystem.achievData.saphireUsed / 5);
            PlatformManager.instance.UnlockAchievement("EightFacetsSapphire", SaveSystem.achievData.saphireUsed / 8);
            PlatformManager.instance.UnlockAchievement("SupremeJewelSapphire", SaveSystem.achievData.saphireUsed / 10);
            /*
                BASE First Enchantment – Sapphire - 1
                DLC1 Trinity of Sapphires - 3
                DLC2 Blue Pentacle - 5
                DLC3 Eight Facets – Sapphire - 8
                DLC4 Supreme Jewel – Sapphire - 10
            */
        }

        public static void RubyUsed()
        {
            SaveSystem.achievData.rubyUsed++;
            Debug.Log($"Achievement RubyUsed: {SaveSystem.achievData.rubyUsed}");

            PlatformManager.instance.UnlockAchievement("FirstEnchantmentRuby", SaveSystem.achievData.rubyUsed / 1);
            PlatformManager.instance.UnlockAchievement("TrinityOfRubies", SaveSystem.achievData.rubyUsed / 3);
            PlatformManager.instance.UnlockAchievement("RedPentacle", SaveSystem.achievData.rubyUsed / 5);
            PlatformManager.instance.UnlockAchievement("EightFacetsRuby", SaveSystem.achievData.rubyUsed / 8);
            PlatformManager.instance.UnlockAchievement("SupremeJewelRuby", SaveSystem.achievData.rubyUsed / 10);
            /*
                BASE First Enchantment – Ruby - 1
                DLC1 Trinity of Rubies - 3
                DLC2 Red Pentacle - 5
                DLC3 Eight Facets – Ruby - 8
                DLC4 Supreme Jewel – Ruby - 10
            */
        }
    }
}
