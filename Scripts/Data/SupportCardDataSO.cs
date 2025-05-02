using System;
using Core.Utils;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Scriptable object for support card data configuration
    /// </summary>
    [CreateAssetMenu(fileName = "NewSupportCard", menuName = "Ngũ Hành/Support Card")]
    public class SupportCardDataSO : CardDataSO
    {
        [Header("Support Card Settings")]
        [Tooltip("The type of support card (Divine Beast, Monster, etc.)")]
        public SupportCardType supportCardType;
        
        [Tooltip("The activation type determines when the effect is triggered")]
        public ActivationType activationType;
        
        [Tooltip("The activation condition type")]
        public ActivationConditionType conditionType;
        
        [Tooltip("The condition parameter (if applicable)")]
        public string conditionParameter;
        
        [Tooltip("The condition value (if applicable)")]
        public float conditionValue;
        
        [Tooltip("The condition value 2 (if applicable)")]
        public float conditionValue2;
        
        [Tooltip("The effect type")]
        public EffectType effectType;
        
        [Tooltip("The effect parameter (if applicable)")]
        public string effectParameter;
        
        [Tooltip("The effect value (if applicable)")]
        public float effectValue;
        
        [Tooltip("The effect value 2 (if applicable)")]
        public float effectValue2;
        
        [Tooltip("The effect duration in turns (-1 for permanent)")]
        public int effectDuration = -1;
        
        [Tooltip("The cooldown time in turns")]
        public int cooldownTime;
        
        [Tooltip("Detailed effect description")]
        [TextArea(3, 10)]
        public string effectDescription;
        
        [Tooltip("Detailed activation condition description")]
        [TextArea(3, 10)]
        public string activationConditionDescription;
    }
    
    /// <summary>
    /// Enum for support card types
    /// </summary>
    public enum SupportCardType
    {
        DivineBeast,
        Monster,
        SpiritAnimal,
        Artifact,
        Talisman,
        DivineWeapon
    }
    
    /// <summary>
    /// Enum for activation condition types
    /// </summary>
    public enum ActivationConditionType
    {
        None,
        HealthPercent,
        ElementType,
        ElementCount,
        AllElements,
        AllElementsPlayed,
        Threshold,
        HandSize,
        EffectTargeted,
        DamageDealt,
        ElementCombo,
        TurnStart,
        TurnEnd
    }
    
    /// <summary>
    /// Enum for effect types
    /// </summary>
    public enum EffectType
    {
        None,
        StatBuff,
        DamageOverTime,
        LifeSteal,
        Reflection,
        Harmony,
        Replay,
        ElementUnity,
        ElementReversal,
        Lightning,
        Thunder,
        Flood,
        Charm,
        Complex
    }
}