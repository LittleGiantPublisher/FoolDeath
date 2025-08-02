using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamepadManager : MonoBehaviour
{
    public static GamepadManager instance;
    private bool joystick = false;

    public bool Joystick { get => joystick; private set { joystick = value; } }
    public bool forceHideCursor;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(instance);
        }
        else if (instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        CheckJoystickOrMouse();

        
        HideShowMouse();
    }    

    public void CheckJoystickOrMouse()
    {

#if UNITY_STANDALONE

        joystick = (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.GAMEPAD);
#else
        joystick = true;
#endif
    }

    public void HideShowMouse()
    {

#if UNITY_STANDALONE

        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "Gameplay" )
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }

        if (ControllerManager.current.currentPCInput == ControllerManager.INPUT_TYPE.GAMEPAD)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = true;
        }
#else
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
   
#endif
    }
}
