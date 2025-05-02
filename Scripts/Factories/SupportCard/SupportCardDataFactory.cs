using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Data;
using UnityEngine;

namespace Factories
{
    /// <summary>
    /// Factory for creating support cards from scriptable object data
    /// </summary>
    public class SupportCardDataFactory
    {
        // Entity manager reference
        private EntityManager _entityManager;
        
        // Constructor
        public SupportCardDataFactory(EntityManager entityManager)
        {
            _entityManager = entityManager;
        }
        
        /// <summary>
        /// Create a support card from scriptable object data
        /// </summary>
        public Entity CreateSupportCard(SupportCardDataSO data)
        {
            if (data == null)
            {
                Debug.LogError("Cannot create support card: Data is null!");
                return null;
            }
            
            // Create entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Id = data.cardId,
                KeyName = data.cardKeyName,
                Name = data.cardName,
                Description = data.description,
                Type = GetCardTypeFromSupportCardType(data.supportCardType),
                Rarity = data.rarity,
                Cost = data.cost,
                Artwork = data.artwork,
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = data.elementType
            };
            card.AddComponent(element);
            
            // Create activation condition
            ActivationCondition condition = CreateActivationCondition(data);
            
            // Create effect
            Effect effect = CreateEffect(data);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = data.activationType,
                Condition = condition,
                Effect = effect,
                CooldownTime = data.cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            // Add StatsComponent for display purposes
            StatsComponent stats = new StatsComponent
            {
                Attack = GetStatForRarity(data.rarity, 3, 5, 7, 10),
                Defense = GetStatForRarity(data.rarity, 3, 5, 7, 10),
                Health = GetStatForRarity(data.rarity, 10, 15, 20, 30),
                MaxHealth = GetStatForRarity(data.rarity, 10, 15, 20, 30),
                Speed = GetStatForRarity(data.rarity, 3, 4, 5, 6)
            };
            card.AddComponent(stats);
            
            return card;
        }
        
        /// <summary>
        /// Create multiple support cards from scriptable object data
        /// </summary>
        public List<Entity> CreateSupportCards(List<SupportCardDataSO> dataList)
        {
            List<Entity> cards = new List<Entity>();
            
            foreach (var data in dataList)
            {
                Entity card = CreateSupportCard(data);
                if (card != null)
                {
                    cards.Add(card);
                }
            }
            
            return cards;
        }
        
        /// <summary>
        /// Create an activation condition from data
        /// </summary>
        private ActivationCondition CreateActivationCondition(SupportCardDataSO data)
        {
            // Get activation condition description
            string description = !string.IsNullOrEmpty(data.activationConditionDescription) 
                ? data.activationConditionDescription 
                : $"When {data.conditionType} condition is met";
            
            // Create condition based on type
            switch (data.conditionType)
            {
                case ActivationConditionType.HealthPercent:
                    bool belowThreshold = data.conditionParameter == "below" || string.IsNullOrEmpty(data.conditionParameter);
                    return new HealthPercentCondition(data.conditionValue, belowThreshold, description);
                    
                case ActivationConditionType.ElementType:
                    ElementType elementType = ParseElementType(data.conditionParameter);
                    return new ElementTypeCondition(elementType, description);
                    
                case ActivationConditionType.ElementCount:
                    ElementType countElementType = ParseElementType(data.conditionParameter);
                    return new ElementCountCondition(countElementType, (int)data.conditionValue, description);
                    
                case ActivationConditionType.AllElements:
                    return new AllElementsInHandCondition(description);
                    
                case ActivationConditionType.AllElementsPlayed:
                    return new AllElementsPlayedCondition(description);
                    
                case ActivationConditionType.Threshold:
                    ThresholdType thresholdType = ParseThresholdType(data.conditionParameter);
                    return new ThresholdCondition(data.conditionValue, thresholdType, description);
                    
                case ActivationConditionType.HandSize:
                    bool greaterThan = data.conditionParameter == "greater" || string.IsNullOrEmpty(data.conditionParameter);
                    return new HandSizeCondition((int)data.conditionValue, greaterThan, description);
                    
                case ActivationConditionType.EffectTargeted:
                    return new EffectTargetedCondition(description);
                    
                case ActivationConditionType.DamageDealt:
                    return new DamageDealtCondition(description);
                    
                case ActivationConditionType.ElementCombo:
                    return new ElementComboCondition((int)data.conditionValue, description);
                    
                case ActivationConditionType.None:
                default:
                    // Return null for no condition (always active)
                    return null;
            }
        }
        
