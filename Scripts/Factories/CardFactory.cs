using System.Collections.Generic;
using Components;
using Core;
using Core.Utils;
using Data;
using UnityEngine;

namespace Factories
{
    /// <summary>
    /// Factory for creating cards
    /// Factory để tạo thẻ bài từ scratch hoặc từ ScriptableObjects
    /// </summary>
    public class CardFactory
    {
        // Reference to EntityManager
        private EntityManager _entityManager;
        
        // Reference to ScriptableObjectFactory
        private ScriptableObjectFactory _scriptableObjectFactory;
        
        // Constructor
        public CardFactory(EntityManager entityManager)
        {
            _entityManager = entityManager;
            
            // Get or create ScriptableObjectFactory instance
            if (ScriptableObjectFactory.HasInstance)
            {
                _scriptableObjectFactory = ScriptableObjectFactory.Instance;
                if (!_scriptableObjectFactory.IsInitialized)
                {
                    _scriptableObjectFactory.Initialize(entityManager);
                }
            }
            else
            {
                _scriptableObjectFactory = ScriptableObjectFactory.Instance;
                _scriptableObjectFactory.Initialize(entityManager);
            }
        }
        
        /// <summary>
        /// Create a basic elemental card
        /// Tạo thẻ nguyên tố cơ bản từ scratch
        /// </summary>
        public Entity CreateBasicCard(string name, string description, ElementType elementType, 
                                     int napAmIndex, Rarity rarity, int cost,
                                     int attack, int defense, int health, int speed)
        {
            // Create a basic entity
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
            
            // Add NapAmComponent
            // NapAmComponent napAm = new NapAmComponent();
            // napAm.Initialize(elementType, napAmIndex);
            // card.AddComponent(napAm);
            
            // Add StatsComponent
            StatsComponent stats = new StatsComponent
            {
                Attack = attack,
                Defense = defense,
                Health = health,
                MaxHealth = health,
                Speed = speed
            };
            card.AddComponent(stats);
            
            return card;
        }

        /// <summary>
        /// Create card from ScriptableObject
        /// Tạo thẻ từ ScriptableObject
        /// </summary>
        public Entity CreateCardFromSO(CardDataSO cardData)
        {
            // Use ScriptableObjectFactory to create the card
            return _scriptableObjectFactory.CreateCardFromSO(cardData);
        }
        
        /// <summary>
        /// Create a sample deck of cards
        /// Tạo một bộ bài mẫu
        /// </summary>
        public List<Entity> CreateSampleDeck()
        {
            List<Entity> deck = new List<Entity>();
            
            // Basic Metal cards
            deck.Add(CreateBasicCard("Kim Chúc Kiếm", "Thanh kiếm bằng kim loại sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 4, 1, 5, 3));
            deck.Add(CreateBasicCard("Thiết Bích Thuẫn", "Tấm khiên kim loại cứng cáp", ElementType.Metal, 1, Rarity.Common, 1, 1, 5, 6, 2));
            
            // Basic Wood cards
            deck.Add(CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4));
            deck.Add(CreateBasicCard("Sinh Mệnh Chi Mộc", "Cây sinh mệnh hồi phục sinh lực", ElementType.Wood, 3, Rarity.Common, 1, 2, 3, 7, 3));
            
            // Basic Water cards
            deck.Add(CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3));
            deck.Add(CreateBasicCard("Thuỷ Tinh Châu", "Viên ngọc chứa năng lượng nước", ElementType.Water, 0, Rarity.Common, 1, 2, 4, 6, 3));
            
            // Basic Fire cards
            deck.Add(CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4));
            deck.Add(CreateBasicCard("Xích Tâm Ấn", "Ấn lửa đỏ thiêu đốt kẻ thù", ElementType.Fire, 1, Rarity.Common, 1, 4, 2, 5, 3));
            
            // Basic Earth cards
            deck.Add(CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1));
            deck.Add(CreateBasicCard("Kiên Thạch Thuẫn", "Khiên đá cứng cáp", ElementType.Earth, 0, Rarity.Common, 1, 1, 6, 8, 1));
            
