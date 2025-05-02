using Core;
using Components;
using UnityEngine;
using System.Collections.Generic;
using Core.Utils;
using RunTime;

namespace Components
{
    /// <summary>
    /// Base class for support card effects
    /// </summary>
    public abstract class SupportCardEffect : Effect
    {
        // Constructor
        public SupportCardEffect(string name, string description, int duration = -1)
        {
            Name = name;
            Description = description;
            Duration = duration;
        }
    }
    
    #region Stat Modification Effects
    
    /// <summary>
    /// Effect that buffs stats
    /// </summary>
    public class StatBuffEffect : SupportCardEffect
    {
        public string StatName { get; set; }
        public float BuffAmount { get; set; }
        
        public StatBuffEffect(string name, string description, int duration, string statName, float buffAmount)
            : base(name, description, duration)
        {
            StatName = statName;
            BuffAmount = buffAmount;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect to {target}: Buffing {StatName} by {BuffAmount}");
            
            StatsComponent stats = target.GetComponent<StatsComponent>();
            if (stats == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target has no StatsComponent");
                return;
            }
            
            // Apply the buff based on the stat name
            switch (StatName.ToLower())
            {
                case "attack":
                    stats.Attack += (int)(stats.Attack * BuffAmount);
                    break;
                case "defense":
                    stats.Defense += (int)(stats.Defense * BuffAmount);
                    break;
                case "health":
                    int healthIncrease = (int)(stats.MaxHealth * BuffAmount);
                    stats.MaxHealth += healthIncrease;
                    stats.Health += healthIncrease;
                    break;
                case "speed":
                    stats.Speed += (int)(stats.Speed * BuffAmount);
                    break;
                case "accuracy":
                    stats.Accuracy += BuffAmount;
                    break;
                case "evasion":
                    stats.Evasion += BuffAmount;
                    break;
                case "penetration":
                    stats.Penetration += BuffAmount;
                    break;
                case "critical":
                    stats.CriticalChance += BuffAmount;
                    break;
                case "elementboost":
                    // This would need to be handled by a special system that tracks element boosts
                    Debug.Log($"Element boost of {BuffAmount * 100}% applied");
                    break;
                default:
                    Debug.LogWarning($"Unknown stat name: {StatName}");
                    break;
            }
        }
    }
    
    /// <summary>
    /// Effect that deals damage over time
    /// </summary>
    public class DotEffect : SupportCardEffect
    {
        public int DamagePerTurn { get; set; }
        
        public DotEffect(string name, string description, int duration, int damagePerTurn)
            : base(name, description, duration)
        {
            DamagePerTurn = damagePerTurn;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect to {target}: {DamagePerTurn} damage per turn for {Duration} turns");
            
            // In a real implementation, we would add a status effect component to the target
            // For now, we'll just simulate the damage application
            
            StatsComponent stats = target.GetComponent<StatsComponent>();
            if (stats == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target has no StatsComponent");
                return;
            }
            
            // Apply immediate damage
            stats.Health -= DamagePerTurn;
            if (stats.Health < 0)
                stats.Health = 0;
            
            // In a real implementation, we would also add a component to track the DoT effect for future turns
            // StatusEffectComponent statusEffects = target.GetComponent<StatusEffectComponent>();
            // if (statusEffects == null)
            // {
            //     statusEffects = new StatusEffectComponent();
            //     target.AddComponent(statusEffects);
            // }
            // statusEffects.AddStatusEffect(new DotStatusEffect(Name, Description, Duration, DamagePerTurn));
        }
    }
    
    #endregion
    
    #region Special Effects
    
    /// <summary>
    /// Effect that provides life steal
    /// </summary>
    public class LifeStealEffect : SupportCardEffect
    {
        public float LifeStealPercentage { get; set; }
        public float AttackBoostPercentage { get; set; }
        
        public LifeStealEffect(string name, string description, int duration, float lifeStealPercentage, float attackBoostPercentage)
            : base(name, description, duration)
        {
            LifeStealPercentage = lifeStealPercentage;
            AttackBoostPercentage = attackBoostPercentage;
        }
        