        /// <summary>
        /// Create an effect from data
        /// </summary>
        private Effect CreateEffect(SupportCardDataSO data)
        {
            // Get effect description
            string description = !string.IsNullOrEmpty(data.effectDescription) 
                ? data.effectDescription 
                : $"{data.effectType} effect";
            
            // Create effect based on type
            switch (data.effectType)
            {
                case EffectType.StatBuff:
                    return new StatBuffEffect(
                        data.cardName + " Effect",
                        description,
                        data.effectDuration,
                        data.effectParameter,
                        data.effectValue
                    );
                    
                case EffectType.DamageOverTime:
                    return new DotEffect(
                        data.cardName + " DoT",
                        description,
                        data.effectDuration,
                        (int)data.effectValue
                    );
                    
                case EffectType.LifeSteal:
                    return new LifeStealEffect(
                        data.cardName + " Life Steal",
                        description,
                        data.effectDuration,
                        data.effectValue,
                        data.effectValue2
                    );
                    
                case EffectType.Reflection:
                    return new ReflectionEffect(
                        data.cardName + " Reflection",
                        description,
                        data.effectDuration,
                        data.effectValue,
                        data.effectValue2
                    );
                    
                case EffectType.Harmony:
                    return new HarmonyEffect(
                        data.cardName + " Harmony",
                        description,
                        data.effectDuration,
                        data.effectValue
                    );
                    
                case EffectType.Replay:
                    return new ReplayEffect(
                        data.cardName + " Replay",
                        description,
                        data.effectDuration
                    );
                    
                case EffectType.ElementUnity:
                    return new ElementUnityEffect(
                        data.cardName + " Unity",
                        description,
                        data.effectDuration,
                        data.effectValue
                    );
                    
                case EffectType.ElementReversal:
                    return new ElementReversalEffect(
                        data.cardName + " Reversal",
                        description,
                        data.effectDuration
                    );
                    
                case EffectType.Lightning:
                    return new LightningEffect(
                        data.cardName + " Lightning",
                        description,
                        data.effectDuration,
                        data.effectValue,
                        (int)data.effectValue2,
                        0.05f, // Default stun chance
                        1      // Default stun duration
                    );
                    
                case EffectType.Thunder:
                    return new ThunderEffect(
                        data.cardName + " Thunder",
                        description,
                        data.effectDuration,
                        (int)data.effectValue,
                        data.effectValue2,
                        (int)ParseEffectExtraParameter(data.effectParameter, 1)
                    );
                    
                case EffectType.Flood:
                    return new FloodEffect(
                        data.cardName + " Flood",
                        description,
                        data.effectDuration,
                        (int)data.effectValue,
                        (int)data.effectValue2
                    );
                    
                case EffectType.Charm:
                    return new CharmEffect(
                        data.cardName + " Charm",
                        description,
                        data.effectDuration,
                        data.effectValue,
                        (int)data.effectValue2
                    );
                    
                case EffectType.Complex:
                    // For complex effects, create a container and add sub-effects if needed
                    ComplexEffect complexEffect = new ComplexEffect(
                        data.cardName + " Complex",
                        description,
                        data.effectDuration
                    );
                    
                    // In a real implementation, you would parse and add sub-effects here
                    
                    return complexEffect;
                    
                case EffectType.None:
                default:
                    // Return a default effect that does nothing
                    return new StatBuffEffect(
                        data.cardName + " Effect",
                        "No effect",
                        0,
                        "none",
                        0
                    );
            }
        }
        
        #region Helper Methods
        
        /// <summary>
        /// Convert SupportCardType to CardType
        /// </summary>
        private CardType GetCardTypeFromSupportCardType(SupportCardType supportCardType)
        {
            switch (supportCardType)
            {
                case SupportCardType.DivineBeast:
                    return CardType.DivineBeast;
                case SupportCardType.Monster:
                    return CardType.Monster;
                case SupportCardType.SpiritAnimal:
                case SupportCardType.Artifact:
                case SupportCardType.Talisman:
                case SupportCardType.DivineWeapon:
                    // Currently all these types map to SpiritAnimal
                    return CardType.SpiritAnimal;
                default:
                    return CardType.SpiritAnimal;
            }
        }
        
        /// <summary>
        /// Parse element type from string
        /// </summary>
        private ElementType ParseElementType(string elementString)
        {
            if (string.IsNullOrEmpty(elementString))
                return ElementType.None;
                
            switch (elementString.ToLower())
            {
                case "metal":
                case "kim":
                    return ElementType.Metal;
                case "wood":
                case "moc":
                case "mộc":
                    return ElementType.Wood;
                case "water":
                case "thuy":
                case "thủy":
                    return ElementType.Water;
                case "fire":
                case "hoa":
                case "hỏa":
                    return ElementType.Fire;
                case "earth":
                case "tho":
                case "thổ":
                    return ElementType.Earth;
                default:
                    return ElementType.None;
            }
        }
        
        /// <summary>
        /// Parse threshold type from string
        /// </summary>
        private ThresholdType ParseThresholdType(string thresholdString)
        {
            if (string.IsNullOrEmpty(thresholdString))
                return ThresholdType.DamageReceived;
                
            switch (thresholdString.ToLower())
            {
                case "damage":
                case "damagereceived":
                    return ThresholdType.DamageReceived;
                case "health":
                case "healthlost":
                case "healthlostpercent":
                    return ThresholdType.HealthLostPercent;
                case "damagedealt":
                    return ThresholdType.DamageDealt;
                case "cardsplayed":
                    return ThresholdType.CardsPlayed;
                default:
                    return ThresholdType.DamageReceived;
            }
        }
        
        /// <summary>
        /// Parse extra parameters from effect parameter string
        /// </summary>
        private float ParseEffectExtraParameter(string paramString, int index)
        {
            if (string.IsNullOrEmpty(paramString))
                return 0;
                
            // Try to parse parameters split by comma
            string[] parameters = paramString.Split(',');
            
            if (parameters.Length > index && float.TryParse(parameters[index], out float value))
            {
                return value;
            }
            
            return 0;
        }
        
        /// <summary>
        /// Get a stat value based on rarity
        /// </summary>
        private int GetStatForRarity(Rarity rarity, int common, int rare, int epic, int legendary)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    return common;
                case Rarity.Rare:
                    return rare;
                case Rarity.Epic:
                    return epic;
                case Rarity.Legendary:
                    return legendary;
                default:
                    return common;
            }
        }
        
        #endregion
    }
}