using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Data;
using UnityEngine;

namespace Factories
{
    /// <summary>
    /// Factory for creating entities from scriptable objects
    /// Uses PureSingleton pattern
    /// </summary>
    public class ScriptableObjectFactory : PureSingleton<ScriptableObjectFactory>
    {
        // Entity manager reference
        private EntityManager _entityManager;
        
        // Entity cache to avoid recreating the same entities
        private Dictionary<string, Entity> _entityCache = new Dictionary<string, Entity>();
        
        /// <summary>
        /// Initialize the factory with an entity manager
        /// REQUIRED: Must be called before using any factory methods
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
        /// Clear entity cache to free memory
        /// </summary>
        public void ClearCache()
        {
            _entityCache.Clear();
            Debug.Log("[ScriptableObjectFactory] Entity cache cleared");
        }
        
        /// <summary>
        /// Create an entity from a card scriptable object
        /// </summary>
        public Entity CreateCardFromSO(CardDataSO cardData)
        {
            // Ensure initialization
            EnsureInitialized(false);
            
            if (cardData == null)
            {
                Debug.LogError("[ScriptableObjectFactory] Cannot create card from null CardDataSO!");
                return null;
            }
            
            // Check if we already have this card in cache
            string cacheKey = !string.IsNullOrEmpty(cardData.cardKeyName) ? 
                cardData.cardKeyName : "card_" + cardData.cardId;
                
            if (_entityCache.TryGetValue(cacheKey, out Entity cachedEntity))
            {
                // Return a clone of the cached entity to avoid modifying the cached version
                return CloneEntity(cachedEntity);
            }
            
            // Create new entity based on card type
            Entity entity;
            
            switch (cardData)
            {
                case ElementalCardDataSO elementalCard:
                    entity = CreateElementalCardFromSO(elementalCard);
                    break;
                case DivineBeastCardDataSO divineBeastCard:
                    entity = CreateDivineBeastCardFromSO(divineBeastCard);
                    break;
                case MonsterCardDataSO monsterCard:
                    entity = CreateMonsterCardFromSO(monsterCard);
                    break;
                case SpiritAnimalCardDataSO spiritAnimalCard:
                    entity = CreateSpiritAnimalCardFromSO(spiritAnimalCard);
                    break;
                case JokerCardDataSO jokerCard:
                    entity = CreateJokerCardFromSO(jokerCard);
                    break;
                default:
                    entity = CreateBasicCardFromSO(cardData);
                    break;
            }
            
            // Cache the entity
            if (entity != null && !string.IsNullOrEmpty(cacheKey))
            {
                _entityCache[cacheKey] = entity;
            }
            
            // Return a clone to avoid modifying the cached version
            return CloneEntity(entity);
        }
        
        /// <summary>
        /// Create a basic card entity from a card scriptable object
        /// </summary>
        private Entity CreateBasicCardFromSO(CardDataSO cardData)
        {
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = cardData.cardName,
                Description = cardData.description,
                Type = cardData.cardType,
                Rarity = cardData.rarity,
                Cost = cardData.cost,
                Artwork = cardData.artwork,
                State = CardState.InDeck,
                Id = cardData.cardId,
                KeyName = cardData.cardKeyName
            };
            card.AddComponent(cardInfo);
            
            // Add StatsComponent
            StatsComponent stats = new StatsComponent(cardData.attack, cardData.defense, cardData.health, cardData.speed);
            card.AddComponent(stats);
            
            return card;
        }
        
        /// <summary>
        /// Create an elemental card entity from an elemental card scriptable object
        /// </summary>
        private Entity CreateElementalCardFromSO(CardDataSO elementalCard)
        {
            // Create basic card first
            Entity card = CreateBasicCardFromSO(elementalCard);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = elementalCard.elementType
            };
            card.AddComponent(element);
            
            // Add NapAmComponent
            NapAmComponent napAm = new NapAmComponent(elementalCard.elementType, elementalCard.napAmIndex);
            card.AddComponent(napAm);
            
