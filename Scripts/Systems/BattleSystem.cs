using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;

namespace Systems
{
    /// <summary>
    /// System that handles battle logic
    /// </summary>
    public class BattleSystem : Core.System
    {
        // References to other systems
        private ElementInteractionSystem elementInteractionSystem;
        private CardSystem cardSystem;
        
        // Battle state
        private Entity player;
        private Entity enemy;
        private List<Entity> playedCards = new List<Entity>();
        
        // Constructor
        public BattleSystem(EntityManager entityManager, ElementInteractionSystem elementInteractionSystem, CardSystem cardSystem) : base(entityManager)
        {
            this.elementInteractionSystem = elementInteractionSystem;
            this.cardSystem = cardSystem;
        }
        
        /// <summary>
        /// Initialize a battle
        /// </summary>
        public void InitializeBattle(Entity player, Entity enemy)
        {
            this.player = player;
            this.enemy = enemy;
            playedCards.Clear();
            
            // Draw initial hand
            cardSystem.DrawCards(5);
        }
        
        /// <summary>
        /// Update method - called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Battle logic updates
            // This would be more complex in a real implementation
        }
        
        /// <summary>
        /// Start a new turn
        /// </summary>
        public void StartNewTurn()
        {
            // Clear played cards list
            playedCards.Clear();
            
            // Discard cards in play zone
            cardSystem.DiscardPlayZone();
            
            // Draw a new card
            cardSystem.DrawCard();
            
            // Update support card cooldowns
            List<Entity> supportZone = cardSystem.GetSupportZone();
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
        /// Play a card
        /// </summary>
        public void PlayCard(Entity card, Entity target)
        {
            if (card == null || target == null)
                return;
            
            // Move the card to the play zone
            cardSystem.PlayCard(card);
            
            // Add to played cards list
            playedCards.Add(card);
            
            // Apply card effects
            ApplyCardEffects(card, target);
            
            // Check for support card activations
            CheckSupportCardActivations(card, target);
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
                float elementAdvantage = elementInteractionSystem.CalculateElementAdvantage(card, target);
                baseDamage *= (1 + elementAdvantage);
            }
            
            // Apply season effects
            float seasonBonus = elementInteractionSystem.GetSeasonBonus(element.Element, Season.Spring); // Use current season instead of hardcoded
            baseDamage *= (1 + seasonBonus);
            
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
            }
        }
        
        /// <summary>
        /// Check for support card activations
        /// </summary>
        private void CheckSupportCardActivations(Entity playedCard, Entity target)
        {
            List<Entity> supportZone = cardSystem.GetSupportZone();
            
            foreach (var supportCard in supportZone)
            {
                SupportCardComponent supportComponent = supportCard.GetComponent<SupportCardComponent>();
                
                if (supportComponent != null && supportComponent.CurrentCooldown <= 0)
                {
                    // Check activation condition
                    bool activationConditionMet = false;
                    
                    switch (supportComponent.ActivationType)
                    {
                        case ActivationType.Recurring:
                        case ActivationType.Triggered:
                            // Check if the condition is met
                            activationConditionMet = supportComponent.Condition.IsMet(supportCard, target, playedCards);
                            break;
                        case ActivationType.Reactive:
                            // Reactive cards activate in response to specific events
                            // This would be more complex in a real implementation
                            break;
                        case ActivationType.Persistent:
                            // Persistent effects are always active, no need to check
                            activationConditionMet = true;
                            break;
                    }
                    
                    if (activationConditionMet)
                    {
                        // Activate the support card
                        supportComponent.Effect.Apply(target);
                        supportComponent.IsActive = true;
                        
                        // Set cooldown if it's not a persistent effect
                        if (supportComponent.ActivationType != ActivationType.Persistent)
                        {
                            supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Check if battle is over
        /// </summary>
        public bool IsBattleOver()
        {
            StatsComponent playerStats = player.GetComponent<StatsComponent>();
            StatsComponent enemyStats = enemy.GetComponent<StatsComponent>();
            
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
            
            StatsComponent playerStats = player.GetComponent<StatsComponent>();
            StatsComponent enemyStats = enemy.GetComponent<StatsComponent>();
            
            if (playerStats != null && playerStats.Health <= 0)
                return enemy;
            else
                return player;
        }
    }
}