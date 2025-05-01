using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Factories;
using Systems;
using UnityEngine;

namespace RunTime
{
    /// <summary>
    /// Main game manager that ties all the systems together and handles game flow
    /// Using Singleton pattern
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        // ECS components
        private EntityManager _entityManager;
        private SystemManager _systemManager;
        
        // Game systems
        private CardSystem _cardSystem;
        private ElementInteractionSystem _elementInteractionSystem;
        private BattleSystem _battleSystem;
        private SupportCardSystem _supportCardSystem;
        
        // Factories 
        private CardFactory _cardFactory;
        private ScriptableObjectFactory _scriptableObjectFactory;
        
        // Game state
        private Entity _playerEntity;
        private Entity _enemyEntity;
        private bool _isBattleActive = false;
        
        // Current season and environment
        private Season _currentSeason = Season.Spring;
        
        // Player stats
        private int _playerHealth = 50;
        private int _maxPlayerHealth = 50;
        private int _playerGold = 0;
        
        [Header("References")]
        [SerializeField] private GameObject battleScreen;
        
        /// <summary>
        /// Initialize the manager on Awake
        /// </summary>
        protected override void OnInitialize()
        {
            InitializeGame();
        }
        
        /// <summary>
        /// Initialize the game systems
        /// </summary>
        private void InitializeGame()
        {
            Debug.Log("Initializing game systems...");
            
            // Create ECS components
            _entityManager = new EntityManager();
            _systemManager = new SystemManager();
            
            // Create game systems
            _cardSystem = new CardSystem(_entityManager);
            _elementInteractionSystem = new ElementInteractionSystem(_entityManager);
            _supportCardSystem = new SupportCardSystem(_entityManager);
            _battleSystem = new BattleSystem(_entityManager, _elementInteractionSystem, _cardSystem);
            
            // Add systems to system manager
            _systemManager.AddSystem(_cardSystem);
            _systemManager.AddSystem(_elementInteractionSystem);
            _systemManager.AddSystem(_supportCardSystem);
            _systemManager.AddSystem(_battleSystem);
            
            // Create factories
            _cardFactory = new CardFactory(_entityManager);
            // _scriptableObjectFactory = new ScriptableObjectFactory(_entityManager);
            
            // Set initial season
            _elementInteractionSystem.SetSeason(_currentSeason);
            
            Debug.Log("Game systems initialized successfully!");
        }
        
        // Update is called once per frame
        private void Update()
        {
            if (_isBattleActive)
            {
                // Update all systems
                _systemManager.UpdateAllSystems(Time.deltaTime);
                
                // Check if battle is over
                if (_battleSystem.IsBattleOver())
                {
                    EndBattle();
                }
            }
        }
        
        public void StartNewGame()
        {
            Debug.Log("Starting new game...");
    
            LoadCardData.Instance.LoadAllCards();
    
            // Reset player stats
            _playerHealth = _maxPlayerHealth;
            _playerGold = 0;
    
            // Create player entity
            _playerEntity = CreatePlayerEntity();
            List<Entity> deck = CreateSampleDeck();
    
            foreach (var card in deck)
            {
                _cardSystem.AddCardToDeck(card);
            }
    
            // Shuffle the deck
            _cardSystem.ShuffleDeck();
    
            Debug.Log("New game started successfully!");
            StartBattle("medium");
        }
        
