using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using UnityEngine;

namespace RunTime
{
    /// <summary>
    /// Utility class for building decks with various strategies
    /// </summary>
    public static class DeckBuilderUtility
    {
        /// <summary>
        /// Create a deck with a specific strategy
        /// </summary>
        public static List<Entity> CreateDeckWithStrategy(DeckStrategy strategy, int deckSize = 10)
        {
            List<Entity> deck = new List<Entity>();
            
            switch (strategy)
            {
                case DeckStrategy.ElementalFocus:
                    CreateElementalFocusDeck(deck, GetRandomElement(), deckSize);
                    break;
                case DeckStrategy.ComboOriented:
                    CreateComboOrientedDeck(deck, deckSize);
                    break;
                case DeckStrategy.DefensiveFocus:
                    CreateDefensiveDeck(deck, deckSize);
                    break;
                case DeckStrategy.AggressiveFocus:
                    CreateAggressiveDeck(deck, deckSize);
                    break;
                case DeckStrategy.SupportFocus:
                    CreateSupportFocusDeck(deck, deckSize);
                    break;
                case DeckStrategy.Random:
                default:
                    deck = LoadCardData.Instance.CreateRandomDeck(deckSize, true);
                    break;
            }
            
            return deck;
        }
        
        /// <summary>
        /// Create a deck focused on a specific element
        /// </summary>
        private static void CreateElementalFocusDeck(List<Entity> deck, ElementType focusElement, int deckSize)
        {
            // 70% of the chosen element, 30% supportive cards
            int primaryCards = Mathf.FloorToInt(deckSize * 0.7f);
            int supportCards = deckSize - primaryCards;
            
            // Get cards of the focus element
            List<Entity> elementCards = LoadCardData.Instance.GetCardsByElement(focusElement);
            if (elementCards != null && elementCards.Count > 0)
            {
                // Shuffle and add cards
                ShuffleList(elementCards);
                for (int i = 0; i < Mathf.Min(primaryCards, elementCards.Count); i++)
                {
                    deck.Add(elementCards[i]);
                }
            }
            
            // Get support cards (preferring those that have synergy with the focus element)
            List<Entity> allCards = new List<Entity>();
            
            // Get divine beasts and spirit animals that match our element
            List<Entity> divineBeasts = LoadCardData.Instance.GetCardsByType(CardType.DivineBeast);
            List<Entity> spiritAnimals = LoadCardData.Instance.GetCardsByType(CardType.SpiritAnimal);
            
            foreach (var entity in divineBeasts)
            {
                ElementComponent elementComp = entity.GetComponent<ElementComponent>();
                if (elementComp != null && elementComp.Element == focusElement)
                {
                    allCards.Add(entity);
                }
            }
            
            foreach (var entity in spiritAnimals)
            {
                ElementComponent elementComp = entity.GetComponent<ElementComponent>();
                if (elementComp != null && elementComp.Element == focusElement)
                {
                    allCards.Add(entity);
                }
            }
            
            // Add elements that have synergy based on Wu Xing cycle
            ElementType generatingElement = GetGeneratingElement(focusElement);
            List<Entity> generatingCards = LoadCardData.Instance.GetCardsByElement(generatingElement);
            if (generatingCards != null && generatingCards.Count > 0)
            {
                allCards.AddRange(generatingCards);
            }
            
            // Shuffle and add support cards
            ShuffleList(allCards);
            for (int i = 0; i < Mathf.Min(supportCards, allCards.Count); i++)
            {
                deck.Add(allCards[i]);
            }
            
            // If we don't have enough cards, add random ones
            if (deck.Count < deckSize)
            {
                List<Entity> randomDeck = LoadCardData.Instance.CreateRandomDeck(deckSize - deck.Count, false);
                deck.AddRange(randomDeck);
            }
            
            // Shuffle the final deck
            ShuffleList(deck);
        }
        
