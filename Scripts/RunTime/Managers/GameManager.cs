using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Factories;
using Systems;
using Systems.States;
using UnityEngine;

namespace RunTime
{
    /// <summary>
    /// Main game manager that ties all the systems together and handles game flow
    /// Updated to use the new state-based battle system
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
        private SupportCardFactory _supportCardFactory;
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
        private int _playerEnergy = 3;
        private int _maxPlayerEnergy = 3;
        
        // Flags
        private bool _isInitialized = false;
        
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
        /// This must be called before starting a game
        /// </summary>
        private void InitializeGame()
        {
            // Avoid duplicate initialization
            if (_isInitialized)
            {
                Debug.Log("Game already initialized, skipping...");
                return;
            }
            
            Debug.Log("Initializing game systems...");
            
            // Create ECS components
            _entityManager = new EntityManager();
            _systemManager = new SystemManager();
            
            // Create game systems
            _cardSystem = new CardSystem(_entityManager);
            _elementInteractionSystem = new ElementInteractionSystem(_entityManager);
            _supportCardSystem = new SupportCardSystem(_entityManager);
            
            // BattleSystem depends on other systems, so create it last
            _battleSystem = new BattleSystem(_entityManager, _elementInteractionSystem, _cardSystem, _supportCardSystem);
            
            // Add systems to system manager
            _systemManager.AddSystem(_cardSystem);
            _systemManager.AddSystem(_elementInteractionSystem);
            _systemManager.AddSystem(_supportCardSystem);
            _systemManager.AddSystem(_battleSystem);
            
            // Create factories
            _cardFactory = new CardFactory(_entityManager);
            _supportCardFactory = new SupportCardFactory(_entityManager);
            
            // Initialize ScriptableObjectFactory
            if (ScriptableObjectFactory.HasInstance && !ScriptableObjectFactory.Instance.IsInitialized)
            {
                ScriptableObjectFactory.Instance.Initialize(_entityManager);
            }
            else if (!ScriptableObjectFactory.HasInstance)
            {
                _scriptableObjectFactory = ScriptableObjectFactory.Instance;
                _scriptableObjectFactory.Initialize(_entityManager);
            }
            else
            {
                _scriptableObjectFactory = ScriptableObjectFactory.Instance;
            }
            
            // Initialize LoadCardData with EntityManager
            if (LoadCardData.HasInstance && !LoadCardData.Instance.IsInitialized)
            {
                LoadCardData.Instance.Initialize(_entityManager);
            }
            else if (!LoadCardData.HasInstance)
            {
                LoadCardData.Instance.Initialize(_entityManager);
            }
            
            // Set initial season
            _elementInteractionSystem.SetSeason(_currentSeason);
            
            _isInitialized = true;
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
        
        /// <summary>
        /// Start a new game, initializing player and creating a deck
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("Starting new game...");
            
            // Ensure game is initialized
            if (!_isInitialized)
            {
                InitializeGame();
            }
            
            // Ensure LoadCardData is initialized and load all cards
            if (!LoadCardData.Instance.IsInitialized)
            {
                LoadCardData.Instance.Initialize(_entityManager);
            }
            LoadCardData.Instance.LoadAllCards();
            
            // Reset player stats
            _playerHealth = _maxPlayerHealth;
            _playerGold = 0;
            _playerEnergy = _maxPlayerEnergy;
            
            // Create player entity
            _playerEntity = CreatePlayerEntity();
            
            // Create a sample deck
            List<Entity> deck = CreateSampleDeck();
            
            if (deck != null && deck.Count > 0)
            {
                foreach (var card in deck)
                {
                    if (card != null)
                    {
                        _cardSystem.AddCardToDeck(card);
                    }
                }
                
                // Shuffle the deck
                _cardSystem.ShuffleDeck();
                
                Debug.Log("New game started successfully!");
                StartBattle("medium");
            }
            else
            {
                Debug.LogError("Failed to create deck! Game cannot start.");
            }
        }
        
        /// <summary>
        /// Create a random deck for the player
        /// Uses LoadCardData to create a balanced deck of random cards
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
            
            List<Entity> deck = null;
            
