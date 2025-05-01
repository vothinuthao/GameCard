using System.Collections.Generic;
using Core;
using Core.Utils;
using Data;
using UnityEngine;

namespace Factories
{
    /// <summary>
    /// Factory for loading card data from Resources and creating card entities
    /// </summary>
    public class CardLoader
    {
        private EntityManager _entityManager;
        private ScriptableObjectFactory _factory;
        
        public CardLoader(EntityManager entityManager)
        {
            _entityManager = entityManager;
            // _factory = new ScriptableObjectFactory(entityManager);
        }
        
        /// <summary>
        /// Load a single card by key name from Resources and convert to Entity
        /// </summary>
        /// <param name="keyName">The key name of the card</param>
        /// <param name="folderPath">Optional folder path within Resources</param>
        /// <returns>Entity representing the card</returns>
        public Entity LoadCardByKeyName(string keyName, string folderPath = "Cards")
        {
            string cardPath = $"{folderPath}/{keyName}";
            CardDataSO cardData = Resources.Load<CardDataSO>(cardPath);
            
            if (cardData == null)
            {
                Debug.LogWarning($"Card not found: {cardPath}");
                return null;
            }
            
            return _factory.CreateCardFromSO(cardData);
        }
        
        /// <summary>
        /// Load a card by ID from Resources
        /// </summary>
        /// <param name="cardId">The ID of the card to load</param>
        /// <param name="folderPath">Optional folder path within Resources</param>
        /// <returns>Entity representing the card</returns>
        public Entity LoadCardById(int cardId, string folderPath = "Cards")
        {
            // Find all card assets in Resources
            CardDataSO[] allCards = Resources.LoadAll<CardDataSO>(folderPath);
            
            // Find the card with the matching ID
            foreach (var cardData in allCards)
            {
                if (cardData.cardId == cardId)
                {
                    return _factory.CreateCardFromSO(cardData);
                }
            }
            
            Debug.LogWarning($"Card with ID {cardId} not found in {folderPath}");
            return null;
        }
        
        /// <summary>
        /// Load all cards from Resources folder/subfolders and create entities
        /// </summary>
        /// <param name="folderPath">Folder path within Resources</param>
        /// <returns>List of card entities</returns>
        public List<Entity> LoadAllCards(string folderPath = "Cards")
        {
            CardDataSO[] allCards = Resources.LoadAll<CardDataSO>(folderPath);
            List<Entity> result = new List<Entity>();
            
            foreach (var cardData in allCards)
            {
                Entity card = _factory.CreateCardFromSO(cardData);
                if (card != null)
                {
                    result.Add(card);
                }
            }
            
            Debug.Log($"Loaded {result.Count} cards from Resources/{folderPath}");
            return result;
        }
        
        /// <summary>
        /// Load all cards of a specific type from Resources and create entities
        /// </summary>
        /// <param name="cardType">Type of cards to load</param>
        /// <param name="baseFolder">Base folder path within Resources</param>
        /// <returns>List of card entities of the specified type</returns>
        public List<Entity> LoadCardsByType(CardType cardType, string baseFolder = "Cards")
        {
            string typeFolder;
            
            // Determine subfolder based on card type
            switch (cardType)
            {
                case CardType.ElementalCard:
                    typeFolder = $"{baseFolder}/ElementalCards";
                    break;
                case CardType.DivineBeast:
                    typeFolder = $"{baseFolder}/DivineBeasts";
                    break;
                case CardType.Monster:
                    typeFolder = $"{baseFolder}/Monsters";
                    break;
                case CardType.SpiritAnimal:
                    typeFolder = $"{baseFolder}/SpiritAnimals";
                    break;
                case CardType.Joker:
                    typeFolder = $"{baseFolder}/Jokers";
                    break;
                default:
                    typeFolder = baseFolder;
                    break;
            }
            
            // Try to load from type folder first
            CardDataSO[] typedCards = Resources.LoadAll<CardDataSO>(typeFolder);
            
            // If no cards found in type folder, try loading from base folder and filtering
            if (typedCards.Length == 0 && typeFolder != baseFolder)
            {
                CardDataSO[] allCards = Resources.LoadAll<CardDataSO>(baseFolder);
                List<CardDataSO> filteredCards = new List<CardDataSO>();
                
                foreach (var card in allCards)
                {
                    if (card.cardType == cardType)
                    {
                        filteredCards.Add(card);
                    }
                }
                
                typedCards = filteredCards.ToArray();
            }
            
            // Create entities from card data
            List<Entity> result = new List<Entity>();
            foreach (var cardData in typedCards)
            {
                Entity card = _factory.CreateCardFromSO(cardData);
                if (card != null)
                {
                    result.Add(card);
                }
            }
            
            Debug.Log($"Loaded {result.Count} {cardType} cards");
            return result;
        }
        
