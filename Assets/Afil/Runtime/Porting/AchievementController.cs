using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AchievementController : MonoBehaviour
{

    private float snakesKilled;
    private float bushKilled;
    private float beesKilled;
    private int playerLevel;
    private float timeSurvived;
   
    private float deaths;

    public static AchievementController Instance { get; private set; }
    private bool Hangin_Tough = false;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.



        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //Atualizei os parametros para v2(dlc1) para evitar que ao entrar no DLC j� libere tudo visto que as variaveis j� est�o salvas
            snakesKilled = SaveCompatibility.LocalPlayerPrefs.GetFloat("snakesKilledv2");
            bushKilled = SaveCompatibility.LocalPlayerPrefs.GetFloat("bushKilledv2");
            beesKilled = SaveCompatibility.LocalPlayerPrefs.GetFloat("beesKilledv2");
            deaths = SaveCompatibility.LocalPlayerPrefs.GetFloat("deathsv2");
            DontDestroyOnLoad(this);
        }
    }
    public void ResetKillsCount()
    {
        snakesKilled = 0;
        bushKilled = 0;
        beesKilled = 0;
    }
    
    public string GetCurrentScene() 
    {

        return SceneManager.GetActiveScene().name;

    }
    public void AchievementsEnemiesKilled(int type) 
    {
#if !UNITY_SWITCH 
        Debug.Log("Archievement kills " + snakesKilled + "::"+ bushKilled + "::" + beesKilled);

        if (type == 0) 
        {
            Porting.PlatformManager.instance.UnlockAchievement("The_First_One");
            //Debug.Log("Archievement 01: THE FIRST ONE");

            snakesKilled++;

            SaveCompatibility.LocalPlayerPrefs.SetFloat("snakesKilledv2", snakesKilled);
            if (snakesKilled >= 15) 
            {
                Porting.PlatformManager.instance.UnlockAchievement("Snake_Hunter");
              //  Debug.Log("Archievement 02: SNAKE HUNTER");
            }
            if (snakesKilled >= 30)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Snake_Destroyer");
                //Debug.Log("Archievement 03: SNAKE DESTROYER");
            }
            if (snakesKilled >= 50)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Snake_Killer");
                //Debug.Log("Archievement 04: SNAKE KILLER");
            }
            if (snakesKilled >= 55)
            {
                Porting.PlatformManager.instance.UnlockAchievement("One_for_your_Money");
                Debug.Log("Archievement 25: One for your Money");
            }
            if (snakesKilled >= 60)
            {
                Porting.PlatformManager.instance.UnlockAchievement("A_Player_on_a_Mission");
                Debug.Log("Archievement 28: A Player on a Mission");
            }
        }
        else if (type == 1)
        {
            bushKilled++;
            SaveCompatibility.LocalPlayerPrefs.SetFloat("bushKilledv2", bushKilled);
            if (bushKilled >= 15)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Gardener_Hunter");
                //Debug.Log("Archievement 11: GARDENER HUNTER");
            }
            if (bushKilled >= 30)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Gardener_Destroyer");
                //Debug.Log("Archievement 12: GARDENER DESTROYER");
            }
            if (bushKilled >= 50)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Gardener_Killer");
                //Debug.Log("Archievement 13: GARDENER KILLER");
            }
            if (bushKilled >= 55)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Ultimate_Gardener");
                Debug.Log("Archievement 27: Ultimate Gardener");
            }
        }
        else if (type == 2)
        {
            beesKilled++;
            SaveCompatibility.LocalPlayerPrefs.SetFloat("beesKilledv2", beesKilled);
            if (beesKilled >= 15)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Bee_Hunter");
                //Debug.Log("Archievement 14: BEE HUNTER");
            }
            if (beesKilled >= 30)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Bee_Destroyer");
                //Debug.Log("Archievement 15: BEE DESTROYER");
            }
            if (beesKilled >= 50)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Bee_Killer");
                //Debug.Log("Archievement 16: BEE KILLER");
            }
            if (beesKilled >= 55)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Makin_Ninety_Miles_an_Hour");
                Debug.Log("Archievement 26: Makin' Ninety Miles an Hour");
            }
        }

#endif
    }

    public void AchievementsPlayer(int type) 
    {
#if !UNITY_SWITCH 

    
        if (type == 2)
        {
            deaths++;
            SaveCompatibility.LocalPlayerPrefs.SetFloat("deathsv2", deaths);

            if (deaths >= 10)
            {
                Porting.PlatformManager.instance.UnlockAchievement("One_More_Try");
                //Debug.Log("Archievement 19: ONE MORE TRY");
            }
            if (deaths >= 15)
            {
                Porting.PlatformManager.instance.UnlockAchievement("Fight_Without_Soul");
                Debug.Log("Archievement 24: Fight Without Soul");
            }
        }
        else if (type == 3)
        {
           
                Porting.PlatformManager.instance.UnlockAchievement("The_Power");
                //Debug.Log("Archievement 20: THE POWER");
            
        }
#endif
    }

    public void AchievementsLevelUp(int playerLevel)
    {
#if !UNITY_SWITCH

        //Debug.LogError($"Level Checked{playerLevel}");
        if (playerLevel >= 5)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Power_Force");
            //Debug.Log("Archievement 08: POWER FORCE");
        }
        if (playerLevel >= 10)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Super_Power_Force");
            //    Debug.Log("Archievement 09: SUPER POWER FORCE");
        }
        if (playerLevel >= 15)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Master_Power_Force");
            //  Debug.Log("Archievement 10: MASTER POWER FORCE");
        }
#endif
    }
    public void TimeAchievements(float time)
    {

#if !UNITY_SWITCH 
        ////Debug.LogError($"time { time}");

        if (time>=2)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Survivor_Beginner");
           // Debug.Log("Archievement 05: SURVIVOR BEGINNER");
        }
        if (time>=5)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Survivor_King");
            //Debug.Log("Archievement 06: SSURVIVOR KING");
        }
        if (time>=9)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Survivor_Master");
            //Debug.Log("Archievement 07: SURVIVOR MASTER");
        }
        if (time >= 10 && !Hangin_Tough )
        {
            Porting.PlatformManager.instance.UnlockAchievement("Hangin_Tough");
            Debug.Log("Archievement 21: Hangin_Tough");
            Hangin_Tough = true;
        }
#endif
    }
    public void AchievementsBoss(int type) 
    {
        if (type == 0)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Wild_Thorn");
            Debug.Log("Archievement 17: WILD THORN");
        }
        else if (type == 1)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Veteran_Hunter");
            Debug.Log("Archievement 17: VETERAN HUNTER");
        }
        else if (type == 2)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Primal_Kong");
            Debug.Log("Archievement 22: KONG");
        }
        else if (type == 3)
        {
            Porting.PlatformManager.instance.UnlockAchievement("Cursed_King_Wasp");
            Debug.Log("Archievement 23: Cursed King Wasp");
        }
    }
}
