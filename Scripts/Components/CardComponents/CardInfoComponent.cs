using Core.Utils;
using UnityEngine;
using Component = Core.Component;

namespace Components
{
    /// <summary>
    /// Contains basic information about a card
    /// </summary>
    public class CardInfoComponent : Component
    {
        // Basic card information
        public string Name { get; set; }
        public string Description { get; set; }
        public CardType Type { get; set; }
        public Rarity Rarity { get; set; }
        public Sprite Artwork { get; set; }
        public int Cost { get; set; }
        public CardState State { get; set; } = CardState.InDeck;
    }
}