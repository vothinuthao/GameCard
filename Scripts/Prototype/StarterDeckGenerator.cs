using System.Collections.Generic;
using Core;
using Factories;
using RunTime;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// Generates starter decks for the game
    /// </summary>
    public class StarterDeckGenerator : MonoBehaviour
    {
        [System.Serializable]
        public class StarterDeck
        {
            public string name;
            public string description;
            public List<string> cardPaths = new List<string>();
        }
        
        [Header("Starter Decks")]
        [SerializeField] private List<StarterDeck> starterDecks = new List<StarterDeck>();
        
        /// <summary>
        /// Create a starter deck based on index
        /// </summary>
        public List<Entity> CreateStarterDeck(int deckIndex)
        {
            // Check if index is valid
            if (deckIndex < 0 || deckIndex >= starterDecks.Count)
            {
                Debug.LogError($"Invalid deck index: {deckIndex}");
                return null;
            }
            
            // Get starter deck
            StarterDeck starterDeck = starterDecks[deckIndex];
            
            // Create deck
            List<Entity> deck = new List<Entity>();
            
            // Get entity manager and factory
            EntityManager entityManager = GameManager.Instance.GetEntityManager();
            ScriptableObjectFactory factory = new ScriptableObjectFactory(entityManager);
            
            // Create entities from card paths
            foreach (var cardPath in starterDeck.cardPaths)
            {
                var cardData = Resources.Load<Data.CardDataSO>(cardPath);
                if (cardData != null)
                {
                    Entity card = factory.CreateCardFromSO(cardData);
                    deck.Add(card);
                }
                else
                {
                    Debug.LogError($"Could not load card data: {cardPath}");
                }
            }
            
            return deck;
        }
        
        // Start is called before the first frame update
        private void Start()
        {
            // Initialize starter decks if not already defined
            if (starterDecks.Count == 0)
            {
                // Metal Mastery Deck
                StarterDeck metalDeck = new StarterDeck
                {
                    name = "Metal Mastery",
                    description = "Focuses on Metal element cards with high attack and penetration."
                };
                
                metalDeck.cardPaths.Add("Cards/Metal_SwordQi_Card");
                metalDeck.cardPaths.Add("Cards/Metal_SwordQi_Card");
                metalDeck.cardPaths.Add("Cards/Metal_Hardness_Card");
                metalDeck.cardPaths.Add("Cards/Metal_Hardness_Card");
                metalDeck.cardPaths.Add("Cards/Metal_Purity_Card");
                metalDeck.cardPaths.Add("Cards/Metal_Reflection_Card");
                metalDeck.cardPaths.Add("Cards/Fire_Burning_Card");
                metalDeck.cardPaths.Add("Cards/Earth_Solidity_Card");
                
                // Wood Wisdom Deck
                StarterDeck woodDeck = new StarterDeck
                {
                    name = "Wood Wisdom",
                    description = "Focuses on Wood element cards with healing and growth abilities."
                };
                
                woodDeck.cardPaths.Add("Cards/Wood_Toxin_Card");
                woodDeck.cardPaths.Add("Cards/Wood_Toxin_Card");
                woodDeck.cardPaths.Add("Cards/Wood_Regeneration_Card");
                woodDeck.cardPaths.Add("Cards/Wood_Flexibility_Card");
                woodDeck.cardPaths.Add("Cards/Wood_Flexibility_Card");
                woodDeck.cardPaths.Add("Cards/Wood_Symbiosis_Card");
                woodDeck.cardPaths.Add("Cards/Water_Ice_Card");
                woodDeck.cardPaths.Add("Cards/Fire_Burning_Card");
                
                // Five Elements Balance Deck
                StarterDeck balanceDeck = new StarterDeck
                {
                    name = "Five Elements Balance",
                    description = "A balanced deck with cards from all five elements."
                };
                
                balanceDeck.cardPaths.Add("Cards/Metal_SwordQi_Card");
                balanceDeck.cardPaths.Add("Cards/Wood_Regeneration_Card");
                balanceDeck.cardPaths.Add("Cards/Water_Ice_Card");
                balanceDeck.cardPaths.Add("Cards/Fire_Burning_Card");
                balanceDeck.cardPaths.Add("Cards/Earth_Solidity_Card");
                balanceDeck.cardPaths.Add("Cards/Metal_Hardness_Card");
                balanceDeck.cardPaths.Add("Cards/Wood_Flexibility_Card");
                balanceDeck.cardPaths.Add("Cards/Special_Rat_Card");
                
                // Add decks to list
                starterDecks.Add(metalDeck);
                starterDecks.Add(woodDeck);
                starterDecks.Add(balanceDeck);
            }
        }
    }
}