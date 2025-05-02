using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using UnityEngine;

namespace Factories
{
    /// <summary>
    /// Factory specifically for creating support cards (Thẻ Phụ)
    /// </summary>
    public class SupportCardFactory
    {
        // Entity manager reference
        private EntityManager _entityManager;
        
        // Constructor
        public SupportCardFactory(EntityManager entityManager)
        {
            this._entityManager = entityManager;
        }
        
        #region Divine Beast Cards (Thần Thú)
        
        /// <summary>
        /// Create a Divine Beast support card (Thần Thú)
        /// </summary>
        public Entity CreateDivineBeastCard(string name, string description, ElementType elementType, 
                                           Rarity rarity, Effect specialEffect, ActivationType activationType,
                                           ActivationCondition condition = null, int cooldownTime = 0)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.DivineBeast,
                Rarity = rarity,
                Cost = 2, // Divine beasts typically cost more
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = elementType
            };
            card.AddComponent(element);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = activationType,
                Condition = condition,
                Effect = specialEffect,
                CooldownTime = cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            // Add Stats (for display purposes, not used in gameplay)
            StatsComponent stats = new StatsComponent
            {
                Attack = GetStatForRarity(rarity, 3, 5, 7, 10),
                Defense = GetStatForRarity(rarity, 3, 5, 7, 10),
                Health = GetStatForRarity(rarity, 10, 15, 20, 30),
                MaxHealth = GetStatForRarity(rarity, 10, 15, 20, 30),
                Speed = GetStatForRarity(rarity, 3, 4, 5, 6)
            };
            card.AddComponent(stats);
            