        public LifeStealEffect(string name, string description, float lifeStealPercentage, float attackBoostPercentage)
            : base(name, description, -1) // -1 means indefinite duration
        {
            LifeStealPercentage = lifeStealPercentage;
            AttackBoostPercentage = attackBoostPercentage;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect: {LifeStealPercentage * 100}% life steal and {AttackBoostPercentage * 100}% attack boost next turn");
            
            // This would need to be handled by a system that tracks attacks and damage
            // For demonstration, we'll simulate a life steal based on the target's attack value
            
            StatsComponent playerStats = GameManager.Instance?.GetPlayerEntity()?.GetComponent<StatsComponent>();
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            
            if (playerStats != null && targetStats != null)
            {
                int damage = targetStats.Attack; // Simulating damage
                int healAmount = (int)(damage * LifeStealPercentage);
                
                // Heal the player
                playerStats.Health += healAmount;
                if (playerStats.Health > playerStats.MaxHealth)
                    playerStats.Health = playerStats.MaxHealth;
                
                // Apply attack boost to player for next turn
                playerStats.Attack += (int)(playerStats.Attack * AttackBoostPercentage);
                
                Debug.Log($"Healed for {healAmount} and boosted attack by {AttackBoostPercentage * 100}%");
            }
        }
    }
    
    /// <summary>
    /// Effect that reflects damage back to the attacker
    /// </summary>
    public class ReflectionEffect : SupportCardEffect
    {
        public float AbsorptionRate { get; set; }
        public float ReflectionRate { get; set; }
        
        public ReflectionEffect(string name, string description, int duration, float absorptionRate, float reflectionRate)
            : base(name, description, duration)
        {
            AbsorptionRate = absorptionRate;
            ReflectionRate = reflectionRate;
        }
        
        public ReflectionEffect(string name, string description, float absorptionRate, float reflectionRate)
            : base(name, description, -1) // -1 means indefinite duration
        {
            AbsorptionRate = absorptionRate;
            ReflectionRate = reflectionRate;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect: Absorbing {AbsorptionRate * 100}% of damage and reflecting {ReflectionRate * 100}% back");
            
            // This would need to be handled by a system that tracks attacks and damage
            // For demonstration, we'll simulate a reflection based on the target's attack value
            
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            if (targetStats != null)
            {
                int damage = targetStats.Attack; // Simulating damage
                int reflectedDamage = (int)(damage * ReflectionRate);
                
                // Apply reflected damage to the target
                targetStats.Health -= reflectedDamage;
                if (targetStats.Health < 0)
                    targetStats.Health = 0;
                
                Debug.Log($"Reflected {reflectedDamage} damage back to attacker");
            }
        }
    }
    
    /// <summary>
    /// Effect that harmonizes all elements
    /// </summary>
    public class HarmonyEffect : SupportCardEffect
    {
        public float EffectivenessBoost { get; set; }
        
        public HarmonyEffect(string name, string description, int duration, float effectivenessBoost)
            : base(name, description, duration)
        {
            EffectivenessBoost = effectivenessBoost;
        }
        
        public HarmonyEffect(string name, string description, float effectivenessBoost)
            : base(name, description, -1) // -1 means indefinite duration
        {
            EffectivenessBoost = effectivenessBoost;
        }
        
