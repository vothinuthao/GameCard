using Core.Utils;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Scriptable object for elemental cards
    /// </summary>
    [CreateAssetMenu(fileName = "New Elemental Card", menuName = "NguhanhGame/Elemental Card Data")]
    public class ElementalCardDataSO : CardDataSO
    {
        [Header("Nạp Âm")]
        public int napAmIndex;
        public string napAmName; 
    }
}