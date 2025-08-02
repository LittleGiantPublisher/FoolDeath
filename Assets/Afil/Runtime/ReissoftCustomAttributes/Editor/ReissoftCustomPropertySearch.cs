using Reissoft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(Search))]
public class ReissoftCustomPropertySearch : PropertyDrawer
{
    float armor = -1;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
            
        if (this.fieldInfo.FieldType.ToString().Contains("[") == false && CustoBaseClass.search != "")
        {
            if (label.text.ToUpper().Contains(CustoBaseClass.search.ToUpper()) == false)
                return;

        }
        
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.PropertyField(position, property, label, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        if (this.fieldInfo.FieldType.ToString().Contains("[") == false && CustoBaseClass.search != "")
        {
            if (label.text.ToUpper().Contains(CustoBaseClass.search.ToUpper()) == false)
                return 0;

        }
        return base.GetPropertyHeight(property, label);

    }

}
