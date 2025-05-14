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
        // Tham chiếu đến EntityManager
        // Reference to EntityManager
        private EntityManager _entityManager;
        
        // Tham chiếu đến ScriptableObjectFactory
        // Reference to ScriptableObjectFactory
        private ScriptableObjectFactory _scriptableObjectFactory;

        // Kích thước bộ bài khởi đầu mặc định
        // Default starter deck size
        private const int DEFAULT_STARTER_DECK_SIZE = 30;
        
        // Constructor
        public CardFactory(EntityManager entityManager)
        {
            _entityManager = entityManager;
            
            // Lấy hoặc tạo instance ScriptableObjectFactory
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
            // Tạo entity cơ bản
            // Create a basic entity
            Entity card = _entityManager.CreateEntity();
            
            // Thêm CardInfoComponent
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
            
            // Thêm ElementComponent
            // Add ElementComponent
            ElementComponent element = new ElementComponent(elementType, napAmIndex);
            card.AddComponent(element);
            
            // Thêm StatsComponent
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
            // Sử dụng ScriptableObjectFactory để tạo thẻ
            // Use ScriptableObjectFactory to create the card
            return _scriptableObjectFactory.CreateCardFromSO(cardData);
        }
        
        /// <summary>
        /// Create a sample deck of cards with a specified size
        /// Tạo một bộ bài mẫu với kích thước xác định
        /// </summary>
        public List<Entity> CreateSampleDeck(int deckSize = DEFAULT_STARTER_DECK_SIZE)
        {
            List<Entity> deck = new List<Entity>();
            
            // Danh sách mẫu thẻ cơ bản
            // List of sample basic cards
            List<System.Func<Entity>> cardCreators = new List<System.Func<Entity>>
            {
                // Thẻ Kim cơ bản
                // Basic Metal cards
                () => CreateBasicCard("Kim Chúc Kiếm", "Thanh kiếm bằng kim loại sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 4, 1, 5, 3),
                () => CreateBasicCard("Thiết Bích Thuẫn", "Tấm khiên kim loại cứng cáp", ElementType.Metal, 1, Rarity.Common, 1, 1, 5, 6, 2),
                () => CreateBasicCard("Kim Châm", "Những mũi kim nhỏ nhưng sắc bén", ElementType.Metal, 2, Rarity.Common, 1, 3, 2, 4, 5),
                
                // Thẻ Mộc cơ bản
                // Basic Wood cards
                () => CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4),
                () => CreateBasicCard("Sinh Mệnh Chi Mộc", "Cây sinh mệnh hồi phục sinh lực", ElementType.Wood, 3, Rarity.Common, 1, 2, 3, 7, 3),
                () => CreateBasicCard("Cây Độc", "Cây có độc tính mạnh", ElementType.Wood, 4, Rarity.Common, 1, 3, 2, 5, 4),
                
                // Thẻ Thủy cơ bản
                // Basic Water cards
                () => CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3),
                () => CreateBasicCard("Thuỷ Tinh Châu", "Viên ngọc chứa năng lượng nước", ElementType.Water, 0, Rarity.Common, 1, 2, 4, 6, 3),
                () => CreateBasicCard("Thuỷ Đao", "Dao tạo từ nước đông cứng", ElementType.Water, 2, Rarity.Common, 1, 4, 2, 5, 4),
                
                // Thẻ Hỏa cơ bản
                // Basic Fire cards
                () => CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4),
                () => CreateBasicCard("Xích Tâm Ấn", "Ấn lửa đỏ thiêu đốt kẻ thù", ElementType.Fire, 1, Rarity.Common, 1, 4, 2, 5, 3),
                () => CreateBasicCard("Hoả Cầu", "Quả cầu lửa phá huỷ mọi thứ", ElementType.Fire, 2, Rarity.Common, 1, 5, 1, 4, 5),
                
                // Thẻ Thổ cơ bản
                // Basic Earth cards
                () => CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1),
                () => CreateBasicCard("Kiên Thạch Thuẫn", "Khiên đá cứng cáp", ElementType.Earth, 0, Rarity.Common, 1, 1, 6, 8, 1),
                () => CreateBasicCard("Thạch Chùy", "Búa đá nặng nề", ElementType.Earth, 2, Rarity.Common, 1, 4, 3, 6, 2),
            };
            
            // Đảm bảo bộ bài có đủ số lượng thẻ bằng cách thêm nhiều lần
            // Ensure deck has the requested number of cards by adding multiple copies
            while (deck.Count < deckSize)
            {
                // Lặp qua danh sách thẻ mẫu
                // Iterate through the sample cards
                foreach (var cardCreator in cardCreators)
                {
                    if (deck.Count < deckSize)
                    {
                        deck.Add(cardCreator());
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
            return deck;
        }
        
        /// <summary>
        /// Create a themed deck based on element with a specified size
        /// Tạo bộ bài theo chủ đề nguyên tố với kích thước xác định
        /// </summary>
        public List<Entity> CreateThemedDeck(ElementType elementType, int deckSize = DEFAULT_STARTER_DECK_SIZE)
        {
            List<Entity> deck = new List<Entity>();
            List<CardDataSO> elementCards = LoadCardsByElement(elementType);
            
            // Nếu không thể tìm thấy đủ thẻ nguyên tố cụ thể, tạo thẻ cơ bản
            // If we can't find enough element-specific cards, create basic ones
            if (elementCards == null || elementCards.Count < deckSize / 2) // Chỉ cần đủ một nửa số thẻ từ SO
            {
                // Danh sách các thẻ theo chủ đề nguyên tố
                // List of cards by element theme
                List<System.Func<Entity>> themeCardCreators = new List<System.Func<Entity>>();
                
                switch (elementType)
                {
                    case ElementType.Metal:
                        themeCardCreators.Add(() => CreateBasicCard("Kim Chúc Kiếm", "Thanh kiếm bằng kim loại sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 4, 1, 5, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Thiết Bích Thuẫn", "Tấm khiên kim loại cứng cáp", ElementType.Metal, 1, Rarity.Common, 1, 1, 5, 6, 2));
                        themeCardCreators.Add(() => CreateBasicCard("Kim Châm", "Những mũi kim nhỏ nhưng sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 3, 2, 4, 5));
                        themeCardCreators.Add(() => CreateBasicCard("Tụ Kim Chủy", "Vũ khí tạo từ kim loại ngưng tụ", ElementType.Metal, 0, Rarity.Common, 2, 5, 2, 6, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Bạch Kim Tiễn", "Mũi tên làm từ bạch kim", ElementType.Metal, 2, Rarity.Common, 1, 4, 1, 5, 4));
                        themeCardCreators.Add(() => CreateBasicCard("Hoàng Kim Giáp", "Áo giáp bằng vàng cứng cáp", ElementType.Metal, 3, Rarity.Common, 2, 1, 6, 7, 1));
                        break;
                        
                    case ElementType.Wood:
                        themeCardCreators.Add(() => CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4));
                        themeCardCreators.Add(() => CreateBasicCard("Sinh Mệnh Chi Mộc", "Cây sinh mệnh hồi phục sinh lực", ElementType.Wood, 3, Rarity.Common, 1, 2, 3, 7, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Cây Độc", "Cây có độc tính mạnh", ElementType.Wood, 4, Rarity.Common, 1, 3, 2, 5, 4));
                        themeCardCreators.Add(() => CreateBasicCard("Mộc Khiên", "Khiên làm từ gỗ linh", ElementType.Wood, 5, Rarity.Common, 2, 1, 6, 8, 2));
                        themeCardCreators.Add(() => CreateBasicCard("Dương Liễu Tiên", "Roi làm từ cành dương liễu", ElementType.Wood, 1, Rarity.Common, 1, 4, 1, 5, 5));
                        themeCardCreators.Add(() => CreateBasicCard("Thanh Đằng", "Dây leo xanh mượt", ElementType.Wood, 2, Rarity.Common, 1, 3, 3, 6, 3));
                        break;
                        
                    case ElementType.Water:
                        themeCardCreators.Add(() => CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Thuỷ Tinh Châu", "Viên ngọc chứa năng lượng nước", ElementType.Water, 0, Rarity.Common, 1, 2, 4, 6, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Thuỷ Đao", "Dao tạo từ nước đông cứng", ElementType.Water, 1, Rarity.Common, 1, 4, 2, 5, 4));
                        themeCardCreators.Add(() => CreateBasicCard("Pháp Thuỷ", "Pháp thuật điều khiển nước", ElementType.Water, 2, Rarity.Common, 2, 5, 3, 6, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Băng Tinh Hoa", "Tinh thể băng vĩnh cửu", ElementType.Water, 4, Rarity.Common, 1, 3, 4, 5, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Thủy Long", "Rồng nước mạnh mẽ", ElementType.Water, 3, Rarity.Common, 2, 5, 2, 7, 4));
                        break;
                        
                    case ElementType.Fire:
                        themeCardCreators.Add(() => CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4));
                        themeCardCreators.Add(() => CreateBasicCard("Xích Tâm Ấn", "Ấn lửa đỏ thiêu đốt kẻ thù", ElementType.Fire, 1, Rarity.Common, 1, 4, 2, 5, 3));
                        themeCardCreators.Add(() => CreateBasicCard("Hoả Cầu", "Quả cầu lửa phá huỷ mọi thứ", ElementType.Fire, 1, Rarity.Common, 1, 5, 1, 4, 5));
                        themeCardCreators.Add(() => CreateBasicCard("Phượng Hoả", "Lửa của phượng hoàng", ElementType.Fire, 0, Rarity.Common, 2, 6, 2, 5, 4));
                        themeCardCreators.Add(() => CreateBasicCard("Diệm Nhận", "Lưỡi đao lửa sắc bén", ElementType.Fire, 2, Rarity.Common, 1, 5, 1, 4, 5));
                        themeCardCreators.Add(() => CreateBasicCard("Liệt Hỏa Quyền", "Quyền pháp lửa mạnh mẽ", ElementType.Fire, 3, Rarity.Common, 2, 6, 1, 5, 3));
                        break;
                        
                    case ElementType.Earth:
                        themeCardCreators.Add(() => CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1));
                        themeCardCreators.Add(() => CreateBasicCard("Kiên Thạch Thuẫn", "Khiên đá cứng cáp", ElementType.Earth, 0, Rarity.Common, 1, 1, 6, 8, 1));
                        themeCardCreators.Add(() => CreateBasicCard("Thạch Chùy", "Búa đá nặng nề", ElementType.Earth, 0, Rarity.Common, 1, 4, 3, 6, 2));
                        themeCardCreators.Add(() => CreateBasicCard("Đại Địa Chi Lực", "Sức mạnh của đất", ElementType.Earth, 5, Rarity.Common, 2, 3, 5, 8, 1));
                        themeCardCreators.Add(() => CreateBasicCard("Địa Liệt", "Đất nứt ra gây sát thương", ElementType.Earth, 1, Rarity.Common, 1, 4, 3, 5, 2));
                        themeCardCreators.Add(() => CreateBasicCard("Thạch Giáp", "Áo giáp làm từ đá", ElementType.Earth, 2, Rarity.Common, 2, 1, 7, 7, 1));
                        break;
                }
                
                // Thêm thẻ chủ đề
                // Add themed cards
                while (deck.Count < deckSize * 0.7f) // 70% thẻ chủ đề
                {
                    foreach (var cardCreator in themeCardCreators)
                    {
                        if (deck.Count < deckSize * 0.7f)
                        {
                            deck.Add(cardCreator());
                        }
                        else
                        {
                            break;
                        }
                    }
                }
                
                // Thêm một số thẻ cơ bản từ các nguyên tố khác để tạo sự đa dạng
                // Add some basic cards from other elements to make the deck diverse
                List<System.Func<Entity>> diverseCardCreators = new List<System.Func<Entity>>
                {
                    () => CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3),
                    () => CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4),
                    () => CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4),
                    () => CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1),
                    () => CreateBasicCard("Kim Chúc Kiếm", "Thanh kiếm bằng kim loại sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 4, 1, 5, 3)
                };
                
                // Lọc ra các thẻ từ nguyên tố khác với chủ đề
                // Filter out cards from elements different from the theme
                diverseCardCreators.RemoveAll(creator => 
                {
                    Entity tempCard = creator();
                    ElementComponent element = tempCard.GetComponent<ElementComponent>();
                    bool isSameElement = element != null && element.Element == elementType;
                    return isSameElement;
                });
                
                // Thêm thẻ đa dạng
                // Add diverse cards
                while (deck.Count < deckSize)
                {
                    foreach (var cardCreator in diverseCardCreators)
                    {
                        if (deck.Count < deckSize)
                        {
                            deck.Add(cardCreator());
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }
            else
            {
                // Tạo thẻ từ ScriptableObjects
                // Create cards from ScriptableObjects
                int cardsFromSO = Mathf.Min(deckSize, elementCards.Count);
                for (int i = 0; i < cardsFromSO; i++)
                {
                    Entity card = CreateCardFromSO(elementCards[i]);
                    if (card != null)
                    {
                        deck.Add(card);
                    }
                }
                
                // Thêm thẻ cơ bản nếu chưa đủ số lượng
                // Add basic cards if not enough
                if (deck.Count < deckSize)
                {
                    // Tạo danh sách các thẻ cơ bản phù hợp với chủ đề
                    // Create a list of basic cards matching the theme
                    List<System.Func<Entity>> basicCardCreators = new List<System.Func<Entity>>();
                    
                    switch (elementType)
                    {
                        case ElementType.Metal:
                            basicCardCreators.Add(() => CreateBasicCard("Kim Chúc Kiếm", "Thanh kiếm bằng kim loại sắc bén", ElementType.Metal, 0, Rarity.Common, 1, 4, 1, 5, 3));
                            basicCardCreators.Add(() => CreateBasicCard("Thiết Bích Thuẫn", "Tấm khiên kim loại cứng cáp", ElementType.Metal, 1, Rarity.Common, 1, 1, 5, 6, 2));
                            break;
                        case ElementType.Wood:
                            basicCardCreators.Add(() => CreateBasicCard("Mộc Linh Trượng", "Gậy pháp thuật làm từ gỗ linh", ElementType.Wood, 0, Rarity.Common, 1, 3, 2, 5, 4));
                            basicCardCreators.Add(() => CreateBasicCard("Sinh Mệnh Chi Mộc", "Cây sinh mệnh hồi phục sinh lực", ElementType.Wood, 3, Rarity.Common, 1, 2, 3, 7, 3));
                            break;
                        case ElementType.Water:
                            basicCardCreators.Add(() => CreateBasicCard("Băng Ấn", "Ấn pháp thuật băng giá", ElementType.Water, 1, Rarity.Common, 1, 3, 3, 5, 3));
                            basicCardCreators.Add(() => CreateBasicCard("Thuỷ Tinh Châu", "Viên ngọc chứa năng lượng nước", ElementType.Water, 0, Rarity.Common, 1, 2, 4, 6, 3));
                            break;
                        case ElementType.Fire:
                            basicCardCreators.Add(() => CreateBasicCard("Hoả Diệm Châm", "Ngọn lửa được phong ấn", ElementType.Fire, 0, Rarity.Common, 1, 5, 1, 4, 4));
                            basicCardCreators.Add(() => CreateBasicCard("Xích Tâm Ấn", "Ấn lửa đỏ thiêu đốt kẻ thù", ElementType.Fire, 1, Rarity.Common, 1, 4, 2, 5, 3));
                            break;
                        case ElementType.Earth:
                            basicCardCreators.Add(() => CreateBasicCard("Thổ Nang", "Túi đất chứa sức mạnh đất", ElementType.Earth, 0, Rarity.Common, 1, 2, 5, 7, 1));
                            basicCardCreators.Add(() => CreateBasicCard("Kiên Thạch Thuẫn", "Khiên đá cứng cáp", ElementType.Earth, 0, Rarity.Common, 1, 1, 6, 8, 1));
                            break;
                    }
                    
                    // Thêm thẻ cơ bản đến khi đủ số lượng
                    // Add basic cards until we reach the desired count
                    while (deck.Count < deckSize)
                    {
                        foreach (var cardCreator in basicCardCreators)
                        {
                            if (deck.Count < deckSize)
                            {
                                deck.Add(cardCreator());
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
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
            CardDataSO[] allCards = Resources.LoadAll<CardDataSO>("Cards");
            
            if (allCards != null && allCards.Length > 0)
            {
                foreach (var card in allCards)
                {
                    if (card is ElementalCardDataSO)
                    {
                        ElementalCardDataSO elementalCard = (ElementalCardDataSO)card;
                        if (elementalCard.elementType == elementType)
                        {
                            result.Add(card);
                        }
                    }
                }
            }
            
            return result;
        }
    }
}