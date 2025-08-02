
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Reflection;
using System;
using System.Linq;

namespace Reissoft
{
    public class ReissoftCustomAttributes : MonoBehaviour
    {

    }

#if UNITY_EDITOR
    #region Custom Class
    [UnityEditor.CustomEditor(typeof(ScriptableObject), true)]
    public class CustomEditorScriptObj : CustomReissoftEditor { }

    [CustomEditor(typeof(MonoBehaviour), true)]
    public class CustoBaseClass : CustomReissoftEditor { }

    [CustomEditor(typeof(Search), true), CanEditMultipleObjects]
    public class CustoBaseClassSearch : CustomReissoftEditor { }

    #endregion

    #region Processor class
    public class CustomReissoftEditor : UnityEditor.Editor
    {
        List<string> listProp = new List<string>();
        List<bool> listBool = new List<bool>();

        List<FieldInfo> fields = new List<FieldInfo>();
        List<PropertyInfo> properties = new List<PropertyInfo>();

        public static string search = "";
        void OnEnable()
        {
            fields = ReflectionUtility.GetAllFields(target);
            properties = ReflectionUtility.GetAllProps(target);
            if (target.GetType().GetCustomAttribute<IgnoreReissoftAttributes>() != null) return;
            if (target.GetType() == typeof(MonoBehaviour) && target.GetType().GetCustomAttribute<UseReissoftAttributes>() == null)
            {
                return;
            }
            FieldInfo[] property = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var prop in property)
            {
                var p = prop.GetCustomAttribute<FoldoutGroup>();
                if (p != null)
                {
                    if (listProp.Contains(p.name) == false)
                    {
                        listProp.Add(p.name);
                        listBool.Add(false);
                    }
                }
            }
            PropertyInfo[] propertys = target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            foreach (var prop in propertys)
            {
                var p = prop.GetCustomAttribute<FoldoutGroup>();
                if (p != null)
                {
                    if (listProp.Contains(p.name) == false)
                    {
                        listProp.Add(p.name);
                        listBool.Add(false);
                    }
                }
            }

        }

        public override void OnInspectorGUI()
        {
          /*  if (target.GetType().GetCustomAttribute<UseSearch>() != null)
            {
                search =  EditorGUILayout.TextField(search);
            }
            else
            {
                search = "";
            }

            */

            if (target.GetType().GetCustomAttribute<IgnoreReissoftAttributes>() != null)
            {
                base.OnInspectorGUI();
                return;

            }
            if (target.GetType().BaseType == typeof(MonoBehaviour) && target.GetType().GetCustomAttribute<UseReissoftAttributes>() == null)
            {
                base.OnInspectorGUI();
                return;
            }
            serializedObject.Update();
            /*

            if (search != "" && (target.GetType().GetCustomAttribute<SearchInField>() != null))
            {

                var listprs = GetProps("all");
                if (listProp.Count > 0 && listprs.Count > 0)
                {
                    EditorGUILayout.Space(5);
                    EditorGUILayout.LabelField("--- Attributes found ---");
                }
                foreach (var prop in listprs)
                {
                    ProcessProp(prop);

                }

                serializedObject.ApplyModifiedProperties();
                return;
            }

            */
            var i = -1;
            string oldProp = "";
            foreach (var l in listProp)
            {
                GUI.enabled = true;
                Rect rect = EditorGUILayout.BeginVertical();
                GUI.Box(rect, GUIContent.none);
                if (l != oldProp)
                {
                    oldProp = l;
                    i++;
                }
                listBool[i] = EditorGUILayout.Foldout(listBool[i], oldProp);// || search != "";
                if (listBool[i])
                {

                    GUI.color = Color.white;

                    foreach (var prop in GetProps(oldProp))
                    {
                        if (ProcessProp(prop))
                        {
                            if (search != "")
                            {
                                listBool[i] = true;
                            }
                        }

                    }


                }
                EditorGUILayout.EndVertical();
                EditorGUILayout.Space(5);
            }
            var listpr = GetProps("");
            if (listProp.Count > 0 && listpr.Count > 0)
            {
                EditorGUILayout.Space(5);
                EditorGUILayout.LabelField("--- Uncategorized attributes ---");
            }
            foreach (var prop in listpr)
            {
                ProcessProp(prop);
                //  if (prop.field != null)
                // EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.field.Name));
            }
            serializedObject.ApplyModifiedProperties();
        }