        public override void Apply(Entity target)
        {
            Debug.Log($"Applying {Name} effect: {EffectivenessBoost * 100}% boost to all cards");
            
            // This would be handled by a system that tracks element effectiveness
            // For now, we'll boost the player's stats as a demonstration
            
            Entity playerEntity = GameManager.Instance?.GetPlayerEntity();
            if (playerEntity != null)
            {
                StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    playerStats.Attack += (int)(playerStats.Attack * EffectivenessBoost);
                    playerStats.Defense += (int)(playerStats.Defense * EffectivenessBoost);
                    playerStats.Speed += (int)(playerStats.Speed * EffectivenessBoost);
                    
                    Debug.Log($"Boosted player stats by {EffectivenessBoost * 100}%");
                }
            }
        }
    }
    
    /// <summary>
    /// Effect that allows replaying a turn
    /// </summary>
    public class ReplayEffect : SupportCardEffect
    {
        public ReplayEffect(string name, string description, int duration = 1)
            : base(name, description, duration)
        {
        }
        
        public ReplayEffect(string name, string description)
            : base(name, description, 1) // Default duration of 1
        {
        }
        
        public override void Apply(Entity target)
        {
            Debug.Log($"Applying {Name} effect: Allowing replay of the previous turn");
            
            // This would be handled by the battle system to reset the turn state
            // For now, just a demonstration
            
            // GameManager.Instance?.GetBattleSystem()?.ReplayTurn();
            Debug.Log("Turn replay activated");
        }
    }
    
    /// <summary>
    /// Effect for element unity (combining all five elements)
    /// </summary>
    public class ElementUnityEffect : SupportCardEffect
    {
        public float EffectivenessBoost { get; set; }
        
        public ElementUnityEffect(string name, string description, int duration, float effectivenessBoost)
            : base(name, description, duration)
        {
            EffectivenessBoost = effectivenessBoost;
        }
        
        public override void Apply(Entity target)
        {
            Debug.Log($"Applying {Name} effect: {EffectivenessBoost * 100}% boost for {Duration} turns");
            
            // This would be handled by a system that tracks element effectiveness
            // For now, we'll boost the player's stats as a demonstration
            
            Entity playerEntity = GameManager.Instance?.GetPlayerEntity();
            if (playerEntity != null)
            {
                StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
                if (playerStats != null)
                {
                    playerStats.Attack += (int)(playerStats.Attack * EffectivenessBoost);
                    playerStats.Defense += (int)(playerStats.Defense * EffectivenessBoost);
                    playerStats.Speed += (int)(playerStats.Speed * EffectivenessBoost);
                    
                    Debug.Log($"Activated Five Elements Unity, boosting player stats by {EffectivenessBoost * 100}%");
                }
            }
        }
    }
    
    /// <summary>
    /// Effect that reverses element relationships
    /// </summary>
    public class ElementReversalEffect : SupportCardEffect
    {
        public ElementReversalEffect(string name, string description, int duration)
            : base(name, description, duration)
        {
        }
        
        public override void Apply(Entity target)
        {
            Debug.Log($"Applying {Name} effect: Reversing element relationships for {Duration} turns");
            
            // This would be handled by the ElementInteractionSystem
            // For now, just a demonstration
            
            // GameManager.Instance?.GetElementInteractionSystem()?.ReverseElementRelationships(Duration);
            Debug.Log("Element relationships reversed!");
        }
    }
    
    /// <summary>
    /// Effect for creating lightning damage
    /// </summary>
    public class LightningEffect : SupportCardEffect
    {
        public float LightningChance { get; set; }
        public int LightningDamage { get; set; }
        public float StunChance { get; set; }
        public int StunDuration { get; set; }
        
        public LightningEffect(string name, string description, int duration, float lightningChance, int lightningDamage, float stunChance, int stunDuration)
            : base(name, description, duration)
        {
            LightningChance = lightningChance;
            LightningDamage = lightningDamage;
            StunChance = stunChance;
            StunDuration = stunDuration;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            // Check if lightning procs
            if (Random.value <= LightningChance)
            {
                Debug.Log($"Lightning proc! Dealing {LightningDamage} lightning damage to {target}");
                
                StatsComponent targetStats = target.GetComponent<StatsComponent>();
                if (targetStats != null)
                {
                    targetStats.Health -= LightningDamage;
                    if (targetStats.Health < 0)
                        targetStats.Health = 0;
                }
                
                // Check if stun procs
                if (Random.value <= StunChance)
                {
                    Debug.Log($"Stun proc! Target stunned for {StunDuration} turns");
                    
                    // In a real implementation, add a stun status effect
                    // StatusEffectComponent statusEffects = target.GetComponent<StatusEffectComponent>();
                    // if (statusEffects == null)
                    // {
                    //     statusEffects = new StatusEffectComponent();
                    //     target.AddComponent(statusEffects);
                    // }
                    // statusEffects.AddStatusEffect(new StunStatusEffect(Name, "Stunned", StunDuration));
                }
            }
            else
            {
                Debug.Log("Lightning did not proc");
            }
        }
    }
    
    /// <summary>
    /// Effect for thunder damage (similar to lightning but more powerful)
    /// </summary>
    public class ThunderEffect : SupportCardEffect
    {
        public int ThunderDamage { get; set; }
        public float StunChance { get; set; }
        public int StunDuration { get; set; }
        
        public ThunderEffect(string name, string description, int duration, int thunderDamage, float stunChance, int stunDuration)
            : base(name, description, duration)
        {
            ThunderDamage = thunderDamage;
            StunChance = stunChance;
            StunDuration = stunDuration;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect: {ThunderDamage} thunder damage with {StunChance * 100}% stun chance");
            
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            if (targetStats != null)
            {
                targetStats.Health -= ThunderDamage;
                if (targetStats.Health < 0)
                    targetStats.Health = 0;
            }
            
            // Check if stun procs
            if (Random.value <= StunChance)
            {
                Debug.Log($"Stun proc! Target stunned for {StunDuration} turns");
                
                // In a real implementation, add a stun status effect
                // StatusEffectComponent statusEffects = target.GetComponent<StatusEffectComponent>();
                // if (statusEffects == null)
                // {
                //     statusEffects = new StatusEffectComponent();
                //     target.AddComponent(statusEffects);
                // }
                // statusEffects.AddStatusEffect(new StunStatusEffect(Name, "Stunned", StunDuration));
            }
        }
    }
    
    /// <summary>
    /// Effect for flood damage
    /// </summary>
    public class FloodEffect : SupportCardEffect
    {
        public int FloodDamage { get; set; }
        public int SlowDuration { get; set; }
        
        public FloodEffect(string name, string description, int duration, int floodDamage, int slowDuration)
            : base(name, description, duration)
        {
            FloodDamage = floodDamage;
            SlowDuration = slowDuration;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect: {FloodDamage} flood damage and slowing for {SlowDuration} turns");
            
            // Apply damage
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            if (targetStats != null)
            {
                targetStats.Health -= FloodDamage;
                if (targetStats.Health < 0)
                    targetStats.Health = 0;
                
                // Apply slow effect
                targetStats.Speed = (int)(targetStats.Speed * 0.7f); // 30% slow
            }
            
            // In a real implementation, we would also add a slow status effect
            // StatusEffectComponent statusEffects = target.GetComponent<StatusEffectComponent>();
            // if (statusEffects == null)
            // {
            //     statusEffects = new StatusEffectComponent();
            //     target.AddComponent(statusEffects);
            // }
            // statusEffects.AddStatusEffect(new SlowStatusEffect(Name, "Slowed", SlowDuration, 0.3f));
        }
    }
    
    /// <summary>
    /// Effect for charm (making enemies attack their allies)
    /// </summary>
    public class CharmEffect : SupportCardEffect
    {
        public float CharmChance { get; set; }
        public int EnergyReduction { get; set; }
        
        public CharmEffect(string name, string description, int duration, float charmChance, int energyReduction)
            : base(name, description, duration)
        {
            CharmChance = charmChance;
            EnergyReduction = energyReduction;
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying {Name} effect: {CharmChance * 100}% charm chance and reducing energy by {EnergyReduction}");
            
            // This would be handled by the AI system to make enemies attack allies
            // For now, just reduce their stats as a demonstration
            
            StatsComponent targetStats = target.GetComponent<StatsComponent>();
            if (targetStats != null)
            {
                targetStats.Attack = (int)(targetStats.Attack * 0.8f); // 20% attack reduction
                Debug.Log($"Reduced target attack by 20% due to charm");
            }
            
            // In a real implementation, we would also apply an energy reduction
            // EnergyComponent energy = target.GetComponent<EnergyComponent>();
            // if (energy != null)
            // {
            //     energy.CurrentEnergy -= EnergyReduction;
            //     if (energy.CurrentEnergy < 0)
            //         energy.CurrentEnergy = 0;
            // }
        }
    }
    
    /// <summary>
    /// Complex effect that combines multiple effects
    /// </summary>
    public class ComplexEffect : SupportCardEffect
    {
        private List<Effect> _subEffects = new List<Effect>();
        
        public ComplexEffect(string name, string description, int duration)
            : base(name, description, duration)
        {
        }
        
        public void AddSubEffect(Effect effect)
        {
            _subEffects.Add(effect);
        }
        
        public override void Apply(Entity target)
        {
            if (target == null)
            {
                Debug.LogWarning($"Cannot apply {Name} effect: target is null");
                return;
            }
            
            Debug.Log($"Applying complex effect {Name} with {_subEffects.Count} sub-effects");
            
            // Apply all sub-effects
            foreach (var effect in _subEffects)
            {
                effect.Apply(target);
            }
            
            // If no sub-effects, apply default behavior
            if (_subEffects.Count == 0)
            {
                // Example complex effect: Reduce damage by 50% and counter-attack for 8 damage
                StatsComponent targetStats = target.GetComponent<StatsComponent>();
                Entity playerEntity = GameManager.Instance?.GetPlayerEntity();
                
                if (targetStats != null && playerEntity != null)
                {
                    // Increase defense to simulate damage reduction
                    targetStats.Defense += (int)(targetStats.Defense * 0.5f);
                    
                    // Apply counter damage
                    StatsComponent playerStats = playerEntity.GetComponent<StatsComponent>();
                    if (playerStats != null)
                    {
                        Debug.Log($"Complex effect: Reducing damage by 50% and countering for 8 damage");
                        
                        playerStats.Health -= 8;
                        if (playerStats.Health < 0)
                            playerStats.Health = 0;
                    }
                }
            }
        }
    }
    
    #endregion
}

