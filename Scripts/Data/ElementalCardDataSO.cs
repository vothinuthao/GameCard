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
        [System.NonSerialized]
        public string napAmName; // Will be set at runtime based on elementType and napAmIndex
    }
}