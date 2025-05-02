using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;

namespace Systems
{
    /// <summary>
    /// System that handles support cards (Thẻ Phụ)
    /// </summary>
    public class SupportCardSystem : Core.System
    {
        // Context for activation conditions
        private List<Entity> _playedCards = new List<Entity>();

        // Constructor
        public SupportCardSystem(EntityManager entityManager) : base(entityManager)
        {
        }

        /// <summary>
        /// Update method - called every frame
        /// </summary>
        public override void Update(float deltaTime)
        {
            // Check for support card activations
            CheckOnEntryActivations();
        }

        /// <summary>
        /// Add a played card to the context
        /// </summary>
        public void AddPlayedCard(Entity card)
        {
            _playedCards.Add(card);
        }

        /// <summary>
        /// Clear the played cards context
        /// </summary>
        public void ClearPlayedCards()
        {
            _playedCards.Clear();
        }

        /// <summary>
        /// Check for OnEntry activation type support cards
        /// </summary>
        private void CheckOnEntryActivations()
        {
            // Get all support cards
            var supportCards = EntityManager.GetEntitiesWithComponents<SupportCardComponent>();

            foreach (var entity in supportCards)
            {
                var supportCard = entity.GetComponent<SupportCardComponent>();

                // Check if it's an OnEntry card and not already active
                if (supportCard.ActivationType == ActivationType.OnEntry && !supportCard.IsActive &&
                    supportCard.CurrentCooldown <= 0)
                {
                    // Activate the card
                    supportCard.Effect.Apply(null); 
                    supportCard.IsActive = true;
                    supportCard.CurrentCooldown = supportCard.CooldownTime;
                }
            }
        }

        /// <summary>
        /// Check for support card activations based on health
        /// </summary>
        public void CheckHealthBasedActivations(Entity entity)
        {
            // Get all support cards
            var supportCards = EntityManager.GetEntitiesWithComponents<SupportCardComponent>();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();

                // Check if it's a Triggered card with a health condition
                if (supportComponent.ActivationType == ActivationType.Triggered &&
                    supportComponent.Condition is HealthPercentCondition &&
                    !supportComponent.IsActive &&
                    supportComponent.CurrentCooldown <= 0)
                {
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, entity, null))
                    {
                        // Activate the card
                        supportComponent.Effect.Apply(entity);
                        supportComponent.IsActive = true;
                        supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                    }
                }
            }
        }

        /// <summary>
        /// Check for element based activations
        /// </summary>
        public void CheckElementBasedActivations()
        {
            // Get all support cards
            var supportCards = EntityManager.GetEntitiesWithComponents<SupportCardComponent>();

            foreach (var supportCard in supportCards)
            {
                var supportComponent = supportCard.GetComponent<SupportCardComponent>();

                // Check if it's a Recurring card with an element condition
                if (supportComponent.ActivationType == ActivationType.Recurring &&
                    supportComponent.Condition is ElementPlayedCondition &&
                    supportComponent.CurrentCooldown <= 0)
                {
                    // Check if the condition is met
                    if (supportComponent.Condition.IsMet(supportCard, null, _playedCards))
                    {
                        // Activate the card
                        // In a real implementation, we'd need a proper way to determine the target
                        var target = _playedCards.Count > 0 ? _playedCards[0] : null;

                        if (target != null)
                        {
                            supportComponent.Effect.Apply(target);
                            supportComponent.CurrentCooldown = supportComponent.CooldownTime;
                        }
                    }
                }
            }
        }
    }
}