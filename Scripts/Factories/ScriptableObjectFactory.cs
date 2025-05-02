using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Data;
using UnityEngine;

namespace Factories
{
    /// <summary>
    /// Factory for creating entities from ScriptableObjects
    /// Implements the Singleton pattern
    /// </summary>
    public class ScriptableObjectFactory : PureSingleton<ScriptableObjectFactory>
    {
        // Reference to EntityManager
        private EntityManager _entityManager;
        
        // Cache for created support card effects
        private Dictionary<string, Effect> _effectCache = new Dictionary<string, Effect>();
        
        // Cache for created activation conditions
        private Dictionary<string, ActivationCondition> _conditionCache = new Dictionary<string, ActivationCondition>();
        
        /// <summary>
        /// Initialize the factory with entity manager
        /// </summary>
        public bool Initialize(EntityManager entityManager)
        {
            if (entityManager == null)
            {
                Debug.LogError("[ScriptableObjectFactory] Cannot initialize with null EntityManager!");
                return false;
            }
            
            _entityManager = entityManager;
            
            Debug.Log("[ScriptableObjectFactory] Initialized successfully");
            return base.Initialize();
        }
        
        /// <summary>
        /// Create entity from ScriptableObject
        /// </summary>
        public Entity CreateCardFromSO(CardDataSO cardData)
        {
            if (cardData == null)
            {
                Debug.LogError("[ScriptableObjectFactory] Cannot create card - CardDataSO is null!");
                return null;
            }
            
            // Check if entity manager is initialized
            if (_entityManager == null)
            {
                Debug.LogError("[ScriptableObjectFactory] Cannot create card - EntityManager is not initialized!");
                return null;
            }
            
            // Create entity based on card type
            switch (cardData.cardType)
            {
                case CardType.ElementalCard:
                    return CreateElementalCard(cardData as ElementalCardDataSO ?? cardData);
                case CardType.DivineBeast:
                case CardType.Monster:
                case CardType.SpiritAnimal:
                case CardType.Joker:
                    if (cardData is SupportCardDataSO supportCardData)
                    {
                        return CreateSupportCard(supportCardData);
                    }
                    else
                    {
                        Debug.LogWarning($"[ScriptableObjectFactory] Expected SupportCardDataSO for {cardData.cardType} but got {cardData.GetType()}");
                        return CreateElementalCard(cardData); // Fallback
                    }
                default:
                    Debug.LogWarning($"[ScriptableObjectFactory] Unknown card type: {cardData.cardType}");
                    return CreateElementalCard(cardData); // Fallback
            }
        }
        
        /// <summary>
        /// Create an elemental card entity
        /// </summary>
        private Entity CreateElementalCard(ElementalCardDataSO cardData)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Id = cardData.cardId,
                KeyName = cardData.cardKeyName,
                Name = cardData.cardName,
                Description = cardData.description,
                Type = cardData.cardType,
                Rarity = cardData.rarity,
                Cost = cardData.cost,
                Artwork = cardData.artwork,
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = cardData.elementType
            };
            card.AddComponent(element);
            
            // Add NapAmComponent
            string napAmName = GetNapAmName(cardData.elementType, cardData.napAmIndex);
            NapAmComponent napAm = new NapAmComponent(cardData.elementType, cardData.napAmIndex);
            card.AddComponent(napAm);
            
            // Add StatsComponent
            StatsComponent stats = new StatsComponent
            {
                Attack = cardData.attack,
                Defense = cardData.defense,
                Health = cardData.health,
                MaxHealth = cardData.health,
                Speed = cardData.speed
            };
            card.AddComponent(stats);
            
