
using UnityEngine;
using F.UI;

namespace F.Cards
{
    [CreateAssetMenu(fileName = "CoinScriptable", menuName = "Cards/CoinScriptable", order = 3)]
    public class CoinScriptable : CardScriptable
    {
        [Header("Tarot Specific")]
        [SerializeField] public int id;

    }
}
