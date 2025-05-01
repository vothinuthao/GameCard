// GameManager.cs

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
    /// Main game manager class that ties all the systems together
    /// Uses the Singleton pattern for easy access
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        // Singleton instance
        public static GameManager Instance { get; private set; }
        
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
        
        // Game state
        private Entity playerEntity;
        private Entity enemyEntity;
        private bool isBattleActive = false;
        
        // Current season and environment
        private Season _currentSeason = Season.Spring;
        
        // Awake is called when the script instance is being loaded
        private void Awake()
        {
            // Singleton pattern
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeGame();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        // Initialize the game systems
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
            
            // Set initial season
            elementInteractionSystem.SetSeason(_currentSeason);
            
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
            
            // Create player entity
            playerEntity = CreatePlayerEntity();
            
            // Create a sample deck
            List<Entity> deck = cardFactory.CreateSampleDeck();
            
            // Add cards to player's deck
            foreach (var card in deck)
            {
                cardSystem.AddCardToDeck(card);
            }
            
            // Shuffle the deck
            cardSystem.ShuffleDeck();
            
            Debug.Log("New game started successfully!");
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
                // Add rewards logic here
            }
            else
            {
                Debug.Log("Player lost the battle!");
                // Add game over logic here
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
                Health = 30,
                MaxHealth = 30,
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
                default:
                    stats.Attack = 4;
                    stats.Defense = 3;
                    stats.Health = 20;
                    stats.MaxHealth = 20;
                    stats.Speed = 4;
                    break;
            }
            
            enemy.AddComponent(stats);
            
            // Add element component (for simplicity, making all enemies Fire element)
            ElementComponent element = new ElementComponent
            {
                Element = ElementType.Fire
            };
            enemy.AddComponent(element);
            
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
            
            // TODO: Implement enemy AI turn
            // For now, just print a message
            Debug.Log("Enemy performed its turn");
            
            // Start new player turn
            battleSystem.StartNewTurn();
        }
        
        /// <summary>
        /// Get the current state of the game
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
            Debug.Log($"Cards in Deck: {cardSystem.GetDeck().Count}");
            Debug.Log($"Cards in Hand: {cardSystem.GetHand().Count}");
            Debug.Log($"Cards in Discard: {cardSystem.GetDiscardPile().Count}");
            Debug.Log($"Support Cards: {cardSystem.GetSupportZone().Count}");
            
            if (isBattleActive && enemyEntity != null)
            {
                StatsComponent enemyStats = enemyEntity.GetComponent<StatsComponent>();
                Debug.Log($"Enemy Health: {enemyStats.Health}/{enemyStats.MaxHealth}");
            }
        }
        
        /// <summary>
        /// Set the current season
        /// </summary>
        public void SetSeason(Season season)
        {
            _currentSeason = season;
            elementInteractionSystem.SetSeason(season);
            Debug.Log($"Season changed to: {season}");
        }
    }
}