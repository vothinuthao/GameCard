using System.Collections.Generic;
using System.Linq;
using Components;
using Core;
using Core.Utils;
using Data;
using Factories;
using RunTime.Managers;
using UnityEngine;

namespace RunTime
{
    /// <summary>
    /// Manager that handles loading and providing card data for the game
    /// Uses PureSingleton pattern
    /// </summary>
    public class LoadCardData : PureSingleton<LoadCardData>
    {
        // Dictionary to cache loaded cards by key name
        private Dictionary<string, Entity> _cardCache = new Dictionary<string, Entity>();
        
        // Dictionary to cache elemental cards by element type
        private Dictionary<ElementType, List<Entity>> _elementalCardCache = 
            new Dictionary<ElementType, List<Entity>>();
        
        // Dictionary to cache cards by card type
        private Dictionary<CardType, List<Entity>> _cardTypeCache = 
            new Dictionary<CardType, List<Entity>>();
        
        // Dictionary to cache cards by rarity
        private Dictionary<Rarity, List<Entity>> _rarityCache = 
            new Dictionary<Rarity, List<Entity>>();
        
        // References
        private EntityManager _entityManager;
        
        // Base folder for card resources
        private const string BASE_FOLDER = "Cards";
        
        // Flag to track if all cards have been loaded
        private bool _allCardsLoaded = false;
        
        /// <summary>
        /// Called when singleton is created
        /// </summary>
        protected override void OnCreated()
        {
            Debug.Log("[LoadCardData] Created, waiting for initialization");
            
            InitializeCaches();
        }
        
        /// <summary>
        /// Initialize with EntityManager reference
        /// Must be called before using any functionality
        /// </summary>
        public bool Initialize(EntityManager entityManager)
        {
            if (entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot initialize with null EntityManager!");
                return false;
            }
            
            _entityManager = entityManager;
            if (!CardDataCacheManager.HasInstance)
            {
                Debug.LogWarning("[LoadCardData] CardDataCache not found, creating a new instance");
                CardDataCacheManager.Instance.Initialize();
            }
            
            // Make sure ScriptableObjectFactory is initialized
            if (!ScriptableObjectFactory.HasInstance)
            {
                Debug.LogWarning("[LoadCardData] ScriptableObjectFactory not found, creating a new instance");
                ScriptableObjectFactory.Instance.Initialize(entityManager);
            }
            else if (!ScriptableObjectFactory.Instance.IsInitialized)
            {
                ScriptableObjectFactory.Instance.Initialize(entityManager);
            }
            
            Debug.Log("[LoadCardData] Initialized successfully");
            
            return base.Initialize();
        }
        
        /// <summary>
        /// Initialize the cache dictionaries
        /// </summary>
        private void InitializeCaches()
        {
            // Initialize elemental card cache for each element type
            foreach (ElementType element in System.Enum.GetValues(typeof(ElementType)))
            {
                _elementalCardCache[element] = new List<Entity>();
            }
            
            // Initialize card type cache for each card type
            foreach (CardType cardType in System.Enum.GetValues(typeof(CardType)))
            {
                _cardTypeCache[cardType] = new List<Entity>();
            }
            
            // Initialize rarity cache for each rarity
            foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
            {
                _rarityCache[rarity] = new List<Entity>();
            }
        }
        
        /// <summary>
        /// Load all cards from Resources folder
        /// </summary>
        public void LoadAllCards()
        {
            // Ensure initialized
            EnsureInitialized();
            
            if (_allCardsLoaded)
                return;
            
            Debug.Log("[LoadCardData] Loading all cards from Resources...");
            
            // Make sure data manager loads all card data
            CardDataCacheManager.Instance.LoadAllCardData();
            
            // Get all card data from CardDataCache
            foreach (ElementType element in System.Enum.GetValues(typeof(ElementType)))
            {
                if (element == ElementType.None)
                    continue;
                    
                List<CardDataSO> elementCards = CardDataCacheManager.Instance.GetCardDataByElement(element);
                if (elementCards != null)
                {
                    foreach (var cardData in elementCards)
                    {
                        LoadCard(cardData);
                    }
                }
            }
            
            _allCardsLoaded = true;
            Debug.Log($"[LoadCardData] Successfully loaded cards into cache. Element cards: {_elementalCardCache.Sum(kv => kv.Value.Count)}, Card types: {_cardTypeCache.Sum(kv => kv.Value.Count)}");
        }
        
