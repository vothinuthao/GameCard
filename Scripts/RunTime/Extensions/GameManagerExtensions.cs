using System.Collections.Generic;
using Core;

namespace RunTime
{
    /// <summary>
    /// Extension methods for GameManager to integrate with LoadCardData
    /// </summary>
    public static class GameManagerCardExtensions
    {
        /// <summary>
        /// Create a random deck for starting a new game
        /// </summary>
        public static List<Entity> CreateRandomDeck(this GameManager gameManager, int deckSize = 10, bool balanced = true)
        {
            // Use LoadCardData to create a random deck
            return LoadCardData.Instance.CreateRandomDeck(deckSize, balanced);
        }
        
        /// <summary>
        /// Create a random starter deck with a specific theme
        /// </summary>
        public static List<Entity> CreateRandomStarterDeck(this GameManager gameManager, string theme = "balanced", int deckSize = 8)
        {
            return LoadCardData.Instance.CreateRandomStarterDeck(theme, deckSize);
        }
        
        /// <summary>
        /// Get a card by key name
        /// </summary>
        public static Entity GetCardByKeyName(this GameManager gameManager, string keyName)
        {
            // Use LoadCardData to get a card by key name
            return LoadCardData.Instance.GetCardByKeyName(keyName);
        }
        
        /// <summary>
        /// Get cards by element
        /// </summary>
        public static List<Entity> GetCardsByElement(this GameManager gameManager, Core.Utils.ElementType elementType)
        {
            // Use LoadCardData to get cards by element
            return LoadCardData.Instance.GetCardsByElement(elementType);
        }
        
        /// <summary>
        /// Get cards by type
        /// </summary>
        public static List<Entity> GetCardsByType(this GameManager gameManager, Core.Utils.CardType cardType)
        {
            // Use LoadCardData to get cards by type
            return LoadCardData.Instance.GetCardsByType(cardType);
        }
        
        /// <summary>
        /// Get cards by rarity
        /// </summary>
        public static List<Entity> GetCardsByRarity(this GameManager gameManager, Core.Utils.Rarity rarity)
        {
            // Use LoadCardData to get cards by rarity
            return LoadCardData.Instance.GetCardsByRarity(rarity);
        }
    }
}