            // First attempt: Use LoadCardData to create a random deck
            try
            {
                if (LoadCardData.HasInstance && LoadCardData.Instance.IsInitialized)
                {
                    deck = LoadCardData.Instance.CreateRandomStarterDeck(deckTheme, deckSize);
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error creating deck with LoadCardData: {e.Message}");
                deck = null;
            }
            
            // Second attempt: Use CardFactory as fallback
            if (deck == null || deck.Count == 0)
            {
                Debug.LogWarning("Failed to create deck using LoadCardData, falling back to manual creation");
                
                try
                {
                    // Use CardFactory to create a basic deck
                    if (_cardFactory != null)
                    {
                        deck = _cardFactory.CreateSampleDeck();
                        Debug.Log("Created basic deck using CardFactory");
                    }
                    else
                    {
                        Debug.LogError("CardFactory is null, cannot create deck!");
                        deck = new List<Entity>();
                    }
                }
                catch (System.Exception e)
                {
                    Debug.LogError($"Error creating deck with CardFactory: {e.Message}");
                    deck = new List<Entity>();
                }
            }
            
            // Ensure deck has at least some cards
            if (deck == null || deck.Count == 0)
            {
                Debug.LogError("Failed to create any cards for the deck!");
                return new List<Entity>();
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
            
            // Set battle active flag
            _isBattleActive = true;
            
            // Start battle in battle system
            _battleSystem.StartBattle(_playerEntity, _enemyEntity);
            
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
                Element = (ElementType)Random.Range(1, 6) // 1-5 = all elements except None(0)
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
            
            // This now needs to handle selection through BattleSystem's state machine
            List<Entity> hand = _cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Invalid card index: {cardIndex}");
                return;
            }
            
            Entity card = hand[cardIndex];
            
            // Use this method to select a card rather than play it directly
            _battleSystem.SelectCard(card);
            
            Debug.Log($"Selected card at index {cardIndex}");
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
            
            // Use BattleSystem's state machine to change to PlayerTurnEnd state
            BattleStateMachine stateMachine = _battleSystem.GetStateMachine();
            if (stateMachine != null)
            {
                stateMachine.ChangeState(BattleStateType.PlayerTurnEnd);
                Debug.Log("Player turn ended");
            }
            else
            {
                // Fallback if state machine is not available
                Debug.LogWarning("Battle state machine not available, using direct method");
                _battleSystem.EndPlayerTurn();
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
            Debug.Log($"Player Energy: {_playerEnergy}/{_maxPlayerEnergy}");
            Debug.Log($"Cards in Deck: {_cardSystem.GetDeck().Count}");
            Debug.Log($"Cards in Hand: {_cardSystem.GetHand().Count}");
            Debug.Log($"Cards in Discard: {_cardSystem.GetDiscardPile().Count}");
            Debug.Log($"Support Cards: {_cardSystem.GetSupportZone().Count}");
            
            if (_isBattleActive && _enemyEntity != null)
            {
                StatsComponent enemyStats = _enemyEntity.GetComponent<StatsComponent>();
                ElementComponent enemyElement = _enemyEntity.GetComponent<ElementComponent>();
                
                Debug.Log($"Enemy Health: {enemyStats.Health}/{enemyStats.MaxHealth}");
                Debug.Log($"Enemy Element: {enemyElement?.Element.ToString() ?? "None"}");
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
        
        /// <summary>
        /// Get player energy
        /// </summary>
        public int GetPlayerEnergy()
        {
            return _playerEnergy;
        }
        
        /// <summary>
        /// Set player energy
        /// </summary>
        public void SetPlayerEnergy(int energy)
        {
            _playerEnergy = Mathf.Clamp(energy, 0, _maxPlayerEnergy);
        }
        
        /// <summary>
        /// Reset player energy to max
        /// </summary>
        public void ResetPlayerEnergy()
        {
            _playerEnergy = _maxPlayerEnergy;
        }
        
        // Getters for systems and entities
        public EntityManager GetEntityManager() => _entityManager;
        public CardSystem GetCardSystem() => _cardSystem;
        public BattleSystem GetBattleSystem() => _battleSystem;
        public ElementInteractionSystem GetElementInteractionSystem() => _elementInteractionSystem;
        public SupportCardSystem GetSupportCardSystem() => _supportCardSystem;
        public SupportCardFactory GetSupportCardFactory() => _supportCardFactory;
        public Entity GetPlayerEntity() => _playerEntity;
        public Entity GetEnemyEntity() => _enemyEntity;
        public Season GetCurrentSeason() => _currentSeason;
    }
}