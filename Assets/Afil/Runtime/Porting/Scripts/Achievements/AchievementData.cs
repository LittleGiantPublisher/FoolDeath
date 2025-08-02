using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AchievementData", menuName = "ScriptableObjects/Afil/AchievementData", order = 1)]
public class AchievementData : ScriptableObject
{
    public AchievementField[] Achievements;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if(name != "AchievementData")
        {
            UnityEditor.EditorUtility.DisplayDialog("Data Renomeado", "AchievementData não pode ser renomeado, por favor ajuste o nome para 'AchievementData'", "Vixi, OK");
        }
    }
#endif
}
[System.Serializable]
public struct AchievementField
{
    public int id;
    public string name;
}