        private bool ProcessProp(FieldMetInfo prop)
        {
           /* if (search != "" && (target.GetType().GetCustomAttribute<SearchInField>() == null))
            {
                if (AttributeFound(prop) == false)
                {
                    return false;
                }
            }
            */
            if (prop.field != null && prop.field.GetType().IsArray)
            {
                GUI.enabled = true;
            }
            var style = new GUIStyle(GUI.skin.button);
            style.normal.textColor = Color.yellow;

            var Cstyle = new GUIStyle(GUI.skin.button);


            if (prop.method != null)
            {
                var sib = prop.method.GetCustomAttribute<Button>();
                if (sib != null)
                {
                    var n = sib.name == "this" ? prop.method.Name : sib.name;
                    if (GUILayout.Button(n))
                    {
                        prop.method.Invoke(target, null);
                    }
                }
                return true;
            }

            if (prop.field != null)
            {

                /* if (prop.field.GetCustomAttribute<HideInInspector>() != null)
                     return;

                 var gc = prop.field.GetCustomAttribute<GUIColor>();
                 if (gc != null)
                 {
                     Cstyle.normal.textColor = gc.color;
                 }
                 var min = prop.field.GetCustomAttribute<MinValue>();
                 if (min != null)
                 {
                     if (prop.field.GetValue(target) != null)
                         if (Convert.ToInt32(prop.field.GetValue(target)) < min.value)
                         {
                             prop.field.SetValue(target, min.value);
                         }
                 }
                 var max = prop.field.GetCustomAttribute<MaxValue>();
                 if (max != null)
                 {
                     if (prop.field.GetValue(target) != null)
                         if (Convert.ToInt32(prop.field.GetValue(target)) > max.value)
                         {
                             prop.field.SetValue(target, min.value);
                         }
                 }
 */
                /*  var reon = prop.field.GetCustomAttribute<ReadOnly>();
                  if (reon != null)
                  {
                      EditorGUILayout.LabelField(prop.field.Name, Cstyle);
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                      return;
                  }

                  var dstring = prop.field.GetCustomAttribute<DisplayAsString>();
                  if (dstring != null)
                  {
                      EditorGUILayout.LabelField(prop.field.Name, Cstyle);
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                      return;
                  }

                  var hideL = prop.field.GetCustomAttribute<HideLabel>();
                  if (hideL != null)
                  {
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                  }

                  var hideLTxt = prop.field.GetCustomAttribute<LabelText>();
                  if (hideLTxt != null)
                  {
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                  }
                */
                var sif = prop.field.GetCustomAttribute<ShowIf>();
                if (sif != null)
                {
                    if (sif.parameter.Contains("@") || sif.parameter.Contains("||") || sif.parameter.Contains("&&"))
                    {
                        var eE = sif.parameter.Contains("&&");
                        var str = sif.parameter.Replace("||", "|").Replace("@", "").Replace(" ", "").Replace("&&", "|");
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
                                var obj = target.GetType().GetField(s2[0])?.GetValue(target);
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
                            /* var gs = new GUIStyle();
                             gs.alignment = TextAnchor.MiddleLeft;
                             if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                             {
                                 EditorUtility.DisplayDialog("Hide because",sif.parameter + " = " + sif.condition,"OK");
                             }*/
                            return false;
                        }
                    }
                    else
                    {

                        var obj = target.GetType().GetField(sif.parameter)?.GetValue(target);
                        if (obj != null)
                            if (obj.ToString() != sif.condition.ToString())
                            {
                                /*  var gs = new GUIStyle();
                                  gs.alignment = TextAnchor.MiddleLeft;
                                  if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                                  {
                                      EditorUtility.DisplayDialog("Hide because", sif.parameter + " = " + sif.condition, "OK");
                                  }*/
                                return false;
                            }
                    }
                }

                var snif = prop.field.GetCustomAttribute<HideIf>();
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
                                var obj = target.GetType().GetField(s2[0])?.GetValue(target);
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
                            /* var gs = new GUIStyle();
                             gs.alignment = TextAnchor.MiddleLeft;
                             if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                             {
                                 EditorUtility.DisplayDialog("Hide because", sif.parameter + " = " + sif.condition, "OK");
                             }*/
                            return false;
                        }

                    }
                    else
                    {

                        var obj = target.GetType().GetField(snif.parameter)?.GetValue(target);
                        if (obj != null)
                            if (obj.ToString() == snif.condition.ToString())
                            {
                                /*  var gs = new GUIStyle();
                                  gs.alignment = TextAnchor.MiddleLeft;
                                  if (GUILayout.Button(prop.field.Name + " - (Hide)",gs))
                                  {
                                      EditorUtility.DisplayDialog("Hide because", snif.parameter + " = " + snif.condition, "OK");
                                  }*/
                                return false;
                            }
                    }
                }

            }
            if (prop.prop != null)
            {

                /* if (prop.prop.GetCustomAttribute<HideInInspector>() != null)
                     return;

                 var gc = prop.prop.GetCustomAttribute<GUIColor>();
                 if (gc != null)
                 {
                     Cstyle.normal.textColor = gc.color;
                 }*/
                /*var min = prop.prop.GetCustomAttribute<MinValue>();
                if (min != null)
                {
                    if (prop.prop.GetValue(target) != null)
                        if (Convert.ToInt32(prop.prop.GetValue(target)) < min.value)
                        {
                            prop.prop.SetValue(target, min.value);
                        }
                }*/
                // var max = prop.prop.GetCustomAttribute<MaxValue>();
                // if (max != null)
                // {
                //     if (prop.prop.GetValue(target) != null)
                //         if (Convert.ToInt32(prop.prop.GetValue(target)) > max.value)
                //         {
                //             prop.prop.SetValue(target, min.value);
                //         }
                // }

                /*  var reon = prop.field.GetCustomAttribute<ReadOnly>();
                  if (reon != null)
                  {
                      EditorGUILayout.LabelField(prop.field.Name, Cstyle);
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                      return;
                  }

                  var dstring = prop.field.GetCustomAttribute<DisplayAsString>();
                  if (dstring != null)
                  {
                      EditorGUILayout.LabelField(prop.field.Name, Cstyle);
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                      return;
                  }

                  var hideL = prop.field.GetCustomAttribute<HideLabel>();
                  if (hideL != null)
                  {
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                  }

                  var hideLTxt = prop.field.GetCustomAttribute<LabelText>();
                  if (hideLTxt != null)
                  {
                      EditorGUILayout.LabelField(prop.field.GetValue(target).ToString(), Cstyle);
                  }
                */
                var sif = prop.prop.GetCustomAttribute<ShowIf>();
                if (sif != null)
                {
                    if (sif.parameter.Contains("@") || sif.parameter.Contains("||") || sif.parameter.Contains("&&"))
                    {
                        var eE = sif.parameter.Contains("&&");
                        var str = sif.parameter.Replace("||", "|").Replace("@", "").Replace(" ", "").Replace("&&", "|");
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
                                var obj = target.GetType().GetProperty(s2[0])?.GetValue(target);
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
                            /*
                            var gs = new GUIStyle();
                            gs.alignment = TextAnchor.MiddleLeft;
                            if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                            {
                                EditorUtility.DisplayDialog("Hide because", sif.parameter + " = " + sif.condition, "OK");
                            }*/
                            return false;
                        }
                    }
                    else
                    {

                        var obj = target.GetType().GetProperty(sif.parameter)?.GetValue(target);
                        if (obj != null)
                            if (obj.ToString() != sif.condition.ToString())
                            {
                                /* var gs = new GUIStyle();
                                 gs.alignment = TextAnchor.MiddleLeft;
                                 if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                                 {
                                     EditorUtility.DisplayDialog("Hide because", sif.parameter + " = " + sif.condition, "OK");
                                 }*/
                                return false;
                            }
                    }
                }

                var snif = prop.prop.GetCustomAttribute<HideIf>();
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
                                var obj = target.GetType().GetProperty(s2[0])?.GetValue(target);
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
                            /* var gs = new GUIStyle();
                             gs.alignment = TextAnchor.MiddleLeft;
                             if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                             {
                                 EditorUtility.DisplayDialog("Hide because", sif.parameter + " = " + sif.condition, "OK");
                             }*/
                            return false;
                        }

                    }
                    else
                    {

                        var obj = target.GetType().GetProperty(snif.parameter)?.GetValue(target);
                        if (obj != null)
                            if (obj.ToString() == snif.condition.ToString())
                            {
                                var gs = new GUIStyle();
                                gs.alignment = TextAnchor.MiddleLeft;
                                if (GUILayout.Button(prop.field.Name + " - (Hide)", gs))
                                {
                                    EditorUtility.DisplayDialog("Hide because", sif.parameter + " = " + sif.condition, "OK");
                                }
                                return false;
                            }
                    }
                }

            }
            /*  var fps = prop.field.GetCustomAttribute<InfoBox>();
              if (fps != null)
              {
                  EditorGUILayout.TextArea(fps.info, style);
              }*/

           /* if (CustoBaseClass.search != "" && prop.field != null)
            {
                GUI.enabled = prop.field.Name.ToUpper().Contains(CustoBaseClass.search.ToUpper());

            }*/

            try
            {
                if ((prop.field != null && prop.field.IsStatic == false && prop.field.IsPrivate == false && prop.field.IsNotSerialized == false && serializedObject.FindProperty(prop.field.Name) != null))
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.field.Name), true);
                if ((prop.prop != null && serializedObject.FindProperty(prop.prop.Name) != null))
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(prop.prop.Name), true);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogWarning(e.Message + ": " + prop.field?.Name);
                return false;
            }
        }

        private bool AttributeFound(FieldMetInfo prop)
        {
            
            if (prop.field != null && prop.field.Name.ToUpper().Contains(search.ToUpper()))
                return true;

            if (prop.method != null && prop.method.Name.ToUpper().Contains(search.ToUpper()))
            {
                return true;
            }
            if (prop.prop != null && prop.prop.Name.ToUpper().Contains(search.ToUpper()))
            {
                return true;
            }
          

            return false;

        }

        private List<FieldMetInfo> GetProps(string attr)
        {
            List<FieldMetInfo> ret = new List<FieldMetInfo>();
            var property = ReissoftHelpers.GetAllFields(target.GetType());// target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);
            foreach (var prop in property)
            {
                // Debug.Log(prop.FieldType + ":::" + prop.Name);
                if (prop.IsStatic) continue;
                var p = prop.GetCustomAttribute<FoldoutGroup>();
                var ph = prop.GetCustomAttribute<HorizontalGroup>();
                var pb = prop.GetCustomAttribute<BoxGroup>();


                if (p != null)
                {
                    if (p.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { field = prop });
                    }

                }
                else if (ph != null)
                {
                    if (ph.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { field = prop });
                    }

                }
                else if (pb != null)
                {
                    if (pb.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { field = prop });
                    }

                }
                else if (attr == "" || attr == "all")
                {
                    ret.Add(new FieldMetInfo { field = prop });
                }
            }

            var propertys = ReissoftHelpers.GetAllProperties(target.GetType());// target.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Default);
            foreach (var prop in propertys)
            {
                // if (prop.IsStatic) continue;
                var p = prop.GetCustomAttribute<FoldoutGroup>();
                var ph = prop.GetCustomAttribute<HorizontalGroup>();
                var pb = prop.GetCustomAttribute<BoxGroup>();
                if (p != null)
                {
                    if (p.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { prop = prop });
                    }

                }
                else if (ph != null)
                {
                    if (ph.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { prop = prop });
                    }

                }
                else if (pb != null)
                {
                    if (pb.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { prop = prop });
                    }

                }
                else if (attr == "" || attr == "all")
                {
                    ret.Add(new FieldMetInfo { prop = prop });
                }
            }

            MethodInfo[] mets = target.GetType().GetMethods();
            foreach (var prop in mets)
            {
                var p = prop.GetCustomAttribute<FoldoutGroup>();
                var ph = prop.GetCustomAttribute<HorizontalGroup>();
                var pb = prop.GetCustomAttribute<BoxGroup>();
                if (p != null)
                {
                    if (p.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { method = prop });
                    }

                }
                else if (ph != null)
                {
                    if (ph.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { method = prop });
                    }

                }
                else if (pb != null)
                {
                    if (pb.name == attr || attr == "all")
                    {
                        ret.Add(new FieldMetInfo { method = prop });
                    }

                }
                else if (attr == "" || attr == "all")
                {
                    ret.Add(new FieldMetInfo { method = prop });
                }
            }
            return ret;
        }

        private List<FieldInfo> GetPropsInfo()
        {
            FieldInfo[] property = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            List<FieldInfo> ret = new List<FieldInfo>();
            foreach (var prop in property)
            {
                var p = prop.GetCustomAttribute<InfoBox>();
                if (p != null)
                {
                    ret.Add(prop);
                }

            }
            return ret;
        }

    }

    
    
    #endregion