         /// <summary>
        /// Create a random deck for the player
        /// Modified to use LoadCardData for randomized deck generation each playthrough
        /// </summary>
        private List<Entity> CreateSampleDeck()
        {
            Debug.Log("Creating random deck for new game...");
            
            // Get player preference for deck theme if available (default to "balanced")
            string deckTheme = "balanced";
            if (PlayerPrefs.HasKey("DeckTheme"))
            {
                deckTheme = PlayerPrefs.GetString("DeckTheme");
            }
            
            // Get player preference for deck size if available (default to 8)
            int deckSize = 8;
            if (PlayerPrefs.HasKey("DeckSize"))
            {
                deckSize = PlayerPrefs.GetInt("DeckSize");
            }
            
            List<Entity> deck = this.CreateRandomStarterDeck(deckTheme, deckSize);
            if (deck == null || deck.Count == 0)
            {
                Debug.LogWarning("Failed to create deck using LoadCardData, falling back to manual creation");
                deck = new List<Entity>();
                bool loadedFromResources = false;
                
                try
                {
                    string[] cardPaths = new string[]
                    {
                        "Cards/Metal_SwordQi_Card",
                        "Cards/Metal_Hardness_Card",
                        "Cards/Wood_Toxin_Card",
                        "Cards/Wood_Regeneration_Card",
                        "Cards/Water_Ice_Card",
                        "Cards/Fire_Burning_Card",
                        "Cards/Earth_Solidity_Card",
                        "Cards/Special_Rat_Card"
                    };
                    
                    foreach (var path in cardPaths)
                    {
                        var cardData = Resources.Load<Data.CardDataSO>(path);
                        if (cardData)
                        {
                            Entity card = _scriptableObjectFactory.CreateCardFromSO(cardData);
                            deck.Add(card);
                            loadedFromResources = true;
                        }
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogWarning($"Error loading cards from resources: {e.Message}");
                    loadedFromResources = false;
                }
                
                // If couldn't load from resources, create cards manually
                if (!loadedFromResources || deck.Count == 0)
                {
                    Debug.Log("Creating cards manually...");
                    deck = _cardFactory.CreateSampleDeck();
                }
            }
            
            Debug.Log($"Created deck with {deck.Count} cards for new game");
            return deck;
        }
        
        /// <summary>
        /// Start a battle with an enemy
        /// </summary>
        public void StartBattle(string enemyType)
        {
            if (_playerEntity == null)
            {
                Debug.LogError("Cannot start battle - player entity not created!");
                return;
            }
            
            Debug.Log($"Starting battle with enemy: {enemyType}");
            
            // Create enemy entity
            _enemyEntity = CreateEnemyEntity(enemyType);
            
            // Initialize battle
            _battleSystem.InitializeBattle(_playerEntity, _enemyEntity);
            _isBattleActive = true;
            
            // Show battle screen
            if (battleScreen != null)
            {
                battleScreen.SetActive(true);
            }
            
            Debug.Log("Battle started successfully!");
        }
        
        /// <summary>
        /// End the current battle
        /// </summary>
        private void EndBattle()
        {
            _isBattleActive = false;
            
            // Get winner
            Entity winner = _battleSystem.GetWinner();
            
            if (winner == _playerEntity)
            {
                Debug.Log("Player won the battle!");
                
                // Add rewards
                _playerGold += 25;
                
                // Heal player a bit
                StatsComponent playerStats = _playerEntity.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    playerStats.Health = Mathf.Min(playerStats.Health + 5, playerStats.MaxHealth);
                }
            }
            else
            {
                Debug.Log("Player lost the battle!");
                
                // Game over logic
                // Automatically restart for simplicity
                Invoke("StartNewGame", 2f);
            }
        }
        
        /// <summary>
        /// Create the player entity
        /// </summary>
        private Entity CreatePlayerEntity()
        {
            Entity player = _entityManager.CreateEntity();
            
            // Add stats component
            StatsComponent stats = new StatsComponent
            {
                Attack = 5,
                Defense = 5,
                Health = _playerHealth,
                MaxHealth = _maxPlayerHealth,
                Speed = 5
            };
            player.AddComponent(stats);
            
            return player;
        }
        
        /// <summary>
        /// Create an enemy entity
        /// </summary>
        private Entity CreateEnemyEntity(string enemyType)
        {
            Entity enemy = _entityManager.CreateEntity();
            
            // Add stats component
            StatsComponent stats = new StatsComponent();
            
            // Set stats based on enemy type
            switch (enemyType.ToLower())
            {
                case "weak":
                    stats.Attack = 3;
                    stats.Defense = 2;
                    stats.Health = 15;
                    stats.MaxHealth = 15;
                    stats.Speed = 3;
                    break;
                case "medium":
                    stats.Attack = 5;
                    stats.Defense = 4;
                    stats.Health = 25;
                    stats.MaxHealth = 25;
                    stats.Speed = 4;
                    break;
                case "strong":
                    stats.Attack = 8;
                    stats.Defense = 6;
                    stats.Health = 40;
                    stats.MaxHealth = 40;
                    stats.Speed = 5;
                    break;
                case "elite":
                    stats.Attack = 10;
                    stats.Defense = 8;
                    stats.Health = 60;
                    stats.MaxHealth = 60;
                    stats.Speed = 6;
                    break;
                case "boss":
                    stats.Attack = 12;
                    stats.Defense = 10;
                    stats.Health = 100;
                    stats.MaxHealth = 100;
                    stats.Speed = 7;
                    break;
                default:
                    stats.Attack = 4;
                    stats.Defense = 3;
                    stats.Health = 20;
                    stats.MaxHealth = 20;
                    stats.Speed = 4;
                    break;
            }
            
            enemy.AddComponent(stats);
            
            // Add element component (random element)
            ElementComponent element = new ElementComponent
            {
                Element = (ElementType)Random.Range(0, 5) // 0-4 = all elements
            };
            enemy.AddComponent(element);
            
            // Add card info component for display
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = $"{enemyType.ToUpper()} Enemy",
                Description = "An enemy that wants to defeat you.",
                Type = CardType.Monster,
                Rarity = Rarity.Common
            };
            enemy.AddComponent(cardInfo);
            
