// CardDataGenerator.cs

using Core.Utils;
using Data;
using UnityEngine;

namespace Prototype
{
    /// <summary>
    /// Generates scriptable objects for all basic cards
    /// </summary>
    public class CardDataGenerator : MonoBehaviour
    {
        [Header("Card Generation")]
        [SerializeField] private string outputFolder = "Assets/Resources/Cards";
        
        // Create Metal Cards
        [ContextMenu("Generate Metal Cards")]
        public void GenerateMetalCards()
        {
            // Create directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);
            
            // Metal Card 1: Kim Chúc Kiếm (Metal Forged Sword)
            ElementalCardDataSO card1 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card1.cardName = "Kim Chúc Kiếm";
            card1.description = "Thanh kiếm bằng kim loại sắc bén, có khả năng chém xuyên giáp.";
            card1.cardType = CardType.ElementalCard;
            card1.rarity = Rarity.Common;
            card1.cost = 1;
            card1.attack = 4;
            card1.defense = 1;
            card1.health = 3;
            card1.speed = 2;
            card1.elementType = ElementType.Metal;
            card1.napAmIndex = (int)MetalNapAm.SwordQi;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card1, $"{outputFolder}/Metal_SwordQi_Card.asset");
            #endif
            
            // Metal Card 2: Thiết Bích Thuẫn (Iron Wall Shield)
            ElementalCardDataSO card2 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card2.cardName = "Thiết Bích Thuẫn";
            card2.description = "Tấm khiên kim loại cứng cáp, tăng khả năng phòng thủ.";
            card2.cardType = CardType.ElementalCard;
            card2.rarity = Rarity.Common;
            card2.cost = 1;
            card2.attack = 1;
            card2.defense = 5;
            card2.health = 4;
            card2.speed = 1;
            card2.elementType = ElementType.Metal;
            card2.napAmIndex = (int)MetalNapAm.Hardness;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card2, $"{outputFolder}/Metal_Hardness_Card.asset");
            #endif
            