#endif
    #region Attribute Class
    public abstract class ReissoftPropertyGroupAttribute : PropertyAttribute { }
    #endregion

    #region Attributes
    /// <summary>
    /// Show the value as a Label in read-only mode
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true,AllowMultiple = true)]
    public class LabelText : ReissoftPropertyGroupAttribute
    {
        public string value;
        public LabelText(string label)
        {
            value = label;
        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class RangeInterval : ReissoftPropertyGroupAttribute
    {
        public int valueMin;
        public int valueMax;
        public string valueLabel = "";
        public RangeInterval(int min, int max)
        {
            valueMin = min;
            valueMax = max;
        }
        public RangeInterval(int min, int max, string label)
        {
            valueMin = min;
            valueMax = max;
            valueLabel = label;
        }
    }
    /// <summary>
    /// Determines the minimum value for a numeric field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class MinValue : ReissoftPropertyGroupAttribute
    {
        public int value;
        public float valueFloat;
        public MinValue(int min)
        {
            value = min;
        }
        public MinValue(float min)
        {
            valueFloat = min;
        }
    }
    /// <summary>
    /// Determines the maximun value for a numeric field
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class MaxValue : ReissoftPropertyGroupAttribute
    {
        public int value;
        public float valueFloat;
        public MaxValue(int max)
        {
            value = max;
        }
        public MaxValue(float max)
        {
            valueFloat = max;
        }
    }
    /// <summary>
    /// Mark the field read-only
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class ReadOnly : ReissoftPropertyGroupAttribute
    {

    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class HorizontalLine : ReissoftPropertyGroupAttribute
    {

    }


    public enum CustomTypeField { STRING, ARRAY }
    /// <summary>
    /// Cria um combobox de itens customozaveis, não pode ser usado em conjunto com outro atibuto
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = true)]
    public class CustomType : ReissoftPropertyGroupAttribute
    {
        public string tp;
        public CustomTypeField typeField;
        public CustomType(string _type, CustomTypeField customTypeField)
        {
            tp = _type;
            typeField = customTypeField;
        }
    }
    /// <summary>
    /// Show value as read-only text
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class DisplayAsString : ReissoftPropertyGroupAttribute
    {

    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class IsDebug : ReissoftPropertyGroupAttribute
    {
        public string Parameter,IsTrue,IsFalse;
        public IsDebug(string parameter, string isTrue, string isFalse)
        {
            Parameter = parameter;
            IsTrue = isTrue;
            IsFalse = isFalse;
        }
    }
    /// <summary>
    /// Show value as read-only text
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class HideLabel : ReissoftPropertyGroupAttribute
    {

    }
    /// <summary>
    /// Change the color of the field in the Inspector
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class GUIColor : ReissoftPropertyGroupAttribute
    {
        public Color color;

        public GUIColor(float r, float g, float b)
        {
            color = new Color(r, g, b);
        }
    }
    [AttributeUsage(AttributeTargets.Field, Inherited = true)]
    public class VOAudio : ReissoftPropertyGroupAttribute {
        public VOAudio()
        {

        }
    }

    /// <summary>
    /// Draw a button in Inpector to invoke a method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class Button : ReissoftPropertyGroupAttribute
    {
        public string name = "";

        public Button()
        {
            this.name = "this";
        }
        public Button(string name)
        {
            this.name = name.Replace("/", "-");
        }
    };
    /// <summary>
    /// Group the Items of the same flag
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method, Inherited = true)]
    public class FoldoutGroup : ReissoftPropertyGroupAttribute
    {
        public string name = "";

        public FoldoutGroup(string name)
        {
            this.name = name.Replace("/", "-");
        }
    };
    /// <summary>
    /// Draw an information above the property in Inpector
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class InfoBox : ReissoftPropertyGroupAttribute
    {
        public string info = "";
        public InfoBox(string info)
        {
            this.info = info;
        }
    };

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class MinMax : ReissoftPropertyGroupAttribute
    {
        public float min;
        public float max;

        public MinMax(float _min, float _max)
        {
            this.min = _min;
            this.max = _max;
        }

    };

    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class ProgressBar : ReissoftPropertyGroupAttribute
    {
        public string title;
        public int min;
        public int max;

        public ProgressBar(string _title, float _min, float _max)
        {
            this.min = Mathf.RoundToInt(_min);
            this.max = Mathf.RoundToInt(_max);
            this.title = _title;
        }

    };
    /// <summary>
    /// Group the Items of the same flag
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class HorizontalGroup : ReissoftPropertyGroupAttribute
    {
        public string name = "";

        public HorizontalGroup(string name)
        {
            this.name = name.Replace("/", "-");
        }
        public HorizontalGroup()
        {
            this.name = "";
        }

    }
    /// <summary>
    /// Group the Items of the same flag
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true)]
    public class BoxGroup : ReissoftPropertyGroupAttribute
    {
        public string name = "";

        public BoxGroup(string name)
        {
            this.name = name.Replace("/", "-");
        }
        public BoxGroup(string name, bool center)
        {
            this.name = name.Replace("/", "-");
        }
        public BoxGroup()
        {
            this.name = "";
        }

    }
    /// <summary>
    /// Only draw the property if the condition is positive.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class ShowIf : ReissoftPropertyGroupAttribute
    {
        public string parameter = "";
        public object condition;
        public ShowIf(string parameter)
        {
            this.parameter = parameter;
            this.condition = true;
        }
        public ShowIf(string parameter, object condition)
        {
            this.parameter = parameter;
            this.condition = condition;
        }
    };


    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class Search : ReissoftPropertyGroupAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class UseSearch : ReissoftPropertyGroupAttribute
    {

    }

    /// <summary>
    /// Only draw the property if the condition is positive.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class EnableIf : ReissoftPropertyGroupAttribute
    {
        public string parameter = "";
        public object condition;
        public EnableIf(string parameter)
        {
            this.parameter = parameter;
            this.condition = true;
        }
        public EnableIf(string parameter, object condition)
        {
            this.parameter = parameter;
            this.condition = condition;
        }
    };

    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class Doc : ReissoftPropertyGroupAttribute
    {
        public string url;
        public Doc(string url)
        {
            this.url = url;
        }
    }

    /// <summary>
    /// Only draw the property if the condition is positive.
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = true, AllowMultiple = true)]
    public class DisableIf : ReissoftPropertyGroupAttribute
    {
        public string parameter = "";
        public object condition;
        public DisableIf(string parameter)
        {
            this.parameter = parameter;
            this.condition = true;
        }
        public DisableIf(string parameter, object condition)
        {
            this.parameter = parameter;
            this.condition = condition;
        }
    };


    /// <summary>
    /// Do not draw property if condition is positive
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class HideIf : ReissoftPropertyGroupAttribute
    {
        public string parameter = "";
        public object condition;
        public HideIf(string parameter)
        {
            this.parameter = parameter;
            this.condition = true;
        }
        public HideIf(string parameter, object condition)
        {
            this.parameter = parameter;
            this.condition = condition;
        }
    };

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class IgnoreReissoftAttributes : Attribute { }

    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public class UseReissoftAttributes : Attribute { }

    #endregion
