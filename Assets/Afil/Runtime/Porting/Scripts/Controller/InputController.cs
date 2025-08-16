using System;
using UnityEngine.InputSystem;
using UnityEngine;

[Serializable]
public class InputController
{
   

    public enum ACTION_MAP
    {
        UI,
        PLAYER,
    }
    PlayerInput playerInput;
    public InputPlayer inputPlayerActions;
    float leftAxisDeadzone = 0.25f;
    float rightAxisDeadzone = 0.25f;

    public bool anyButton;
    //Left Axis
    [SerializeField]
    public Vector2 leftStickAxis;
    [SerializeField]
    public bool leftStickButton;
    [SerializeField]
    bool leftAxis_UpPressed;
    [SerializeField]
    public bool leftAxis_LeftPressed;
    [SerializeField]
    public bool leftAxis_DownPressed;
    [SerializeField]
    public bool leftAxis_RightPressed;


    //Right Axis
    [SerializeField]
    public Vector2 rightStickAxis;
    [SerializeField]
    public bool rightStickButton;
    [SerializeField]
    public bool rightAxis_UpPressed;
    [SerializeField]
    public bool rightAxis_LeftPressed;
    [SerializeField]
    public bool rightAxis_DownPressed;
    [SerializeField]
    public bool rightAxis_RightPressed;



    [SerializeField]
    Vector2 dPad;
    [SerializeField]
    public bool dPad_UpPressed;
    [SerializeField]
    public bool dPad_LeftPressed;
    [SerializeField]
    public bool dPad_DownPressed;
    [SerializeField]
    public bool dPad_RightPressed;



    [SerializeField]
    public bool buttonSouth;
    [SerializeField]
    public bool buttonEast;
    [SerializeField]
    public bool buttonNorth;
    [SerializeField]
    public bool buttonWest;

    [SerializeField]
    public bool confirmButton;
    [SerializeField]
    public bool cancelButton;



    [SerializeField]
    public bool buttonLeftBumper;
    [SerializeField]
    public bool buttonLeftTrigger;
    [SerializeField]
    public bool buttonRightBumper;
    [SerializeField]
    public bool buttonRightTrigger;


    [SerializeField]
    public bool buttonStart;
    [SerializeField]
    public bool buttonSelect;

    public InputController (PlayerInput player, ref InputPlayer inputActions)
    {
        playerInput = player;
        inputPlayerActions = inputActions;
        Init();
    }