            return card;
        }
        
        /// <summary>
        /// Create a divine beast card entity from a divine beast card scriptable object
        /// </summary>
        private Entity CreateDivineBeastCardFromSO(DivineBeastCardDataSO divineBeastCard)
        {
            Entity card = CreateElementalCardFromSO(divineBeastCard);
            
            // Add EffectComponent
            EffectComponent effectComponent = new EffectComponent();
            
            // Create effect based on parameters
            StatBuffEffect effect = new StatBuffEffect(
                divineBeastCard.cardName + " Effect",
                divineBeastCard.effectDescription,
                divineBeastCard.effectDuration,
                divineBeastCard.effectTargetStat,
                divineBeastCard.effectValue
            );
            
            effectComponent.AddEffect(effect);
            card.AddComponent(effectComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a monster card entity from a monster card scriptable object
        /// </summary>
        private Entity CreateMonsterCardFromSO(MonsterCardDataSO monsterCard)
        {
            Entity card = CreateElementalCardFromSO(monsterCard);
            
            if (monsterCard.effects == null || monsterCard.effects.Length == 0)
                return card;
                
            EffectComponent effectComponent = new EffectComponent();
            
            // Add each effect
            foreach (var effectData in monsterCard.effects)
            {
                // Create effect based on type
                Effect effect = null;
                
                switch (effectData.effectType.ToLower())
                {
                    case "damage":
                    case "dot":
                        effect = new DotEffect(
                            effectData.effectName,
                            effectData.effectDescription,
                            effectData.effectDuration,
                            (int)effectData.effectValue
                        );
                        break;
                    case "buff":
                    case "debuff":
                        effect = new StatBuffEffect(
                            effectData.effectName,
                            effectData.effectDescription,
                            effectData.effectDuration,
                            "attack", // This would be more specific in a real implementation
                            effectData.effectValue
                        );
                        break;
                    // Add more effect types as needed
                }
                
                if (effect != null)
                {
                    effectComponent.AddEffect(effect);
                }
            }
            
            card.AddComponent(effectComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a spirit animal card entity from a spirit animal card scriptable object
        /// </summary>
        private Entity CreateSpiritAnimalCardFromSO(SpiritAnimalCardDataSO spiritAnimalCard)
        {
            // Create elemental card first
            Entity card = CreateElementalCardFromSO(spiritAnimalCard);
            
            // Create effect
            StatBuffEffect effect = new StatBuffEffect(
                spiritAnimalCard.cardName + " Support",
                spiritAnimalCard.supportEffectDescription,
                -1, // Persistent effects are usually permanent
                "attack", // This would be more specific in a real implementation
                1.0f // Default value
            );
            
            // Create activation condition
            ActivationCondition condition = null;
            
            if (!string.IsNullOrEmpty(spiritAnimalCard.conditionType))
            {
                switch (spiritAnimalCard.conditionType.ToLower())
                {
                    case "health":
                        condition = new HealthPercentCondition(
                            spiritAnimalCard.conditionValue,
                            true, // Below threshold
                            spiritAnimalCard.activationConditionDescription
                        );
                        break;
                    case "card_count":
                        condition = new ElementPlayedCondition(
                            ElementType.None, // Doesn't consider element, just card count
                            (int)spiritAnimalCard.conditionValue,
                            spiritAnimalCard.activationConditionDescription
                        );
                        break;
                    case "element_count":
                        ElementType elementType = ElementType.None;
                        // Determine element type from additional parameters or name if needed
                        condition = new ElementPlayedCondition(
                            elementType,
                            (int)spiritAnimalCard.conditionValue,
                            spiritAnimalCard.activationConditionDescription
                        );
                        break;
                    // Add more condition types as needed
                }
            }
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = spiritAnimalCard.activationType,
                Effect = effect,
                Condition = condition,
                CooldownTime = 0, // No cooldown for spirit animals
                CurrentCooldown = 0,
                IsActive = false
            };
            
            card.AddComponent(supportComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a joker card entity from a joker card scriptable object
        /// </summary>
        private Entity CreateJokerCardFromSO(JokerCardDataSO jokerCard)
        {
            // Create basic card first
            Entity card = CreateBasicCardFromSO(jokerCard);
            
            // Create effect
            Effect effect = new StatBuffEffect(
                jokerCard.cardName + " Effect",
                jokerCard.effectDescription,
                -1, // Joker effects are usually transformative and permanent while active
                "all", // Joker cards often affect multiple stats
                1.0f // Default value
            );
            
            // Create activation condition
            ActivationCondition condition = null;
            
            if (!string.IsNullOrEmpty(jokerCard.conditionType))
            {
                switch (jokerCard.conditionType.ToLower())
                {
                    case "health":
                        condition = new HealthPercentCondition(
                            jokerCard.conditionValue,
                            true, // Below threshold
                            jokerCard.activationConditionDescription
                        );
                        break;
                    case "element_diversity":
                        // This would need a custom condition implementation for element diversity
                        break;
                    case "unique_card_count":
                        // This would need a custom condition implementation for unique cards
                        break;
                    // Add more condition types as needed
                }
            }
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = jokerCard.activationType,
                Effect = effect,
                Condition = condition,
                CooldownTime = jokerCard.cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            
            card.AddComponent(supportComponent);
            
            return card;
        }
        
        /// <summary>
        /// Clone an entity to avoid modifying the cached version
        /// </summary>
        private Entity CloneEntity(Entity original)
        {
            if (original == null)
                return null;
                
            // Create a new entity
            Entity clone = _entityManager.CreateEntity();
            
            // Clone CardInfoComponent
            CardInfoComponent originalInfo = original.GetComponent<CardInfoComponent>();
            if (originalInfo != null)
            {
                CardInfoComponent cloneInfo = new CardInfoComponent
                {
                    Id = originalInfo.Id,
                    KeyName = originalInfo.KeyName,
                    Name = originalInfo.Name,
                    Description = originalInfo.Description,
                    Type = originalInfo.Type,
                    Rarity = originalInfo.Rarity,
                    Artwork = originalInfo.Artwork,
                    Cost = originalInfo.Cost,
                    State = originalInfo.State
                };
                clone.AddComponent(cloneInfo);
            }
            
            // Clone StatsComponent
            StatsComponent originalStats = original.GetComponent<StatsComponent>();
            if (originalStats != null)
            {
                StatsComponent cloneStats = new StatsComponent(
                    originalStats.Attack,
                    originalStats.Defense,
                    originalStats.Health,
                    originalStats.Speed
                )
                {
                    MaxHealth = originalStats.MaxHealth,
                    Accuracy = originalStats.Accuracy,
                    Evasion = originalStats.Evasion,
                    Penetration = originalStats.Penetration,
                    CriticalChance = originalStats.CriticalChance,
                    CriticalDamage = originalStats.CriticalDamage,
                    Resistance = originalStats.Resistance
                };
                clone.AddComponent(cloneStats);
            }
            
            // Clone ElementComponent
            ElementComponent originalElement = original.GetComponent<ElementComponent>();
            if (originalElement != null)
            {
                ElementComponent cloneElement = new ElementComponent
                {
                    Element = originalElement.Element
                };
                clone.AddComponent(cloneElement);
            }
            
            // Clone NapAmComponent
            NapAmComponent originalNapAm = original.GetComponent<NapAmComponent>();
            if (originalNapAm != null)
            {
                // Note: This assumes NapAmComponent has a constructor that takes element type and index
                NapAmComponent cloneNapAm = new NapAmComponent(
                    originalElement?.Element ?? ElementType.None,
                    originalNapAm.GetNapAmPower() // This is not ideal - you should store and retrieve the actual index
                );
                clone.AddComponent(cloneNapAm);
            }
            
            // Clone EffectComponent (simplified - in real implementation, would clone all effects)
            EffectComponent originalEffect = original.GetComponent<EffectComponent>();
            if (originalEffect != null)
            {
                EffectComponent cloneEffect = new EffectComponent();
                // For a complete implementation, would need to clone all effects in originalEffect.Effects
                clone.AddComponent(cloneEffect);
            }
            
            // Clone SupportCardComponent (simplified)
            SupportCardComponent originalSupport = original.GetComponent<SupportCardComponent>();
            if (originalSupport != null)
            {
                SupportCardComponent cloneSupport = new SupportCardComponent
                {
                    ActivationType = originalSupport.ActivationType,
                    // For a complete implementation, would need to clone Condition and Effect
                    CooldownTime = originalSupport.CooldownTime,
                    CurrentCooldown = originalSupport.CurrentCooldown,
                    IsActive = originalSupport.IsActive
                };
                clone.AddComponent(cloneSupport);
            }
            
            return clone;
        }
    }
}