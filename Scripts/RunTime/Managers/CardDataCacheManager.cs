using System.Collections.Generic;
using Core.Utils;
using Data;
using UnityEngine;

namespace RunTime.Managers
{
    /// <summary>
    /// Helper class to cache and manage ScriptableObject card data
    /// Uses PureSingleton pattern
    /// </summary>
    public class CardDataCacheManager : PureSingleton<CardDataCacheManager>
    {
        // Dictionary to cache card data by key name
        private Dictionary<string, CardDataSO> _cardDataCache = new Dictionary<string, CardDataSO>();
        
        // Dictionary to cache card data by ID
        private Dictionary<int, CardDataSO> _cardDataByIdCache = new Dictionary<int, CardDataSO>();
        
        // Dictionaries to cache card data by type
        private Dictionary<ElementType, List<CardDataSO>> _elementalCardDataCache = 
            new Dictionary<ElementType, List<CardDataSO>>();
        
        private Dictionary<CardType, List<CardDataSO>> _cardTypeDataCache = 
            new Dictionary<CardType, List<CardDataSO>>();
        
        private Dictionary<Rarity, List<CardDataSO>> _rarityDataCache = 
            new Dictionary<Rarity, List<CardDataSO>>();
        
        // Flag to track if all cards have been loaded
        private bool _allCardsLoaded = false;
        
        // Base folder for card resources
        private const string BASE_FOLDER = "Cards";
        
        /// <summary>
        /// Initialize the cache manager
        /// </summary>
        public override bool Initialize()
        {
            // Initialize dictionaries
            InitializeCaches();
            
            Debug.Log("[CardDataCache] Initialized successfully");
            return base.Initialize();
        }
        
        /// <summary>
        /// Initialize when singleton is created
        /// </summary>
        protected override void OnCreated()
        {
            // Initialize dictionaries
            InitializeCaches();
        }
        
        /// <summary>
        /// Initialize the cache dictionaries
        /// </summary>
        private void InitializeCaches()
        {
            // Initialize elemental card cache for each element type
            foreach (ElementType element in System.Enum.GetValues(typeof(ElementType)))
            {
                _elementalCardDataCache[element] = new List<CardDataSO>();
            }
            
            // Initialize card type cache for each card type
            foreach (CardType cardType in System.Enum.GetValues(typeof(CardType)))
            {
                _cardTypeDataCache[cardType] = new List<CardDataSO>();
            }
            
            // Initialize rarity cache for each rarity
            foreach (Rarity rarity in System.Enum.GetValues(typeof(Rarity)))
            {
                _rarityDataCache[rarity] = new List<CardDataSO>();
            }
        }
        
        /// <summary>
        /// Load all cards from Resources folder
        /// </summary>
        public void LoadAllCardData()
        {
            // Ensure initialized
            EnsureInitialized();
            
            if (_allCardsLoaded)
                return;
            
            Debug.Log("[CardDataCache] Loading all card data from Resources...");
            
            // Load all cards from Resources
            CardDataSO[] allCardDataSOs = Resources.LoadAll<CardDataSO>(BASE_FOLDER);
            
            if (allCardDataSOs == null || allCardDataSOs.Length == 0)
            {
                Debug.LogWarning("[CardDataCache] No card data found in Resources folder!");
                return;
            }
            
            int loadedCount = 0;
            
            // Add each card data to the appropriate caches
            foreach (var cardData in allCardDataSOs)
            {
                // Skip if already in cache
                if (!string.IsNullOrEmpty(cardData.cardKeyName) && _cardDataCache.ContainsKey(cardData.cardKeyName))
                    continue;
                
                // Add to main caches
                if (!string.IsNullOrEmpty(cardData.cardKeyName))
                {
                    _cardDataCache[cardData.cardKeyName] = cardData;
                }
                
                if (cardData.cardId > 0)
                {
                    _cardDataByIdCache[cardData.cardId] = cardData;
                }
                
                // Add to element cache
                // _elementalCardDataCache[cardData.elementType].Add(cardData);
                
                // Add to card type cache
                _cardTypeDataCache[cardData.cardType].Add(cardData);
                
                // Add to rarity cache
                _rarityDataCache[cardData.rarity].Add(cardData);
                
                loadedCount++;
            }
            
            _allCardsLoaded = true;
            Debug.Log($"[CardDataCache] Successfully loaded {loadedCount} card data ScriptableObjects from Resources");
        }
        
        /// <summary>
        /// Get card data by key name
        /// </summary>
        public CardDataSO GetCardDataByKeyName(string keyName)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCardData();
            
