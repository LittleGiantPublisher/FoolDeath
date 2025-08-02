using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using Reissoft;
#endif
public enum AfilPlatforms {STANDALONE, GAME_CORE,PLAYSTATION_4,PLAYSTATION_5,NINTENDO_SWITCH,MS_STORE,ALL }
public class AfilPortingSystems : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Usado para definir quais prefabs serão instanciados por plataforma no inicio do jogo (Start)
    /// </summary>    
    [SerializeField] private ObjectsByPlatform[] objectsByPlatform;
#if UNITY_EDITOR
    [InfoBox("SCID para MS Store")]
#endif
    [SerializeField] private string scid = "";
#if UNITY_EDITOR
    [HideLabel, InfoBox("Este Component é usado para definir quais prefabs serão instanciados por plataforma no inicio do jogo (Start)")]
    public bool info;
#endif
    [Space(50)]
#if UNITY_EDITOR
    [InfoBox("Patch atual do jogo")]
#endif
    [SerializeField] private UNLOCK_CHECK patch = UNLOCK_CHECK.GAME_BASE;

    #endregion
    #region Messages

    private void Awake()
    {
#if(MICROSOFT_GAME_CORE)
        MS_Platform.scid = scid;
#endif

        Porting.PlatformManager.CUR_PATCH = patch;
    }
    // Start is called before the first frame update
    void Start()
    {
        PopulePrefabs();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    #endregion
    #region Methods
    private void PopulePrefabs()
    {
        foreach(var platform in objectsByPlatform)
        {
            Popule(platform);
        }
    }

    private void Popule(ObjectsByPlatform platform)
    {
        //Cria os em comum (todos)
        if(platform.platform == AfilPlatforms.ALL)
        {
            Create(platform.gameObjects,platform.dontDestroy);
        }

#if UNITY_GAMECORE 
        //Cria XBox Game Core
        if (platform.platform == AfilPlatforms.GAME_CORE)
        {
            Create(platform.gameObjects, platform.dontDestroy);
        }
#elif UNITY_PS4 
        //Cria PS4
        if (platform.platform == AfilPlatforms.PLAYSTATION_4)
        {
            Create(platform.gameObjects, platform.dontDestroy);
        }
#elif UNITY_PS5 
        //Cria PS4
        if (platform.platform == AfilPlatforms.PLAYSTATION_5)
        {
            Create(platform.gameObjects, platform.dontDestroy);
        }
#elif UNITY_SWITCH 
    //Cria Switch
        if (platform.platform == AfilPlatforms.NINTENDO_SWITCH)
        {
            Create(platform.gameObjects, platform.dontDestroy);
        }
#elif MICROSOFT_GAME_CORE || UNITY_STANDALONE
        //Cria MS Store
        if (platform.platform == AfilPlatforms.MS_STORE)
        {
            Create(platform.gameObjects, platform.dontDestroy);
        }
#endif

    }

    private void Create(GameObject[] gameObjects, bool dontDestroy)
    {
        if (gameObject == null)
            return;

        foreach(var obj in gameObjects)
        {
            if (obj == null)
                continue;

            GameObject go = GameObject.Instantiate(obj);
            go.name = "[Afil_Porting] " + obj.name;

            if (dontDestroy)
                DontDestroyOnLoad(go);
        }
    }
    #endregion

}
#region Structs
/// <summary>
/// Usado para definir quais prefabs serão instanciados por plataforma no inicio do jogo
/// </summary>
[System.Serializable]
public struct ObjectsByPlatform
{
    public AfilPlatforms platform;
    public GameObject[] gameObjects;
    public bool dontDestroy;
}
#endregion