#region Activation Conditions

/// <summary>
/// Condition based on health percentage
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
        if (target == null) return false;
        
        StatsComponent stats = target.GetComponent<StatsComponent>();
        if (stats == null) return false;
        
        float currentPercent = (float)stats.Health / stats.MaxHealth;
        
        if (BelowThreshold)
            return currentPercent <= HealthPercent;
        else
            return currentPercent >= HealthPercent;
    }
}

/// <summary>
/// Condition based on element type
/// </summary>
public class ElementTypeCondition : ActivationCondition
{
    public ElementType TargetElement { get; private set; }
    
    public ElementTypeCondition(ElementType targetElement, string description)
    {
        TargetElement = targetElement;
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // If context is a list of played cards, check if any of them is the target element
        if (context is List<Entity> playedCards)
        {
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element == TargetElement)
                {
                    return true;
                }
            }
        }
        
        return false;
    }
}

/// <summary>
/// Condition based on element count in hand
/// </summary>
public class ElementCountCondition : ActivationCondition
{
    public ElementType TargetElement { get; private set; }
    public int RequiredCount { get; private set; }
    
    public ElementCountCondition(ElementType targetElement, int requiredCount, string description)
    {
        TargetElement = targetElement;
        RequiredCount = requiredCount;
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to check the player's hand
        // For simplicity, we'll assume the condition is met if GameManager exists
        
        var cardSystem = GameManager.Instance?.GetCardSystem();
        if (cardSystem == null) return false;
        
        var hand = cardSystem.GetHand();
        int count = 0;
        
        foreach (var card in hand)
        {
            ElementComponent element = card.GetComponent<ElementComponent>();
            if (element != null && element.Element == TargetElement)
            {
                count++;
            }
        }
        
        return count >= RequiredCount;
    }
}