            // Check if in cache
            if (_cardDataCache.TryGetValue(keyName, out CardDataSO cardData))
            {
                return cardData;
            }
            
            // Try to load directly from Resources
            string path = $"{BASE_FOLDER}/{keyName}";
            CardDataSO loadedData = Resources.Load<CardDataSO>(path);
            
            if (loadedData != null && !string.IsNullOrEmpty(loadedData.cardKeyName))
            {
                _cardDataCache[loadedData.cardKeyName] = loadedData;
                
                if (loadedData.cardId > 0)
                {
                    _cardDataByIdCache[loadedData.cardId] = loadedData;
                }
                
                // Add to element cache
                // _elementalCardDataCache[loadedData.elementType].Add(loadedData);
                
                // Add to card type cache
                _cardTypeDataCache[loadedData.cardType].Add(loadedData);
                
                // Add to rarity cache
                _rarityDataCache[loadedData.rarity].Add(loadedData);
            }
            
            return loadedData;
        }
        
        /// <summary>
        /// Get card data by ID
        /// </summary>
        public CardDataSO GetCardDataById(int id)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCardData();
            
            // Check if in cache
            if (_cardDataByIdCache.TryGetValue(id, out CardDataSO cardData))
            {
                return cardData;
            }
            
            return null;
        }
        
        /// <summary>
        /// Get all card data of a specific element
        /// </summary>
        public List<CardDataSO> GetCardDataByElement(ElementType elementType)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCardData();
            
            // Return a copy of the list to avoid modification
            return new List<CardDataSO>(_elementalCardDataCache[elementType]);
        }
        
        /// <summary>
        /// Get all card data of a specific type
        /// </summary>
        public List<CardDataSO> GetCardDataByType(CardType cardType)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCardData();
            
            // Return a copy of the list to avoid modification
            return new List<CardDataSO>(_cardTypeDataCache[cardType]);
        }
        
        /// <summary>
        /// Get all card data of a specific rarity
        /// </summary>
        public List<CardDataSO> GetCardDataByRarity(Rarity rarity)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCardData();
            
            // Return a copy of the list to avoid modification
            return new List<CardDataSO>(_rarityDataCache[rarity]);
        }
        
        /// <summary>
        /// Get a list of random card data
        /// </summary>
        public List<CardDataSO> GetRandomCardData(int count, ElementType? elementType = null, 
                                                CardType? cardType = null, Rarity? rarity = null)
        {
            // Ensure initialized
            EnsureInitialized();
            
            // Make sure all cards are loaded
            if (!_allCardsLoaded)
                LoadAllCardData();
            
            List<CardDataSO> result = new List<CardDataSO>();
            List<CardDataSO> availableCards = new List<CardDataSO>();
            
            // Filter cards based on parameters
            if (elementType.HasValue)
            {
                availableCards.AddRange(_elementalCardDataCache[elementType.Value]);
            }
            else if (cardType.HasValue)
            {
                availableCards.AddRange(_cardTypeDataCache[cardType.Value]);
            }
            else if (rarity.HasValue)
            {
                availableCards.AddRange(_rarityDataCache[rarity.Value]);
            }
            else
            {
                // Use all cards
                foreach (var pair in _cardDataCache)
                {
                    availableCards.Add(pair.Value);
                }
            }
            
            // Shuffle the available cards
            ShuffleList(availableCards);
            
            // Add cards up to the requested count
            for (int i = 0; i < Mathf.Min(count, availableCards.Count); i++)
            {
                result.Add(availableCards[i]);
            }
            
            return result;
        }
        
        /// <summary>
        /// Clear all caches
        /// </summary>
        public void ClearAllCaches()
        {
            _cardDataCache.Clear();
            _cardDataByIdCache.Clear();
            
            foreach (var element in _elementalCardDataCache)
            {
                element.Value.Clear();
            }
            
            foreach (var type in _cardTypeDataCache)
            {
                type.Value.Clear();
            }
            
            foreach (var rarity in _rarityDataCache)
            {
                rarity.Value.Clear();
            }
            
            _allCardsLoaded = false;
            
            Debug.Log("[CardDataCache] All caches cleared");
        }
        
        /// <summary>
        /// Cleanup resources when done
        /// </summary>
        public override void Cleanup()
        {
            ClearAllCaches();
            base.Cleanup();
        }
        
        /// <summary>
        /// Shuffle a list
        /// </summary>
        private void ShuffleList<T>(List<T> list)
        {
            System.Random random = new System.Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                (list[k], list[n]) = (list[n], list[k]);
            }
        }
    }
}