        /// <summary>
        /// Load a custom deck from a list of card IDs
        /// </summary>
        /// <param name="cardIds">List of card IDs to include in the deck</param>
        /// <param name="folderPath">Folder path within Resources</param>
        /// <returns>List of card entities forming the deck</returns>
        public List<Entity> LoadDeckFromCardIds(List<int> cardIds, string folderPath = "Cards")
        {
            List<Entity> deck = new List<Entity>();
            
            foreach (int cardId in cardIds)
            {
                Entity card = LoadCardById(cardId, folderPath);
                if (card != null)
                {
                    deck.Add(card);
                }
            }
            
            return deck;
        }
        
        /// <summary>
        /// Load a custom deck from a list of card key names
        /// </summary>
        /// <param name="cardKeyNames">List of card key names to include in the deck</param>
        /// <param name="folderPath">Folder path within Resources</param>
        /// <returns>List of card entities forming the deck</returns>
        public List<Entity> LoadDeckFromKeyNames(List<string> cardKeyNames, string folderPath = "Cards")
        {
            List<Entity> deck = new List<Entity>();
            
            foreach (string keyName in cardKeyNames)
            {
                Entity card = LoadCardByKeyName(keyName, folderPath);
                if (card != null)
                {
                    deck.Add(card);
                }
            }
            
            return deck;
        }
        
        /// <summary>
        /// Load a starter deck based on an index/preset
        /// </summary>
        /// <param name="deckIndex">Index of the starter deck to load</param>
        /// <returns>List of card entities forming the starter deck</returns>
        public List<Entity> LoadStarterDeck(int deckIndex)
        {
            // Define some predefined starter decks
            switch (deckIndex)
            {
                case 0: // Metal Mastery Deck
                    return LoadDeckFromKeyNames(new List<string>
                    {
                        "ElementalCards/metal_sword",
                        "ElementalCards/metal_sword", // Duplicate for 2 copies
                        "ElementalCards/metal_shield",
                        "ElementalCards/metal_shield", // Duplicate for 2 copies
                        "ElementalCards/metal_pure",
                        "DivineBeasts/white_tiger",
                        "ElementalCards/fire_flame",
                        "ElementalCards/earth_rock"
                    });
                    
                case 1: // Wood Wisdom Deck
                    return LoadDeckFromKeyNames(new List<string>
                    {
                        "ElementalCards/wood_needle",
                        "ElementalCards/wood_needle", // Duplicate for 2 copies
                        "ElementalCards/wood_life",
                        "ElementalCards/wood_flexibility",
                        "ElementalCards/wood_flexibility", // Duplicate for 2 copies
                        "DivineBeasts/azure_dragon",
                        "ElementalCards/water_arrow",
                        "ElementalCards/fire_flame"
                    });
                    
                case 2: // Five Elements Balance Deck
                    return LoadDeckFromKeyNames(new List<string>
                    {
                        "ElementalCards/metal_sword",
                        "ElementalCards/wood_life",
                        "ElementalCards/water_arrow",
                        "ElementalCards/fire_flame",
                        "ElementalCards/earth_rock",
                        "ElementalCards/metal_shield",
                        "ElementalCards/wood_flexibility",
                        "SpiritAnimals/rat_swift"
                    });
                    
                default:
                    Debug.LogWarning($"Starter deck index {deckIndex} not defined, loading balanced deck instead");
                    return LoadStarterDeck(2); // Default to balanced deck
            }
        }
    }
}