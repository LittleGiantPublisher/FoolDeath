#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[UnityEditor.InitializeOnLoad]
public class AfilPortingEditor : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths, bool didDomainReload)
    {
        CheckAchievementsData();
        CheckScenes();
    }

    

    [UnityEditor.MenuItem("Afil/Porting/Check Achievement Data")]
    public static void CheckAchievementsData()
    {
        string[] datas = UnityEditor.AssetDatabase.FindAssets("t:AchievementData", null);
        if (datas.Length == 0)
        {
            UnityEditor.EditorUtility.DisplayDialog("Faltando data", "AchievementData não foi encontrado, um novo data será criado", "Blz");
            var d = ScriptableObject.CreateInstance<AchievementData>();
            if (UnityEditor.AssetDatabase.IsValidFolder("Assets/Resources") == false)
                UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");


            UnityEditor.AssetDatabase.CreateAsset(d, "Assets/Resources/AchievementData.asset");
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            UnityEditor.EditorUtility.FocusProjectWindow();
            UnityEditor.EditorUtility.DisplayDialog("OK", "AchievementData criado no caminho:\n'Assets/Resources/AchievementData.asset'\nNão esqueça de preencher os dados'", "OK");
        }
        else
        {
            Debug.Log("AchievementData ok");
        }
    }

    private static void CheckScenes()
    {
        if (UnityEditor.AssetDatabase.IsValidFolder("Assets/PortingScenes") == false)
        {
            UnityEditor.AssetDatabase.CreateFolder("Assets", "PortingScenes");
        }

        if (UnityEditor.AssetDatabase.FindAssets("AfilLogo t:Scene", new string[] { "Assets" }).Length > 0)
        {
            Debug.Log("Cenas já no projeto");
            return;
        }

            string[] datas = UnityEditor.AssetDatabase.FindAssets("AfilLogo t:Scene", new string[] { "Packages"});
            foreach (var d in datas)
            {
                
                UnityEditor.AssetDatabase.CopyAsset(UnityEditor.AssetDatabase.GUIDToAssetPath(d), "Assets/PortingScenes/AfilLogo.unity");
            }
            string[] datas2 = UnityEditor.AssetDatabase.FindAssets("LoadControlleManager t:Scene", new string[] { "Packages" });
            foreach (var d in datas2)
                UnityEditor.AssetDatabase.CopyAsset(UnityEditor.AssetDatabase.GUIDToAssetPath(d), "Assets/PortingScenes/LoadControlleManager.unity");

        Debug.Log("Ajustou as cenas ");
    }

    [UnityEditor.MenuItem("Afil/Porting/Download dependences")]
    public static void Download()
    {

        Application.OpenURL("https://drive.google.com/drive/folders/1KdAKCFjblFQBiWF-v4kjf5BdjKzs1igR?usp=drive_link");

    }
    [UnityEditor.MenuItem("Afil/Porting/Check Scenes")]
    public static void TestGame()
    {

        CheckScenes();

    }
}
#endif