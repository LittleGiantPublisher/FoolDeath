
#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(SpriteButton))]
public class ButtonSpriteInspector : UnityEditor.Editor
{
    GUILayoutOption option;
    public override void OnInspectorGUI()
    {
        SpriteButton spriteButton = (SpriteButton)target;
        EditorGUILayout.TextArea("MULTIPLYING SCALE:");
        spriteButton.MULTIPLYING = (SpriteButton.MULTIPLYING_PLATFORM)EditorGUILayout.EnumPopup(spriteButton.MULTIPLYING, "Set Mode");
        
        if (spriteButton.MULTIPLYING == SpriteButton.MULTIPLYING_PLATFORM.USE) 
        {
            EditorGUILayout.TextArea("SELECT PLATFORM:");

            spriteButton.ms = EditorGUILayout.Toggle("MS STORE:", spriteButton.ms);
            spriteButton.xb1 = EditorGUILayout.Toggle("XB1:", spriteButton.xb1);
            spriteButton.nintendo = EditorGUILayout.Toggle("SWITCH:", spriteButton.nintendo);
            spriteButton.ps4 = EditorGUILayout.Toggle("PS4:", spriteButton.ps4);
            spriteButton.ps5 = EditorGUILayout.Toggle("PS5:", spriteButton.ps5);
            spriteButton.multiplyScale = EditorGUILayout.FloatField("MULTIPLYING SCALE:", spriteButton.multiplyScale);

        }
        else 
        {
            spriteButton.multiplyScale = 1;
        }
        spriteButton.MODE_BUTTON = (SpriteButton.MODE)EditorGUILayout.EnumPopup(spriteButton.MODE_BUTTON, "Set Mode");
        
        if (spriteButton.MODE_BUTTON == SpriteButton.MODE.BUTTONS)
        {
            spriteButton.BUTTON_ID = (BUTTON_ID)EditorGUILayout.EnumPopup(spriteButton.BUTTON_ID, "BUTTON_ID");
        }
        else if (spriteButton.MODE_BUTTON == SpriteButton.MODE.DPAD)
        {
            spriteButton.DPAD_ID = (DPAD_ID)EditorGUILayout.EnumPopup(spriteButton.DPAD_ID, "DPAD_ID");
        }
        else if (spriteButton.MODE_BUTTON == SpriteButton.MODE.L_STICK)
        {
            spriteButton.L_STICK_ID = (L_STICK_ID)EditorGUILayout.EnumPopup(spriteButton.L_STICK_ID, "L_STICK_ID");
        }
        else if (spriteButton.MODE_BUTTON == SpriteButton.MODE.R_STICK)
        {
            spriteButton.R_STICK_ID = (R_STICK_ID)EditorGUILayout.EnumPopup(spriteButton.R_STICK_ID, "R_STICK_ID");
        }

        EditorGUILayout.TextArea("USE PC GLIPHOS");
        spriteButton.UsePCButtons = EditorGUILayout.Toggle(spriteButton.UsePCButtons);
        EditorGUILayout.TextArea("FORCE ALT (PC ONLY)");
        spriteButton.ForceAlt = EditorGUILayout.Toggle(spriteButton.ForceAlt);
    }
   

}
#endif