/// <summary>
/// Condition based on having all five elements in hand
/// </summary>
public class AllElementsInHandCondition : ActivationCondition
{
    public AllElementsInHandCondition(string description)
    {
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to check the player's hand
        // For simplicity, we'll assume the condition is met if GameManager exists
        
        var cardSystem = GameManager.Instance?.GetCardSystem();
        if (cardSystem == null) return false;
        
        var hand = cardSystem.GetHand();
        
        // Create a set to track which elements we've found
        HashSet<ElementType> elementsFound = new HashSet<ElementType>();
        
        foreach (var card in hand)
        {
            ElementComponent element = card.GetComponent<ElementComponent>();
            if (element != null && element.Element != ElementType.None)
            {
                elementsFound.Add(element.Element);
            }
        }
        
        // Check if we found all five elements
        return elementsFound.Count >= 5;
    }
}

/// <summary>
/// Condition based on having used all five elements in battle
/// </summary>
public class AllElementsPlayedCondition : ActivationCondition
{
    public AllElementsPlayedCondition(string description)
    {
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to track which elements have been played
        // For now, we'll assume it's a randomly met condition
        
        return Random.value < 0.1f; // 10% chance of being met for demonstration
    }
}

/// <summary>
/// Condition based on a threshold value (damage, health lost, etc.)
/// </summary>
public class ThresholdCondition : ActivationCondition
{
    public float Threshold { get; private set; }
    public ThresholdType Type { get; private set; }
    
