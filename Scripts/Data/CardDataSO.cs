using Core.Utils;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Base class for all card scriptable objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Card", menuName = "NguhanhGame/Card Data")]
    public class CardDataSO : ScriptableObject
    {
        [Header("Card Identification")]
        public int cardId;
        public string cardKeyName; 
        
        [Header("Basic Information")]
        public string cardName;
        public string description;
        public CardType cardType;
        public Rarity rarity;
        public int cost;
        public Sprite artwork;
        public ElementType elementType;

        [Header("Other")]
        public int napAmIndex;
        
        [Header("Stats")]
        public int attack;
        public int defense;
        public int health;
        public int speed;
    }
}