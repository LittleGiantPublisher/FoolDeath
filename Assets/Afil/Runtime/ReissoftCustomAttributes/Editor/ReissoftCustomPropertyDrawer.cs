using Reissoft;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
[CustomPropertyDrawer(typeof(EnableIf), true)]
[CustomPropertyDrawer(typeof(DisableIf), false)]
[CustomPropertyDrawer(typeof(ReadOnly))]
[CustomPropertyDrawer(typeof(DisplayAsString))]
[CustomPropertyDrawer(typeof(HideLabel))]
[CustomPropertyDrawer(typeof(MaxValue))]
[CustomPropertyDrawer(typeof(MinValue))]
[CustomPropertyDrawer(typeof(HideInInspector))]
[CustomPropertyDrawer(typeof(GUIColor))]
[CustomPropertyDrawer(typeof(IgnoreReissoftAttributes))]
[CustomPropertyDrawer(typeof(LabelText))]
[CustomPropertyDrawer(typeof(MinMax))]
//[CustomPropertyDrawer(typeof(UnityEngine.UIElements.ProgressBar))]
[CustomPropertyDrawer(typeof(Doc))]
[CustomPropertyDrawer(typeof(InfoBox), true)]
[CustomPropertyDrawer(typeof(HorizontalLine), true)]
[CustomPropertyDrawer(typeof(Search))]
[CustomPropertyDrawer(typeof(RangeInterval))]
[CustomPropertyDrawer(typeof(IsDebug))]
public class ReissoftCustomPropertyDrawer : PropertyDrawer
{
    float armor = -1;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {

        GUI.enabled = true;
        int selected = 999;
        var color = GUI.color;
        color.a = 1f;
        if (this.fieldInfo.GetCustomAttribute<IgnoreReissoftAttributes>() != null)
        {
            base.OnGUI(position, property, label);
            return;

        }

        if (this.fieldInfo.GetCustomAttribute<Search>() !=  null && this.fieldInfo.FieldType.ToString().Contains("[") == false && CustoBaseClass.search != "")
        {
            if (label.text.ToUpper().Contains(CustoBaseClass.search.ToUpper()) == false)
            {
                color.a = 0.1f;
                
            }

        }
        GUI.color = color;
        if (this.fieldInfo.GetCustomAttribute<HideInInspector>() != null)
            return;

        if (this.fieldInfo.GetType().IsArray == false)
        {
            if (this.fieldInfo.GetCustomAttribute<EnableIf>() != null)
            {
                if (!isOk(property))
                {
                    GUI.enabled = false;
                }
            }

            if (this.fieldInfo.GetCustomAttribute<DisableIf>() != null)
            {

                if (isNOk(property))
                {

                    GUI.enabled = false;
                }
            }

            if (this.fieldInfo.GetCustomAttribute<ShowIf>() != null)
            {
                if (!isOkS(property))
                {
                    return;
                }
            }

            if (this.fieldInfo.GetCustomAttribute<HideIf>() != null)
            {

                if (isNOkS(property))
                {

                    return;
                }
            }
        }
        if (this.fieldInfo.GetCustomAttribute<CustomType>() != null)
        {
            CustomType ct = this.fieldInfo.GetCustomAttribute<CustomType>();
            EditorGUI.BeginProperty(position, label, property);

            Debug.Log(ct.tp);
            if (ct.tp != "Tag" && ct.tp != "Layer" && !System.IO.File.Exists(Application.streamingAssetsPath + "/CustomTypes/" + ct.tp))
            {
                EditorGUI.LabelField(position, "System Type " + ct.tp + " not found");
                return;
            }


            string[] options = ct.tp == "Tag" ? UnityEditorInternal.InternalEditorUtility.tags : (ct.tp == "Layer") ? UnityEditorInternal.InternalEditorUtility.layers : System.IO.File.ReadAllLines(Application.streamingAssetsPath + "/CustomTypes/" + ct.tp);
            if (ct.typeField == CustomTypeField.STRING)
            {
                if (selected == 999)
                    selected = GetSel(property.stringValue, options);
                selected = EditorGUI.Popup(position, label.text, selected, options);// EditorGUILayout.Popup(label.text, selected, options);
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
            return;
        }

        EditorGUI.BeginProperty(position, label, property);

        if (this.fieldInfo.GetCustomAttribute<ReadOnly>() != null)
            GUI.enabled = false;
        if(this.fieldInfo.GetCustomAttribute<DisplayAsString>() != null)
        {
            GUIStyle style = GUI.skin.box;
            style.fontStyle = FontStyle.Bold;         
            EditorGUI.LabelField(new Rect(position.x, position.y, position.width, position.height), property.stringValue,style);
            return;
        }
        if (this.fieldInfo.GetCustomAttribute<IsDebug>() != null)
        {
            IsDebug infoBox = attribute as IsDebug;
            if (infoBox.Parameter == "PlayerManager.IsDebug")
            {
                var cond = PlayerPrefs.GetInt("PlayerManagerIsDebug") == 1;
                GUIStyle style = GUI.skin.box;
                style.fontStyle = FontStyle.Bold;
                GUI.color = cond ? Color.red : Color.green;
                EditorGUI.LabelField(new Rect(position.x, position.y, position.width, position.height), cond ? infoBox.IsTrue : infoBox.IsFalse, style);
                GUI.color = Color.white;
            }
            return;
        }
        //InfoBox
        if (GUI.enabled && this.fieldInfo.GetCustomAttribute<InfoBox>() != null)
        {
            InfoBox infoBox = attribute as InfoBox;
            if (infoBox != null)
            {
                 EditorGUI.HelpBox(new Rect( position.x,position.y,position.width,27), infoBox.info, MessageType.Info);
                //GUI.Button(new Rect(position.x, position.y, position.width, 27), infoBox.info);
                position.y += 30;
            }
        }

        //Horizontal
        if (this.fieldInfo.GetCustomAttribute<HorizontalLine>() != null)
        {
            EditorGUI.DrawRect(new Rect(position.x, position.y, position.width, 1f), Color.grey);
            position.y += 5;
        }


        



        var gc = this.fieldInfo.GetCustomAttribute<GUIColor>();
        if (gc != null)
        {
            GUI.color = gc.color;
        }


        //MaxValue
        MaxValue max = this.fieldInfo.GetCustomAttribute<MaxValue>();
        if (max != null)
        {
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                    if (property.intValue > max.value)
                    {
                        property.intValue = max.value;
                    }

                if (property.propertyType == SerializedPropertyType.Float)
                    if (property.floatValue > max.valueFloat)
                    {
                        property.floatValue = max.valueFloat;
                    }
            }
        }
        //MaxValue
        MinValue min = this.fieldInfo.GetCustomAttribute<MinValue>();
        if (min != null)
        {
            {
                if (property.propertyType == SerializedPropertyType.Integer)
                    if (property.intValue < min.value)
                    {
                        property.intValue = min.value;
                    }

                if (property.propertyType == SerializedPropertyType.Float)
                    if (property.floatValue < min.valueFloat)
                    {
                        property.floatValue = min.valueFloat;
                    }
            }
        }

        if (this.fieldInfo.GetCustomAttribute<Doc>() != null)
        {
            if (GUI.Button(new Rect(position.x, position.y, 30f, position.height), "?"))
            {
                Application.OpenURL(fieldInfo.GetCustomAttribute<Doc>().url);
            }
            position.width -= 30f;
            position.x += 30;

        }

        if (this.fieldInfo.GetCustomAttribute<LabelText>() != null)
        {
            LabelText labelText = attribute as LabelText;
            if(labelText != null)
            label.text = labelText.value;
        }

        //HideLabel
        if (this.fieldInfo.GetCustomAttribute<HideLabel>() != null)
        {
            EditorGUI.PropertyField(position, property, GUIContent.none);
            return;
        }
        if (this.fieldInfo.GetCustomAttribute<RangeInterval>() != null)
        {
            var minmax = this.fieldInfo.GetCustomAttribute<RangeInterval>();
            var newValue = EditorGUI.IntSlider(new Rect(position.x, position.y, position.width , position.height),(minmax.valueLabel == "" ? label.text :  minmax.valueLabel), property.intValue,minmax.valueMin,minmax.valueMax);
           
            property.intValue = newValue;
            return;
        }
        if (this.fieldInfo.GetCustomAttribute<MinMax>() != null)
        {
            var minmax = this.fieldInfo.GetCustomAttribute<MinMax>();
            float minValue = property.vector2Value.x;
            float maxValue = property.vector2Value.y;
            EditorGUI.MinMaxSlider(new Rect(position.x, position.y, position.width - 80, position.height), label, ref minValue, ref maxValue, minmax.min, minmax.max);
            EditorGUI.LabelField(new Rect(position.width - 60, position.y, 100, position.height), minValue.ToString("N2") + " | " + maxValue.ToString("N2"));
            var vec = Vector2.zero;
            vec.x = minValue;
            vec.y = maxValue;

            property.vector2Value = vec;

        }
        /*else if (this.fieldInfo.GetCustomAttribute<UnityEngine.UIElements.ProgressBar>() != null)
        {
            var pb = this.fieldInfo.GetCustomAttribute<UnityEngine.UIElements.ProgressBar>();
            if (armor == -1)
                armor = property.floatValue;
            //  var armor = EditorGUI.IntSlider(position,Mathf.RoundToInt(property.floatValue + property.intValue),Mathf.RoundToInt(pb.min),Mathf.RoundToInt(pb.max));
            //EditorGUI.ProgressBar(position,Mathf.RoundToInt(property.floatValue/100),pb.title);
            GUI.Box(position, "");
            armor = EditorGUI.IntSlider(new Rect(position.x, position.y, position.width - 6, 15), pb.title, Mathf.RoundToInt(property.floatValue), pb.min, pb.max);
            EditorGUI.ProgressBar(new Rect(position.x, position.y + 20, position.width - 6, 20), ((armor - pb.min) / ((pb.max - pb.min))), pb.title);
            property.floatValue = armor;
        }*/
        else
        {

            EditorGUI.PropertyField(position, property, label, true);
        }

        EditorGUI.EndProperty();
        GUI.color = Color.white;
        GUI.enabled = true;
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


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

      /*  if (this.fieldInfo.FieldType.ToString().Contains("[") == false && CustoBaseClass.search != "")
        {
            if (label.text.ToUpper().Contains(CustoBaseClass.search.ToUpper()) == false)
                return 0;

        }*/

        var nt = base.GetPropertyHeight(property, label);
        /*if (this.fieldInfo.GetCustomAttribute<UnityEngine.UIElements.ProgressBar>() != null)
        {
            
            nt += 30f;
            return nt;
        }*/
        if (property.hasVisibleChildren)
        {
           // var nt = base.GetPropertyHeight(property, label);
            nt += (25 * property.CountInProperty());

        }

        if (this.fieldInfo.GetCustomAttribute<HorizontalLine>() != null)
        {
           // var nt = base.GetPropertyHeight(property, label);
            nt += 6f;
           
        }

        if (GUI.enabled && this.fieldInfo.GetCustomAttribute<InfoBox>() != null)
        {
            InfoBox infoBox = attribute as InfoBox;
            if (infoBox != null)
            {
                //  var nt = base.GetPropertyHeight(property, label);
                nt += 30f;
            }
           // return nt;
        }
        return nt;
      //  return base.GetPropertyHeight(property, label);

    }