            return card;
        }
        
        /// <summary>
        /// Create the Azure Dragon (Thanh Long) divine beast
        /// </summary>
        public Entity CreateAzureDragon()
        {
            // Create wood enhancement effect
            StatBuffEffect woodEnhanceEffect = new StatBuffEffect(
                "Azure Dragon's Blessing",
                "Increases the power of all Wood element cards by 30%",
                -1, // Permanent while active
                "ElementBoost",
                0.3f
            );
            
            // The Azure Dragon's special effect will be to enhance Wood element cards
            ElementTypeCondition woodElementCondition = new ElementTypeCondition(
                ElementType.Wood,
                "When playing a Wood element card"
            );
            
            return CreateDivineBeastCard(
                "Azure Dragon", 
                "The azure dragon of the east, master of wood",
                ElementType.Wood,
                Rarity.Legendary,
                woodEnhanceEffect,
                ActivationType.Persistent,
                woodElementCondition
            );
        }
        
        /// <summary>
        /// Create the White Tiger (Bạch Hổ) divine beast
        /// </summary>
        public Entity CreateWhiteTiger()
        {
            // Create defensive effect
            ComplexEffect defensiveEffect = new ComplexEffect(
                "White Tiger's Protection",
                "Reduces damage taken by 50% for 2 turns, then deals 8 damage to the attacker",
                2
            );

            // Condition: When taking large damage
            ThresholdCondition damageCondition = new ThresholdCondition(
                10, // Threshold value
                ThresholdType.DamageReceived,
                "When receiving more than 10 damage in a single attack"
            );
            
            return CreateDivineBeastCard(
                "White Tiger",
                "The white tiger of the west, master of metal",
                ElementType.Metal,
                Rarity.Legendary,
                defensiveEffect,
                ActivationType.Triggered,
                damageCondition,
                3 // Cooldown
            );
        }
        
        /// <summary>
        /// Create the Vermilion Bird (Chu Tước) divine beast
        /// </summary>
        public Entity CreateVermilionBird()
        {
            // Create fire damage over time effect
            DotEffect burnEffect = new DotEffect(
                "Vermilion Bird's Flame",
                "Deals 3 additional fire damage over 2 turns",
                2,
                3
            );
            
            // Condition: When playing a Fire card
            ElementTypeCondition fireElementCondition = new ElementTypeCondition(
                ElementType.Fire,
                "When playing a Fire element card"
            );
            
            return CreateDivineBeastCard(
                "Vermilion Bird",
                "The vermilion bird of the south, master of fire",
                ElementType.Fire,
                Rarity.Legendary,
                burnEffect,
                ActivationType.Recurring,
                fireElementCondition,
                1 // Cooldown
            );
        }
        
        /// <summary>
        /// Create the Black Turtle (Huyền Vũ) divine beast
        /// </summary>
        public Entity CreateBlackTurtle()
        {
            // Create reflection effect
            ReflectionEffect reflectionEffect = new ReflectionEffect(
                "Black Turtle's Shell",
                "Absorbs 60% of damage and reflects 30% back to the attacker",
                0.6f, // Absorption rate
                0.3f  // Reflection rate
            );
            
            // Condition: When targeted by an effect-based attack
            EffectTargetedCondition effectCondition = new EffectTargetedCondition(
                "When targeted by a card effect"
            );
            
            return CreateDivineBeastCard(
                "Black Turtle",
                "The black turtle of the north, master of water",
                ElementType.Water,
                Rarity.Legendary,
                reflectionEffect,
                ActivationType.Reactive,
                effectCondition,
                2 // Cooldown
            );
        }
        
        /// <summary>
        /// Create the Yellow Dragon (Hoàng Long) divine beast
        /// </summary>
        public Entity CreateYellowDragon()
        {
            // Create harmony effect
            HarmonyEffect harmonyEffect = new HarmonyEffect(
                "Yellow Dragon's Harmony",
                "Increases effectiveness of all cards by 20% and activates 'Five Elements Unity'",
                0.2f
            );
            
            // Condition: All five elements used
            AllElementsPlayedCondition allElementsCondition = new AllElementsPlayedCondition(
                "After using all five elements in the battle"
            );
            
            return CreateDivineBeastCard(
                "Yellow Dragon",
                "The yellow dragon of the center, master of all elements",
                ElementType.Earth, // Neutral element
                Rarity.Legendary,
                harmonyEffect,
                ActivationType.Transformative,
                allElementsCondition,
                5 // Cooldown
            );
        }
        
        #endregion
        
        #region Monster Cards (Yêu Quái)
        
        /// <summary>
        /// Create a Monster support card (Yêu Quái)
        /// </summary>
        public Entity CreateMonsterCard(string name, string description, ElementType elementType, 
                                       Rarity rarity, Effect specialEffect, ActivationType activationType,
                                       ActivationCondition condition = null, int cooldownTime = 0)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.Monster,
                Rarity = rarity,
                Cost = 1, // Monsters typically cost less than divine beasts
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = elementType
            };
            card.AddComponent(element);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = activationType,
                Condition = condition,
                Effect = specialEffect,
                CooldownTime = cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            // Add Stats (for display purposes, not used in gameplay)
            StatsComponent stats = new StatsComponent
            {
                Attack = GetStatForRarity(rarity, 2, 4, 6, 8),
                Defense = GetStatForRarity(rarity, 2, 3, 4, 6),
                Health = GetStatForRarity(rarity, 8, 12, 16, 24),
                MaxHealth = GetStatForRarity(rarity, 8, 12, 16, 24),
                Speed = GetStatForRarity(rarity, 3, 4, 5, 7)
            };
            card.AddComponent(stats);
            
            return card;
        }
        
        /// <summary>
        /// Create a Vampire (Ma Cà Rồng) monster
        /// </summary>
        public Entity CreateVampire()
        {
            // Create life steal effect
            LifeStealEffect lifeStealEffect = new LifeStealEffect(
                "Blood Thirst",
                "Absorbs 20% of damage dealt as health. Increases attack power by 5% next turn.",
                0.2f, // Life steal percentage
                0.05f // Attack boost percentage
            );
            
            // Condition: When dealing damage
            DamageDealtCondition damageCondition = new DamageDealtCondition(
                "When dealing damage to an enemy"
            );
            
            return CreateMonsterCard(
                "Vampire",
                "An immortal creature that feeds on the life force of others",
                ElementType.Water,
                Rarity.Epic,
                lifeStealEffect,
                ActivationType.Reactive,
                damageCondition,
                1 // Cooldown
            );
        }
        
        #endregion
        
        #region Artifact Cards (Bảo Vật)
        
        /// <summary>
        /// Create an Artifact support card (Bảo Vật)
        /// </summary>
        public Entity CreateArtifactCard(string name, string description, Rarity rarity, 
                                        Effect specialEffect, ActivationType activationType,
                                        ActivationCondition condition = null, int cooldownTime = 0)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.SpiritAnimal, // Using SpiritAnimal for artifacts for now
                Rarity = rarity,
                Cost = 2, // Artifacts typically cost more
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = activationType,
                Condition = condition,
                Effect = specialEffect,
                CooldownTime = cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create the Time Mirror (Tấm Kính Thời Gian) artifact
        /// </summary>
        public Entity CreateTimeMirror()
        {
            // Create replay effect
            ReplayEffect replayEffect = new ReplayEffect(
                "Time Rewind",
                "Allows replaying the previous turn. Can only be used once per battle."
            );
            
            // Condition: Low health
            HealthPercentCondition lowHealthCondition = new HealthPercentCondition(
                0.2f, // 20% health threshold
                true, // Below threshold
                "When health falls below 20%"
            );
            
            return CreateArtifactCard(
                "Time Mirror",
                "A mystical mirror that can bend the flow of time",
                Rarity.Legendary,
                replayEffect,
                ActivationType.Triggered,
                lowHealthCondition,
                999 // Essentially one-time use
            );
        }
        
        /// <summary>
        /// Create the Dragon Pearl Bagua (Ngọc Rồng Bát Quái) artifact
        /// </summary>
        public Entity CreateDragonPearlBagua()
        {
            // Create unity effect
            ElementUnityEffect unityEffect = new ElementUnityEffect(
                "Five Elements Unity",
                "Activates 'Five Elements Unity' combo, increasing effectiveness of all cards by 100% for 3 turns",
                3, // Duration
                1.0f // Boost percentage
            );
            
            // Condition: All five elements in hand
            AllElementsInHandCondition allElementsCondition = new AllElementsInHandCondition(
                "When having all five elements in hand"
            );
            
            return CreateArtifactCard(
                "Dragon Pearl Bagua",
                "An ancient artifact containing the essence of the five elements",
                Rarity.Legendary,
                unityEffect,
                ActivationType.Transformative,
                allElementsCondition,
                5 // Cooldown
            );
        }
        
        /// <summary>
        /// Create the Heaven and Earth Reversal (Càn Khôn Đảo Chuyển) artifact
        /// </summary>
        public Entity CreateHeavenEarthReversal()
        {
            // Create reversal effect
            ElementReversalEffect reversalEffect = new ElementReversalEffect(
                "Cycle Reversal",
                "Reverses all generating and overcoming relationships for 3 turns",
                3 // Duration
            );
            
            // Condition: After taking heavy damage
            ThresholdCondition heavyDamageCondition = new ThresholdCondition(
                0.3f, // 30% max health threshold
                ThresholdType.HealthLostPercent,
                "After losing more than 30% of maximum health in a single hit"
            );
            
            return CreateArtifactCard(
                "Heaven and Earth Reversal",
                "A mystical scroll that can reverse the natural order",
                Rarity.Legendary,
                reversalEffect,
                ActivationType.Transformative,
                heavyDamageCondition,
                4 // Cooldown
            );
        }
        
        #endregion
        
        #region Talisman Cards (Phù Chú)
        
        /// <summary>
        /// Create a Talisman support card (Phù Chú)
        /// </summary>
        public Entity CreateTalismanCard(string name, string description, ElementType elementType, 
                                        Rarity rarity, Effect specialEffect, ActivationType activationType,
                                        ActivationCondition condition = null, int cooldownTime = 0)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.SpiritAnimal, // Using SpiritAnimal for talismans for now
                Rarity = rarity,
                Cost = 1, // Talismans typically cost less
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = elementType
            };
            card.AddComponent(element);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = activationType,
                Condition = condition,
                Effect = specialEffect,
                CooldownTime = cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            return card;
        }
        #endregion
        
        #region Divine Weapon Cards (Thần Khí)
        
        /// <summary>
        /// Create a Divine Weapon support card (Thần Khí)
        /// </summary>
        public Entity CreateDivineWeaponCard(string name, string description, ElementType elementType, 
                                            Rarity rarity, Effect specialEffect, ActivationType activationType,
                                            ActivationCondition condition = null, int cooldownTime = 0)
        {
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.SpiritAnimal, // Using SpiritAnimal for divine weapons for now
                Rarity = rarity,
                Cost = 2, // Divine weapons typically cost more
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = elementType
            };
            card.AddComponent(element);
            
            // Add SupportCardComponent
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = activationType,
                Condition = condition,
                Effect = specialEffect,
                CooldownTime = cooldownTime,
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            // Add Stats (for display purposes, not used in gameplay)
            StatsComponent stats = new StatsComponent
            {
                Attack = GetStatForRarity(rarity, 3, 5, 7, 9),
                Defense = GetStatForRarity(rarity, 2, 3, 4, 5),
                Health = GetStatForRarity(rarity, 5, 8, 12, 15),
                MaxHealth = GetStatForRarity(rarity, 5, 8, 12, 15),
                Speed = GetStatForRarity(rarity, 2, 3, 4, 5)
            };
            card.AddComponent(stats);
            
            return card;
        }
        
        /// <summary>
        /// Create the Great Blade (Đại Đao Phong Vân) divine weapon
        /// </summary>
        public Entity CreateGreatBlade()
        {
            // Create attack boost effect
            StatBuffEffect attackBoostEffect = new StatBuffEffect(
                "Wind and Cloud Slash",
                "Increases attack damage by 30% and penetration by 15%",
                -1, // Permanent while active
                "Attack",
                0.3f
            );
            
            return CreateDivineWeaponCard(
                "Great Blade of Wind and Cloud",
                "A legendary blade that can cut through anything",
                ElementType.Metal,
                Rarity.Legendary,
                attackBoostEffect,
                ActivationType.Persistent,
                null,
                0 // No cooldown for persistent effects
            );
        }
        
        #endregion
        
        #region Helper Methods
        
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