        /// <summary>
        /// Create a deck focused on combos and synergy
        /// </summary>
        private static void CreateComboOrientedDeck(List<Entity> deck, int deckSize)
        {
            // For combo decks, we want sets of cards that work well together
            
            // We'll focus on element pairs with generating relationships
            List<ElementPair> generatingPairs = new List<ElementPair>
            {
                new ElementPair(ElementType.Metal, ElementType.Water), // Metal generates Water
                new ElementPair(ElementType.Water, ElementType.Wood),  // Water generates Wood
                new ElementPair(ElementType.Wood, ElementType.Fire),   // Wood generates Fire
                new ElementPair(ElementType.Fire, ElementType.Earth),  // Fire generates Earth
                new ElementPair(ElementType.Earth, ElementType.Metal)  // Earth generates Metal
            };
            
            // Shuffle the pairs
            ShuffleList(generatingPairs);
            
            // Take the first 2 pairs
            List<ElementPair> selectedPairs = generatingPairs.GetRange(0, Mathf.Min(2, generatingPairs.Count));
            
            // For each pair, add cards from both elements
            int cardsPerPair = deckSize / 2;
            
            foreach (var pair in selectedPairs)
            {
                List<Entity> firstElementCards = LoadCardData.Instance.GetCardsByElement(pair.First);
                List<Entity> secondElementCards = LoadCardData.Instance.GetCardsByElement(pair.Second);
                
                ShuffleList(firstElementCards);
                ShuffleList(secondElementCards);
                
                // Add cards from both elements, alternating
                int cardsAdded = 0;
                for (int i = 0; i < cardsPerPair; i++)
                {
                    if (i % 2 == 0 && firstElementCards.Count > (i / 2))
                    {
                        deck.Add(firstElementCards[i / 2]);
                        cardsAdded++;
                    }
                    else if (secondElementCards.Count > (i / 2))
                    {
                        deck.Add(secondElementCards[i / 2]);
                        cardsAdded++;
                    }
                    
                    if (cardsAdded >= cardsPerPair)
                        break;
                }
            }
            
            // Add joker cards for special effects
            List<Entity> jokerCards = LoadCardData.Instance.GetCardsByType(CardType.Joker);
            ShuffleList(jokerCards);
            
            for (int i = 0; i < Mathf.Min(2, jokerCards.Count); i++)
            {
                if (deck.Count < deckSize)
                {
                    deck.Add(jokerCards[i]);
                }
            }
            
            // Fill any remaining slots with random cards
            if (deck.Count < deckSize)
            {
                List<Entity> randomDeck = LoadCardData.Instance.CreateRandomDeck(deckSize - deck.Count, false);
                deck.AddRange(randomDeck);
            }
            
            // Shuffle the final deck
            ShuffleList(deck);
        }
        