    public ThresholdCondition(float threshold, ThresholdType type, string description)
    {
        Threshold = threshold;
        Type = type;
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        switch (Type)
        {
            case ThresholdType.DamageReceived:
                // Check if context contains damage info
                if (context is Dictionary<string, object> contextDict &&
                    contextDict.TryGetValue("Damage", out object damageObj) &&
                    damageObj is int damage)
                {
                    return damage > Threshold;
                }
                break;
                
            case ThresholdType.HealthLostPercent:
                if (target != null)
                {
                    StatsComponent stats = target.GetComponent<StatsComponent>();
                    if (stats != null)
                    {
                        // Assuming we have some way to track recent health changes
                        float healthLostPercent = 1f - ((float)stats.Health / stats.MaxHealth);
                        return healthLostPercent > Threshold;
                    }
                }
                break;
        }
        
        return false;
    }
}

/// <summary>
/// Threshold types for threshold conditions
/// </summary>
public enum ThresholdType
{
    DamageReceived,
    HealthLostPercent,
    DamageDealt,
    CardsPlayed
}

/// <summary>
/// Condition based on hand size
/// </summary>
public class HandSizeCondition : ActivationCondition
{
    public int Threshold { get; private set; }
    public bool GreaterThan { get; private set; }
    
    public HandSizeCondition(int threshold, bool greaterThan, string description)
    {
        Threshold = threshold;
        GreaterThan = greaterThan;
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to check the enemy's hand size
        // For simplicity, we'll assume it's met if GameManager exists
        
        var cardSystem = GameManager.Instance?.GetCardSystem();
        if (cardSystem == null) return false;
        
        var hand = cardSystem.GetHand();
        
        if (GreaterThan)
            return hand.Count > Threshold;
        else
            return hand.Count < Threshold;
    }
}

/// <summary>
/// Condition based on being targeted by a card effect
/// </summary>
public class EffectTargetedCondition : ActivationCondition
{
    public EffectTargetedCondition(string description)
    {
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to check if the target was targeted by an effect
        // For simplicity, we'll assume it's a randomly met condition
        
        return Random.value < 0.2f; // 20% chance of being met for demonstration
    }
}

/// <summary>
/// Condition based on dealing damage
/// </summary>
public class DamageDealtCondition : ActivationCondition
{
    public DamageDealtCondition(string description)
    {
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to check if damage was dealt
        // For now, assume true if target exists and has StatsComponent
        
        return target != null && target.GetComponent<StatsComponent>() != null;
    }
}

/// <summary>
/// Condition based on playing a combo of cards with the same element
/// </summary>
public class ElementComboCondition : ActivationCondition
{
    public int ComboRequired { get; private set; }
    
    public ElementComboCondition(int comboRequired, string description)
    {
        ComboRequired = comboRequired;
        Description = description;
    }
    
    public override bool IsMet(Entity entity, Entity target, object context)
    {
        // This would need to check if a combo of cards with the same element was played
        // For simplicity, we'll track the same elements in the played cards
        
        if (context is List<Entity> playedCards && playedCards.Count >= ComboRequired)
        {
            Dictionary<ElementType, int> elementCounts = new Dictionary<ElementType, int>();
            
            foreach (var card in playedCards)
            {
                ElementComponent element = card.GetComponent<ElementComponent>();
                if (element != null && element.Element != ElementType.None)
                {
                    if (!elementCounts.ContainsKey(element.Element))
                    {
                        elementCounts[element.Element] = 0;
                    }
                    
                    elementCounts[element.Element]++;
                    
                    // If we have enough cards of the same element, condition is met
                    if (elementCounts[element.Element] >= ComboRequired)
                    {
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
}
#endregion