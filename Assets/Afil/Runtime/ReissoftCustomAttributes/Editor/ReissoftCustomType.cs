using Reissoft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Resources = UnityEngine.Resources;

[CustomPropertyDrawer(typeof(CustomType))]
public class ReissoftCustomType : PropertyDrawer
{

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        int selected = 999;
        CustomType ct = attribute as CustomType;
        EditorGUI.BeginProperty(position, label, property);



        if (ct.tp != "LevelToOpenData" && ct.tp != "Tag" && ct.tp != "Layer" && !System.IO.File.Exists(Application.streamingAssetsPath + "/CustomTypes/" + ct.tp))
        {
            EditorGUI.LabelField(position, "System Type " + ct.tp + " not found");
            return;
        }

        string[] options = new string[0];
        if (ct.tp == "LevelToOpenData")
        {
            
           /* var data = Resources.Load<LevelToOpenData>("UI/Map/LevelToOpenData");
            options = new string[data.levelToOpenInfos.Count];
            for (var index = 0; index < data.levelToOpenInfos.Count; index++)
            {
                var sn = data.levelToOpenInfos[index];
                options[index] = sn.Scene;
            }*/
        }
        else
        {
            options = ct.tp == "Tag" ? UnityEditorInternal.InternalEditorUtility.tags :
                (ct.tp == "Layer") ? UnityEditorInternal.InternalEditorUtility.layers :
                System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/CustomTypes/" + ct.tp);
        }

        // string[] options = ct.tp == "Tag" ? UnityEditorInternal.InternalEditorUtility.tags : System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/CustomTypes/" + ct.tp);
        if (ct.typeField == CustomTypeField.STRING)
        {
            if (selected == 999)
                selected = GetSel(property.stringValue, options);
            selected = EditorGUI.Popup(position, label.text, selected, options);// EditorGUILayout.Popup(label.text, selected, options);
            if (selected <= options.Length - 1)
                property.stringValue = options[selected];
        }
        else
        {
            if (selected == 999)
                selected = GetSelArray(property.stringValue, options);
            selected = EditorGUILayout.MaskField(label.text, selected, options);
            property.ClearArray();
            List<string> selectedOptions = new List<string>();
            for (int i = 0; i < options.Length; i++)
            {
                if ((selected & (1 << i)) == (1 << i)) selectedOptions.Add(options[i]);
            }
            property.arraySize = selectedOptions.Count;
            property.stringValue = "";
            for (int i = 0; i <= selectedOptions.Count - 1; i++)
            {
                property.stringValue += i == 0 ? selectedOptions[i] : ";" + selectedOptions[i];

            }
        }
        EditorGUI.EndProperty();

    }

    private int GetSel(string stringValue, string[] options)
    {
        for (int i = 0; i <= options.Length - 1; i++)
        {
            if (options[i] == stringValue)
                return i;
        }
        return 0;
    }
    private int GetSelArray(string stringValue, string[] options)
    {
        int ret = 0;

        var str = stringValue.Split(';');
        for (int i = 0; i <= str.Length - 1; i++)
        {
            int pot = 1;
            for (int ii = 0; ii <= options.Length - 1; ii++)
            {
                if (str[i] == options[ii])
                    if (ii == 0)
                    {
                        ret = 1;
                    }
                    else
                    {
                        ret += pot;

                    }
                pot *= 2;
            }

        }

        return ret;
    }


}
