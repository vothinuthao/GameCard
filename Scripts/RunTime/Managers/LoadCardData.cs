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
            
            // Initialize CardDataCacheManager if needed
            if (!CardDataCacheManager.HasInstance)
            {
                Debug.Log("[LoadCardData] CardDataCache not found, creating a new instance");
                CardDataCacheManager.Instance.Initialize();
            }
            else if (!CardDataCacheManager.Instance.IsInitialized)
            {
                Debug.Log("[LoadCardData] Initializing existing CardDataCache");
                CardDataCacheManager.Instance.Initialize();
            }
            
            // Initialize ScriptableObjectFactory if needed
            if (!ScriptableObjectFactory.HasInstance)
            {
                Debug.Log("[LoadCardData] ScriptableObjectFactory not found, creating a new instance");
                ScriptableObjectFactory.Instance.Initialize(entityManager);
            }
            else if (!ScriptableObjectFactory.Instance.IsInitialized)
            {
                Debug.Log("[LoadCardData] Initializing existing ScriptableObjectFactory");
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
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot load cards - not properly initialized with EntityManager!");
                return;
            }
            
            if (_allCardsLoaded)
            {
                Debug.Log("[LoadCardData] Cards already loaded, skipping...");
                return;
            }
            
            Debug.Log("[LoadCardData] Loading all cards from Resources...");
            
            try
            {
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
                            if (cardData != null)
                            {
                                LoadCard(cardData);
                            }
                        }
                    }
                }
                
                _allCardsLoaded = true;
                
                int totalCards = _elementalCardCache.Sum(kv => kv.Value.Count);
                Debug.Log($"[LoadCardData] Successfully loaded {totalCards} cards into cache.");
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LoadCardData] Error loading cards: {e.Message}\n{e.StackTrace}");
                _allCardsLoaded = false;
            }
        }
        
        /// <summary>
        /// Load a single card into cache
        /// </summary>
        private Entity LoadCard(CardDataSO cardData)
        {
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot load card - not properly initialized with EntityManager!");
                return null;
            }
            
            if (cardData == null)
                return null;
                
            // Skip if already in cache and has key name
            if (!string.IsNullOrEmpty(cardData.cardKeyName) && _cardCache.ContainsKey(cardData.cardKeyName))
                return _cardCache[cardData.cardKeyName];
            
            // Ensure ScriptableObjectFactory is initialized
            if (!ScriptableObjectFactory.Instance.IsInitialized)
            {
                ScriptableObjectFactory.Instance.Initialize(_entityManager);
            }
            
            // Create entity from card data using the factory
            Entity cardEntity = null;
            
            try
            {
                cardEntity = ScriptableObjectFactory.Instance.CreateCardFromSO(cardData);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LoadCardData] Error creating entity from card data: {e.Message}");
                return null;
            }
            
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
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot get card - not properly initialized with EntityManager!");
                return null;
            }
            
            // Make sure cards are loaded
            if (!_allCardsLoaded)
            {
                LoadAllCards();
            }
            
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
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot get cards - not properly initialized with EntityManager!");
                return new List<Entity>();
            }
            
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
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot get cards - not properly initialized with EntityManager!");
                return new List<Entity>();
            }
            
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
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot get cards - not properly initialized with EntityManager!");
                return new List<Entity>();
            }
            
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
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot create deck - not properly initialized with EntityManager!");
                return new List<Entity>();
            }
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            List<Entity> deck = new List<Entity>();
            
            try 
            {
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
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LoadCardData] Error creating random deck: {e.Message}");
                return new List<Entity>();
            }
            
            return deck;
        }
        
        /// <summary>
        /// Create a random starter deck based on a theme
        /// </summary>
        public List<Entity> CreateRandomStarterDeck(string theme = "balanced", int deckSize = 8)
        {
            // Ensure initialized with proper entity manager
            if (!IsInitialized || _entityManager == null)
            {
                Debug.LogError("[LoadCardData] Cannot create deck - not properly initialized with EntityManager!");
                return new List<Entity>();
            }
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCards();
            
            List<Entity> deck = new List<Entity>();
            
            try
            {
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
            }
            catch (System.Exception e)
            {
                Debug.LogError($"[LoadCardData] Error creating {theme} starter deck: {e.Message}");
                
                // Fallback to creating any cards possible
                var allElementalCards = new List<Entity>();
                foreach (var element in _elementalCardCache.Values)
                {
                    allElementalCards.AddRange(element);
                }
                
                if (allElementalCards.Count > 0)
                {
                    int count = Mathf.Min(deckSize, allElementalCards.Count);
                    for (int i = 0; i < count; i++)
                    {
                        deck.Add(CloneEntity(allElementalCards[i % allElementalCards.Count]));
                    }
                }
            }
            
            Debug.Log($"[LoadCardData] Created {theme} starter deck with {deck.Count} cards");
            return deck;
        }
        
        /// <summary>
        /// Create a balanced random deck with cards from all elements
        /// Only uses ElementalCard type cards (not any other card types)
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
                
                if (!_elementalCardCache.ContainsKey(element) || _elementalCardCache[element].Count == 0)
                    continue;
                    
                // Get all elemental cards for this element
                List<Entity> elementCards = new List<Entity>();
                
                // Filter to only include ElementalCard type cards
                foreach (var card in _elementalCardCache[element])
                {
                    if (card != null)
                    {
                        CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                        if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                        {
                            elementCards.Add(card);
                        }
                    }
                }
                
                if (elementCards.Count > 0)
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
                List<Entity> allElementalCards = new List<Entity>();
                
                // Collect all ElementalCard type cards from all elements
                foreach (var pair in _elementalCardCache)
                {
                    if (pair.Key != ElementType.None && pair.Value != null && pair.Value.Count > 0)
                    {
                        foreach (var card in pair.Value)
                        {
                            if (card != null)
                            {
                                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                                if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                                {
                                    allElementalCards.Add(card);
                                }
                            }
                        }
                    }
                }
                
                if (allElementalCards.Count > 0)
                {
                    List<Entity> shuffled = ShuffleList(allElementalCards);
                    
                    // Add more cards until we reach the deck size
                    int index = 0;
                    while (deck.Count < deckSize && index < shuffled.Count)
                    {
                        // Skip if this card is already in the deck
                        bool alreadyInDeck = false;
                        CardInfoComponent newInfo = shuffled[index].GetComponent<CardInfoComponent>();
                        
                        if (newInfo != null && !string.IsNullOrEmpty(newInfo.KeyName))
                        {
                            foreach (var existingCard in deck)
                            {
                                CardInfoComponent existingInfo = existingCard.GetComponent<CardInfoComponent>();
                                
                                if (existingInfo != null && existingInfo.KeyName == newInfo.KeyName)
                                {
                                    alreadyInDeck = true;
                                    break;
                                }
                            }
                        }
                        
                        if (!alreadyInDeck)
                        {
                            deck.Add(CloneEntity(shuffled[index]));
                        }
                        
                        index++;
                    }
                }
            }
            
            // Trim the deck if it's too large
            if (deck.Count > deckSize)
            {
                deck = deck.GetRange(0, deckSize);
            }
            
            Debug.Log($"[LoadCardData] Created balanced deck with {deck.Count} ElementalCard cards");
        }
        
        /// <summary>
        /// Create a fully random deck without balancing
        /// Only uses ElementalCard type cards
        /// </summary>
        private void CreateFullyRandomDeck(List<Entity> deck, int deckSize)
        {
            // Get all elemental cards and filter to only ElementalCard type
            List<Entity> allElementalCards = new List<Entity>();
    
            foreach (var pair in _elementalCardCache)
            {
                if (pair.Key != ElementType.None && pair.Value != null && pair.Value.Count > 0)
                {
                    foreach (var card in pair.Value)
                    {
                        if (card != null)
                        {
                            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                            if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                            {
                                allElementalCards.Add(card);
                            }
                        }
                    }
                }
            }
    
            if (allElementalCards.Count > 0)
            {
                // Shuffle the cards
                List<Entity> shuffled = ShuffleList(allElementalCards);
        
                // Add cards up to the deck size
                for (int i = 0; i < Mathf.Min(deckSize, shuffled.Count); i++)
                {
                    deck.Add(CloneEntity(shuffled[i]));
                }
        
                Debug.Log($"[LoadCardData] Created fully random deck with {deck.Count} ElementalCard cards");
            }
            else
            {
                Debug.LogError("[LoadCardData] No ElementalCard cards available to create a deck!");
            }
        }
        
        /// <summary>
        /// Create a deck focused on a specific element
        /// Only uses ElementalCard type cards
        /// </summary>
        private void CreateElementFocusedDeck(List<Entity> deck, ElementType focusElement, int deckSize)
        {
            // Filter focus element cards to only include ElementalCard type
            List<Entity> focusElementCards = new List<Entity>();
            
            if (_elementalCardCache.ContainsKey(focusElement) && 
                _elementalCardCache[focusElement] != null && 
                _elementalCardCache[focusElement].Count > 0)
            {
                foreach (var card in _elementalCardCache[focusElement])
                {
                    if (card != null)
                    {
                        CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                        if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                        {
                            focusElementCards.Add(card);
                        }
                    }
                }
            }
            if (focusElementCards.Count == 0)
            {
                Debug.LogWarning($"[LoadCardData] No ElementalCard cards found for element {focusElement}! Falling back to balanced deck.");
                CreateBalancedRandomDeck(deck, deckSize);
                return;
            }
            
            // Calculate card distribution
            int focusCards = Mathf.FloorToInt(deckSize * 0.6f); // 60% focus element
            int supportCards = deckSize - focusCards; // 40% other elements
            
            // Add focus element cards
            List<Entity> shuffledFocusCards = ShuffleList(focusElementCards);
            for (int i = 0; i < Mathf.Min(focusCards, shuffledFocusCards.Count); i++)
            {
                deck.Add(CloneEntity(shuffledFocusCards[i]));
            }
            
            // Get support elements based on the Wu Xing cycle
            ElementType supportElement1 = GetGeneratingElement(focusElement); // Element that generates this one
            ElementType supportElement2 = GetGeneratedElement(focusElement); // Element generated by this one
            
            // Add support element cards (ElementalCard type only)
            List<Entity> supportElementCards = new List<Entity>();
            
            // Process first support element
            if (_elementalCardCache.ContainsKey(supportElement1) && 
                _elementalCardCache[supportElement1] != null && 
                _elementalCardCache[supportElement1].Count > 0)
            {
                foreach (var card in _elementalCardCache[supportElement1])
                {
                    if (card != null)
                    {
                        CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                        if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                        {
                            supportElementCards.Add(card);
                        }
                    }
                }
            }
            
            // Process second support element
            if (_elementalCardCache.ContainsKey(supportElement2) && 
                _elementalCardCache[supportElement2] != null && 
                _elementalCardCache[supportElement2].Count > 0)
            {
                foreach (var card in _elementalCardCache[supportElement2])
                {
                    if (card != null)
                    {
                        CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                        if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                        {
                            supportElementCards.Add(card);
                        }
                    }
                }
            }
            
            if (supportElementCards.Count > 0)
            {
                // Shuffle and add support cards
                List<Entity> shuffledSupport = ShuffleList(supportElementCards);
                for (int i = 0; i < Mathf.Min(supportCards, shuffledSupport.Count); i++)
                {
                    deck.Add(CloneEntity(shuffledSupport[i]));
                }
            }
            
            // Add more focus element cards if we still need cards
            if (deck.Count < deckSize && shuffledFocusCards.Count > focusCards)
            {
                for (int i = focusCards; i < Mathf.Min(deckSize - deck.Count + focusCards, shuffledFocusCards.Count); i++)
                {
                    deck.Add(CloneEntity(shuffledFocusCards[i]));
                }
            }
            
            // Fallback for any remaining slots
            if (deck.Count < deckSize)
            {
                List<Entity> allElementalCards = new List<Entity>();
                
                // Collect all ElementalCard type cards from all elements
                foreach (var element in _elementalCardCache.Values)
                {
                    if (element != null && element.Count > 0)
                    {
                        foreach (var card in element)
                        {
                            if (card != null)
                            {
                                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                                if (cardInfo != null && cardInfo.Type == CardType.ElementalCard)
                                {
                                    allElementalCards.Add(card);
                                }
                            }
                        }
                    }
                }
                
                if (allElementalCards.Count > 0)
                {
                    List<Entity> shuffled = ShuffleList(allElementalCards);
                    
                    while (deck.Count < deckSize && shuffled.Count > 0)
                    {
                        deck.Add(CloneEntity(shuffled[deck.Count % shuffled.Count]));
                    }
                }
            }
            
            Debug.Log($"[LoadCardData] Created {focusElement} focused deck with {deck.Count} ElementalCard cards");
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
                Entity clone = CloneEntity(entity);
                if (clone != null)
                {
                    clonedList.Add(clone);
                }
            }
            
            return clonedList;
        }
    }
}