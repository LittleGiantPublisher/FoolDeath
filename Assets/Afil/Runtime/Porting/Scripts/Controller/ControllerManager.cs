using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;
using TMPro;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
#if UNITY_SWITCH
using nn.hid;
#endif
public class ControllerManager : MonoBehaviour
{

    [SerializeField] public PlayerInput playerInput;
    [SerializeField] public InputPlayer inputPlayerActions;
    [SerializeField] public InputController thisController;
    public bool OnTestController;
    public INPUT_TYPE currentPCInput;
    public static Action UpdateGlyphs;
    public InputController Controller { get { return thisController; } }
    public InputSystemUIInputModule inputSystemUI;
    public static bool controllerDisconnected = false;
    public static bool tryReconnectController = false;

    public bool onReconec;
    public static bool startFindInput = true;
    public static bool firstInput = false;
    public bool hasGamepadConected = false;
    public static ControllerManager current;
    InputDevice userDevice;


    public static Action OnControllerDisconnnect;
    public static Action OnControllerConnnect;
    public static Action OnInputChanged;
    public static Action OnAnyKeyPressed;

    [SerializeField] private bool anyKeyEventCalled = false;

    [SerializeField] GameObject ControllerDisconnectionPrefab;
     GameObject ControllerDisconnection;
     GameObject disconectionScreen;
   //   CanvasGroup disconnectedScreenCanvas;
    private bool desactiveInLoading;

#if UNITY_SWITCH
    bool disconected;

