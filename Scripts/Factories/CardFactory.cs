using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;

namespace Factories
{
    /// <summary>
    /// Factory for creating different types of cards
    /// </summary>
    public class CardFactory
    {
        private EntityManager _entityManager;
        
        // Constructor
        public CardFactory(EntityManager entityManager)
        {
            this._entityManager = entityManager;
        }
        
        /// <summary>
        /// Create a basic elemental card
        /// </summary>
        public Entity CreateElementalCard(string name, string description, ElementType elementType, 
                                          Rarity rarity, int attack, int defense, int health, int speed, int cost)
        {
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.ElementalCard,
                Rarity = rarity,
                Cost = cost,
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add ElementComponent
            ElementComponent element = new ElementComponent
            {
                Element = elementType
            };
            card.AddComponent(element);
            
            // Add StatsComponent
            StatsComponent stats = new StatsComponent(attack, defense, health, speed);
            card.AddComponent(stats);
            
            // TODO: Add artwork loading logic - in a real implementation, this would load the appropriate sprite
            // cardInfo.Artwork = Resources.Load<Sprite>($"Cards/{elementType}/{name}");
            
            return card;
        }
        
        /// <summary>
        /// Create a card with a NapAm
        /// </summary>
        public Entity CreateElementalCardWithNapAm(string name, string description, ElementType elementType, 
                                                 int napAmIndex, Rarity rarity, int attack, int defense, 
                                                 int health, int speed, int cost)
        {
            Entity card = CreateElementalCard(name, description, elementType, rarity, attack, defense, health, speed, cost);
            
            // Add NapAmComponent
            NapAmComponent napAm = new NapAmComponent(elementType, napAmIndex);
            card.AddComponent(napAm);
            
            return card;
        }
        
        /// <summary>
        /// Create a Divine Beast card (Thẻ Thần Thú)
        /// </summary>
        public Entity CreateDivineBeastCard(string name, string description, ElementType elementType, 
                                            Rarity rarity, int attack, int defense, int health, int speed,
                                            Effect specialEffect, int cost)
        {
            Entity card = CreateElementalCard(name, description, elementType, rarity, attack, defense, health, speed, cost);
            
            // Change type to Divine Beast
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardInfo.Type = CardType.DivineBeast;
            }
            