    void Init()
    {
        inputPlayerActions = new InputPlayer();
    
        inputPlayerActions.Player.LeftStick.started += LeftAxis;
        inputPlayerActions.Player.LeftStick.performed += LeftAxis;
        inputPlayerActions.Player.LeftStick.canceled += LeftAxis;

        inputPlayerActions.Player.LeftStickButton.started += LeftAxisButton;
        inputPlayerActions.Player.LeftStickButton.performed += LeftAxisButton;
        inputPlayerActions.Player.LeftStickButton.canceled += LeftAxisButton;




        inputPlayerActions.Player.RightStick.started += RightAxis;
        inputPlayerActions.Player.RightStick.performed += RightAxis;
        inputPlayerActions.Player.RightStick.canceled += RightAxis;

        inputPlayerActions.Player.RightStickButton.started += RightAxisButton;
        inputPlayerActions.Player.RightStickButton.performed += RightAxisButton;
        inputPlayerActions.Player.RightStickButton.canceled += RightAxisButton;





        inputPlayerActions.Player.Dpad.started += Dpad;
        inputPlayerActions.Player.Dpad.performed += Dpad;
        inputPlayerActions.Player.Dpad.canceled += Dpad;



        inputPlayerActions.Player.ButtonSouth.started += ButtonSouth;
        inputPlayerActions.Player.ButtonSouth.performed += ButtonSouth;
        inputPlayerActions.Player.ButtonSouth.canceled += ButtonSouth;


        inputPlayerActions.Player.ButtonEast.started += ButtonEast;
        inputPlayerActions.Player.ButtonEast.performed += ButtonEast;
        inputPlayerActions.Player.ButtonEast.canceled += ButtonEast;


        inputPlayerActions.Player.ButtonNorth.started += ButtonNorth;
        inputPlayerActions.Player.ButtonNorth.performed += ButtonNorth;
        inputPlayerActions.Player.ButtonNorth.canceled += ButtonNorth;


        inputPlayerActions.Player.ButtonWest.started += ButtonWest;
        inputPlayerActions.Player.ButtonWest.performed += ButtonWest;
        inputPlayerActions.Player.ButtonWest.canceled += ButtonWest;

        inputPlayerActions.UI.Confirm.started += Confirm;
        inputPlayerActions.UI.Confirm.performed += Confirm;
        inputPlayerActions.UI.Confirm.canceled += Confirm;


        inputPlayerActions.UI.Cancel.started += Cancel;
        inputPlayerActions.UI.Cancel.performed += Cancel;
        inputPlayerActions.UI.Cancel.canceled += Cancel;

        inputPlayerActions.UI.Nav.started += Nav;
        inputPlayerActions.UI.Nav.performed += Nav;
        inputPlayerActions.UI.Nav.canceled += Nav;


        inputPlayerActions.Player.LeftBumper.started += ButtonLeftBumper;
        inputPlayerActions.Player.LeftBumper.performed += ButtonLeftBumper;
        inputPlayerActions.Player.LeftBumper.canceled += ButtonLeftBumper;


        inputPlayerActions.Player.LeftTrigger.started += ButtonLeftTrigger;
        inputPlayerActions.Player.LeftTrigger.performed += ButtonLeftTrigger;
        inputPlayerActions.Player.LeftTrigger.canceled += ButtonLeftTrigger;


        inputPlayerActions.Player.RightBumper.started += ButtonRightBumper;
        inputPlayerActions.Player.RightBumper.performed += ButtonRightBumper;
        inputPlayerActions.Player.RightBumper.canceled += ButtonRightBumper;


        inputPlayerActions.Player.RightTrigger.started += ButtonRightTrigger;
        inputPlayerActions.Player.RightTrigger.performed += ButtonRightTrigger;
        inputPlayerActions.Player.RightTrigger.canceled += ButtonRightTrigger;


        inputPlayerActions.Player.Start.started += ButtonStart;
        inputPlayerActions.Player.Start.performed += ButtonStart;
        inputPlayerActions.Player.Start.canceled += ButtonStart;


        inputPlayerActions.Player.Select.started += ButtonSelect;
        inputPlayerActions.Player.Select.performed += ButtonSelect;
        inputPlayerActions.Player.Select.canceled += ButtonSelect;

        inputPlayerActions.Player.Mouse.started += Mouse;
        inputPlayerActions.Player.Mouse.performed += Mouse;
        inputPlayerActions.Player.Mouse.canceled += Mouse;

        inputPlayerActions.Player.RightClick.started += RightClick;
        inputPlayerActions.Player.RightClick.performed += RightClick;
        inputPlayerActions.Player.RightClick.canceled += RightClick;

        inputPlayerActions.Player.LeftClick.started += LeftClick;
        inputPlayerActions.Player.LeftClick.performed += LeftClick;
        inputPlayerActions.Player.LeftClick.canceled += LeftClick;

        inputPlayerActions.Player.AnyKeyDown.started += AnyKeyDown;
        inputPlayerActions.Player.AnyKeyDown.performed += AnyKeyDown;
        inputPlayerActions.Player.AnyKeyDown.canceled += AnyKeyDown;

        inputPlayerActions.Enable();
    }

    public void UpdateInput()
    {
        

    }
  
    public void ActiveInput(bool active)
    {
        if (active)
        {
            if (!playerInput.inputIsActive)
            {
                playerInput.ActivateInput();
            }
        }
        else
        {
            if (playerInput.inputIsActive)
            {
                playerInput.DeactivateInput();
            }
        }
    }