    private NpadId[] npadIds = { NpadId.Handheld, NpadId.No1, NpadId.No2, NpadId.No3, NpadId.No4 };
    private NpadState npadState = new NpadState();
    private ControllerSupportArg controllerSupportArg = new ControllerSupportArg();
    private nn.Result result = new nn.Result();
#endif
    void Awake()
    {
        if (current != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        current = this;
       

        DontDestroyOnLoad(this);
        InitInputSystem();
#if UNITY_SWITCH
        InitializeNPAD();
#else
        ControllerDisconnection = Instantiate(ControllerDisconnectionPrefab, null);
        //Debug.Log($"Controller object name: {ControllerDisconnection.name}");
        disconectionScreen = ControllerDisconnection.transform.GetChild(0).gameObject;
#endif
        //userDevice = new InputDevice();
        //InputSystem.EnableDevice(userDevice);
        //InputSystem.FlushDisconnectedDevices();

        ConectFirstInput();
        //Debug.Log($"[DeviceCap]: {this.playerInput.devices[0].device.description.capabilities}");


#if UNITY_EDITOR
        if (OnTestController)
            startFindInput = true;
#endif
    }

    private void Update()
    {
        if (EventSystem.current.currentInputModule != inputSystemUI && EventSystem.current.currentInputModule.IsActive())
        {
            Debug.Log("Mudando input event");
            EventSystem.current.currentInputModule.DeactivateModule();
            inputSystemUI.ActivateModule();
        }
        //Debug.LogError("TIRA ESSA PORRA DAQUI CARALHO");
        //Porting.PlatformManager.enterButtonParam = 0;
        onReconec = tryReconnectController;
#if UNITY_SWITCH
        if (!disconected)
        {
            if (!CheckNpad(npadIds[0]) && !CheckNpad(npadIds[1]))
            {
                disconected = true;
                ShowControllerSupport();
            }
    	}
#endif

        CheckControllerReconnection();
        UpdatePCGlyphs();
        thisController.UpdateInput();

        ////Debug.LogError("Desativando checagem de State");
        //return;

        if (StateMachine.current.OnTheState(StateMachine.PAGE.LOADING) && !desactiveInLoading)
        {
            desactiveInLoading = true;

            EventSystem.current.sendNavigationEvents = false;
            Debug.Log("Desativando input do controller");
            thisController.ActiveInput(false, InputController.ACTION_MAP.UI);
        }
        else if (!StateMachine.current.OnTheState(StateMachine.PAGE.LOADING) && desactiveInLoading)
        {
            desactiveInLoading = false;
            EventSystem.current.sendNavigationEvents = true;
            Debug.Log("Ativando input do controller");
            thisController.ActiveInput(true, InputController.ACTION_MAP.UI);
        }
        
        if (!anyKeyEventCalled && thisController != null && thisController.anyButton)
        {
            anyKeyEventCalled = true;
            OnAnyKeyPressed?.Invoke();
        }
        else if (!thisController.anyButton)
        {
            anyKeyEventCalled = false;
        }
    }

    void InitInputSystem()
    {
        InputSystem.onDeviceChange += DeviceChange;
        inputPlayerActions = new InputPlayer();
        inputPlayerActions.Enable();
        InvertAssignButton();
        
        thisController = new InputController(playerInput, ref inputPlayerActions);
      
    }

    void ConectFirstInput()
    {

        Debug.LogError("ENTROU no ConectFirstInput");
#if (UNITY_PS4 || UNITY_PS5 || UNITY_SWITCH)
        for (int i = 0; i < InputSystem.devices.Count; i++)
        {
      
#if UNITY_PS4
            if (InputSystem.devices[i] is UnityEngine.InputSystem.PS4.DualShockGamepadPS4)
            {
                var temp = InputSystem.devices[i] as UnityEngine.InputSystem.PS4.DualShockGamepadPS4;
#elif UNITY_PS5
            if (InputSystem.devices[i] is UnityEngine.InputSystem.PS5.DualSenseGamepad)
            {
                var temp = InputSystem.devices[i] as UnityEngine.InputSystem.PS5.DualSenseGamepad;
#elif UNITY_SWITCH
            if (InputSystem.devices[i] is UnityEngine.InputSystem.Switch.NPad)
            {
                var temp = InputSystem.devices[i] as UnityEngine.InputSystem.Switch.NPad;

                if (temp.npadId == UnityEngine.InputSystem.Switch.NPad.NpadId.Handheld ||
                    temp.npadId == UnityEngine.InputSystem.Switch.NPad.NpadId.No1)
#endif
                {
                    Debug.LogError("ACHOU UM CONTROLE, INICIALIZANDO");
                    ControllerConnected(temp);

                }
            }
        }

#endif

    }

    void DeviceChange(InputDevice device, InputDeviceChange deviceChange)
    {
        switch (deviceChange)
        {
            case InputDeviceChange.Added:
                Debug.LogError("New device added: " + device);
                ControllerConnected(device);
                
                break;

            case InputDeviceChange.Disconnected:
                ControllerDisconnected(device);
               // Debug.Log("Device disconnected: " + device);
                break;

            case InputDeviceChange.Removed:
               // Debug.Log("Device removed: " + device);
                ControllerDisconnected(device);
                break;
        }
    }

    void ControllerConnected(InputDevice device)
    {


#if !UNITY_SWITCH


#endif
        bool canEnable = false;
        Debug.LogError($"[ ControllerConnected ] device.deviceId : {device.deviceId}");
        hasGamepadConected = true;


#if UNITY_EDITOR
        canEnable = false;
#elif UNITY_PS4
        var temp = device as UnityEngine.InputSystem.PS4.DualShockGamepadPS4;
        canEnable = temp.slotIndex == 0;
#elif UNITY_PS5
        var temp = device as UnityEngine.InputSystem.PS5.DualSenseGamepad;
        canEnable = temp.slotIndex == 0;
#elif UNITY_SWITCH

        NpadId handheldId = npadIds[0];
        NpadId no01Id = npadIds[1];
        NpadStyle handheldStyle = Npad.GetStyleSet(handheldId);
        NpadStyle no01Style = Npad.GetStyleSet(no01Id);
        if ((handheldStyle == NpadStyle.None || handheldStyle == NpadStyle.Invalid) &&
            (no01Style == NpadStyle.None || no01Style == NpadStyle.Invalid)) 
        { 
            canEnable = false; 
        }
        else
        {
            if (handheldStyle != NpadStyle.None)
            {
                Npad.GetState(ref npadState, handheldId, handheldStyle);
                if (handheldStyle == NpadStyle.Handheld)
                {
                    canEnable = true;

                }

            }
            else if (no01Style != NpadStyle.None)
            {
                Npad.GetState(ref npadState, no01Id, no01Style);
                if ((no01Style == NpadStyle.FullKey) || (no01Style == NpadStyle.JoyDual))
                {
                    canEnable = true;

                }
            }
            
        }
        
#elif UNITY_GAMECORE || MICROSOFT_GAME_CORE
    canEnable = false;

    Debug.LogError($"[ ControllerManager ] device.deviceId : {device.deviceId} - device type {device.GetType()} - name {device.name}");
#endif

        Debug.LogError("** Pass 1");
        if (canEnable)
        {

            Debug.LogError("** Pass 2");

            var reconecting = controllerDisconnected;
            firstInput = true;
            
            var confirm = new InputActionReference();
            var cancel = new InputActionReference();
            var nav = new InputActionReference();

            confirm.Set(thisController.inputPlayerActions.UI.Confirm);
            cancel.Set(thisController.inputPlayerActions.UI.Cancel);
            nav.Set(thisController.inputPlayerActions.UI.Nav);
            Debug.LogError("** Pass 3");

            if (Porting.PlatformManager.enterButtonParam == 0)
            {

                inputSystemUI.submit = cancel;
                inputSystemUI.cancel = confirm;


            }
            else
            {
                inputSystemUI.submit = confirm;
                inputSystemUI.cancel = cancel;
            }
            inputSystemUI.move = nav;

            Debug.LogError("** Pass 4");
            StartCoroutine(ActiveMapWithTime(0.5f,canEnable, device, reconecting));
        }



    }

    IEnumerator ActiveMapWithTime(float f,bool canEnable, InputDevice device,bool reconectRotine = false)
    {

        Debug.LogError("Entrou NO ACTIVE MAP WITH TIME ");
        if (canEnable)
        {
            InputSystem.EnableDevice(device);
        }
        else
        {
            InputSystem.DisableDevice(device);
        }
       // //Debug.LogError("Ativando mapa ");
        thisController.ActiveInput(true);
        EventSystem.current.sendNavigationEvents =true;
#if !UNITY_SWITCH

        if (reconectRotine)
        {
            OnControllerConnnect?.Invoke();
            controllerDisconnected = false;
        }
#else

        if (reconectRotine)
        {
            Debug.Log("Invocando OnControllerConnnect");
            OnControllerConnnect?.Invoke();
            controllerDisconnected = false;
        }
#endif
        yield return new WaitForSeconds(f);
        ////Debug.LogError("Conectado controle: " + thisController.InputIsActive(InputController.ACTION_MAP.PLAYER));
    }

    IEnumerator ReconectController_KeyboardCallback(float f, bool reconectRotine = false)
    {

      
        yield return new WaitForSeconds(f);
       // //Debug.LogError("Ativando mapa ");
        thisController.ActiveInput(true);
        EventSystem.current.sendNavigationEvents = true;

        if (reconectRotine)
        {
            OnControllerConnnect?.Invoke();
            controllerDisconnected = false;
        }
       // //Debug.LogError("Conectado controle: " + thisController.InputIsActive(InputController.ACTION_MAP.PLAYER));
    }

    void ControllerDisconnected(InputDevice device)
    {

        bool canDisable = false;

       // Debug.Log($"Disconection screen: {disconectionScreen.name}");


#if UNITY_EDITOR && !MICROSOFT_GAME_CORE
        canDisable = true;
#elif UNITY_PS4
        var temp = device as UnityEngine.InputSystem.PS4.DualShockGamepadPS4;

        canDisable = temp.slotIndex == 0;
#elif UNITY_PS5
        var temp = device as UnityEngine.InputSystem.PS5.DualSenseGamepad;

        canDisable = temp.slotIndex == 0;

#elif UNITY_GAMECORE
        canDisable = (userDevice == device);
        
        Debug.LogError($"[ ControllerDisconnected ] userDevice.deviceId : {userDevice.deviceId} - device.deviceId : {device.deviceId} - canDisable: {canDisable}");
#elif UNITY_STANDALONE || MICROSOFT_GAME_CORE

        if (!hasGamepadConected)
            return;

        canDisable = (device is Gamepad || device is Joystick);

#endif

        if (canDisable)
        {
            InputSystem.FlushDisconnectedDevices();

            OnControllerDisconnnect?.Invoke();
            Debug.LogError($"[ ControllerDisconnected ] device.deviceId : {device.deviceId}");
            Porting.PlatformManager.ForcePause?.Invoke();
            thisController.ActiveInput(false);
            EventSystem.current.sendNavigationEvents = false;
            tryReconnectController = true;
            controllerDisconnected = true;
        }
    }

    void CheckControllerReconnection()
    {
#if (UNITY_STANDALONE)
        if (tryReconnectController)
            hasGamepadConected = false;

        if (tryReconnectController || !firstInput && startFindInput || firstInput && !hasGamepadConected)
        {
            for (int I = 0; I < InputSystem.devices.Count; I++)
            {
                if (InputSystem.devices[I] is Keyboard)
                {

                    var device = InputSystem.devices[I] as Keyboard;

                    if (device.enterKey.isPressed || device.numpadEnterKey.isPressed)
                    {
                        if (userDevice is Gamepad || userDevice is Joystick)
                        {
                           // //Debug.LogError("ENTROU MEU BOM");
                            userDevice = new InputDevice();
                            InputSystem.FlushDisconnectedDevices();
                            
                        }

                        var confirm = new InputActionReference();
                        var cancel = new InputActionReference();
                        var nav = new InputActionReference();

                        confirm.Set(thisController.inputPlayerActions.UI.Confirm);
                        cancel.Set(thisController.inputPlayerActions.UI.Cancel);
                        nav.Set(thisController.inputPlayerActions.UI.Nav);

                        if (Porting.PlatformManager.enterButtonParam == 0)
                        {

                            inputSystemUI.submit = cancel;
                            inputSystemUI.cancel = confirm;


                        }
                        else
                        {
                            inputSystemUI.submit = confirm;
                            inputSystemUI.cancel = cancel;
                        }
                        inputSystemUI.move = nav;


                        StartCoroutine(ReconectController_KeyboardCallback(0.5f, true));
                        tryReconnectController = false;

                        if (hasGamepadConected)
                            hasGamepadConected = false;

                        if (!firstInput)
                        {
                            firstInput = true;
                            startFindInput = false;
                        }
                    }
                    ////Debug.LogError("Keyboard");
                }
                else if (InputSystem.devices[I] is Mouse)
                {
                    ////Debug.LogError("Mouse");
                }
                else if (InputSystem.devices[I] is Gamepad)
                {
                    var device = InputSystem.devices[I] as Gamepad;
                    if (!firstInput || tryReconnectController)
                    {

                         if ((((device.crossButton.isPressed || device.aButton.isPressed || device.buttonSouth.isPressed) && Porting.PlatformManager.enterButtonParam == 1)
                            ||((device.circleButton.isPressed || device.buttonEast.isPressed || device.bButton.isPressed) && Porting.PlatformManager.enterButtonParam == 0)) && !device.synthetic)
        {

                            userDevice = device;
                            if (!firstInput)
                                firstInput = true;
                            if (!hasGamepadConected)
                                hasGamepadConected = true;
                            var confirm = new InputActionReference();
                            var cancel = new InputActionReference();
                            var nav = new InputActionReference();

                            confirm.Set(thisController.inputPlayerActions.UI.Confirm);
                            cancel.Set(thisController.inputPlayerActions.UI.Cancel);
                            nav.Set(thisController.inputPlayerActions.UI.Nav);

                            if (Porting.PlatformManager.enterButtonParam == 0)
                            {
                                
                                inputSystemUI.submit = cancel;
                                inputSystemUI.cancel = confirm;


                            }
                            else
                            {
                                inputSystemUI.submit = confirm;
                                inputSystemUI.cancel = cancel;
                            }
                            inputSystemUI.move = nav;

                            StartCoroutine(ActiveMapWithTime(0.5f, true, device, true));
                            tryReconnectController = false;

                        }
                    }
                    else
                    {
                        for (int b = 0; b < device.allControls.Count; b++)
                        {


                            if (device.allControls[b].IsPressed()  && !device.allControls[b].synthetic)
                            {
                                userDevice = device;
                                if (!firstInput)
                                    firstInput = true;
                                if (!hasGamepadConected)
                                    hasGamepadConected = true;

                                var confirm = new InputActionReference();
                                var cancel = new InputActionReference();
                                var nav = new InputActionReference();

                                confirm.Set(thisController.inputPlayerActions.UI.Confirm);
                                cancel.Set(thisController.inputPlayerActions.UI.Cancel);
                                nav.Set(thisController.inputPlayerActions.UI.Nav);

                                if (Porting.PlatformManager.enterButtonParam == 0)
                                {

                                    inputSystemUI.submit = cancel;
                                    inputSystemUI.cancel = confirm;


                                }
                                else
                                {
                                    inputSystemUI.submit = confirm;
                                    inputSystemUI.cancel = cancel;
                                }
                                inputSystemUI.move = nav;

                                StartCoroutine(ActiveMapWithTime(0.5f, true, device, true));
                                  tryReconnectController = false;

                            }
                        }
                    }


                }
            }
        }

#elif (UNITY_GAMECORE)
        if (tryReconnectController || !firstInput && startFindInput)
        {
            for (int I = 0; I < InputSystem.devices.Count; I++)
            {
                if (InputSystem.devices[I] is Gamepad)
                {
                    var device = InputSystem.devices[I] as Gamepad;
                    if ((((device.crossButton.isPressed || device.aButton.isPressed || device.buttonSouth.isPressed) && Porting.PlatformManager.enterButtonParam == 1)
                       || ((device.circleButton.isPressed || device.buttonEast.isPressed || device.bButton.isPressed) && Porting.PlatformManager.enterButtonParam == 0)) && !device.synthetic)
                    {
                        currentPCInput = INPUT_TYPE.GAMEPAD;
                        OnInputChanged?.Invoke();
                        Debug.LogError($"[ ControllerManager ] FIRST INPUT FOUND - device.deviceId : {device.deviceId} - device type {device.GetType()} - name {device.name}");
                        userDevice = device;
                        if(!firstInput)
                        {
                            firstInput = true;
                            startFindInput = false;
                        }

                        if (!hasGamepadConected)
                            hasGamepadConected = true;
                                var confirm = new InputActionReference();
                                var cancel = new InputActionReference();
                                var nav = new InputActionReference();

                                confirm.Set(thisController.inputPlayerActions.UI.Confirm);
                                cancel.Set(thisController.inputPlayerActions.UI.Cancel);
                                nav.Set(thisController.inputPlayerActions.UI.Nav);

                                if (Porting.PlatformManager.enterButtonParam == 0)
                                {

                                    inputSystemUI.submit = cancel;
                                    inputSystemUI.cancel = confirm;


                                }
                                else
                                {
                                    inputSystemUI.submit = confirm;
                                    inputSystemUI.cancel = cancel;
                                }
                                inputSystemUI.move = nav;

                        StartCoroutine(ActiveMapWithTime(0.5f, true, device, true));
                        tryReconnectController = false;
                    }

                }
            }
        }
#else
        /*if (controllerDisconnected)
        {
            if (!tryReconnectController)
            {
                InputSystem.onAnyButtonPress.CallOnce(me =>
                {
                    tryReconnectController = true;
                });
            }
            else
            {
                ReconnectController(new InputAction.CallbackContext());
            }
        }*/
#endif
    }

    public void GamepadScreen(bool isEnabled) 
    {

        //ControllerDisconnection.SetActive(isEnabled); 
#if !UNITY_STANDALONE && !UNITY_GAMECORE
        controllerDisconnected = true;
#endif
    }
     
    public IEnumerator ConnectController()
    {
        Debug.Log("RECONNECT AFIL PACKAGE");
        
        tryReconnectController = false;
        yield return new WaitForEndOfFrame();
        GamepadScreen(false);
        yield return new WaitForSeconds(0.1f);
        controllerDisconnected = false;
    }

    public void UpdatePCGlyphs()
    {
#if !UNITY_STANDALONE && !UNITY_EDITOR && !MICROSOFT_GAME_CORE
    return;
#endif
        for (int I = 0; I < InputSystem.devices.Count; I++)
        {
            if (InputSystem.devices[I] is Keyboard)
            {

                var device = InputSystem.devices[I] as Keyboard;

                if (device.anyKey.isPressed)
                {
                    if (currentPCInput != INPUT_TYPE.KEYBOARD)
                    {
                        currentPCInput = INPUT_TYPE.KEYBOARD;
                        UpdateGlyphs?.Invoke();
                        OnInputChanged?.Invoke();
                    }
                }
                ////Debug.LogError("Keyboard");
            }
            else if (InputSystem.devices[I] is Mouse)
            {
                var device = InputSystem.devices[I] as Mouse;

                if (device.leftButton.isPressed || device.rightButton.isPressed || device.middleButton.isPressed)
                {
                    if (currentPCInput != INPUT_TYPE.KEYBOARD)
                    {
                        currentPCInput = INPUT_TYPE.KEYBOARD;
                        UpdateGlyphs?.Invoke();
                        OnInputChanged?.Invoke();
                    }
                }
            }
            else if (InputSystem.devices[I] is Gamepad)
            {
                var device = InputSystem.devices[I] as Gamepad;

                for (int b = 0; b < device.allControls.Count; b++)
                {
                    if (device.allControls[b].IsPressed())
                    {
                        if (currentPCInput != INPUT_TYPE.GAMEPAD)
                        {
                            currentPCInput = INPUT_TYPE.GAMEPAD;
                            UpdateGlyphs?.Invoke();
                            OnInputChanged?.Invoke();
                        }
                    }
                }





            }
        }
    }

    public void SetPCInputType(INPUT_TYPE inputType)
    {
#if UNITY_GAMECORE || MICROSOFT_GAME_CORE || UNITY_EDITOR
        currentPCInput = inputType;
        UpdateGlyphs?.Invoke();
        OnInputChanged?.Invoke();
#endif
    }

    public void ReconnectController(InputAction.CallbackContext ctx)
    {
        StartCoroutine(ConnectController());

        Debug.Log("Receive the command");
    }

    void InvertAssignButton()
    {
#if UNITY_PS4
        if (Porting.PlatformManager.enterButtonParam == 0)
        {
           // playerInput.SwitchCurrentActionMap("Player");
           // playerInput.SwitchCurrentControlScheme("PS4");

           // playerInput.actions["Confirm"].ApplyBindingOverride("<Gamepad>/buttonEast");
           // playerInput.actions["Cancel"].ApplyBindingOverride("<Gamepad>/buttonSouth");
        }
#endif


    }

    void InitializeNPAD()
    {
#if UNITY_SWITCH
        Npad.Initialize();
        Npad.SetSupportedIdType(npadIds);
        NpadJoy.SetHoldType(NpadJoyHoldType.Horizontal);

        Npad.SetSupportedStyleSet(NpadStyle.FullKey | NpadStyle.Handheld | NpadStyle.JoyDual);
#endif
        }

    void ShowControllerSupport()
    {
#if UNITY_SWITCH
       
        controllerSupportArg.SetDefault();
        controllerSupportArg.enableSingleMode = true;

        ControllerSupportResultInfo resultInfo = new ControllerSupportResultInfo();
       // Debug.Log(controllerSupportArg);
        controllerSupportArg.enableExplainText = true;
        result = ControllerSupport.Show(ref resultInfo, controllerSupportArg, true);
        if (result.IsSuccess()) 
        {
            disconected = false;
            ////Debug.LogError(result);
        }
        
#endif
    }
#if UNITY_SWITCH
    bool CheckNpad(NpadId npadId)
    {
        ////Debug.LogError("CHECANDO VALIDADE DO NPAD");
        NpadStyle npadStyle = Npad.GetStyleSet(npadId);
        if (npadStyle == NpadStyle.None) { return false; }

        Npad.GetState(ref npadState, npadId, npadStyle);

        

        if (npadStyle == NpadStyle.JoyLeft)
        {
            return true;
        }
        else if (npadStyle == NpadStyle.JoyRight)
        {
            return true;
        }
        else if ((npadStyle == NpadStyle.FullKey) || (npadStyle == NpadStyle.JoyDual) || (npadStyle == NpadStyle.Handheld))
        {
            return true;
        }

        return false;
    }
#endif
    public enum INPUT_TYPE
    {
        NONE,
        GAMEPAD,
        KEYBOARD,
    }
}
