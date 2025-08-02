using Reissoft;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
//[CustomPropertyDrawer(typeof(HorizontalLine),true)]
public class ReissoftCustomHorizontalLine : DecoratorDrawer
{
    public override void OnGUI(Rect position)
    {
        if (CustoBaseClass.search != "")
            return;
        //base.OnGUI(position, property, label);
        EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, 1f), Color.grey);
    }

    public override float GetHeight()
    {
        InfoBox infoBox = attribute as InfoBox;
        var h = base.GetHeight();
        return h + 6;
    }
}
