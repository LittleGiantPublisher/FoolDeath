using UnityEngine;
using F.UI;

namespace F.Cards
{
    [CreateAssetMenu(fileName = "TarotScriptable", menuName = "Cards/TarotScriptable", order = 3)]
    public class TarotScriptable : CardScriptable
    {
        [Header("Tarot Specific")]
        [SerializeField] public int value;
        [SerializeField] public Sprite foolArt;

    }
}
