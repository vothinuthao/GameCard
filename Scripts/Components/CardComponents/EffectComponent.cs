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
    
    /// <summary>
    /// Effect: Deal damage over time (DoT)
    /// </summary>
    public class DotEffect : Effect
    {
        public int DamagePerTurn { get; set; }
        
        public DotEffect(string name, string description, int duration, int damagePerTurn)
        {
            Name = name;
            Description = description;
            Duration = duration;
            DamagePerTurn = damagePerTurn;
        }
        
        public override void Apply(Entity target)
        {
            // Implementation would apply the DoT effect to the target
            // In a real implementation, we would add a status effect component to the target
            Console.WriteLine($"Applied {Name} to target, dealing {DamagePerTurn} damage for {Duration} turns");
            
            // Example implementation:
            // StatusEffectComponent statusEffects = target.GetComponent<StatusEffectComponent>();
            // if (statusEffects == null)
            // {
            //     statusEffects = new StatusEffectComponent();
            //     target.AddComponent(statusEffects);
            // }
            // statusEffects.AddStatusEffect(new DotStatusEffect(Name, Description, Duration, DamagePerTurn));
        }
    }
    
    /// <summary>
    /// Effect: Buff a stat
    /// </summary>
    public class StatBuffEffect : Effect
    {
        public string StatName { get; set; }
        public float BuffAmount { get; set; }
        
        public StatBuffEffect(string name, string description, int duration, string statName, float buffAmount)
        {
            Name = name;
            Description = description;
            Duration = duration;
            StatName = statName;
            BuffAmount = buffAmount;
        }
        
        public override void Apply(Entity target)
        {
            Console.WriteLine($"Applied {Name} to target, buffing {StatName} by {BuffAmount} for {Duration} turns");
            
            // Example implementation:
            // StatsComponent stats = target.GetComponent<StatsComponent>();
            // if (stats != null)
            // {
            //     // Apply the buff based on the stat name
            //     switch (StatName.ToLower())
            //     {
            //         case "attack":
            //             stats.Attack += (int)BuffAmount;
            //             break;
            //         case "defense":
            //             stats.Defense += (int)BuffAmount;
            //             break;
            //         // ... other stats
            //     }
            // }
        }
    }
}