            // Metal Card 3: Thanh Tịnh Chi Kim (Pure Metal)
            ElementalCardDataSO card3 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card3.cardName = "Thanh Tịnh Chi Kim";
            card3.description = "Kim loại tinh khiết, có khả năng thanh lọc các hiệu ứng tiêu cực.";
            card3.cardType = CardType.ElementalCard;
            card3.rarity = Rarity.Rare;
            card3.cost = 2;
            card3.attack = 3;
            card3.defense = 3;
            card3.health = 5;
            card3.speed = 2;
            card3.elementType = ElementType.Metal;
            card3.napAmIndex = (int)MetalNapAm.Purity;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card3, $"{outputFolder}/Metal_Purity_Card.asset");
            #endif
            
            // Metal Card 4: Kim Loại Phản Chiếu (Reflective Metal)
            ElementalCardDataSO card4 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card4.cardName = "Kim Loại Phản Chiếu";
            card4.description = "Kim loại phản chiếu, có khả năng phản lại một phần sát thương.";
            card4.cardType = CardType.ElementalCard;
            card4.rarity = Rarity.Rare;
            card4.cost = 2;
            card4.attack = 2;
            card4.defense = 4;
            card4.health = 6;
            card4.speed = 2;
            card4.elementType = ElementType.Metal;
            card4.napAmIndex = (int)MetalNapAm.Reflection;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card4, $"{outputFolder}/Metal_Reflection_Card.asset");
            #endif
            
            // Metal Card 5: Bạch Hổ Thần Thú (White Tiger Divine Beast)
            DivineBeastCardDataSO card5 = ScriptableObject.CreateInstance<DivineBeastCardDataSO>();
            card5.cardName = "Bạch Hổ Thần Thú";
            card5.description = "Thần thú phương Tây, chúa tể của Kim, tăng sức mạnh cho các thẻ Kim.";
            card5.cardType = CardType.DivineBeast;
            card5.rarity = Rarity.Epic;
            card5.cost = 3;
            card5.attack = 6;
            card5.defense = 4;
            card5.health = 12;
            card5.speed = 5;
            card5.elementType = ElementType.Metal;
            card5.napAmIndex = (int)MetalNapAm.Spirit;
            card5.effectDescription = "Tăng sức tấn công của tất cả thẻ Kim.";
            card5.effectTargetStat = "attack";
            card5.effectValue = 2;
            card5.effectDuration = 3;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card5, $"{outputFolder}/Metal_WhiteTiger_Card.asset");
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Generated 5 Metal cards!");
        }
        
        // Create Wood Cards
        [ContextMenu("Generate Wood Cards")]
        public void GenerateWoodCards()
        {
            // Create directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);
            
            // Wood Card 1: Thanh Mộc Châm (Green Wood Needle)
            ElementalCardDataSO card1 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card1.cardName = "Thanh Mộc Châm";
            card1.description = "Gai nhọn bằng gỗ, có thể xuyên thấu phòng thủ và gây độc.";
            card1.cardType = CardType.ElementalCard;
            card1.rarity = Rarity.Common;
            card1.cost = 1;
            card1.attack = 3;
            card1.defense = 1;
            card1.health = 2;
            card1.speed = 3;
            card1.elementType = ElementType.Wood;
            card1.napAmIndex = (int)WoodNapAm.Toxin;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card1, $"{outputFolder}/Wood_Toxin_Card.asset");
            #endif
            
            // Wood Card 2: Sinh Mệnh Chi Mộc (Life Tree)
            ElementalCardDataSO card2 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card2.cardName = "Sinh Mệnh Chi Mộc";
            card2.description = "Cây đời sống có khả năng hồi phục và chữa lành.";
            card2.cardType = CardType.ElementalCard;
            card2.rarity = Rarity.Rare;
            card2.cost = 2;
            card2.attack = 2;
            card2.defense = 3;
            card2.health = 7;
            card2.speed = 2;
            card2.elementType = ElementType.Wood;
            card2.napAmIndex = (int)WoodNapAm.Regeneration;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card2, $"{outputFolder}/Wood_Regeneration_Card.asset");
            #endif
            
            // Wood Card 3: Linh Hoạt Chi Mộc (Flexible Wood)
            ElementalCardDataSO card3 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card3.cardName = "Linh Hoạt Chi Mộc";
            card3.description = "Gỗ dẻo dai và linh hoạt, tăng khả năng né tránh.";
            card3.cardType = CardType.ElementalCard;
            card3.rarity = Rarity.Common;
            card3.cost = 1;
            card3.attack = 2;
            card3.defense = 2;
            card3.health = 4;
            card3.speed = 4;
            card3.elementType = ElementType.Wood;
            card3.napAmIndex = (int)WoodNapAm.Flexibility;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card3, $"{outputFolder}/Wood_Flexibility_Card.asset");
            #endif
            
            // Wood Card 4: Cộng Sinh Lâm Mộc (Symbiotic Forest)
            ElementalCardDataSO card4 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card4.cardName = "Cộng Sinh Lâm Mộc";
            card4.description = "Rừng cây cộng sinh, càng nhiều thẻ Mộc càng mạnh.";
            card4.cardType = CardType.ElementalCard;
            card4.rarity = Rarity.Rare;
            card4.cost = 2;
            card4.attack = 2;
            card4.defense = 2;
            card4.health = 5;
            card4.speed = 3;
            card4.elementType = ElementType.Wood;
            card4.napAmIndex = (int)WoodNapAm.Symbiosis;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card4, $"{outputFolder}/Wood_Symbiosis_Card.asset");
            #endif
            
            // Wood Card 5: Thanh Long Thần Thú (Azure Dragon Divine Beast)
            DivineBeastCardDataSO card5 = ScriptableObject.CreateInstance<DivineBeastCardDataSO>();
            card5.cardName = "Thanh Long Thần Thú";
            card5.description = "Thần thú phương Đông, chúa tể của Mộc, tăng sức mạnh cho các thẻ Mộc.";
            card5.cardType = CardType.DivineBeast;
            card5.rarity = Rarity.Epic;
            card5.cost = 3;
            card5.attack = 5;
            card5.defense = 5;
            card5.health = 10;
            card5.speed = 6;
            card5.elementType = ElementType.Wood;
            card5.napAmIndex = 0;
            card5.effectDescription = "Tăng sức mạnh hồi phục và tấn công của tất cả thẻ Mộc.";
            card5.effectTargetStat = "all";
            card5.effectValue = 1.5f;
            card5.effectDuration = 3;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card5, $"{outputFolder}/Wood_AzureDragon_Card.asset");
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Generated 5 Wood cards!");
        }
        
        // Create Water Cards
        [ContextMenu("Generate Water Cards")]
        public void GenerateWaterCards()
        {
            // Create directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);
            
            // Water Card 1: Hàn Băng Tiễn (Ice Arrow)
            ElementalCardDataSO card1 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card1.cardName = "Hàn Băng Tiễn";
            card1.description = "Mũi tên băng lạnh giá, có khả năng làm chậm đối thủ.";
            card1.cardType = CardType.ElementalCard;
            card1.rarity = Rarity.Common;
            card1.cost = 1;
            card1.attack = 3;
            card1.defense = 2;
            card1.health = 3;
            card1.speed = 3;
            card1.elementType = ElementType.Water;
            card1.napAmIndex = (int)WaterNapAm.Ice;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card1, $"{outputFolder}/Water_Ice_Card.asset");
            #endif
            
            // Water Card 2: Thuỷ Lưu Truy Tung (Flowing Water)
            ElementalCardDataSO card2 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card2.cardName = "Thuỷ Lưu Truy Tung";
            card2.description = "Dòng nước chảy liên tục, tăng tốc độ rút bài.";
            card2.cardType = CardType.ElementalCard;
            card2.rarity = Rarity.Common;
            card2.cost = 1;
            card2.attack = 2;
            card2.defense = 2;
            card2.health = 3;
            card2.speed = 5;
            card2.elementType = ElementType.Water;
            card2.napAmIndex = (int)WaterNapAm.Flow;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card2, $"{outputFolder}/Water_Flow_Card.asset");
            #endif
            
            // Water Card 3: Thuỷ Mị Sương Mù (Misty Water)
            ElementalCardDataSO card3 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card3.cardName = "Thuỷ Mị Sương Mù";
            card3.description = "Sương mù dày đặc, giảm độ chính xác của đối thủ.";
            card3.cardType = CardType.ElementalCard;
            card3.rarity = Rarity.Rare;
            card3.cost = 2;
            card3.attack = 2;
            card3.defense = 4;
            card3.health = 5;
            card3.speed = 3;
            card3.elementType = ElementType.Water;
            card3.napAmIndex = (int)WaterNapAm.Mist;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card3, $"{outputFolder}/Water_Mist_Card.asset");
            #endif
            
            // Water Card 4: Thuỷ Tinh Phản Ảnh (Mirror Water)
            ElementalCardDataSO card4 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card4.cardName = "Thuỷ Tinh Phản Ảnh";
            card4.description = "Nước trong như gương, có khả năng bắt chước khả năng của đối thủ.";
            card4.cardType = CardType.ElementalCard;
            card4.rarity = Rarity.Rare;
            card4.cost = 2;
            card4.attack = 3;
            card4.defense = 3;
            card4.health = 4;
            card4.speed = 3;
            card4.elementType = ElementType.Water;
            card4.napAmIndex = (int)WaterNapAm.Reflection;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card4, $"{outputFolder}/Water_Reflection_Card.asset");
            #endif
            
            // Water Card 5: Huyền Vũ Thần Thú (Black Turtle Divine Beast)
            DivineBeastCardDataSO card5 = ScriptableObject.CreateInstance<DivineBeastCardDataSO>();
            card5.cardName = "Huyền Vũ Thần Thú";
            card5.description = "Thần thú phương Bắc, chúa tể của Thuỷ và Thổ, tăng khả năng phòng thủ.";
            card5.cardType = CardType.DivineBeast;
            card5.rarity = Rarity.Epic;
            card5.cost = 3;
            card5.attack = 4;
            card5.defense = 7;
            card5.health = 15;
            card5.speed = 4;
            card5.elementType = ElementType.Water;
            card5.napAmIndex = 0;
            card5.effectDescription = "Tăng khả năng phòng thủ và hồi phục mỗi lượt.";
            card5.effectTargetStat = "defense";
            card5.effectValue = 3;
            card5.effectDuration = 3;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card5, $"{outputFolder}/Water_BlackTurtle_Card.asset");
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Generated 5 Water cards!");
        }
        
        // Create Fire Cards
        [ContextMenu("Generate Fire Cards")]
        public void GenerateFireCards()
        {
            // Create directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);
            
            // Fire Card 1: Hoả Diệm Phần Thiên (Burning Fire)
            ElementalCardDataSO card1 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card1.cardName = "Hoả Diệm Phần Thiên";
            card1.description = "Ngọn lửa thiêu đốt mọi thứ, gây sát thương liên tục.";
            card1.cardType = CardType.ElementalCard;
            card1.rarity = Rarity.Common;
            card1.cost = 1;
            card1.attack = 5;
            card1.defense = 1;
            card1.health = 3;
            card1.speed = 3;
            card1.elementType = ElementType.Fire;
            card1.napAmIndex = (int)FireNapAm.Burning;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card1, $"{outputFolder}/Fire_Burning_Card.asset");
            #endif
            
            // Fire Card 2: Hỏa Linh Bùng Nổ (Explosive Fire)
            ElementalCardDataSO card2 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card2.cardName = "Hỏa Linh Bùng Nổ";
            card2.description = "Lửa có khả năng bùng nổ, gây sát thương diện rộng.";
            card2.cardType = CardType.ElementalCard;
            card2.rarity = Rarity.Rare;
            card2.cost = 2;
            card2.attack = 4;
            card2.defense = 2;
            card2.health = 4;
            card2.speed = 3;
            card2.elementType = ElementType.Fire;
            card2.napAmIndex = (int)FireNapAm.Explosion;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card2, $"{outputFolder}/Fire_Explosion_Card.asset");
            #endif
            
            // Fire Card 3: Nhiệt Huyết Chi Hỏa (Passionate Fire)
            ElementalCardDataSO card3 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card3.cardName = "Nhiệt Huyết Chi Hỏa";
            card3.description = "Lửa nhiệt huyết, tăng sức mạnh tấn công.";
            card3.cardType = CardType.ElementalCard;
            card3.rarity = Rarity.Common;
            card3.cost = 1;
            card3.attack = 4;
            card3.defense = 1;
            card3.health = 4;
            card3.speed = 3;
            card3.elementType = ElementType.Fire;
            card3.napAmIndex = (int)FireNapAm.Passion;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card3, $"{outputFolder}/Fire_Passion_Card.asset");
            #endif
            
            // Fire Card 4: Minh Hỏa Chi Quang (Light of Fire)
            ElementalCardDataSO card4 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card4.cardName = "Minh Hỏa Chi Quang";
            card4.description = "Ánh sáng từ lửa, phát hiện điểm yếu của đối thủ.";
            card4.cardType = CardType.ElementalCard;
            card4.rarity = Rarity.Rare;
            card4.cost = 2;
            card4.attack = 3;
            card4.defense = 2;
            card4.health = 5;
            card4.speed = 4;
            card4.elementType = ElementType.Fire;
            card4.napAmIndex = (int)FireNapAm.Light;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card4, $"{outputFolder}/Fire_Light_Card.asset");
            #endif
            
            // Fire Card 5: Chu Tước Thần Thú (Vermilion Bird Divine Beast)
            DivineBeastCardDataSO card5 = ScriptableObject.CreateInstance<DivineBeastCardDataSO>();
            card5.cardName = "Chu Tước Thần Thú";
            card5.description = "Thần thú phương Nam, chúa tể của Hỏa, tăng sức mạnh tấn công.";
            card5.cardType = CardType.DivineBeast;
            card5.rarity = Rarity.Epic;
            card5.cost = 3;
            card5.attack = 7;
            card5.defense = 3;
            card5.health = 10;
            card5.speed = 7;
            card5.elementType = ElementType.Fire;
            card5.napAmIndex = 0;
            card5.effectDescription = "Tăng sức tấn công và tạo hiệu ứng đốt cho các thẻ Hỏa.";
            card5.effectTargetStat = "attack";
            card5.effectValue = 3;
            card5.effectDuration = 3;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card5, $"{outputFolder}/Fire_VermilionBird_Card.asset");
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Generated 5 Fire cards!");
        }
        
        // Create Earth Cards
        [ContextMenu("Generate Earth Cards")]
        public void GenerateEarthCards()
        {
            // Create directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);
            
            // Earth Card 1: Thổ Nham Kiên Cố (Solid Rock)
            ElementalCardDataSO card1 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card1.cardName = "Thổ Nham Kiên Cố";
            card1.description = "Đá cứng như bàn thạch, tăng khả năng phòng thủ.";
            card1.cardType = CardType.ElementalCard;
            card1.rarity = Rarity.Common;
            card1.cost = 1;
            card1.attack = 2;
            card1.defense = 5;
            card1.health = 6;
            card1.speed = 1;
            card1.elementType = ElementType.Earth;
            card1.napAmIndex = (int)EarthNapAm.Solidity;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card1, $"{outputFolder}/Earth_Solidity_Card.asset");
            #endif
            
            // Earth Card 2: Trọng Lực Chi Ngọc (Gravity Stone)
            ElementalCardDataSO card2 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card2.cardName = "Trọng Lực Chi Ngọc";
            card2.description = "Đá nặng như núi, có khả năng làm chậm đối thủ.";
            card2.cardType = CardType.ElementalCard;
            card2.rarity = Rarity.Common;
            card2.cost = 1;
            card2.attack = 2;
            card2.defense = 4;
            card2.health = 5;
            card2.speed = 2;
            card2.elementType = ElementType.Earth;
            card2.napAmIndex = (int)EarthNapAm.Gravity;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card2, $"{outputFolder}/Earth_Gravity_Card.asset");
            #endif
            
            // Earth Card 3: Màu Mỡ Chi Thổ (Fertile Earth)
            ElementalCardDataSO card3 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card3.cardName = "Màu Mỡ Chi Thổ";
            card3.description = "Đất màu mỡ, tăng cường hiệu quả của thẻ Mộc.";
            card3.cardType = CardType.ElementalCard;
            card3.rarity = Rarity.Rare;
            card3.cost = 2;
            card3.attack = 2;
            card3.defense = 3;
            card3.health = 7;
            card3.speed = 2;
            card3.elementType = ElementType.Earth;
            card3.napAmIndex = (int)EarthNapAm.Fertility;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card3, $"{outputFolder}/Earth_Fertility_Card.asset");
            #endif
            
            // Earth Card 4: Tinh Thể Chi Thổ (Crystal Earth)
            ElementalCardDataSO card4 = ScriptableObject.CreateInstance<ElementalCardDataSO>();
            card4.cardName = "Tinh Thể Chi Thổ";
            card4.description = "Đất kết tinh thành pha lê, tăng hiệu quả của các nguyên tố khác.";
            card4.cardType = CardType.ElementalCard;
            card4.rarity = Rarity.Rare;
            card4.cost = 2;
            card4.attack = 3;
            card4.defense = 4;
            card4.health = 6;
            card4.speed = 2;
            card4.elementType = ElementType.Earth;
            card4.napAmIndex = (int)EarthNapAm.Crystal;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card4, $"{outputFolder}/Earth_Crystal_Card.asset");
            #endif
            
            // Earth Card 5: Hoàng Long Thần Thú (Yellow Dragon Divine Beast)
            DivineBeastCardDataSO card5 = ScriptableObject.CreateInstance<DivineBeastCardDataSO>();
            card5.cardName = "Hoàng Long Thần Thú";
            card5.description = "Thần thú trung tâm, chúa tể của Ngũ Hành, tăng cường sức mạnh cho tất cả nguyên tố.";
            card5.cardType = CardType.DivineBeast;
            card5.rarity = Rarity.Legendary;
            card5.cost = 5;
            card5.attack = 6;
            card5.defense = 6;
            card5.health = 15;
            card5.speed = 5;
            card5.elementType = ElementType.Earth;
            card5.napAmIndex = 0;
            card5.effectDescription = "Tăng sức mạnh cho tất cả các thẻ nguyên tố.";
            card5.effectTargetStat = "all";
            card5.effectValue = 2;
            card5.effectDuration = 5;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card5, $"{outputFolder}/Earth_YellowDragon_Card.asset");
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Generated 5 Earth cards!");
        }
        
        // Create Special Cards
        [ContextMenu("Generate Special Cards")]
        public void GenerateSpecialCards()
        {
            // Create directory if it doesn't exist
            if (!System.IO.Directory.Exists(outputFolder))
                System.IO.Directory.CreateDirectory(outputFolder);
            
            // Special Card 1: Cửu Vĩ Hồ (Nine-Tailed Fox)
            MonsterCardDataSO card1 = ScriptableObject.CreateInstance<MonsterCardDataSO>();
            card1.cardName = "Cửu Vĩ Hồ";
            card1.description = "Hồ ly chín đuôi với sức mạnh mê hoặc, có thể khiến đối thủ tấn công đồng đội.";
            card1.cardType = CardType.Monster;
            card1.rarity = Rarity.Epic;
            card1.cost = 3;
            card1.attack = 5;
            card1.defense = 3;
            card1.health = 10;
            card1.speed = 6;
            card1.elementType = ElementType.Fire;
            card1.napAmIndex = 0;
            
            // Add effect data
            card1.effects = new MonsterCardDataSO.EffectData[1];
            card1.effects[0] = new MonsterCardDataSO.EffectData
            {
                effectName = "Mê Hoặc",
                effectDescription = "30% cơ hội làm đối thủ đánh nhầm vào đồng đội.",
                effectType = "control",
                effectValue = 0.3f,
                effectDuration = 2
            };
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card1, $"{outputFolder}/Special_NineTailedFox_Card.asset");
            #endif
            
            // Special Card 2: Càn Khôn Đảo Chuyển (Heaven and Earth Reversal)
            JokerCardDataSO card2 = ScriptableObject.CreateInstance<JokerCardDataSO>();
            card2.cardName = "Càn Khôn Đảo Chuyển";
            card2.description = "Đảo ngược tất cả hiệu ứng tương sinh thành tương khắc và ngược lại.";
            card2.cardType = CardType.Joker;
            card2.rarity = Rarity.Legendary;
            card2.cost = 4;
            card2.attack = 0;
            card2.defense = 0;
            card2.health = 8;
            card2.speed = 0;
            card2.effectDescription = "Đảo ngược tất cả hiệu ứng tương sinh thành tương khắc và ngược lại trong 3 lượt.";
            card2.activationType = ActivationType.Transformative;
            card2.activationConditionDescription = "Kích hoạt khi có ít nhất 3 nguyên tố khác nhau trong tay.";
            card2.conditionType = "element_diversity";
            card2.conditionValue = 3;
            card2.cooldownTime = 5;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card2, $"{outputFolder}/Special_HeavenEarthReversal_Card.asset");
            #endif
            
            // Special Card 3: Tý (Rat) - Thập Nhị Chi
            SpiritAnimalCardDataSO card3 = ScriptableObject.CreateInstance<SpiritAnimalCardDataSO>();
            card3.cardName = "Tý - Thần Tốc";
            card3.description = "Chuột nhanh nhẹn, tăng tốc độ và khả năng né tránh.";
            card3.cardType = CardType.SpiritAnimal;
            card3.rarity = Rarity.Rare;
            card3.cost = 2;
            card3.attack = 2;
            card3.defense = 1;
            card3.health = 4;
            card3.speed = 7;
            card3.elementType = ElementType.Water; // Chuột thuộc hành Thủy
            card3.napAmIndex = 0;
            card3.zodiacAnimal = "Rat";
            card3.supportEffectDescription = "Tăng tốc độ và né tránh mỗi khi chơi 2 lá bài.";
            card3.activationType = ActivationType.Recurring;
            card3.activationConditionDescription = "Mỗi khi chơi 2 lá bài.";
            card3.conditionType = "card_count";
            card3.conditionValue = 2;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card3, $"{outputFolder}/Special_Rat_Card.asset");
            #endif
            
            // Special Card 4: Âm Dương Giao Hòa (Yin-Yang Harmony)
            JokerCardDataSO card4 = ScriptableObject.CreateInstance<JokerCardDataSO>();
            card4.cardName = "Âm Dương Giao Hòa";
            card4.description = "Kích hoạt đồng thời hiệu ứng của tất cả nạp âm trong tay bài.";
            card4.cardType = CardType.Joker;
            card4.rarity = Rarity.Epic;
            card4.cost = 3;
            card4.attack = 3;
            card4.defense = 3;
            card4.health = 9;
            card4.speed = 3;
            card4.effectDescription = "Kích hoạt đồng thời hiệu ứng của tất cả nạp âm trong tay bài.";
            card4.activationType = ActivationType.Triggered;
            card4.activationConditionDescription = "Kích hoạt khi có ít nhất 3 lá bài khác nhau trong tay.";
            card4.conditionType = "unique_card_count";
            card4.conditionValue = 3;
            card4.cooldownTime = 4;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card4, $"{outputFolder}/Special_YinYangHarmony_Card.asset");
            #endif
            
            // Special Card 5: Hỗn Độn Nguyên Thủy (Primordial Chaos)
            JokerCardDataSO card5 = ScriptableObject.CreateInstance<JokerCardDataSO>();
            card5.cardName = "Hỗn Độn Nguyên Thủy";
            card5.description = "Xáo trộn ngẫu nhiên tất cả thẻ bài trên bàn, tạo ra kết quả không thể dự đoán.";
            card5.cardType = CardType.Joker;
            card5.rarity = Rarity.Legendary;
            card5.cost = 5;
            card5.attack = 0;
            card5.defense = 0;
            card5.health = 10;
            card5.speed = 0;
            card5.effectDescription = "Xáo trộn ngẫu nhiên tất cả thẻ bài trên bàn, tạo ra kết quả không thể dự đoán.";
            card5.activationType = ActivationType.Triggered;
            card5.activationConditionDescription = "Kích hoạt khi máu dưới 30%.";
            card5.conditionType = "health";
            card5.conditionValue = 0.3f;
            card5.cooldownTime = 6;
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.CreateAsset(card5, $"{outputFolder}/Special_PrimordialChaos_Card.asset");
            #endif
            
            #if UNITY_EDITOR
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
            #endif
            
            Debug.Log("Generated 5 Special cards!");
        }
        
        // Generate all cards
        [ContextMenu("Generate All Cards")]
        public void GenerateAllCards()
        {
            GenerateMetalCards();
            GenerateWoodCards();
            GenerateWaterCards();
            GenerateFireCards();
            GenerateEarthCards();
            GenerateSpecialCards();
            
            Debug.Log("Generated all card data!");
        }
    }
}