            // Add effect
            EffectComponent effectComponent = new EffectComponent();
            effectComponent.AddEffect(specialEffect);
            card.AddComponent(effectComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a Monster card (Thẻ Yêu Quái)
        /// </summary>
        public Entity CreateMonsterCard(string name, string description, ElementType elementType, 
                                       Rarity rarity, int attack, int defense, int health, int speed,
                                       Effect[] effects, int cost)
        {
            Entity card = CreateElementalCard(name, description, elementType, rarity, attack, defense, health, speed, cost);
            
            // Change type to Monster
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardInfo.Type = CardType.Monster;
            }
            
            // Add effects
            EffectComponent effectComponent = new EffectComponent();
            foreach (var effect in effects)
            {
                effectComponent.AddEffect(effect);
            }
            card.AddComponent(effectComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a Spirit Animal card (Thẻ Linh Thú)
        /// </summary>
        public Entity CreateSpiritAnimalCard(string name, string description, ElementType elementType, 
                                           Rarity rarity, int attack, int defense, int health, int speed,
                                           Effect supportEffect, int cost)
        {
            Entity card = CreateElementalCard(name, description, elementType, rarity, attack, defense, health, speed, cost);
            
            // Change type to Spirit Animal
            CardInfoComponent cardInfo = card.GetComponent<CardInfoComponent>();
            if (cardInfo != null)
            {
                cardInfo.Type = CardType.SpiritAnimal;
            }
            
            // Add support component
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = ActivationType.Persistent, // Default activation type
                Effect = supportEffect,
                CooldownTime = 0, // No cooldown for persistent effects
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a Joker card (Thẻ Joker)
        /// </summary>
        public Entity CreateJokerCard(string name, string description, Rarity rarity, 
                                    Effect transformativeEffect, ActivationCondition condition, int cost)
        {
            Entity card = _entityManager.CreateEntity();
            
            // Add CardInfoComponent
            CardInfoComponent cardInfo = new CardInfoComponent
            {
                Name = name,
                Description = description,
                Type = CardType.Joker,
                Rarity = rarity,
                Cost = cost,
                State = CardState.InDeck
            };
            card.AddComponent(cardInfo);
            
            // Add support component with transformative activation
            SupportCardComponent supportComponent = new SupportCardComponent
            {
                ActivationType = ActivationType.Transformative,
                Effect = transformativeEffect,
                Condition = condition,
                CooldownTime = 5, // Joker cards typically have long cooldowns
                CurrentCooldown = 0,
                IsActive = false
            };
            card.AddComponent(supportComponent);
            
            return card;
        }
        
        /// <summary>
        /// Create a preset Metal element card
        /// </summary>
        public Entity CreateMetalCard(string name, string description, MetalNapAm napAm, Rarity rarity, int cost)
        {
            // Default stats based on rarity
            int attack, defense, health, speed;
            GetDefaultStats(rarity, out attack, out defense, out health, out speed);
            
            // Adjust stats based on NapAm
            switch (napAm)
            {
                case MetalNapAm.SwordQi: // Offensive
                    attack += 2;
                    break;
                case MetalNapAm.Hardness: // Defensive
                    defense += 2;
                    break;
                case MetalNapAm.Purity: // Balanced
                    attack += 1;
                    defense += 1;
                    break;
                case MetalNapAm.Reflection: // Counter
                    // Special counter ability would be implemented through effects
                    break;
                case MetalNapAm.Spirit: // Support
                    // Special support ability would be implemented through effects
                    break;
                case MetalNapAm.Calmness: // Resistance
                    health += 2;
                    break;
            }
            
            return CreateElementalCardWithNapAm(name, description, ElementType.Metal, (int)napAm, 
                                              rarity, attack, defense, health, speed, cost);
        }
        
        /// <summary>
        /// Create a preset Wood element card
        /// </summary>
        public Entity CreateWoodCard(string name, string description, WoodNapAm napAm, Rarity rarity, int cost)
        {
            // Default stats based on rarity
            int attack, defense, health, speed;
            GetDefaultStats(rarity, out attack, out defense, out health, out speed);
            
            // Adjust stats based on NapAm
            switch (napAm)
            {
                case WoodNapAm.Growth: // Health regen
                    health += 2;
                    break;
                case WoodNapAm.Flexibility: // Speed
                    speed += 2;
                    break;
                case WoodNapAm.Symbiosis: // Synergy
                    // Special synergy ability would be implemented through effects
                    break;
                case WoodNapAm.Regeneration: // Healing
                    health += 3;
                    attack -= 1;
                    break;
                case WoodNapAm.Toxin: // DoT
                    attack += 1;
                    // DoT ability would be implemented through effects
                    break;
                case WoodNapAm.Shelter: // Protection
                    defense += 3;
                    attack -= 1;
                    break;
            }
            
            return CreateElementalCardWithNapAm(name, description, ElementType.Wood, (int)napAm, 
                                              rarity, attack, defense, health, speed, cost);
        }
        
        /// <summary>
        /// Get default stats based on card rarity
        /// </summary>
        private void GetDefaultStats(Rarity rarity, out int attack, out int defense, out int health, out int speed)
        {
            switch (rarity)
            {
                case Rarity.Common:
                    attack = 2;
                    defense = 2;
                    health = 5;
                    speed = 2;
                    break;
                case Rarity.Rare:
                    attack = 3;
                    defense = 3;
                    health = 7;
                    speed = 3;
                    break;
                case Rarity.Epic:
                    attack = 5;
                    defense = 4;
                    health = 10;
                    speed = 4;
                    break;
                case Rarity.Legendary:
                    attack = 7;
                    defense = 6;
                    health = 15;
                    speed = 5;
                    break;
                default:
                    attack = 1;
                    defense = 1;
                    health = 3;
                    speed = 1;
                    break;
            }
        }
        
        /// <summary>
        /// Create a sample deck for testing
        /// </summary>
        public List<Entity> CreateSampleDeck()
        {
            List<Entity> deck = new List<Entity>();
            
            // Add some Metal cards
            deck.Add(CreateMetalCard("Kim Chúc Kiếm", "A powerful metal sword that cuts through defenses", MetalNapAm.SwordQi, Rarity.Common, 1));
            deck.Add(CreateMetalCard("Thiết Bích Thuẫn", "A sturdy shield that provides strong defense", MetalNapAm.Hardness, Rarity.Common, 1));
            
            // Add some Wood cards
            deck.Add(CreateWoodCard("Thanh Mộc Châm", "Sharp wooden needles that can pierce enemies", WoodNapAm.Toxin, Rarity.Common, 1));
            deck.Add(CreateWoodCard("Sinh Mệnh Chi Mộc", "A tree of life that provides healing", WoodNapAm.Regeneration, Rarity.Rare, 2));
            
            // Add a Divine Beast
            deck.Add(CreateDivineBeastCard("Bạch Hổ", "The white tiger of the west, master of metal", ElementType.Metal, 
                                         Rarity.Epic, 6, 4, 12, 5, 
                                         new StatBuffEffect("Tiger's Might", "Increases attack power", 3, "attack", 2f), 
                                         3));
            
            // Add a Monster
            deck.Add(CreateMonsterCard("Cửu Vĩ Hồ", "Nine-tailed fox with mesmerizing powers", ElementType.Fire,
                                     Rarity.Epic, 5, 3, 10, 6,
                                     new Effect[] { 
                                         new DotEffect("Fox Fire", "Burns the target over time", 3, 2)
                                     },
                                     3));
            
            return deck;
        }
    }
}