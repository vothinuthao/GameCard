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
        [Header("Element Information")]
        public ElementType elementType;
        
        // Use property drawer in Inspector to show the appropriate NapAm enum based on selected element
        [Header("NapAm Information")]
        public int napAmIndex;
        
        [System.NonSerialized]
        public string napAmName; // Will be set at runtime based on elementType and napAmIndex
    }
}