    bool isOk(SerializedProperty property)
    {
        EnableIf snif = attribute as EnableIf;
        var target = fieldInfo.DeclaringType;

        if (snif != null)
        {
            if (snif.parameter.Contains("@") || snif.parameter.Contains("||") || snif.parameter.Contains("&&"))
            {
                var eE = snif.parameter.Contains("&&");
                var str = snif.parameter.Replace("||", "|").Replace("@", "").Replace(" ", "").Replace("&&", "|");
                var lstr = str.Split('|');
                int pass = 0;
                foreach (var ll in lstr)
                {
                    var s2 = ll.Replace("==", "=").Split('=');
                    if (s2.Length < 2)
                    {

                    }
                    else
                    {
                        var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                        var field = ReflectionUtility.GetField(objp, s2[0]);
                        var obj = field.GetValue(objp);
                        if (obj != null)
                        {
                            var res = obj.ToString();
                            if (res.ToLower() == s2[1].ToLower())
                            {
                                pass++;
                            }
                        }

                    }

                }
                if (pass == lstr.Length || (!eE && pass > 0))
                {
                    //Not
                }
                else
                {
                    return false;
                }

            }
            else
            {

                var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                var field = ReflectionUtility.GetField(objp, snif.parameter);
                var obj = field.GetValue(objp);
                if (obj != null)
                    if (obj.ToString() != snif.condition.ToString())
                    {
                        return false;
                    }
            }
        }
        return true;
    }
    bool isNOk(SerializedProperty property)
    {
        DisableIf snif = attribute as DisableIf;
        var target = fieldInfo.DeclaringType;

        if (snif != null)
        {
            if (snif.parameter.Contains("@") || snif.parameter.Contains("||") || snif.parameter.Contains("&&"))
            {
                var eE = snif.parameter.Contains("&&");
                var str = snif.parameter.Replace("||", "|").Replace("@", "").Replace(" ", "").Replace("&&", "|");
                var lstr = str.Split('|');
                int pass = 0;
                foreach (var ll in lstr)
                {
                    var s2 = ll.Replace("==", "=").Split('=');
                    if (s2.Length < 2)
                    {

                    }
                    else
                    {
                        var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                        var field = ReflectionUtility.GetField(objp, s2[0]);
                        var obj = field.GetValue(objp);
                        if (obj != null)
                        {
                            var res = obj.ToString();
                            if (res.ToLower() == s2[1].ToLower())
                            {
                                pass++;
                            }
                        }

                    }

                }
                if (pass == lstr.Length || (!eE && pass > 0))
                {
                    //Not
                }
                else
                {
                    return false;
                }

            }
            else
            {

                var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                var field = ReflectionUtility.GetField(objp, snif.parameter);
                var obj = field.GetValue(objp);
                if (obj != null)
                    if (obj.ToString() != snif.condition.ToString())
                    {
                        return false;
                    }
            }
        }
        return true;
    }