            return enemy;
        }
        
        /// <summary>
        /// Play a card from the player's hand
        /// </summary>
        public void PlayCard(int cardIndex)
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Cannot play card - no active battle!");
                return;
            }
            
            List<Entity> hand = _cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Invalid card index: {cardIndex}");
                return;
            }
            
            Entity card = hand[cardIndex];
            _battleSystem.PlayCard(card, _enemyEntity);
            
            Debug.Log($"Played card at index {cardIndex}");
        }
        
        /// <summary>
        /// Play a card as a support card
        /// </summary>
        public void PlayAsSupport(int cardIndex)
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Cannot play support card - no active battle!");
                return;
            }
            
            List<Entity> hand = _cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Invalid card index: {cardIndex}");
                return;
            }
            
            // Check if the card can be played as a support
            Entity card = hand[cardIndex];
            if (card.HasComponent<SupportCardComponent>())
            {
                _cardSystem.PlayAsSupport(card);
                Debug.Log($"Played card at index {cardIndex} as support");
            }
            else
            {
                Debug.LogWarning("This card cannot be played as a support card!");
            }
        }
        
        /// <summary>
        /// End the player's turn
        /// </summary>
        public void EndTurn()
        {
            if (!_isBattleActive)
            {
                Debug.LogWarning("Cannot end turn - no active battle!");
                return;
            }
            
            _battleSystem.StartNewTurn();
            Debug.Log("Player turn ended");
            
            // Simple enemy AI
            PerformEnemyTurn();
            
            // Start new player turn
            _battleSystem.StartNewTurn();
        }
        
        /// <summary>
        /// Perform enemy turn
        /// </summary>
        private void PerformEnemyTurn()
        {
            if (_playerEntity == null || _enemyEntity == null)
                return;
            
            StatsComponent playerStats = _playerEntity.GetComponent<StatsComponent>();
            StatsComponent enemyStats = _enemyEntity.GetComponent<StatsComponent>();
            
            if (playerStats == null || enemyStats == null)
                return;
            
            // Simple AI: 70% chance to attack, 30% chance to defend
            if (Random.value < 0.7f)
            {
                // Attack
                float damage = enemyStats.Attack;
                
                // Apply defense
                float damageReduction = playerStats.Defense / 100f;
                damage = Mathf.Max(0, damage * (1 - damageReduction));
                
                // Apply damage
                playerStats.Health -= (int)damage;
                if (playerStats.Health < 0)
                    playerStats.Health = 0;
                
                Debug.Log($"Enemy attacks for {(int)damage} damage!");
            }
            else
            {
                // Defend
                enemyStats.Defense += 2;
                
                Debug.Log("Enemy increases its defense by 2!");
            }
        }
        
        /// <summary>
        /// Get the game state for UI
        /// </summary>
        public void GetGameState()
        {
            if (_playerEntity == null)
            {
                Debug.Log("No active game");
                return;
            }
            
            StatsComponent playerStats = _playerEntity.GetComponent<StatsComponent>();
            
            Debug.Log($"Player Health: {playerStats.Health}/{playerStats.MaxHealth}");
            Debug.Log($"Player Gold: {_playerGold}");
            Debug.Log($"Cards in Deck: {_cardSystem.GetDeck().Count}");
            Debug.Log($"Cards in Hand: {_cardSystem.GetHand().Count}");
            Debug.Log($"Cards in Discard: {_cardSystem.GetDiscardPile().Count}");
            Debug.Log($"Support Cards: {_cardSystem.GetSupportZone().Count}");
            
            if (_isBattleActive && _enemyEntity != null)
            {
                StatsComponent enemyStats = _enemyEntity.GetComponent<StatsComponent>();
                ElementComponent enemyElement = _enemyEntity.GetComponent<ElementComponent>();
                
                Debug.Log($"Enemy Health: {enemyStats.Health}/{enemyStats.MaxHealth}");
                Debug.Log($"Enemy Element: {enemyElement?.GetElementName() ?? "None"}");
            }
        }
        
        /// <summary>
        /// Set the current season
        /// </summary>
        public void SetSeason(Season season)
        {
            _currentSeason = season;
            _elementInteractionSystem.SetSeason(season);
            Debug.Log($"Season changed to: {season}");
        }
        
        // Getters for systems and entities
        public EntityManager GetEntityManager() => _entityManager;
        public CardSystem GetCardSystem() => _cardSystem;
        public BattleSystem GetBattleSystem() => _battleSystem;
        public ElementInteractionSystem GetElementInteractionSystem() => _elementInteractionSystem;
        public SupportCardSystem GetSupportCardSystem() => _supportCardSystem;
        public Entity GetPlayerEntity() => _playerEntity;
        public Entity GetEnemyEntity() => _enemyEntity;
        public Season GetCurrentSeason() => _currentSeason;
    }
}