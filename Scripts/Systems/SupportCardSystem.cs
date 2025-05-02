using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// System that handles support cards (Thẻ Phụ)
    /// Updated to work with the state-based battle system
    /// </summary>
    public class SupportCardSystem : Core.System
    {
        // Context for activation conditions
        private List<Entity> _playedCards = new List<Entity>();
        
        // List of activated cards this turn to prevent multiple activations
        private List<Entity> _activatedCardsThisTurn = new List<Entity>();

        // Constructor
        public SupportCardSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        /// <summary>
        /// Update method - called every frame but with state-based battle system
        /// this is less important
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Most checks are now driven by the battle state machine
            // This method is kept for compatibility
        }

        /// <summary>
        /// Set the played cards for this turn
        /// </summary>
        public void SetPlayedCards(List<Entity> playedCards)
        {
            _playedCards = new List<Entity>(playedCards);
        }

        /// <summary>
        /// Clear played cards and activated cards at the end of a turn
        /// </summary>
        public void ClearTurnData()
        {
            _playedCards.Clear();
            _activatedCardsThisTurn.Clear();
        }

        /// <summary>
        /// Check for OnEntry activation type support cards
        /// </summary>
        public void CheckOnEntryActivations()
        {
            // Get all support cards from the support zone
            var supportCards = GetSupportCards();

            foreach (var entity in supportCards)
            {
                var supportCard = entity.GetComponent<SupportCardComponent>();
                if (supportCard == null) continue;

                // Check if it's an OnEntry card and not already active
                if (supportCard.ActivationType == ActivationType.OnEntry && 
                    !supportCard.IsActive &&
                    supportCard.CurrentCooldown <= 0)
                {
                    // Log activation
                    CardInfoComponent cardInfo = entity.GetComponent<CardInfoComponent>();
                    Debug.Log($"Activating OnEntry support card: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                    
                    // Activate the card effect
                    if (supportCard.Effect != null)
                    {
                        supportCard.Effect.Apply(null);
                    }
                    
                    // Mark as active
                    supportCard.IsActive = true;
                    supportCard.CurrentCooldown = supportCard.CooldownTime;
                    _activatedCardsThisTurn.Add(entity);
                }
            }
        }

        /// <summary>
        /// Check for support card activations at the start of a turn
        /// </summary>
        public void CheckOnTurnStartActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Persistent effects are always active
                if (supportComponent.ActivationType == ActivationType.Persistent)
                {
                    if (!_activatedCardsThisTurn.Contains(supportCard))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Persistent support card: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Apply persistent effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }

                // Recurring effects at turn start
                if (supportComponent.ActivationType == ActivationType.Recurring &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Check turn start condition - this would need proper implementation
                    // based on the type of condition
                    bool isTurnStartCondition = IsTurnStartCondition(supportComponent.Condition);
                    
                    if (isTurnStartCondition && supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Recurring support card at turn start: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for support card activations at the end of a turn
        /// </summary>
        public void CheckOnTurnEndActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Check for turn end activations
                if (supportComponent.ActivationType == ActivationType.Recurring &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Check turn end condition
                    bool isTurnEndCondition = IsTurnEndCondition(supportComponent.Condition);
                    
                    if (isTurnEndCondition && supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Recurring support card at turn end: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for support card activations based on health
        /// </summary>
        public void CheckHealthBasedActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Check if it's a Triggered card with a health condition
                if ((supportComponent.ActivationType == ActivationType.Triggered ||
                     supportComponent.ActivationType == ActivationType.Reactive) &&
                    supportComponent.Condition is HealthPercentCondition &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Health-based support card: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Activate the card
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        // For Triggered cards, activate once and set cooldown
                        if (supportComponent.ActivationType == ActivationType.Triggered)
                        {
                            supportComponent.IsActive = true;
                            supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        }
                        
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for element based activations
        /// </summary>
        public void CheckElementBasedActivations(Entity target)
        {
            if (_playedCards.Count == 0) return;
            
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Check for element-based activations
                if ((supportComponent.ActivationType == ActivationType.Recurring ||
                     supportComponent.ActivationType == ActivationType.Triggered) &&
                    supportComponent.Condition is ElementPlayedCondition &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, null, _playedCards))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Element-based support card: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(target ?? _playedCards[0]);
                        }
                        
                        // For Triggered cards, activate once and set cooldown
                        if (supportComponent.ActivationType == ActivationType.Triggered)
                        {
                            supportComponent.IsActive = true;
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for reactive activations when player is targeted
        /// </summary>
        public void CheckReactiveActivations(Entity entity, Entity attacker, int damage)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Check if it's a Reactive card
                if (supportComponent.ActivationType == ActivationType.Reactive &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Additional context for reactive conditions
                    Dictionary<string, object> context = new Dictionary<string, object>
                    {
                        { "Attacker", attacker },
                        { "Damage", damage }
                    };

                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, context))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Reactive support card: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Apply the effect - for reactive cards, usually target the attacker
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(attacker);
                        }
                        
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }

        /// <summary>
        /// Check for transformative activations
        /// </summary>
        public void CheckTransformativeActivations(Entity entity)
        {
            var supportCards = GetSupportCards();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();
                if (supportComponent == null) continue;

                // Check if it's a Transformative card
                if (supportComponent.ActivationType == ActivationType.Transformative &&
                    supportComponent.CurrentCooldown <= 0 &&
                    !_activatedCardsThisTurn.Contains(supportCard))
                {
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Log activation
                        CardInfoComponent cardInfo = supportCard.GetComponent<CardInfoComponent>();
                        Debug.Log($"Activating Transformative support card: {(cardInfo != null ? cardInfo.Name : "Unknown")}");
                        
                        // Apply the effect
                        if (supportComponent.Effect != null)
                        {
                            supportComponent.Effect.Apply(entity);
                        }
                        
                        supportComponent.IsActive = true;
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        _activatedCardsThisTurn.Add(supportCard);
                    }
                }
            }
        }
        
        /// <summary>
        /// Get all support cards in the support zone
        /// </summary>
        private List<Entity> GetSupportCards()
        {
            // This would ideally be retrieved from the CardSystem
            // For now, we'll just query all entities with SupportCardComponent
            return new List<Entity>(EntityManager.GetEntitiesWithComponents<SupportCardComponent>());
        }

        /// <summary>
        /// Determine if a condition is a turn start condition
        /// </summary>
        private bool IsTurnStartCondition(ActivationCondition condition)
        {
            // This would need proper implementation based on the type of condition
            // For now, just a placeholder
            return condition.GetType().Name.Contains("TurnStart");
        }

        /// <summary>
        /// Determine if a condition is a turn end condition
        /// </summary>
        private bool IsTurnEndCondition(ActivationCondition condition)
        {
            // This would need proper implementation based on the type of condition
            // For now, just a placeholder
            return condition.GetType().Name.Contains("TurnEnd");
        }
    }
}