        /// <summary>
        /// Create a defensive-focused deck
        /// </summary>
        private static void CreateDefensiveDeck(List<Entity> deck, int deckSize)
        {
            // Defensive decks focus on Metal and Earth elements, with high defense stats
            List<Entity> metalCards = LoadCardData.Instance.GetCardsByElement(ElementType.Metal);
            List<Entity> earthCards = LoadCardData.Instance.GetCardsByElement(ElementType.Earth);
            
            // Filter for cards with good defense
            List<Entity> defensiveCards = new List<Entity>();
            
            foreach (var card in metalCards)
            {
                StatsComponent stats = card.GetComponent<StatsComponent>();
                if (stats != null && stats.Defense >= 3)
                {
                    defensiveCards.Add(card);
                }
            }
            
            foreach (var card in earthCards)
            {
                StatsComponent stats = card.GetComponent<StatsComponent>();
                if (stats != null && stats.Defense >= 3)
                {
                    defensiveCards.Add(card);
                }
            }
            
            // Shuffle defensive cards
            ShuffleList(defensiveCards);
            
            // Add defensive cards (about 70% of deck)
            int defensiveCount = Mathf.FloorToInt(deckSize * 0.7f);
            for (int i = 0; i < Mathf.Min(defensiveCount, defensiveCards.Count); i++)
            {
                deck.Add(defensiveCards[i]);
            }
            
            // Add some water cards for healing (about 20% of deck)
            List<Entity> waterCards = LoadCardData.Instance.GetCardsByElement(ElementType.Water);
            ShuffleList(waterCards);
            
            int waterCount = Mathf.FloorToInt(deckSize * 0.2f);
            for (int i = 0; i < Mathf.Min(waterCount, waterCards.Count); i++)
            {
                deck.Add(waterCards[i]);
            }
            
            // Add a divine beast for support if available
            List<Entity> divineBeasts = LoadCardData.Instance.GetCardsByType(CardType.DivineBeast);
            if (divineBeasts != null && divineBeasts.Count > 0)
            {
                foreach (var beast in divineBeasts)
                {
                    ElementComponent elementComp = beast.GetComponent<ElementComponent>();
                    if (elementComp != null && 
                        (elementComp.Element == ElementType.Metal || elementComp.Element == ElementType.Earth))
                    {
                        deck.Add(beast);
                        break;
                    }
                }
            }
            
            // Fill any remaining slots with random cards
            if (deck.Count < deckSize)
            {
                List<Entity> randomDeck = LoadCardData.Instance.CreateRandomDeck(deckSize - deck.Count, false);
                deck.AddRange(randomDeck);
            }
            
            // Shuffle the final deck
            ShuffleList(deck);
        }
        
        /// <summary>
        /// Create an aggressive-focused deck
        /// </summary>
        private static void CreateAggressiveDeck(List<Entity> deck, int deckSize)
        {
            // Aggressive decks focus on Fire and Wood elements, with high attack stats
            List<Entity> fireCards = LoadCardData.Instance.GetCardsByElement(ElementType.Fire);
            List<Entity> woodCards = LoadCardData.Instance.GetCardsByElement(ElementType.Wood);
            
            // Filter for cards with good attack
            List<Entity> attackCards = new List<Entity>();
            
            foreach (var card in fireCards)
            {
                StatsComponent stats = card.GetComponent<StatsComponent>();
                if (stats != null && stats.Attack >= 3)
                {
                    attackCards.Add(card);
                }
            }
            
            foreach (var card in woodCards)
            {
                StatsComponent stats = card.GetComponent<StatsComponent>();
                if (stats != null && stats.Attack >= 3)
                {
                    attackCards.Add(card);
                }
            }
            
            // Shuffle attack cards
            ShuffleList(attackCards);
            
            // Add attack cards (about 70% of deck)
            int attackCount = Mathf.FloorToInt(deckSize * 0.7f);
            for (int i = 0; i < Mathf.Min(attackCount, attackCards.Count); i++)
            {
                deck.Add(attackCards[i]);
            }
            
            // Add some metal cards for additional damage (about 20% of deck)
            List<Entity> metalCards = LoadCardData.Instance.GetCardsByElement(ElementType.Metal);
            ShuffleList(metalCards);
            
            int metalCount = Mathf.FloorToInt(deckSize * 0.2f);
            for (int i = 0; i < Mathf.Min(metalCount, metalCards.Count); i++)
            {
                deck.Add(metalCards[i]);
            }
            
            // Add a divine beast for support if available
            List<Entity> divineBeasts = LoadCardData.Instance.GetCardsByType(CardType.DivineBeast);
            if (divineBeasts != null && divineBeasts.Count > 0)
            {
                foreach (var beast in divineBeasts)
                {
                    ElementComponent elementComp = beast.GetComponent<ElementComponent>();
                    if (elementComp != null && 
                        (elementComp.Element == ElementType.Fire || elementComp.Element == ElementType.Wood))
                    {
                        deck.Add(beast);
                        break;
                    }
                }
            }
            
            // Fill any remaining slots with random cards
            if (deck.Count < deckSize)
            {
                List<Entity> randomDeck = LoadCardData.Instance.CreateRandomDeck(deckSize - deck.Count, false);
                deck.AddRange(randomDeck);
            }
            
            // Shuffle the final deck
            ShuffleList(deck);
        }
        
