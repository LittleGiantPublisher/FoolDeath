using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;
using F.UI;

namespace F.Cards
{
    [CreateAssetMenu(fileName = "CardScriptable", menuName = "Cards/CardScriptable", order = 2)]
    public class CardScriptable : ScriptableObject
    {

        [Header("Visual")]
        [SerializeField] public Sprite cardArt;
    }
}
