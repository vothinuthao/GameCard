using System.Collections.Generic;
using Core;
using Core.Utils;

namespace Components
{
    /// <summary>
    /// Component for cards that can be played as support cards (Thẻ Phụ)
    /// </summary>
    public class SupportCardComponent : Component
    {
        public ActivationType ActivationType { get; set; }
        public ActivationCondition Condition { get; set; }
        public Effect Effect { get; set; }
        public int CooldownTime { get; set; }
        public int CurrentCooldown { get; set; }
        public bool IsActive { get; set; }
    }
    
    /// <summary>
    /// Base class for all activation conditions
    /// </summary>
    public abstract class ActivationCondition
    {
        public string Description { get; set; }
        
        // Check if the condition is met
        public abstract bool IsMet(Entity entity, Entity target, object context);
    }
    
    /// <summary>
    /// Condition: Based on health percentage
    /// </summary>
    public class HealthPercentCondition : ActivationCondition
    {
        public float HealthPercent { get; private set; }
        public bool BelowThreshold { get; private set; }
        
        public HealthPercentCondition(float healthPercent, bool belowThreshold, string description)
        {
            HealthPercent = healthPercent;
            BelowThreshold = belowThreshold;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            StatsComponent stats = target.GetComponent<StatsComponent>();
            if (stats == null)
                return false;
            
            float currentPercent = (float)stats.Health / stats.MaxHealth;
            
            if (BelowThreshold)
                return currentPercent <= HealthPercent;
            else
                return currentPercent >= HealthPercent;
        }
    }
    
    /// <summary>
    /// Condition: Based on played cards of a specific element
    /// </summary>
    public class ElementPlayedCondition : ActivationCondition
    {
        public ElementType Element { get; private set; }
        public int Count { get; private set; }
        
        public ElementPlayedCondition(ElementType element, int count, string description)
        {
            Element = element;
            Count = count;
            Description = description;
        }
        
        public override bool IsMet(Entity entity, Entity target, object context)
        {
            // Context should be a list of played cards this turn
            if (!(context is List<Entity> playedCards))
                return false;
            
            int elementCount = 0;
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element == Element)
                    elementCount++;
            }
            
            return elementCount >= Count;
        }
    }
}