    bool isOkS(SerializedProperty property)
    {
        ShowIf snif = attribute as ShowIf;
        var target = fieldInfo.DeclaringType;

        if (snif != null)
        {
            if (snif.parameter.Contains("@") || snif.parameter.Contains("||") || snif.parameter.Contains("&&"))
            {
                var eE = snif.parameter.Contains("&&");
                var str = snif.parameter.Replace("||", "|").Replace("@", "").Replace(" ", "").Replace("&&", "|");
                var lstr = str.Split('|');
                int pass = 0;
                foreach (var ll in lstr)
                {
                    var s2 = ll.Replace("==", "=").Split('=');
                    if (s2.Length < 2)
                    {

                    }
                    else
                    {
                        var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                        var field = ReflectionUtility.GetField(objp, s2[0]);
                        var obj = field.GetValue(objp);
                        if (obj != null)
                        {
                            var res = obj.ToString();
                            if (res.ToLower() == s2[1].ToLower())
                            {
                                pass++;
                            }
                        }

                    }

                }
                if (pass == lstr.Length || (!eE && pass > 0))
                {
                    //Not
                }
                else
                {
                    return false;
                }

            }
            else
            {

                var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                var field = ReflectionUtility.GetField(objp, snif.parameter);
                var obj = field.GetValue(objp);
                if (obj != null)
                    if (obj.ToString() != snif.condition.ToString())
                    {
                        return false;
                    }
            }
        }
        return true;
    }
    bool isNOkS(SerializedProperty property)
    {
        HideIf snif = attribute as HideIf;
        var target = fieldInfo.DeclaringType;

        if (snif != null)
        {
            if (snif.parameter.Contains("@") || snif.parameter.Contains("||") || snif.parameter.Contains("&&"))
            {
                var eE = snif.parameter.Contains("&&");
                var str = snif.parameter.Replace("||", "|").Replace("@", "").Replace(" ", "").Replace("&&", "|");
                var lstr = str.Split('|');
                int pass = 0;
                foreach (var ll in lstr)
                {
                    var s2 = ll.Replace("==", "=").Split('=');
                    if (s2.Length < 2)
                    {

                    }
                    else
                    {
                        var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                        var field = ReflectionUtility.GetField(objp, s2[0]);
                        var obj = field.GetValue(objp);
                        if (obj != null)
                        {
                            var res = obj.ToString();
                            if (res.ToLower() == s2[1].ToLower())
                            {
                                pass++;
                            }
                        }

                    }

                }
                if (pass == lstr.Length || (!eE && pass > 0))
                {
                    //Not
                }
                else
                {
                    return false;
                }

            }
            else
            {

                var objp = PropertyUtility.GetTargetObjectWithProperty(property);//.FindPropertyRelative(snif.parameter);// target.GetField(snif.parameter);
                var field = ReflectionUtility.GetField(objp, snif.parameter);
                var obj = field.GetValue(objp);
                if (obj != null)
                    if (obj.ToString() != snif.condition.ToString())
                    {
                        return false;
                    }
            }
        }
        return true;
    }



    private object GetValues(SerializedProperty property, string valuesName)
    {
        object target = PropertyUtility.GetTargetObjectWithProperty(property);

        FieldInfo valuesFieldInfo = ReflectionUtility.GetField(target, valuesName);
        if (valuesFieldInfo != null)
        {
            return valuesFieldInfo.GetValue(target);
        }

        PropertyInfo valuesPropertyInfo = ReflectionUtility.GetProperty(target, valuesName);
        if (valuesPropertyInfo != null)
        {
            return valuesPropertyInfo.GetValue(target);
        }

        MethodInfo methodValuesInfo = ReflectionUtility.GetMethod(target, valuesName);
        if (methodValuesInfo != null &&
            methodValuesInfo.ReturnType != typeof(void) &&
            methodValuesInfo.GetParameters().Length == 0)
        {
            return methodValuesInfo.Invoke(target, null);
        }

        return null;
    }
}
