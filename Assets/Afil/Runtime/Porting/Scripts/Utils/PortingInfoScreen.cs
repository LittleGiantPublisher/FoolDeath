using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
[RequireComponent(typeof(TextMeshProUGUI))]
public class PortingInfoScreen : MonoBehaviour
{
    private void OnEnable()
    {
        if(string.IsNullOrEmpty(Porting.PlatformManager.UserNickName) == false)
            GetComponent<TextMeshProUGUI>().text = Porting.PlatformManager.UserNickName + " - " + Application.version;
    }
}
