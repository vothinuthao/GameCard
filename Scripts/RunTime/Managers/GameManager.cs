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
        private EntityManager entityManager;
        private SystemManager systemManager;
        
        // Game systems
        private CardSystem cardSystem;
        private ElementInteractionSystem elementInteractionSystem;
        private BattleSystem battleSystem;
        private SupportCardSystem supportCardSystem;
        
        // Factories
        private CardFactory cardFactory;
        private ScriptableObjectFactory scriptableObjectFactory;
        
        // Game state
        private Entity playerEntity;
        private Entity enemyEntity;
        private bool isBattleActive = false;
        
        // Current season and environment
        private Season currentSeason = Season.Spring;
        
        // Player stats
        private int playerHealth = 50;
        private int maxPlayerHealth = 50;
        private int playerGold = 0;
        
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
            entityManager = new EntityManager();
            systemManager = new SystemManager();
            
            // Create game systems
            cardSystem = new CardSystem(entityManager);
            elementInteractionSystem = new ElementInteractionSystem(entityManager);
            supportCardSystem = new SupportCardSystem(entityManager);
            battleSystem = new BattleSystem(entityManager, elementInteractionSystem, cardSystem);
            
            // Add systems to system manager
            systemManager.AddSystem(cardSystem);
            systemManager.AddSystem(elementInteractionSystem);
            systemManager.AddSystem(supportCardSystem);
            systemManager.AddSystem(battleSystem);
            
            // Create factories
            cardFactory = new CardFactory(entityManager);
            scriptableObjectFactory = new ScriptableObjectFactory(entityManager);
            
            // Set initial season
            elementInteractionSystem.SetSeason(currentSeason);
            
            Debug.Log("Game systems initialized successfully!");
        }
        
        // Update is called once per frame
        private void Update()
        {
            if (isBattleActive)
            {
                // Update all systems
                systemManager.UpdateAllSystems(Time.deltaTime);
                
                // Check if battle is over
                if (battleSystem.IsBattleOver())
                {
                    EndBattle();
                }
            }
        }
        
        /// <summary>
        /// Start a new game
        /// </summary>
        public void StartNewGame()
        {
            Debug.Log("Starting new game...");
            
            // Reset player stats
            playerHealth = maxPlayerHealth;
            playerGold = 0;
            
            // Create player entity
            playerEntity = CreatePlayerEntity();
            
            // Create a sample deck
            List<Entity> deck = CreateSampleDeck();
            
            // Add cards to player's deck
            foreach (var card in deck)
            {
                cardSystem.AddCardToDeck(card);
            }
            
            // Shuffle the deck
            cardSystem.ShuffleDeck();
            
            Debug.Log("New game started successfully!");
            
            // Automatically start a battle
            StartBattle("medium");
        }
        
        /// <summary>
        /// Create a sample deck
        /// </summary>
        private List<Entity> CreateSampleDeck()
        {
            // Try to load cards from Resources first
            List<Entity> deck = new List<Entity>();
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
                    if (cardData != null)
                    {
                        Entity card = scriptableObjectFactory.CreateCardFromSO(cardData);
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
                deck = cardFactory.CreateSampleDeck();
            }
            
            return deck;
        }
        
        /// <summary>
        /// Start a battle with an enemy
        /// </summary>
        public void StartBattle(string enemyType)
        {
            if (playerEntity == null)
            {
                Debug.LogError("Cannot start battle - player entity not created!");
                return;
            }
            
            Debug.Log($"Starting battle with enemy: {enemyType}");
            
            // Create enemy entity
            enemyEntity = CreateEnemyEntity(enemyType);
            
            // Initialize battle
            battleSystem.InitializeBattle(playerEntity, enemyEntity);
            isBattleActive = true;
            
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
            isBattleActive = false;
            
            // Get winner
            Entity winner = battleSystem.GetWinner();
            
            if (winner == playerEntity)
            {
                Debug.Log("Player won the battle!");
                
                // Add rewards
                playerGold += 25;
                
                // Heal player a bit
                StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
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
            Entity player = entityManager.CreateEntity();
            
            // Add stats component
            StatsComponent stats = new StatsComponent
            {
                Attack = 5,
                Defense = 5,
                Health = playerHealth,
                MaxHealth = maxPlayerHealth,
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
            Entity enemy = entityManager.CreateEntity();
            
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
            if (!isBattleActive)
            {
                Debug.LogWarning("Cannot play card - no active battle!");
                return;
            }
            
            List<Entity> hand = cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Invalid card index: {cardIndex}");
                return;
            }
            
            Entity card = hand[cardIndex];
            battleSystem.PlayCard(card, enemyEntity);
            
            Debug.Log($"Played card at index {cardIndex}");
        }
        
        /// <summary>
        /// Play a card as a support card
        /// </summary>
        public void PlayAsSupport(int cardIndex)
        {
            if (!isBattleActive)
            {
                Debug.LogWarning("Cannot play support card - no active battle!");
                return;
            }
            
            List<Entity> hand = cardSystem.GetHand();
            
            if (cardIndex < 0 || cardIndex >= hand.Count)
            {
                Debug.LogError($"Invalid card index: {cardIndex}");
                return;
            }
            
            // Check if the card can be played as a support
            Entity card = hand[cardIndex];
            if (card.HasComponent<SupportCardComponent>())
            {
                cardSystem.PlayAsSupport(card);
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
            if (!isBattleActive)
            {
                Debug.LogWarning("Cannot end turn - no active battle!");
                return;
            }
            
            battleSystem.StartNewTurn();
            Debug.Log("Player turn ended");
            
            // Simple enemy AI
            PerformEnemyTurn();
            
            // Start new player turn
            battleSystem.StartNewTurn();
        }
        
        /// <summary>
        /// Perform enemy turn
        /// </summary>
        private void PerformEnemyTurn()
        {
            if (playerEntity == null || enemyEntity == null)
                return;
            
            StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
            StatsComponent enemyStats = enemyEntity.GetComponent<StatsComponent>();
            
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
            if (playerEntity == null)
            {
                Debug.Log("No active game");
                return;
            }
            
            StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
            
            Debug.Log($"Player Health: {playerStats.Health}/{playerStats.MaxHealth}");
            Debug.Log($"Player Gold: {playerGold}");
            Debug.Log($"Cards in Deck: {cardSystem.GetDeck().Count}");
            Debug.Log($"Cards in Hand: {cardSystem.GetHand().Count}");
            Debug.Log($"Cards in Discard: {cardSystem.GetDiscardPile().Count}");
            Debug.Log($"Support Cards: {cardSystem.GetSupportZone().Count}");
            
            if (isBattleActive && enemyEntity != null)
            {
                StatsComponent enemyStats = enemyEntity.GetComponent<StatsComponent>();
                ElementComponent enemyElement = enemyEntity.GetComponent<ElementComponent>();
                
                Debug.Log($"Enemy Health: {enemyStats.Health}/{enemyStats.MaxHealth}");
                Debug.Log($"Enemy Element: {enemyElement?.GetElementName() ?? "None"}");
            }
        }
        
        /// <summary>
        /// Set the current season
        /// </summary>
        public void SetSeason(Season season)
        {
            currentSeason = season;
            elementInteractionSystem.SetSeason(season);
            Debug.Log($"Season changed to: {season}");
        }
        
        // Getters for systems and entities
        public EntityManager GetEntityManager() => entityManager;
        public CardSystem GetCardSystem() => cardSystem;
        public BattleSystem GetBattleSystem() => battleSystem;
        public ElementInteractionSystem GetElementInteractionSystem() => elementInteractionSystem;
        public SupportCardSystem GetSupportCardSystem() => supportCardSystem;
        public Entity GetPlayerEntity() => playerEntity;
        public Entity GetEnemyEntity() => enemyEntity;
        public Season GetCurrentSeason() => currentSeason;
    }
}