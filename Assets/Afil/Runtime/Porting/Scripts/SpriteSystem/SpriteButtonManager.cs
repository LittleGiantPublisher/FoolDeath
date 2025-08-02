using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteButtonManager : MonoBehaviour
{


    public static SpriteButtonManager current;

    public List<BUTTON_SPRITE> PS4;
    public List<BUTTON_SPRITE> PS5;
    public List<BUTTON_SPRITE> XB1;
    public List<BUTTON_SPRITE> SWITCH;
    public List<BUTTON_SPRITE> KEYBOARD;

    private void Awake()
    {
        if (current != null)
            Destroy(gameObject);

        current = this;
        DontDestroyOnLoad(this);
    }

    public Sprite GetSpriteButton(BUTTON_ID button)
    {

#if UNITY_PS4 
        foreach(BUTTON_SPRITE b in PS4)
#elif UNITY_PS5
        foreach(BUTTON_SPRITE b in PS5)
#elif UNITY_SWITCH
        foreach (BUTTON_SPRITE b in SWITCH)
#elif UNITY_GAMECORE || UNITY_STANDALONE || MICROSOFT_GAME_CORE || UNITY_WSA
        foreach (BUTTON_SPRITE b in XB1)
#endif
        {
            if (b.BUTTON_ID == button)
            {
#if UNITY_PS4 || UNITY_SWITCH || UNITY_EDITOR
               if(Porting.PlatformManager.enterButtonParam == 0 && 
                 (button == BUTTON_ID.SOUTH || button == BUTTON_ID.EAST))
                {
                    return b.Alt_Sprite;
                }
#endif
                return b.Sprite;
            }
        }
        return null;
    }
    public Sprite GetSpriteButton(L_STICK_ID stick)
    {

#if UNITY_PS4
        foreach(BUTTON_SPRITE b in PS4)
#elif UNITY_PS5
        foreach(BUTTON_SPRITE b in PS5)
#elif UNITY_SWITCH
        foreach (BUTTON_SPRITE b in SWITCH)
#elif UNITY_GAMECORE || UNITY_STANDALONE|| MICROSOFT_GAME_CORE || UNITY_WSA
        foreach (BUTTON_SPRITE b in XB1)
#endif
        {
            if (b.L_STICK_ID == stick)
            {
                return b.Sprite;
            }
        }
        return null;
    }
    public Sprite GetSpriteButton(R_STICK_ID stick)
    {

#if UNITY_PS4
        foreach(BUTTON_SPRITE b in PS4)
#elif UNITY_PS5
        foreach(BUTTON_SPRITE b in PS5)
#elif UNITY_SWITCH
        foreach (BUTTON_SPRITE b in SWITCH)
#elif UNITY_GAMECORE || UNITY_STANDALONE|| MICROSOFT_GAME_CORE || UNITY_WSA
        foreach (BUTTON_SPRITE b in XB1)
#endif
        {
            if (b.R_STICK_ID == stick)
            {
                return b.Sprite;
            }
        }
        return null;
    }
    public Sprite GetSpriteButton(DPAD_ID dpad)
    {

#if UNITY_PS4
        foreach(BUTTON_SPRITE b in PS4)
#elif UNITY_PS5
        foreach(BUTTON_SPRITE b in PS5)
#elif UNITY_SWITCH
        foreach (BUTTON_SPRITE b in SWITCH)
#elif UNITY_GAMECORE || UNITY_STANDALONE || MICROSOFT_GAME_CORE || UNITY_WSA
        foreach (BUTTON_SPRITE b in XB1)
#endif
        {
            if (b.DPAD_ID == dpad)
            {
                return b.Sprite;
            }
        }
        return null;
    }


    public Sprite GetPCSpriteButton(BUTTON_ID button,bool forceAlt =false)
    {
        foreach (BUTTON_SPRITE b in KEYBOARD)
        {
            if (b.BUTTON_ID == button)
            {
                if(!forceAlt)
                    return b.Sprite;
                else
                    return b.Alt_Sprite;
            }
        }
        return null;
    }
    public Sprite GetPCSpriteButton(L_STICK_ID stick, bool forceAlt = false)
    {



        foreach (BUTTON_SPRITE b in KEYBOARD)
        {
            if (b.L_STICK_ID == stick)
            {
                if (!forceAlt)
                    return b.Sprite;
                else
                    return b.Alt_Sprite;
            }
        }
        return null;
    }
    public Sprite GetPCSpriteButton(R_STICK_ID stick, bool forceAlt = false)
    {


        foreach (BUTTON_SPRITE b in KEYBOARD)
        {
            if (b.R_STICK_ID == stick)
            {
                if (!forceAlt)
                    return b.Sprite;
                else
                    return b.Alt_Sprite;
            }
        }
        return null;
    }
    public Sprite GetPCSpriteButton(DPAD_ID dpad, bool forceAlt = false)
    {


        foreach (BUTTON_SPRITE b in KEYBOARD)
        {
            if (b.DPAD_ID == dpad)
            {
                if (!forceAlt)
                    return b.Sprite;
                else
                    return b.Alt_Sprite;
            }
        }
        return null;
    }
}

[System.Serializable]
public struct BUTTON_SPRITE
{
    public BUTTON_ID BUTTON_ID;
    public L_STICK_ID L_STICK_ID;
    public R_STICK_ID R_STICK_ID;
    public DPAD_ID DPAD_ID;

    public Sprite Sprite;
    public Sprite Alt_Sprite;
}


public enum BUTTON_ID
{
    NONE=-1,
    SOUTH,
    EAST,
    NORTH, 
    WEST,
    
    RIGHT_BUMPER,
    LEFT_BUMPER,
    RIGHT_TRIGGER,
    LEFT_TRIGGER,

    SELECT,
    START,


}
public enum L_STICK_ID
{
    NONE = -1,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ALL,
   

}
public enum R_STICK_ID
{
    NONE = -1,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ALL,


}
public enum DPAD_ID
{
    NONE = -1,
    UP,
    DOWN,
    LEFT,
    RIGHT,
    ALL,


}