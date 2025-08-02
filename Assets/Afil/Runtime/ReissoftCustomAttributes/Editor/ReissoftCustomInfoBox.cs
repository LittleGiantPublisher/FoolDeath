using Reissoft;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomPropertyDrawer(typeof(InfoBox),true)]
public class ReissoftCustomInfoBox : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        if (CustoBaseClass.search != "")
            return;
        InfoBox infoBox = attribute as InfoBox;

        EditorGUI.HelpBox(position, infoBox.info,MessageType.Info);

    }

   


}