            return card;
        }
        
        /// <summary>
        /// Create a support card entity
        /// </summary>
        private Entity CreateSupportCard(SupportCardDataSO cardData)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Id = cardData.cardId,
                KeyName = cardData.cardKeyName,
                Name = cardData.cardName,
                Description = cardData.description,
                Type = cardData.cardType,
                Rarity = cardData.rarity,
                Cost = cardData.cost,
                Artwork = cardData.artwork,
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = cardData.elementType
            };
            card.AddComponent(element);
            
            // Add StatsComponent (for display purposes)
            StatsComponent stats = new StatsComponent
            {
                Attack = cardData.attack,
                Defense = cardData.defense,
                Health = cardData.health,
                MaxHealth = cardData.health,
                Speed = cardData.speed
            };
            card.AddComponent(stats);
            
            // Create activation condition
            ActivationCondition condition = CreateActivationCondition(cardData);
            
            // Create effect
            Effect effect = CreateEffect(cardData);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = cardData.activationType,
                Condition = condition,
                Effect = effect,
                CooldownTime = cardData.cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create activation condition from data
        /// </summary>
        private ActivationCondition CreateActivationCondition(SupportCardDataSO data)
        {
            // Get activation condition description
            string description = !string.IsNullOrEmpty(data.activationConditionDescription) 
                ? data.activationConditionDescription 
                : $"When {data.conditionType} condition is met";
            
            // Cache key
            string cacheKey = $"{data.conditionType}_{data.conditionParameter}_{data.conditionValue}";
            
            // Return from cache if exists
            if (_conditionCache.TryGetValue(cacheKey, out var cachedCondition))
            {
                return cachedCondition;
            }
            
            // Create condition based on type
            ActivationCondition condition = null;
            
            switch (data.conditionType)
            {
                case ActivationConditionType.HealthPercent:
                    bool belowThreshold = data.conditionParameter == "below" || string.IsNullOrEmpty(data.conditionParameter);
                    condition = new HealthPercentCondition(data.conditionValue, belowThreshold, description);
                    break;
                    
                case ActivationConditionType.ElementType:
                    ElementType elementType = ParseElementType(data.conditionParameter);
                    condition = new ElementTypeCondition(elementType, description);
                    break;
                    
                case ActivationConditionType.ElementCount:
                    ElementType countElementType = ParseElementType(data.conditionParameter);
                    condition = new ElementCountCondition(countElementType, (int)data.conditionValue, description);
                    break;
                    
                case ActivationConditionType.AllElements:
                    condition = new AllElementsInHandCondition(description);
                    break;
                    
                case ActivationConditionType.AllElementsPlayed:
                    condition = new AllElementsPlayedCondition(description);
                    break;
                    
                case ActivationConditionType.Threshold:
                    ThresholdType thresholdType = ParseThresholdType(data.conditionParameter);
                    condition = new ThresholdCondition(data.conditionValue, thresholdType, description);
                    break;
                    
                case ActivationConditionType.HandSize:
                    bool greaterThan = data.conditionParameter == "greater" || string.IsNullOrEmpty(data.conditionParameter);
                    condition = new HandSizeCondition((int)data.conditionValue, greaterThan, description);
                    break;
                    
                case ActivationConditionType.EffectTargeted:
                    condition = new EffectTargetedCondition(description);
                    break;
                    
                case ActivationConditionType.DamageDealt:
                    condition = new DamageDealtCondition(description);
                    break;
                    
                case ActivationConditionType.ElementCombo:
                    condition = new ElementComboCondition((int)data.conditionValue, description);
                    break;
                    
                case ActivationConditionType.None:
                default:
                    // Return null for no condition (always active)
                    return null;
            }
            
            // Add to cache
            if (condition != null)
            {
                _conditionCache[cacheKey] = condition;
            }
            
            return condition;
        }
        
        /// <summary>
        /// Create effect from data
        /// </summary>
        private Effect CreateEffect(SupportCardDataSO data)
        {
            // Get effect description
            string description = !string.IsNullOrEmpty(data.effectDescription) 
                ? data.effectDescription 
                : $"{data.effectType} effect";
            
            // Cache key
            string cacheKey = $"{data.effectType}_{data.effectParameter}_{data.effectValue}_{data.effectValue2}_{data.effectDuration}";
            
            // Return from cache if exists
            if (_effectCache.TryGetValue(cacheKey, out var cachedEffect))
            {
                return cachedEffect;
            }
            
            // Create effect based on type
            Effect effect = null;
            
            switch (data.effectType)
            {
                case EffectType.StatBuff:
                    effect = new StatBuffEffect(
                        data.cardName + " Effect",
                        description,
                        data.effectDuration,
                        data.effectParameter,
                        data.effectValue
                    );
                    break;
                    
                case EffectType.DamageOverTime:
                    effect = new DotEffect(
                        data.cardName + " DoT",
                        description,
                        data.effectDuration,
                        (int)data.effectValue
                    );
                    break;
                    
                case EffectType.LifeSteal:
                    effect = new LifeStealEffect(
                        data.cardName + " Life Steal",
                        description,
                        data.effectValue,
                        data.effectValue2
                    );
                    break;
                    
                case EffectType.Reflection:
                    effect = new ReflectionEffect(
                        data.cardName + " Reflection",
                        description,
                        data.effectValue,
                        data.effectValue2
                    );
                    break;
                    
                case EffectType.Harmony:
                    effect = new HarmonyEffect(
                        data.cardName + " Harmony",
                        description,
                        data.effectValue
                    );
                    break;
                    
                case EffectType.Replay:
                    effect = new ReplayEffect(
                        data.cardName + " Replay",
                        description
                    );
                    break;
                    
                case EffectType.ElementUnity:
                    effect = new ElementUnityEffect(
                        data.cardName + " Unity",
                        description,
                        data.effectDuration,
                        data.effectValue
                    );
                    break;
                    
                case EffectType.ElementReversal:
                    effect = new ElementReversalEffect(
                        data.cardName + " Reversal",
                        description,
                        data.effectDuration
                    );
                    break;
                    
                case EffectType.Lightning:
                    effect = new LightningEffect(
                        data.cardName + " Lightning",
                        description,
                        data.effectDuration,
                        data.effectValue,
                        (int)data.effectValue2,
                        0.05f, // Default stun chance
                        1      // Default stun duration
                    );
                    break;
                    
                case EffectType.Thunder:
                    effect = new ThunderEffect(
                        data.cardName + " Thunder",
                        description,
                        data.effectDuration,
                        (int)data.effectValue,
                        data.effectValue2,
                        1 // Default stun duration
                    );
                    break;
                    
                case EffectType.Flood:
                    effect = new FloodEffect(
                        data.cardName + " Flood",
                        description,
                        data.effectDuration,
                        (int)data.effectValue,
                        (int)data.effectValue2
                    );
                    break;
                    
                case EffectType.Charm:
                    effect = new CharmEffect(
                        data.cardName + " Charm",
                        description,
                        data.effectDuration,
                        data.effectValue,
                        (int)data.effectValue2
                    );
                    break;
                    
                case EffectType.Complex:
                    // For complex effects, create a container
                    effect = new ComplexEffect(
                        data.cardName + " Complex",
                        description,
                        data.effectDuration
                    );
                    break;
                    
                case EffectType.None:
                default:
                    // Return a default effect that does nothing
                    effect = new StatBuffEffect(
                        data.cardName + " Effect",
                        "No effect",
                        0,
                        "none",
                        0
                    );
                    break;
            }
            
            // Add to cache
            if (effect != null)
            {
                _effectCache[cacheKey] = effect;
            }
            
            return effect;
        }
        
        /// <summary>
        /// Get the NapAm name based on element type and index
        /// </summary>
        private string GetNapAmName(ElementType elementType, int napAmIndex)
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    return GetMetalNapAmName(napAmIndex);
                case ElementType.Wood:
                    return GetWoodNapAmName(napAmIndex);
                case ElementType.Water:
                    return GetWaterNapAmName(napAmIndex);
                case ElementType.Fire:
                    return GetFireNapAmName(napAmIndex);
                case ElementType.Earth:
                    return GetEarthNapAmName(napAmIndex);
                default:
                    return "Unknown";
            }
        }
        
        /// <summary>
        /// Get Metal NapAm name
        /// </summary>
        private string GetMetalNapAmName(int index)
        {
            string[] names = {
                "Sword Qi",
                "Hardness",
                "Purity",
                "Reflection",
                "Spirit",
                "Calmness"
            };
            
            return (index >= 0 && index < names.Length) ? names[index] : "Unknown";
        }
        
        /// <summary>
        /// Get Wood NapAm name
        /// </summary>
        private string GetWoodNapAmName(int index)
        {
            string[] names = {
                "Growth",
                "Flexibility",
                "Symbiosis",
                "Regeneration",
                "Toxin",
                "Shelter"
            };
            
            return (index >= 0 && index < names.Length) ? names[index] : "Unknown";
        }
        
        /// <summary>
        /// Get Water NapAm name
        /// </summary>
        private string GetWaterNapAmName(int index)
        {
            string[] names = {
                "Adaptation",
                "Ice",
                "Flow",
                "Mist",
                "Reflection",
                "Purification"
            };
            
            return (index >= 0 && index < names.Length) ? names[index] : "Unknown";
        }
        
        /// <summary>
        /// Get Fire NapAm name
        /// </summary>
        private string GetFireNapAmName(int index)
        {
            string[] names = {
                "Burning",
                "Explosion",
                "Passion",
                "Light",
                "Forging",
                "Incineration"
            };
            
            return (index >= 0 && index < names.Length) ? names[index] : "Unknown";
        }
        
        /// <summary>
        /// Get Earth NapAm name
        /// </summary>
        private string GetEarthNapAmName(int index)
        {
            string[] names = {
                "Solidity",
                "Gravity",
                "Fertility",
                "Volcano",
                "Crystal",
                "Terra"
            };
            
            return (index >= 0 && index < names.Length) ? names[index] : "Unknown";
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
        /// Clear caches
        /// </summary>
        public void ClearCaches()
        {
            _effectCache.Clear();
            _conditionCache.Clear();
            Debug.Log("[ScriptableObjectFactory] Caches cleared");
        }
        
        /// <summary>
        /// Cleanup resources
        /// </summary>
        public override void Cleanup()
        {
            ClearCaches();
            _entityManager = null;
            base.Cleanup();
        }
    }
}