            return deck;
        }
        
        /// <summary>
        /// Create a themed deck based on element
        /// Tạo bộ bài theo chủ đề nguyên tố
        /// </summary>
        public List<Entity> CreateThemedDeck(ElementType elementType, int cardCount = 8)
        {
            List<Entity> deck = new List<Entity>();
            List<CardDataSO> elementCards = LoadCardsByElement(elementType);
            
            // If we can't find enough element-specific cards, create basic ones
            if (elementCards == null || elementCards.Count < cardCount)
            {
                // Create basic cards of the specified element
                switch (elementType)
                {
                    case ElementType.Metal:
                        deck.Add(CreateBasicCard("Kim Chúc Kiếm", "Thanh kiếm bằng kim loại sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 4, 1, 5, 3));
                        deck.Add(CreateBasicCard("Thiết Bích Thuẫn", "Tấm khiên kim loại cứng cáp", ElementType.Metal, 1, Rarity.Common, 1, 1, 5, 6, 2));
                        deck.Add(CreateBasicCard("Kim Châm", "Những mũi kim nhỏ nhưng sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 3, 2, 4, 5));
                        deck.Add(CreateBasicCard("Tụ Kim Chủy", "Vũ khí tạo từ kim loại ngưng tụ", ElementType.Metal, 0, Rarity.Common, 2, 5, 2, 6, 3));
                        break;
                        
                    case ElementType.Wood:
                        deck.Add(CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4));
                        deck.Add(CreateBasicCard("Sinh Mệnh Chi Mộc", "Cây sinh mệnh hồi phục sinh lực", ElementType.Wood, 3, Rarity.Common, 1, 2, 3, 7, 3));
                        deck.Add(CreateBasicCard("Cây Độc", "Cây có độc tính mạnh", ElementType.Wood, 4, Rarity.Common, 1, 3, 2, 5, 4));
                        deck.Add(CreateBasicCard("Mộc Khiên", "Khiên làm từ gỗ linh", ElementType.Wood, 5, Rarity.Common, 2, 1, 6, 8, 2));
                        break;
                        
                    case ElementType.Water:
                        deck.Add(CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3));
                        deck.Add(CreateBasicCard("Thuỷ Tinh Châu", "Viên ngọc chứa năng lượng nước", ElementType.Water, 0, Rarity.Common, 1, 2, 4, 6, 3));
                        deck.Add(CreateBasicCard("Thuỷ Đao", "Dao tạo từ nước đông cứng", ElementType.Water, 1, Rarity.Common, 1, 4, 2, 5, 4));
                        deck.Add(CreateBasicCard("Pháp Thuỷ", "Pháp thuật điều khiển nước", ElementType.Water, 2, Rarity.Common, 2, 5, 3, 6, 3));
                        break;
                        
                    case ElementType.Fire:
                        deck.Add(CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4));
                        deck.Add(CreateBasicCard("Xích Tâm Ấn", "Ấn lửa đỏ thiêu đốt kẻ thù", ElementType.Fire, 1, Rarity.Common, 1, 4, 2, 5, 3));
                        deck.Add(CreateBasicCard("Hoả Cầu", "Quả cầu lửa phá huỷ mọi thứ", ElementType.Fire, 1, Rarity.Common, 1, 5, 1, 4, 5));
                        deck.Add(CreateBasicCard("Phượng Hoả", "Lửa của phượng hoàng", ElementType.Fire, 0, Rarity.Common, 2, 6, 2, 5, 4));
                        break;
                        
                    case ElementType.Earth:
                        deck.Add(CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1));
                        deck.Add(CreateBasicCard("Kiên Thạch Thuẫn", "Khiên đá cứng cáp", ElementType.Earth, 0, Rarity.Common, 1, 1, 6, 8, 1));
                        deck.Add(CreateBasicCard("Thạch Chùy", "Búa đá nặng nề", ElementType.Earth, 0, Rarity.Common, 1, 4, 3, 6, 2));
                        deck.Add(CreateBasicCard("Đại Địa Chi Lực", "Sức mạnh của đất", ElementType.Earth, 5, Rarity.Common, 2, 3, 5, 8, 1));
                        break;
                }
                
                // Add some basic cards from other elements to make the deck diverse
                deck.Add(CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3));
                deck.Add(CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4));
                deck.Add(CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4));
                deck.Add(CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1));
            }
            else
            {
                // Create cards from ScriptableObjects
                for (int i = 0; i < Mathf.Min(cardCount, elementCards.Count); i++)
                {
                    Entity card = CreateCardFromSO(elementCards[i]);
                    if (card != null)
                    {
                        deck.Add(card);
                    }
                }
            }
            
            // Ensure deck has the requested number of cards
            while (deck.Count < cardCount && deck.Count > 0)
            {
                // Duplicate random cards if we don't have enough
                int randomIndex = Random.Range(0, deck.Count);
                deck.Add(deck[randomIndex]);
            }
            
            return deck;
        }
        
        /// <summary>
        /// Load cards by element from Resources
        /// Tải thẻ theo nguyên tố từ Resources
        /// </summary>
        private List<CardDataSO> LoadCardsByElement(ElementType elementType)
        {
            List<CardDataSO> result = new List<CardDataSO>();
            
            // Try to load cards from Resources
            CardDataSO[] allCards = Resources.LoadAll<CardDataSO>("Cards");
            
            if (allCards != null && allCards.Length > 0)
            {
                foreach (var card in allCards)
                {
                    if (card.elementType == elementType)
                    {
                        result.Add(card);
                    }
                }
            }
            
            return result;
        }
    }
}