    public void ActiveInput(bool active, ACTION_MAP map)
    {
        switch (map)
        {
            case  ACTION_MAP.UI:
                if (active)
                {

                    if (!inputPlayerActions.UI.enabled)
                    {
                        inputPlayerActions.UI.Enable();
                    }
                }
                else
                {
                    if (inputPlayerActions.UI.enabled)
                    {
                        inputPlayerActions.UI.Disable();
                    }
                }
                break;
                case  ACTION_MAP.PLAYER:
                if (active)
                {

                    if (!inputPlayerActions.Player.enabled)
                    {
                        inputPlayerActions.Player.Enable();
                    }
                }
                else
                {
                    if (inputPlayerActions.Player.enabled)
                    {
                        inputPlayerActions.Player.Disable();
                    }
                }
                break;
        }
        
        
    }

    public bool InputIsActive( ACTION_MAP map)
    {
        switch (map)
        {
            case ACTION_MAP.UI:
                return inputPlayerActions.UI.enabled;
                break;

            case ACTION_MAP.PLAYER:
                return inputPlayerActions.Player.enabled;
                break;
        }

        return false;
    }
    void LeftAxis(InputAction.CallbackContext context)
    {


       
        Vector2 input = context.ReadValue<Vector2>();
        leftStickAxis = input;

        leftAxis_UpPressed = input.y > leftAxisDeadzone;
        leftAxis_DownPressed = input.y < -leftAxisDeadzone;
        leftAxis_LeftPressed = input.x < -leftAxisDeadzone;
        leftAxis_RightPressed = input.x > leftAxisDeadzone;


       
    }
    void LeftAxisButton(InputAction.CallbackContext context)
    {
        bool button = context.ReadValueAsButton();
        anyButton =  leftStickButton = button;

    }

    void AnyKeyDown(InputAction.CallbackContext context)
    {

    }

    void RightAxis(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        rightStickAxis = input;

        rightAxis_UpPressed = input.y > rightAxisDeadzone;
        rightAxis_DownPressed = input.y < -rightAxisDeadzone;
        rightAxis_LeftPressed = input.x < -rightAxisDeadzone;
        rightAxis_RightPressed = input.x > rightAxisDeadzone;

       
    }
    void RightAxisButton(InputAction.CallbackContext context)
    {
        bool button = context.ReadValueAsButton();
        rightStickButton = button;
    }


    void Dpad(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        dPad = input;

        dPad_UpPressed = input.y > 0;
        dPad_DownPressed = input.y < 0;
        dPad_LeftPressed = input.x < 0;
        dPad_RightPressed = input.x > 0;

        anyButton = (input.y > 0 || input.y < 0 ||
                     input.x < 0 || input.x > 0);
    } void ButtonSouth(InputAction.CallbackContext context)
    {
        bool button = context.ReadValueAsButton();
        anyButton = buttonSouth = button;
    }
    void ButtonEast(InputAction.CallbackContext context)
    {
        bool button = context.ReadValueAsButton();
        anyButton = buttonEast = button;
    }
    void ButtonNorth(InputAction.CallbackContext context)
    {
        bool button = context.ReadValueAsButton();
        anyButton = buttonNorth = button;
    }
    void ButtonWest(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();
        anyButton = buttonWest = button;
    }
    void ButtonLeftBumper(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();
        anyButton = buttonLeftBumper = button;
    }
    void ButtonRightBumper(InputAction.CallbackContext context)
    {


        bool button = context.ReadValueAsButton();
        anyButton = buttonRightBumper = button;
    }
    void ButtonLeftTrigger(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();
        anyButton = buttonLeftTrigger = button;

 
    }
    void ButtonRightTrigger(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();
        anyButton = buttonRightTrigger = button;


    }
    void ButtonStart(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();
        anyButton = buttonStart = button;

       
    }
    void ButtonSelect(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();
        buttonSelect = button;
    }
    void Confirm(InputAction.CallbackContext context)
    {
        bool button = context.ReadValueAsButton();
        anyButton = confirmButton = button;
    }
    void Cancel(InputAction.CallbackContext context)
    {

        bool button = context.ReadValueAsButton();

        anyButton = cancelButton = button;

    }

    void Nav(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        anyButton = (input.y > 0 || input.y < 0 ||
                     input.x < 0 || input.x > 0);

    }

    void Mouse (InputAction.CallbackContext context)
    {
  
    }

    void RightClick(InputAction.CallbackContext context)
    {

    }
    void LeftClick(InputAction.CallbackContext context)
    {

    }
}