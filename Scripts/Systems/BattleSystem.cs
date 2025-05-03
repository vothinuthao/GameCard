using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Systems.States;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// System that handles battle logic with state machine integration
    /// </summary>
    public class BattleSystem : Core.System
    {
        // References to other systems
        private ElementInteractionSystem _elementInteractionSystem;
        private CardSystem _cardSystem;
        private SupportCardSystem _supportCardSystem;
        
        // State machine
        private BattleStateMachine _stateMachine;
        
        // Battle state
        private Entity _playerEntity;
        private Entity _enemyEntity;
        private List<Entity> _selectedCards = new List<Entity>();
        private List<Entity> _playedCards = new List<Entity>();
        
        // Battle stats
        private int _currentTurn = 1;
        private int _playerEnergy = 3;
        private int _maxPlayerEnergy = 3;
        
        // Constructor
        public BattleSystem(EntityManager entityManager, ElementInteractionSystem elementInteractionSystem, 
                            CardSystem cardSystem, SupportCardSystem supportCardSystem) : base(entityManager)
        {
            _elementInteractionSystem = elementInteractionSystem;
            _cardSystem = cardSystem;
            _supportCardSystem = supportCardSystem;
            
            // Create the state machine
            _stateMachine = new BattleStateMachine(this);
        }
        
        /// <summary>
        /// Initialize a battle
        /// </summary>
        public void InitializeBattle()
        {
            Debug.Log("Initializing battle...");
            
            // Reset battle state
            _currentTurn = 1;
            _playerEnergy = _maxPlayerEnergy;
            _selectedCards.Clear();
            _playedCards.Clear();
            
            // Draw initial hand
            _cardSystem.DrawCards(5);
            
            Debug.Log("Battle initialized!");
        }
        
        /// <summary>
        /// Update method - called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Update the state machine
            _stateMachine.Update();
        }
        
        /// <summary>
        /// Start the battle
        /// </summary>
        public void StartBattle(Entity player, Entity enemy)
        {
            _playerEntity = player;
            _enemyEntity = enemy;
            
            // Start the state machine
            _stateMachine.Start();
        }
        
        /// <summary>
        /// Start a new player turn
        /// </summary>
        public void StartPlayerTurn()
        {
            Debug.Log($"Starting player turn {_currentTurn}");
            
            // Reset player energy
            _playerEnergy = _maxPlayerEnergy;
            
            // Draw a card if hand is less than 5
            if (_cardSystem.GetHand().Count < 5)
            {
                _cardSystem.DrawCard();
            }
            
            // Reset selected cards
            _selectedCards.Clear();
            
            // Clear played cards for this turn
            _playedCards.Clear();
            
            // Update support card cooldowns
            List<Entity> supportZone = _cardSystem.GetSupportZone();
            foreach (var card in supportZone)
            {
                SupportCardComponent supportComponent = card.GetComponent<SupportCardComponent>();
                if (supportComponent != null && supportComponent.CurrentCooldown > 0)
                {
                    supportComponent.CurrentCooldown--;
                }
            }
        }
        
        /// <summary>
        /// End the player's turn
        /// </summary>
        public void EndPlayerTurn()
        {
            // Discard cards in play zone
            _cardSystem.DiscardPlayZone();
            
            // Reset played cards list
            _playedCards.Clear();
        }
        
        /// <summary>
        /// Start the enemy's turn
        /// </summary>
        public void StartEnemyTurn()
        {
            Debug.Log("Starting enemy turn");
            
            // Any enemy turn initialization logic
        }
        
        /// <summary>
        /// Enemy performs its action
        /// </summary>
        public void PerformEnemyAction()
        {
            Debug.Log("Enemy performing action");
            
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
        /// End the enemy's turn
        /// </summary>
        public void EndEnemyTurn()
        {
            Debug.Log("Ending enemy turn");
            
            // Increment turn counter
            _currentTurn++;
        }
        
        /// <summary>
        /// Select a card to play
        /// </summary>
        public void SelectCard(Entity card)
        {
            if (card == null)
                return;
                
            // Check if card is already selected
            if (_selectedCards.Contains(card))
            {
                _selectedCards.Remove(card);
                Debug.Log("Card deselected");
                return;
            }
            
            // Check if we already have 2 cards selected
            if (_selectedCards.Count >= 2)
            {
                Debug.Log("Already have 2 cards selected, deselect one first");
                return;
            }
            
            // Get card cost
            int cardCost = 1; // Default cost
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardCost = cardInfo.Cost;
            }
            
            // Check if we have enough energy
            int totalCost = 0;
            foreach (var selectedCard in _selectedCards)
            {
                CardInfoComponent selectedCardInfo = selectedCard.GetComponent<CardInfoComponent>();
                if (selectedCardInfo != null)
                {
                    totalCost += selectedCardInfo.Cost;
                }
            }
            
            totalCost += cardCost;
            
            if (_playerEnergy < totalCost)
            {
                Debug.Log("Not enough energy to select this card");
                return;
            }
            
            // Add to selected cards
            _selectedCards.Add(card);
            Debug.Log("Card selected");
            
            // If we have 2 cards or this card is a support card, trigger card resolution
            if (_selectedCards.Count == 2 || card.HasComponent<SupportCardComponent>())
            {
                // Change state to card resolution
                _stateMachine.ChangeState(BattleStateType.CardResolution);
            }
        }
        
        /// <summary>
        /// Resolve the selected cards
        /// </summary>
        public void ResolveSelectedCards()
        {
            if (_selectedCards.Count == 0)
                return;
                
            // Check for support cards
            foreach (var card in _selectedCards)
            {
                if (card.HasComponent<SupportCardComponent>())
                {
                    // Play as support
                    _cardSystem.PlayAsSupport(card);
                    
                    // Remove from selected cards
                    _selectedCards.Remove(card);
                    
                    Debug.Log("Played card as support");
                    continue;
                }
                
                // Play the card
                _cardSystem.PlayCard(card);
                
                // Add to played cards
                _playedCards.Add(card);
                
                // Apply card effects
                ApplyCardEffects(card, _enemyEntity);
                
                // Deduct energy
                CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
                if (cardInfo != null)
                {
                    _playerEnergy -= cardInfo.Cost;
                }
                
                Debug.Log("Played card");
            }
            
            // Update support card system with played cards
            _supportCardSystem.SetPlayedCards(_playedCards);
            
            // Clear selected cards
            _selectedCards.Clear();
        }
        
        /// <summary>
        /// Apply the effects of a card to a target
        /// </summary>
        private void ApplyCardEffects(Entity card, Entity target)
        {
            // Get components
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            ElementComponent element = card.GetComponent<ElementComponent>();
            StatsComponent cardStats = card.GetComponent<StatsComponent>();
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            
            if (cardInfo == null || element == null || cardStats == null || targetStats == null)
                return;
            
            // Calculate damage
            float baseDamage = cardStats.Attack;
            
            // Apply element bonuses
            ElementComponent targetElement = target.GetComponent<ElementComponent>();
            if (targetElement != null)
            {
                float elementAdvantage = _elementInteractionSystem.CalculateElementAdvantage(card, target);
                baseDamage *= (1 + elementAdvantage);
            }
            
            // Apply defense
            float damageReduction = targetStats.Defense / 100f; // Simple formula, can be more complex
            float finalDamage = baseDamage * (1 - damageReduction);
            
            // Apply the damage
            ApplyDamage(target, (int)finalDamage);
            
            // Apply effects
            EffectComponent effectComponent = card.GetComponent<EffectComponent>();
            if (effectComponent != null)
            {
                foreach (var effect in effectComponent.Effects)
                {
                    effect.Apply(target);
                }
            }
        }
        
        /// <summary>
        /// Apply damage to a target
        /// </summary>
        private void ApplyDamage(Entity target, int damage)
        {
            StatsComponent stats = target.GetComponent<StatsComponent>();
            if (stats != null)
            {
                stats.Health -= damage;
                if (stats.Health < 0)
                    stats.Health = 0;
                
                Debug.Log($"Applied {damage} damage to target");
            }
        }
        
        /// <summary>
        /// Check for support card activations
        /// </summary>
        public void CheckSupportCards()
        {
            // Get current battle state
            BattleStateType currentState = _stateMachine.GetCurrentStateType();
            
            // Check for specific support card activations based on the state
            switch (currentState)
            {
                case BattleStateType.PlayerTurnStart:
                    _supportCardSystem.CheckOnTurnStartActivations(_playerEntity);
                    break;
                case BattleStateType.PlayerTurnEnd:
                    _supportCardSystem.CheckOnTurnEndActivations(_playerEntity);
                    break;
                case BattleStateType.EnemyTurnStart:
                    // Could check enemy-specific activations here
                    break;
                default:
                    // Default checks for all states
                    _supportCardSystem.CheckOnEntryActivations();
                    break;
            }
        }
        
        /// <summary>
        /// End the battle
        /// </summary>
        public void EndBattle()
        {
            Debug.Log("Battle ended");
            
            // Additional cleanup or state transitions could happen here
        }
        
        /// <summary>
        /// Give rewards to the player after a successful battle
        /// </summary>
        public void GiveRewards()
        {
            Debug.Log("Giving rewards to player");
            
            // Reward logic would go here
        }
        
        /// <summary>
        /// Check if battle is over
        /// </summary>
        public bool IsBattleOver()
        {
            StatsComponent playerStats = _playerEntity?.GetComponent<StatsComponent>();
            StatsComponent enemyStats = _enemyEntity?.GetComponent<StatsComponent>();
            
            return (playerStats != null && playerStats.Health <= 0) || 
                   (enemyStats != null && enemyStats.Health <= 0);
        }
        
        /// <summary>
        /// Get the winner of the battle
        /// </summary>
        public Entity GetWinner()
        {
            if (!IsBattleOver())
                return null;
            
            StatsComponent playerStats = _playerEntity?.GetComponent<StatsComponent>();
            StatsComponent enemyStats = _enemyEntity?.GetComponent<StatsComponent>();
            
            if (playerStats != null && playerStats.Health <= 0)
                return _enemyEntity;
            else
                return _playerEntity;
        }
        
        /// <summary>
        /// Get player entity
        /// </summary>
        public Entity GetPlayerEntity() => _playerEntity;
        
        /// <summary>
        /// Get enemy entity
        /// </summary>
        public Entity GetEnemyEntity() => _enemyEntity;
        
        /// <summary>
        /// Get the battle state machine
        /// </summary>
        public BattleStateMachine GetStateMachine() => _stateMachine;
        
        /// <summary>
        /// Get player energy
        /// </summary>
        public int GetPlayerEnergy() => _playerEnergy;
        
        /// <summary>
        /// Get current turn
        /// </summary>
        public int GetCurrentTurn() => _currentTurn;

        public int GetMaxPlayerEnergy() => _maxPlayerEnergy;
    }
}