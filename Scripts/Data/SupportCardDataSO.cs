// Data/SupportCardDataSO.cs
using Core.Utils;
using Systems.StatesMachine;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Base class for all support card scriptable objects
    /// </summary>
    [CreateAssetMenu(fileName = "New Support Card", menuName = "NguhanhGame/Support Card Data")]
    public class SupportCardDataSO : CardDataSO
    {
        [Header("Support Card Settings")]
        [Tooltip("The type of support card")]
        public SupportCardType supportCardType;
        
        // Other shared support card properties
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
}