        /// <summary>
        /// Create a support-focused deck
        /// </summary>
        private static void CreateSupportFocusDeck(List<Entity> deck, int deckSize)
        {
            // Support decks focus on special cards and balanced elements
            
            // Add divine beasts and spirit animals (about 40% of deck)
            List<Entity> divineBeasts = LoadCardData.Instance.GetCardsByType(CardType.DivineBeast);
            List<Entity> spiritAnimals = LoadCardData.Instance.GetCardsByType(CardType.SpiritAnimal);
            List<Entity> jokers = LoadCardData.Instance.GetCardsByType(CardType.Joker);
            
            List<Entity> specialCards = new List<Entity>();
            if (divineBeasts != null) specialCards.AddRange(divineBeasts);
            if (spiritAnimals != null) specialCards.AddRange(spiritAnimals);
            if (jokers != null) specialCards.AddRange(jokers);
            
            ShuffleList(specialCards);
            
            int specialCount = Mathf.FloorToInt(deckSize * 0.4f);
            for (int i = 0; i < Mathf.Min(specialCount, specialCards.Count); i++)
            {
                deck.Add(specialCards[i]);
            }
            
            // Add a balanced selection of elemental cards
            int elementsCount = deckSize - deck.Count;
            int cardsPerElement = Mathf.Max(1, elementsCount / 5);
            
            for (int elementIndex = 1; elementIndex <= 5; elementIndex++) // Skip ElementType.None (0)
            {
                ElementType element = (ElementType)elementIndex;
                List<Entity> elementCards = LoadCardData.Instance.GetCardsByElement(element);
                
                if (elementCards != null && elementCards.Count > 0)
                {
                    ShuffleList(elementCards);
                    
                    for (int i = 0; i < Mathf.Min(cardsPerElement, elementCards.Count); i++)
                    {
                        if (deck.Count < deckSize)
                        {
                            deck.Add(elementCards[i]);
                        }
                    }
                }
            }
            
            // Fill any remaining slots with random cards
            if (deck.Count < deckSize)
            {
                List<Entity> randomDeck = LoadCardData.Instance.CreateRandomDeck(deckSize - deck.Count, false);
                deck.AddRange(randomDeck);
            }
            
            // Shuffle the final deck
            ShuffleList(deck);
        }
        
        /// <summary>
        /// Get the element that generates the given element
        /// </summary>
        private static ElementType GetGeneratingElement(ElementType element)
        {
            switch (element)
            {
                case ElementType.Metal:
                    return ElementType.Earth; // Earth generates Metal
                case ElementType.Wood:
                    return ElementType.Water; // Water generates Wood
                case ElementType.Water:
                    return ElementType.Metal; // Metal generates Water
                case ElementType.Fire:
                    return ElementType.Wood; // Wood generates Fire
                case ElementType.Earth:
                    return ElementType.Fire; // Fire generates Earth
                default:
                    return ElementType.None;
            }
        }
        
        /// <summary>
        /// Get a random element (excluding None)
        /// </summary>
        private static ElementType GetRandomElement()
        {
            int randomValue = Random.Range(1, 6); // 1 to 5 (skipping None which is 0)
            return (ElementType)randomValue;
        }
        
        /// <summary>
        /// Shuffle a list
        /// </summary>
        private static void ShuffleList<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T temp = list[k];
                list[k] = list[n];
                list[n] = temp;
            }
        }
        
        /// <summary>
        /// Helper class for element pairs
        /// </summary>
        private class ElementPair
        {
            public ElementType First { get; private set; }
            public ElementType Second { get; private set; }
            
            public ElementPair(ElementType first, ElementType second)
            {
                First = first;
                Second = second;
            }
        }
    }
    
    /// <summary>
    /// Enum for deck building strategies
    /// </summary>
    public enum DeckStrategy
    {
        Random,
        ElementalFocus,
        ComboOriented,
        DefensiveFocus,
        AggressiveFocus,
        SupportFocus
    }
}