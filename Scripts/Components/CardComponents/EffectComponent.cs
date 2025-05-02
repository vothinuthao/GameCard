using System;
using System.Collections.Generic;
using Core;

namespace Components
{
    /// <summary>
    /// Contains effects that can be applied by a card
    /// </summary>
    public class EffectComponent : Component
    {
        // List of effects this card has
        public List<Effect> Effects { get; set; } = new List<Effect>();
        
        // Add an effect
        public void AddEffect(Effect effect)
        {
            Effects.Add(effect);
        }
    }
    
    /// <summary>
    /// Base class for all effects
    /// </summary>
    public abstract class Effect
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; } // In turns, -1 for permanent
        
        // Apply the effect to a target entity
        public abstract void Apply(Entity target);
    }
}