#if UNITY_EDITOR
    //[CustomPropertyDrawer(typeof(AnimationClip))]
    //[CustomPropertyDrawer(typeof(ScriptableObject), true)]
    public class AddButtonSelection : PropertyDrawer
    {

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);
            EditorGUI.PropertyField(new Rect(position.x, position.y, position.width - 30, position.height), property, label);
            if (GUI.Button(new Rect(position.width + 20, position.y, 20, position.height), "I"))
            {
                SelectItemEditor select = new SelectItemEditor();
                select.Init(property);
            }
            EditorGUI.EndProperty();

        }
    }

    public class SelectItemEditor : EditorWindow
    {

        // Add menu named "My Window" to the Window menu
        public void Init(SerializedProperty property)
        {
            ActiveEditorTracker.sharedTracker.isLocked = true;

            Selection.activeObject = property.objectReferenceValue;
            // Retrieve the existing Inspector tab, or create a new one if none is open
            EditorWindow inspectorWindow = EditorWindow.GetWindow(typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.InspectorWindow"));
            // Get the size of the currently window
            Vector2 size = new Vector2(inspectorWindow.position.width, inspectorWindow.position.height);
            // Clone the inspector tab (optionnal step)
            inspectorWindow = Instantiate(inspectorWindow);
            // inspectorWindow.ShowPopup();
            // Set min size, and focus the window
            inspectorWindow.minSize = size;
            inspectorWindow.Focus();

        }

    }

#endif

    public static class ReissoftHelpers
    {
        public static IEnumerable<FieldInfo> GetAllFields(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<FieldInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            return type.GetFields(flags).Union(GetAllFields(type.BaseType));
        }
        public static IEnumerable<PropertyInfo> GetAllProperties(this Type type)
        {
            if (type == null)
            {
                return Enumerable.Empty<PropertyInfo>();
            }

            BindingFlags flags = BindingFlags.Public |
                                 BindingFlags.NonPublic |
                                 BindingFlags.Static |
                                 BindingFlags.Instance |
                                 BindingFlags.DeclaredOnly;

            return type.GetProperties(flags).Union(GetAllProperties(type.BaseType));
        }

    }


}
