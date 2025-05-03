// File: Scripts/Factories/ScriptableObjectFactory.cs
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
                    return CreateElementalCard(cardData as ElementalCardDataSO);
                case CardType.SupportCard:
                    return CreateSupportCard(cardData as SupportCardDataSO);
                default:
                    Debug.LogWarning($"[ScriptableObjectFactory] Unknown card type: {cardData.cardType}");
                    return CreateElementalCard(null); // Fallback
            }
        }
        
        /// <summary>
        /// Create an elemental card entity with appropriate Nap Am based on element type
        /// </summary>
        private Entity CreateElementalCard(ElementalCardDataSO cardData)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // If card data is null, create a default card
            if (!cardData)
            {
                // Add default components
                CardInfoComponent defaultInfo = new CardInfoComponent
                {
                    Id = 0,
                    KeyName = "default_card",
                    Name = "Default Card",
                    Description = "A default card created when data was missing",
                    Type = CardType.ElementalCard,
                    Rarity = Rarity.Common,
                    Cost = 1,
                    State = CardState.InDeck
                };
                card.AddComponent(defaultInfo);
                
                ElementComponent defaultElement = new ElementComponent(ElementType.None,0);
                card.AddComponent(defaultElement);
                
                StatsComponent defaultStats = new StatsComponent
                {
                    Attack = 1,
                    Defense = 1,
                    Health = 1,
                    MaxHealth = 1,
                    Speed = 1
                };
                card.AddComponent(defaultStats);
                
                return card;
            }
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
            ElementComponent element = new ElementComponent(cardData.elementType, cardData.napAmIndex);
            card.AddComponent(element);
            
            // switch (cardData.elementType)
            // {
            //     case ElementType.Metal:
            //         napAm = new NapAmComponent(ElementType.Metal, cardData.metalNapAm);
            //         break;
            //     case ElementType.Wood:
            //         napAm = new NapAmComponent(ElementType.Wood, cardData.woodNapAm);
            //         break;
            //     case ElementType.Water:
            //         napAm = new NapAmComponent(ElementType.Water, cardData.waterNapAm);
            //         break;
            //     case ElementType.Fire:
            //         napAm = new NapAmComponent(ElementType.Fire, cardData.fireNapAm);
            //         break;
            //     case ElementType.Earth:
            //         napAm = new NapAmComponent(ElementType.Earth, cardData.earthNapAm);
            //         break;
            //     default:
            //         napAm = new NapAmComponent(ElementType.None, 0);
            //         break;
            // }
            
            // card.AddComponent(napAm);
            
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
            // Implementation remains the same as in original
            // This is just a placeholder - in a real implementation, this would create
            // the appropriate effect based on the data
            return null;
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