        /// <summary>
        /// Load a single card into cache
        /// </summary>
        private Entity LoadCard(CardDataSO cardData)
        {
            // Ensure initialized
            EnsureInitialized();
            
            if (cardData == null)
                return null;
                
            // Skip if already in cache and has key name
            if (!string.IsNullOrEmpty(cardData.cardKeyName) && _cardCache.ContainsKey(cardData.cardKeyName))
                return _cardCache[cardData.cardKeyName];
            
            // Create entity from card data using the factory
            Entity cardEntity = ScriptableObjectFactory.Instance.CreateCardFromSO(cardData);
            
            if (cardEntity != null)
            {
                // Add to main cache
                CardInfoComponent cardInfo = cardEntity.GetComponent<CardInfoComponent>();
                if (cardInfo != null && !string.IsNullOrEmpty(cardInfo.KeyName))
                {
                    _cardCache[cardInfo.KeyName] = cardEntity;
                }
                
                // Add to element cache
                ElementComponent element = cardEntity.GetComponent<ElementComponent>();
                if (element != null)
                {
                    if (!_elementalCardCache.ContainsKey(element.Element))
                        _elementalCardCache[element.Element] = new List<Entity>();
                        
                    _elementalCardCache[element.Element].Add(cardEntity);
                }
                
                // Add to card type cache
                if (cardInfo != null)
                {
                    if (!_cardTypeCache.ContainsKey(cardInfo.Type))
                        _cardTypeCache[cardInfo.Type] = new List<Entity>();
                        
                    _cardTypeCache[cardInfo.Type].Add(cardEntity);
                }
                
                // Add to rarity cache
                if (cardInfo != null)
                {
                    if (!_rarityCache.ContainsKey(cardInfo.Rarity))
                        _rarityCache[cardInfo.Rarity] = new List<Entity>();
                        
                    _rarityCache[cardInfo.Rarity].Add(cardEntity);
                }
            }
            
            return cardEntity;
        }
        
        /// <summary>
        /// Get a card by key name
        /// </summary>
        public Entity GetCardByKeyName(string keyName)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Check if in cache first
            if (_cardCache.TryGetValue(keyName, out Entity cachedCard))
            {
                return CloneEntity(cachedCard);
            }
            
            // Try to load from Resources through CardDataCache
            CardDataSO cardData = CardDataCacheManager.Instance.GetCardDataByKeyName(keyName);
            if (cardData != null)
            {
                Entity card = LoadCard(cardData);
                return CloneEntity(card);
            }
            
            Debug.LogWarning($"[LoadCardData] Card with key '{keyName}' not found!");
            return null;
        }
        
