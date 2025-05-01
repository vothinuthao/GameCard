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
    /// </summary>
    public class ScriptableObjectFactory
    {
        private EntityManager _entityManager;
        
        // Constructor
        public ScriptableObjectFactory(EntityManager entityManager)
        {
            this._entityManager = entityManager;
        }
        
        /// <summary>
        /// Create an entity from a card scriptable object
        /// </summary>
        public Entity CreateCardFromSO(CardDataSO cardData)
        {
            switch (cardData)
            {
                case ElementalCardDataSO elementalCard:
                    return CreateElementalCardFromSO(elementalCard);
                case DivineBeastCardDataSO divineBeastCard:
                    return CreateDivineBeastCardFromSO(divineBeastCard);
                case MonsterCardDataSO monsterCard:
                    return CreateMonsterCardFromSO(monsterCard);
                case SpiritAnimalCardDataSO spiritAnimalCard:
                    return CreateSpiritAnimalCardFromSO(spiritAnimalCard);
                case JokerCardDataSO jokerCard:
                    return CreateJokerCardFromSO(jokerCard);
                default:
                    return CreateBasicCardFromSO(cardData);
            }
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
        /// Load all card scriptable objects from Resources and create a deck
        /// </summary>
        public List<Entity> CreateDeckFromResources(string folderPath = "Cards")
        {
            List<Entity> deck = new List<Entity>();
            
            // Load all card scriptable objects from Resources
            CardDataSO[] cardDataArray = Resources.LoadAll<CardDataSO>(folderPath);
            
            foreach (var cardData in cardDataArray)
            {
                Entity card = CreateCardFromSO(cardData);
                deck.Add(card);
            }
            
            return deck;
        }
    }
}