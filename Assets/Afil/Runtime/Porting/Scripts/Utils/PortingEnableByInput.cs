using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InputType { CONTROLLER, KEYBOARD, BOTH, NONE }
public class PortingEnableByInput : MonoBehaviour
{
    public PortingEnableByInputType[] portingEnableByInputs;
    private void OnEnable()
    {
        StartCoroutine(Process());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator Process()
    {
        while (true)
        {
            if (GamepadManager.instance != null)
            {
                foreach (var item in portingEnableByInputs)
                {
                    bool valid = (item.conditions.Contains(InputType.KEYBOARD) && !GamepadManager.instance.Joystick)
                        || (item.conditions.Contains(InputType.CONTROLLER) && GamepadManager.instance.Joystick)
                        || item.conditions.Contains(InputType.BOTH);
                    foreach (var go in item.gameObjects)
                    {
                        if (go.activeSelf != valid)
                            go.SetActive(valid);
                    }
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }
}
[System.Serializable]
public struct PortingEnableByInputType
{
    public List<InputType> conditions;
    public List<GameObject> gameObjects;
}