        /// <summary>
        /// Get all cards of a specific element
        /// </summary>
        public List<Entity> GetCardsByElement(ElementType elementType)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            // Return a copy of the list to avoid modification
            return CloneEntityList(_elementalCardCache.TryGetValue(elementType, out var value) ? 
                value : new List<Entity>());
        }
        
        /// <summary>
        /// Get all cards of a specific type
        /// </summary>
        public List<Entity> GetCardsByType(CardType cardType)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            // Return a copy of the list to avoid modification
            return CloneEntityList(_cardTypeCache.ContainsKey(cardType) ? 
                _cardTypeCache[cardType] : new List<Entity>());
        }
        
        /// <summary>
        /// Get all cards of a specific rarity
        /// </summary>
        public List<Entity> GetCardsByRarity(Rarity rarity)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            // Return a copy of the list to avoid modification
            return CloneEntityList(_rarityCache.ContainsKey(rarity) ? 
                _rarityCache[rarity] : new List<Entity>());
        }
        
        /// <summary>
        /// Create a random deck with balanced elements
        /// </summary>
        public List<Entity> CreateRandomDeck(int deckSize = 10, bool balanced = true)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            List<Entity> deck = new List<Entity>();
            
            if (balanced)
            {
                // Create a balanced deck with cards from all elements
                CreateBalancedRandomDeck(deck, deckSize);
            }
            else
            {
                // Create a fully random deck
                CreateFullyRandomDeck(deck, deckSize);
            }
            
            return deck;
        }
        
        /// <summary>
        /// Create a random starter deck based on a theme
        /// </summary>
        public List<Entity> CreateRandomStarterDeck(string theme = "balanced", int deckSize = 8)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            List<Entity> deck = new List<Entity>();
            
            switch (theme.ToLower())
            {
                case "metal":
                    CreateElementFocusedDeck(deck, ElementType.Metal, deckSize);
                    break;
                case "wood":
                    CreateElementFocusedDeck(deck, ElementType.Wood, deckSize);
                    break;
                case "water":
                    CreateElementFocusedDeck(deck, ElementType.Water, deckSize);
                    break;
                case "fire":
                    CreateElementFocusedDeck(deck, ElementType.Fire, deckSize);
                    break;
                case "earth":
                    CreateElementFocusedDeck(deck, ElementType.Earth, deckSize);
                    break;
                case "balanced":
                default:
                    CreateBalancedRandomDeck(deck, deckSize);
                    break;
            }
            
            return deck;
        }
        
        /// <summary>
        /// Create a balanced random deck with cards from all elements
        /// </summary>
        private void CreateBalancedRandomDeck(List<Entity> deck, int deckSize)
        {
            // Calculate how many cards per element (at least 1)
            int cardsPerElement = Mathf.Max(1, deckSize / 5);
            
            // Add cards from each element
            foreach (ElementType element in System.Enum.GetValues(typeof(ElementType)))
            {
                if (element == ElementType.None)
                    continue;
                
                if (!_elementalCardCache.ContainsKey(element))
                    continue;
                    
                List<Entity> elementCards = _elementalCardCache[element];
                
                if (elementCards != null && elementCards.Count > 0)
                {
                    // Shuffle the element cards
                    List<Entity> shuffled = ShuffleList(elementCards);
                    
                    // Add cards up to the per-element limit
                    for (int i = 0; i < Mathf.Min(cardsPerElement, shuffled.Count); i++)
                    {
                        deck.Add(CloneEntity(shuffled[i]));
                    }
                }
            }
            
            // If we need more cards to reach the deck size
            if (deck.Count < deckSize)
            {
                // Get all elemental cards and shuffle them
                List<Entity> allCards = new List<Entity>();
                foreach (var pair in _elementalCardCache)
                {
                    if (pair.Key != ElementType.None && pair.Value != null)
                        allCards.AddRange(pair.Value);
                }
                
                List<Entity> shuffled = ShuffleList(allCards);
                
                // Add more cards until we reach the deck size
                int index = 0;
                while (deck.Count < deckSize && index < shuffled.Count)
                {
                    // Skip if this card is already in the deck
                    bool alreadyInDeck = false;
                    foreach (var existingCard in deck)
                    {
                        CardInfoComponent existingInfo = existingCard.GetComponent<CardInfoComponent>();
                        CardInfoComponent newInfo = shuffled[index].GetComponent<CardInfoComponent>();
                        
                        if (existingInfo != null && newInfo != null && existingInfo.KeyName == newInfo.KeyName)
                        {
                            alreadyInDeck = true;
                            break;
                        }
                    }
                    
                    if (!alreadyInDeck)
                    {
                        deck.Add(CloneEntity(shuffled[index]));
                    }
                    
                    index++;
                }
            }
            
            // Trim the deck if it's too large
            if (deck.Count > deckSize)
            {
                deck = deck.GetRange(0, deckSize);
            }
        }
        
        /// <summary>
        /// Create a fully random deck without balancing
        /// </summary>
        private void CreateFullyRandomDeck(List<Entity> deck, int deckSize)
        {
            // Get all elemental cards and shuffle them
            List<Entity> allCards = new List<Entity>();
            foreach (var pair in _elementalCardCache)
            {
                if (pair.Key != ElementType.None && pair.Value != null)
                    allCards.AddRange(pair.Value);
            }
            
            // Add special cards
            if (_cardTypeCache.ContainsKey(CardType.DivineBeast) && _cardTypeCache[CardType.DivineBeast] != null)
                allCards.AddRange(_cardTypeCache[CardType.DivineBeast]);
                
            if (_cardTypeCache.ContainsKey(CardType.SpiritAnimal) && _cardTypeCache[CardType.SpiritAnimal] != null)
                allCards.AddRange(_cardTypeCache[CardType.SpiritAnimal]);
            
            // Shuffle the cards
            List<Entity> shuffled = ShuffleList(allCards);
            
            // Add cards up to the deck size
            for (int i = 0; i < Mathf.Min(deckSize, shuffled.Count); i++)
            {
                deck.Add(CloneEntity(shuffled[i]));
            }
        }
        
        /// <summary>
        /// Create a deck focused on a specific element
        /// </summary>
        private void CreateElementFocusedDeck(List<Entity> deck, ElementType focusElement, int deckSize)
        {
            // Calculate card distribution
            int focusCards = Mathf.FloorToInt(deckSize * 0.6f); // 60% focus element
            int supportCards = deckSize - focusCards; // 40% other elements
            
            // Add focus element cards
            if (!_elementalCardCache.ContainsKey(focusElement) || _elementalCardCache[focusElement] == null)
            {
                Debug.LogWarning($"[LoadCardData] No cards found for element {focusElement}! Falling back to balanced deck.");
                CreateBalancedRandomDeck(deck, deckSize);
                return;
            }
            
            List<Entity> focusElementCards = ShuffleList(_elementalCardCache[focusElement]);
            for (int i = 0; i < Mathf.Min(focusCards, focusElementCards.Count); i++)
            {
                deck.Add(CloneEntity(focusElementCards[i]));
            }
            
            // Get support elements based on the Wu Xing cycle
            ElementType supportElement1 = GetGeneratingElement(focusElement); // Element that generates this one
            ElementType supportElement2 = GetGeneratedElement(focusElement); // Element generated by this one
            
            // Add support element cards
            List<Entity> supportElementCards = new List<Entity>();
            
            if (_elementalCardCache.ContainsKey(supportElement1) && _elementalCardCache[supportElement1] != null)
                supportElementCards.AddRange(_elementalCardCache[supportElement1]);
                
            if (_elementalCardCache.ContainsKey(supportElement2) && _elementalCardCache[supportElement2] != null)
                supportElementCards.AddRange(_elementalCardCache[supportElement2]);
            
            // Add some supporting special cards
            if (_cardTypeCache.ContainsKey(CardType.DivineBeast) && _cardTypeCache[CardType.DivineBeast] != null)
                supportElementCards.AddRange(_cardTypeCache[CardType.DivineBeast]);
                
            if (_cardTypeCache.ContainsKey(CardType.SpiritAnimal) && _cardTypeCache[CardType.SpiritAnimal] != null)
                supportElementCards.AddRange(_cardTypeCache[CardType.SpiritAnimal]);
            
            // Shuffle and add support cards
            List<Entity> shuffledSupport = ShuffleList(supportElementCards);
            for (int i = 0; i < Mathf.Min(supportCards, shuffledSupport.Count); i++)
            {
                deck.Add(CloneEntity(shuffledSupport[i]));
            }
            
            // Add more focus element cards if we still need cards
            if (deck.Count < deckSize && focusElementCards.Count > focusCards)
            {
                for (int i = focusCards; i < Mathf.Min(deckSize - deck.Count + focusCards, focusElementCards.Count); i++)
                {
                    deck.Add(CloneEntity(focusElementCards[i]));
                }
            }
        }
        
        /// <summary>
        /// Get the element that generates the given element
        /// </summary>
        private ElementType GetGeneratingElement(ElementType element)
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
        /// Get the element generated by the given element
        /// </summary>
        private ElementType GetGeneratedElement(ElementType element)
        {
            switch (element)
            {
                case ElementType.Metal:
                    return ElementType.Water; // Metal generates Water
                case ElementType.Wood:
                    return ElementType.Fire; // Wood generates Fire
                case ElementType.Water:
                    return ElementType.Wood; // Water generates Wood
                case ElementType.Fire:
                    return ElementType.Earth; // Fire generates Earth
                case ElementType.Earth:
                    return ElementType.Metal; // Earth generates Metal
                default:
                    return ElementType.None;
            }
        }
        
        /// <summary>
        /// Clear card caches
        /// </summary>
        public void ClearCaches()
        {
            _cardCache.Clear();
            
            foreach (var element in _elementalCardCache)
            {
                element.Value.Clear();
            }
            
            foreach (var type in _cardTypeCache)
            {
                type.Value.Clear();
            }
            
            foreach (var rarity in _rarityCache)
            {
                rarity.Value.Clear();
            }
            
            _allCardsLoaded = false;
            
            Debug.Log("[LoadCardData] All caches cleared");
        }
        
        /// <summary>
        /// Cleanup resources when done
        /// </summary>
        public override void Cleanup()
        {
            ClearCaches();
            base.Cleanup();
        }
        
        /// <summary>
        /// Shuffle a list of entities
        /// </summary>
        private List<Entity> ShuffleList(List<Entity> list)
        {
            if (list == null)
                return new List<Entity>();
                
            List<Entity> shuffled = new List<Entity>(list);
            
            // Fisher-Yates shuffle algorithm
            System.Random random = new System.Random();
            int n = shuffled.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                Entity temp = shuffled[k];
                shuffled[k] = shuffled[n];
                shuffled[n] = temp;
            }
            
            return shuffled;
        }
        
        /// <summary>
        /// Clone an entity to avoid modifying the cached version
        /// </summary>
        private Entity CloneEntity(Entity original)
        {
            if (original == null)
                return null;
                
            // Use the factory to clone this entity
            if (original.GetComponent<CardInfoComponent>() != null)
            {
                string keyName = original.GetComponent<CardInfoComponent>().KeyName;
                if (!string.IsNullOrEmpty(keyName))
                {
                    CardDataSO cardData = CardDataCacheManager.Instance.GetCardDataByKeyName(keyName);
                    if (cardData != null)
                    {
                        return ScriptableObjectFactory.Instance.CreateCardFromSO(cardData);
                    }
                }
            }
            
            // Fallback to returning the original for now
            // In a real implementation, you'd want a proper deep clone
            return original;
        }
        
        /// <summary>
        /// Clone a list of entities
        /// </summary>
        private List<Entity> CloneEntityList(List<Entity> originalList)
        {
            if (originalList == null)
                return new List<Entity>();
            
            List<Entity> clonedList = new List<Entity>();
            foreach (var entity in originalList)
            {
                clonedList.Add(CloneEntity(entity));
            }
            
            return clonedList;
        }
    }
}