using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class SpriteButton : MonoBehaviour
{
    Image img;
   SpriteRenderer sprite;
    public bool UsePCButtons;
    public bool ForceAlt;
    public MULTIPLYING_PLATFORM MULTIPLYING = MULTIPLYING_PLATFORM.NOT_USE;
    public bool xb1,
        ms,
        nintendo,
        ps4,
        ps5;
  
    public Vector3 base_scale;
    public float multiplyScale = 1;
    public MODE MODE_BUTTON;
    public BUTTON_ID BUTTON_ID = BUTTON_ID.NONE;
    public L_STICK_ID L_STICK_ID = L_STICK_ID.NONE;
    public R_STICK_ID R_STICK_ID = R_STICK_ID.NONE;
    public DPAD_ID DPAD_ID = DPAD_ID.NONE;

    private void Start()
    {
        base_scale = transform.localScale;
        if (MULTIPLYING == MULTIPLYING_PLATFORM.USE)
        {
            Vector3 temp = base_scale * multiplyScale;

            if (ms)
            {
#if (UNITY_STANDALONE)
                transform.localScale = temp;
#endif
            }
            if (xb1)
            {
#if (UNITY_GAMECORE)
                    transform.localScale = temp;
#endif
            }
            if (nintendo)
            {
#if (UNITY_SWITCH)
                    transform.localScale = temp;
#endif
            }
            if (ps4)
            {
#if (UNITY_PS4)
                    transform.localScale = temp;
#endif
            }
            if (ps5)
            {
#if (UNITY_PS5)
                    transform.localScale = temp;
#endif
            }
        }
    }

    private void OnEnable()
    {
        
        img = GetComponent<Image>();
        if (img == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }
        if (MULTIPLYING == MULTIPLYING_PLATFORM.USE)
        {
            Vector3 temp = base_scale * multiplyScale;

            if (ms)
            {
#if (UNITY_STANDALONE)
                transform.localScale = temp;
#endif
            }
            if (xb1)
            {
#if (UNITY_GAMECORE)
                    transform.localScale = temp;
#endif
            }
            if (nintendo)
            {
#if (UNITY_SWITCH)
                    transform.localScale = temp;
#endif
            }
            if (ps4)
            {
#if (UNITY_PS4)
                    transform.localScale = temp;
#endif
            }
            if (ps5)
            {
#if (UNITY_PS5)
                    transform.localScale = temp;
#endif
            }
        }
        ControllerManager.UpdateGlyphs += UpdateGlyphs;

#if UNITY_STANDALONE
        StartCoroutine(UpdateSprites(UsePCButtons));
#else
        StartCoroutine(UpdateSprites());
#endif
    }
    private void OnDisable()
    {
        ControllerManager.UpdateGlyphs -= UpdateGlyphs;
    }


    // Update is called once per frame
    IEnumerator UpdateSprites()
    {
        while (SpriteButtonManager.current == null)
            yield return new WaitForEndOfFrame();


        switch (MODE_BUTTON)
        {
            case MODE.BUTTONS:
                if(img != null)
                    img.sprite = SpriteButtonManager.current.GetSpriteButton(BUTTON_ID);
                else
                    sprite.sprite = SpriteButtonManager.current.GetSpriteButton(BUTTON_ID);
                break;
            case MODE.DPAD:
                

                if (img != null)
                    img.sprite = SpriteButtonManager.current.GetSpriteButton(DPAD_ID);
                else
                    sprite.sprite = SpriteButtonManager.current.GetSpriteButton(DPAD_ID);
                break;
            case MODE.L_STICK:

                if (img != null)
                    img.sprite = SpriteButtonManager.current.GetSpriteButton(L_STICK_ID); 
                else
                    sprite.sprite = SpriteButtonManager.current.GetSpriteButton(L_STICK_ID);
                break;
                
            case MODE.R_STICK:
                
                if (img != null)
                    img.sprite = SpriteButtonManager.current.GetSpriteButton(R_STICK_ID);
                else
                    sprite.sprite = SpriteButtonManager.current.GetSpriteButton(R_STICK_ID);
                break;
        }
        if (MULTIPLYING == MULTIPLYING_PLATFORM.USE)
        {
            Vector3 temp = base_scale * multiplyScale;

            if (ms)
            {
#if (UNITY_STANDALONE)
                transform.localScale = temp;
#endif
            }
            if (xb1)
            {
#if (UNITY_GAMECORE)
                    transform.localScale = temp;
#endif
            }
            if (nintendo)
            {
#if (UNITY_SWITCH)
                    transform.localScale = temp;
#endif
            }
            if (ps4)
            {
#if (UNITY_PS4)
                    transform.localScale = temp;
#endif
            }
            if (ps5)
            {
#if (UNITY_PS5)
                    transform.localScale = temp;
#endif
            }
        }

    }
    IEnumerator UpdateSprites(bool usePC)
    {
        while (SpriteButtonManager.current == null)
            yield return new WaitForEndOfFrame();


        switch (MODE_BUTTON)
        {
            case MODE.BUTTONS:

                if (usePC)
                {
                    if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.KEYBOARD)
                    {
                        if (img != null)
                        {
                            img.sprite = SpriteButtonManager.current.GetPCSpriteButton(BUTTON_ID,ForceAlt);
                        }
                        else
                            sprite.sprite = SpriteButtonManager.current.GetPCSpriteButton(BUTTON_ID, ForceAlt);
                    }
                    else
                    {
                        if (img != null)
                            img.sprite = SpriteButtonManager.current.GetSpriteButton(BUTTON_ID);
                        else
                            sprite.sprite = SpriteButtonManager.current.GetSpriteButton(BUTTON_ID);
                    }
                }
                else
                {
                    if (img != null)
                        img.sprite = SpriteButtonManager.current.GetSpriteButton(BUTTON_ID);
                    else
                        sprite.sprite = SpriteButtonManager.current.GetSpriteButton(BUTTON_ID);
                }
                break;
            case MODE.DPAD:
                if (usePC)
                {
                    if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.KEYBOARD)
                    {
                        if (img != null)
                        {
                            img.sprite = SpriteButtonManager.current.GetPCSpriteButton(DPAD_ID, ForceAlt);
                            //img.SetNativeSize();
                        }
                        else
                            sprite.sprite = SpriteButtonManager.current.GetPCSpriteButton(DPAD_ID, ForceAlt);
                    }
                    else
                    {
                        if (img != null)
                            img.sprite = SpriteButtonManager.current.GetSpriteButton(DPAD_ID);
                        else
                            sprite.sprite = SpriteButtonManager.current.GetSpriteButton(DPAD_ID);
                    }
                }
                else
                {
                    if (img != null)
                        img.sprite = SpriteButtonManager.current.GetSpriteButton(DPAD_ID);
                    else
                        sprite.sprite = SpriteButtonManager.current.GetSpriteButton(DPAD_ID);
                }
                break;
            case MODE.L_STICK:

                if (usePC)
                {
                    if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.KEYBOARD)
                    {
                        if (img != null)
                        {
                            img.sprite = SpriteButtonManager.current.GetPCSpriteButton(L_STICK_ID, ForceAlt);
                            img.transform.localScale = new Vector3(1.5f, 1.5f);
                        }
                        else
                        {
                            sprite.sprite = SpriteButtonManager.current.GetPCSpriteButton(L_STICK_ID, ForceAlt);
                            sprite.transform.localScale = new Vector3(1.5f, 1.5f);
                        }
                    }
                    else
                    {
                        if (img != null)
                        {
                            img.sprite = SpriteButtonManager.current.GetSpriteButton(L_STICK_ID);
                            img.transform.localScale = Vector3.one;
                        }
                        else
                        {
                            sprite.sprite = SpriteButtonManager.current.GetSpriteButton(L_STICK_ID);
                            sprite.transform.localScale = Vector3.one;
                        }
                    }
                }
                else
                {
                    if (img != null)
                        img.sprite = SpriteButtonManager.current.GetSpriteButton(L_STICK_ID);
                    else
                        sprite.sprite = SpriteButtonManager.current.GetSpriteButton(L_STICK_ID);
                }
                break;

            case MODE.R_STICK:


                if (usePC)
                {
                    if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.KEYBOARD)
                    {
                        if (img != null)
                        {
                            img.sprite = SpriteButtonManager.current.GetPCSpriteButton(R_STICK_ID, ForceAlt);
                            img.transform.localScale = new Vector3(1.5f, 1.5f);
                        }
                        else
                        {
                            sprite.sprite = SpriteButtonManager.current.GetPCSpriteButton(R_STICK_ID, ForceAlt);
                            sprite.transform.localScale = new Vector3(1.5f, 1.5f);
                        }
                    }
                    else
                    {
                        if (img != null)
                        {
                            img.sprite = SpriteButtonManager.current.GetSpriteButton(R_STICK_ID);
                            img.transform.localScale = Vector3.one;
                        }
                        else
                        {
                            sprite.sprite = SpriteButtonManager.current.GetSpriteButton(R_STICK_ID);
                            sprite.transform.localScale = Vector3.one;
                        } 
                    }
                }
                else
                {
                    if (img != null)
                        img.sprite = SpriteButtonManager.current.GetSpriteButton(R_STICK_ID);
                    else
                        sprite.sprite = SpriteButtonManager.current.GetSpriteButton(R_STICK_ID);
                }
                break;
        }


        if (MULTIPLYING == MULTIPLYING_PLATFORM.USE)
        {
            Vector3 temp = base_scale * multiplyScale;

            if (ms)
            {
#if (UNITY_STANDALONE)
                transform.localScale = temp;
#endif
            }
            if (xb1)
            {
#if (UNITY_GAMECORE)
                    transform.localScale = temp;
#endif
            }
            if (nintendo)
            {
#if (UNITY_SWITCH)
                    transform.localScale = temp;
#endif
            }
            if (ps4)
            {
#if (UNITY_PS4)
                    transform.localScale = temp;
#endif
            }
            if (ps5)
            {
#if (UNITY_PS5)
                    transform.localScale = temp;
#endif
            }
        }

    }
    private void UpdateGlyphs()
    {
      //  //Debug.LogError("FLASJFLAJFLAJFLA");

        img = GetComponent<Image>();
        if (img == null)
        {
            sprite = GetComponent<SpriteRenderer>();
        }

        StartCoroutine(UpdateSprites(UsePCButtons));
    }

    public enum MODE
    {
        BUTTONS,
        L_STICK,
        R_STICK,
        DPAD,
    }

    public enum MULTIPLYING_PLATFORM
    {
        NOT_USE,
        USE,
    }
}
