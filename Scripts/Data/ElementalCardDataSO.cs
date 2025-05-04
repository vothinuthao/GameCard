using Core.Utils;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Scriptable object for elemental cards with Nap Am specialization
    /// </summary>
    [CreateAssetMenu(fileName = "New Elemental Card", menuName = "NguhanhGame/Elemental Card Data")]
    public class ElementalCardDataSO : CardDataSO
    {
        [Header("Nạp Âm Configuration")]
        [Tooltip("The element type of this card")]
        public ElementType elementType = ElementType.None;
        
        [Tooltip("Nạp Âm index (1-6) for this element")]
        [Range(1, 6)]
        public int napAmIndex = 1;
        
        /// <summary>
        /// Validate and set the Nap Am index
        /// </summary>
        public void SetNapAmIndex(int index)
        {
            // Ensure index is between 1 and 6
            napAmIndex = Mathf.Clamp(index, 1, 6);
        }
    }
}