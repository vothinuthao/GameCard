using Core;
using Core.Utils;

namespace Components
{
    /// <summary>
    /// Represents the elemental aspect of a card (Ngũ Hành) with integrated Nạp Âm
    /// </summary>
    public class ElementComponent : Component
    {
        // Element related data
        public ElementType Element { get; set; }
        public int NapAmIndex { get; set; }

        private MetalNapAm _metalNapAm;
        private WoodNapAm _woodNapAm;
        private WaterNapAm _waterNapAm;
        private FireNapAm _fireNapAm;
        private EarthNapAm _earthNapAm;
        
        /// <summary>
        /// Default constructor
        /// </summary>
        public ElementComponent(ElementType element)
        {
            Element = element;
            switch (element)
            {
                case ElementType.Metal:
                    _metalNapAm = MetalNapAm.SwordQi;
                    break;
                case ElementType.Wood:
                    _woodNapAm = WoodNapAm.Growth;
                    break;
                case ElementType.Water:
                    _waterNapAm = WaterNapAm.Flow;
                    break;
                case ElementType.Fire:
                    _fireNapAm = FireNapAm.Burning;
                    break;
                case ElementType.Earth:
                    _earthNapAm = EarthNapAm.Solidity;
                    break;
            }
        }
        
        /// <summary>
        /// Constructor with specified Nạp Âm
        /// </summary>
        public ElementComponent(ElementType element, int napAmIndex)
        {
            Element = element;
            
            switch (element)
            {
                case ElementType.Metal:
                    _metalNapAm = (MetalNapAm)napAmIndex;
                    break;
                case ElementType.Wood:
                    _woodNapAm = (WoodNapAm)napAmIndex;
                    break;
                case ElementType.Water:
                    _waterNapAm = (WaterNapAm)napAmIndex;
                    break;
                case ElementType.Fire:
                    _fireNapAm = (FireNapAm)napAmIndex;
                    break;
                case ElementType.Earth:
                    _earthNapAm = (EarthNapAm)napAmIndex;
                    break;
            }
        }
        
        // Element-specific constructors
        public ElementComponent(ElementType element, MetalNapAm napAm)
        {
            Element = element;
            _metalNapAm = napAm;
        }
        
        public ElementComponent(ElementType element, WoodNapAm napAm)
        {
            Element = element;
            _woodNapAm = napAm;
        }
        
        public ElementComponent(ElementType element, WaterNapAm napAm)
        {
            Element = element;
            _waterNapAm = napAm;
        }
        
        public ElementComponent(ElementType element, FireNapAm napAm)
        {
            Element = element;
            _fireNapAm = napAm;
        }
        
        public ElementComponent(ElementType element, EarthNapAm napAm)
        {
            Element = element;
            _earthNapAm = napAm;
        }


        /// <summary>
        /// Returns the bonus damage against another element based on Wu Xing rules
        /// </summary>
        public float GetElementBonus(ElementType targetElement)
        {
            // Tương khắc: +50% damage
            switch (Element)
            {
                case ElementType.Metal when targetElement == ElementType.Wood:
                case ElementType.Wood when targetElement == ElementType.Earth:
                case ElementType.Earth when targetElement == ElementType.Water:
                case ElementType.Water when targetElement == ElementType.Fire:
                case ElementType.Fire when targetElement == ElementType.Metal:
                    return 0.5f; // 50% bonus damage
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Returns the synergy bonus when combined with another element
        /// </summary>
        public float GetElementSynergy(ElementType targetElement)
        {
            // Tương sinh: +30% effectiveness
            switch (Element)
            {
                case ElementType.Metal when targetElement == ElementType.Water:
                case ElementType.Water when targetElement == ElementType.Wood:
                case ElementType.Wood when targetElement == ElementType.Fire:
                case ElementType.Fire when targetElement == ElementType.Earth:
                case ElementType.Earth when targetElement == ElementType.Metal:
                    return 0.3f; // 30% synergy bonus
                default:
                    return 0f;
            }
        }
        
        /// <summary>
        /// Gets the Vietnamese name for this element
        /// </summary>
        public string GetElementName()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    return "Kim";
                case ElementType.Wood:
                    return "Mộc";
                case ElementType.Water:
                    return "Thủy";
                case ElementType.Fire:
                    return "Hỏa";
                case ElementType.Earth:
                    return "Thổ";
                default:
                    return "Unknown";
            }
        }
        
        #region Nạp Âm Methods
        
        /// <summary>
        /// Get the NapAm name in English
        /// </summary>
        public string GetNapAmName()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    return _metalNapAm.ToString();
                case ElementType.Wood:
                    return _woodNapAm.ToString();
                case ElementType.Water:
                    return _waterNapAm.ToString();
                case ElementType.Fire:
                    return _fireNapAm.ToString();
                case ElementType.Earth:
                    return _earthNapAm.ToString();
                default:
                    return "Unknown";
            }
        }
        
        /// <summary>
        /// Get the Vietnamese name for this NapAm
        /// </summary>
        public string GetNapAmVietnameseName()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    switch (_metalNapAm)
                    {
                        case MetalNapAm.SwordQi: return "Kiếm Khí";
                        case MetalNapAm.Hardness: return "Cương Nghị";
                        case MetalNapAm.Purity: return "Thanh Tịnh";
                        case MetalNapAm.Reflection: return "Phản Chiếu";
                        case MetalNapAm.Spirit: return "Linh Khí";
                        case MetalNapAm.Calmness: return "Trầm Tĩnh";
                        default: return "Kim Không Xác Định";
                    }
                    
                case ElementType.Wood:
                    switch (_woodNapAm)
                    {
                        case WoodNapAm.Growth: return "Sinh Trưởng";
                        case WoodNapAm.Flexibility: return "Linh Hoạt";
                        case WoodNapAm.Symbiosis: return "Cộng Sinh";
                        case WoodNapAm.Regeneration: return "Tái Sinh";
                        case WoodNapAm.Toxin: return "Độc Tố";
                        case WoodNapAm.Shelter: return "Che Chắn";
                        default: return "Mộc Không Xác Định";
                    }
                    
                case ElementType.Water:
                    switch (_waterNapAm)
                    {
                        case WaterNapAm.Adaptation: return "Thích Nghi";
                        case WaterNapAm.Ice: return "Băng Giá";
                        case WaterNapAm.Flow: return "Dòng Chảy";
                        case WaterNapAm.Mist: return "Sương Mù";
                        case WaterNapAm.Reflection: return "Phản Ánh";
                        case WaterNapAm.Purification: return "Thanh Tẩy";
                        default: return "Thủy Không Xác Định";
                    }
                    
                case ElementType.Fire:
                    switch (_fireNapAm)
                    {
                        case FireNapAm.Burning: return "Thiêu Đốt";
                        case FireNapAm.Explosion: return "Bùng Nổ";
                        case FireNapAm.Passion: return "Nhiệt Huyết";
                        case FireNapAm.Light: return "Ánh Sáng";
                        case FireNapAm.Forging: return "Rèn Luyện";
                        case FireNapAm.Incineration: return "Thiêu Rụi";
                        default: return "Hỏa Không Xác Định";
                    }
                    
                case ElementType.Earth:
                    switch (_earthNapAm)
                    {
                        case EarthNapAm.Solidity: return "Kiên Cố";
                        case EarthNapAm.Gravity: return "Trọng Lực";
                        case EarthNapAm.Fertility: return "Màu Mỡ";
                        case EarthNapAm.Volcano: return "Núi Lửa";
                        case EarthNapAm.Crystal: return "Tinh Thể";
                        case EarthNapAm.Terra: return "Đại Địa";
                        default: return "Thổ Không Xác Định";
                    }
                    
                default:
                    return "Nạp Âm Không Xác Định";
            }
        }
        
        /// <summary>
        /// Set Nạp Âm for this element
        /// </summary>
        public void SetNapAm(int napAmIndex)
        {
            switch (Element)
            {
                case ElementType.Metal:
                    _metalNapAm = (MetalNapAm)napAmIndex;
                    break;
                case ElementType.Wood:
                    _woodNapAm = (WoodNapAm)napAmIndex;
                    break;
                case ElementType.Water:
                    _waterNapAm = (WaterNapAm)napAmIndex;
                    break;
                case ElementType.Fire:
                    _fireNapAm = (FireNapAm)napAmIndex;
                    break;
                case ElementType.Earth:
                    _earthNapAm = (EarthNapAm)napAmIndex;
                    break;
            }
        }
        
        /// <summary>
        /// Get Specific Nạp Âm for Metal element
        /// </summary>
        public MetalNapAm GetMetalNapAm()
        {
            return _metalNapAm;
        }
        
        /// <summary>
        /// Get Specific Nạp Âm for Wood element
        /// </summary>
        public WoodNapAm GetWoodNapAm()
        {
            return _woodNapAm;
        }
        
        /// <summary>
        /// Get Specific Nạp Âm for Water element
        /// </summary>
        public WaterNapAm GetWaterNapAm()
        {
            return _waterNapAm;
        }
        
        /// <summary>
        /// Get Specific Nạp Âm for Fire element
        /// </summary>
        public FireNapAm GetFireNapAm()
        {
            return _fireNapAm;
        }
        
        /// <summary>
        /// Get Specific Nạp Âm for Earth element
        /// </summary>
        public EarthNapAm GetEarthNapAm()
        {
            return _earthNapAm;
        }
        
        /// <summary>
        /// Get Nạp Âm index for serialization
        /// </summary>
        public int GetNapAmIndex()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    return (int)_metalNapAm;
                case ElementType.Wood:
                    return (int)_woodNapAm;
                case ElementType.Water:
                    return (int)_waterNapAm;
                case ElementType.Fire:
                    return (int)_fireNapAm;
                case ElementType.Earth:
                    return (int)_earthNapAm;
                default:
                    return 0;
            }
        }
        
        /// <summary>
        /// Get the base power/effect of the current Nạp Âm (1-5)
        /// </summary>
        public int GetNapAmPower()
        {
            switch (Element)
            {
                case ElementType.Metal:
                    switch (_metalNapAm)
                    {
                        case MetalNapAm.SwordQi: return 5;     // High attack
                        case MetalNapAm.Hardness: return 4;    // High defense
                        case MetalNapAm.Purity: return 3;      // Medium cleansing
                        case MetalNapAm.Reflection: return 4;  // High reflection
                        case MetalNapAm.Spirit: return 3;      // Medium spirit
                        case MetalNapAm.Calmness: return 3;    // Medium resistance
                        default: return 1;
                    }
                    
                case ElementType.Wood:
                    switch (_woodNapAm)
                    {
                        case WoodNapAm.Growth: return 4;       // High healing
                        case WoodNapAm.Flexibility: return 3;  // Medium evasion
                        case WoodNapAm.Symbiosis: return 3;    // Medium support
                        case WoodNapAm.Regeneration: return 5; // High regeneration
                        case WoodNapAm.Toxin: return 4;        // High damage over time
                        case WoodNapAm.Shelter: return 3;      // Medium protection
                        default: return 1;
                    }
                    
                case ElementType.Water:
                    switch (_waterNapAm)
                    {
                        case WaterNapAm.Adaptation: return 3;  // Medium flexibility
                        case WaterNapAm.Ice: return 4;         // High control
                        case WaterNapAm.Flow: return 3;        // Medium mobility
                        case WaterNapAm.Mist: return 5;        // High evasion
                        case WaterNapAm.Reflection: return 4;  // High counter
                        case WaterNapAm.Purification: return 3;// Medium cleanse
                        default: return 1;
                    }
                    
                case ElementType.Fire:
                    switch (_fireNapAm)
                    {
                        case FireNapAm.Burning: return 4;      // High damage over time
                        case FireNapAm.Explosion: return 5;    // Very high burst damage
                        case FireNapAm.Passion: return 3;      // Medium buff
                        case FireNapAm.Light: return 4;        // High reveal
                        case FireNapAm.Forging: return 3;      // Medium strengthen
                        case FireNapAm.Incineration: return 5; // High destruction
                        default: return 1;
                    }
                    
                case ElementType.Earth:
                    switch (_earthNapAm)
                    {
                        case EarthNapAm.Solidity: return 5;    // High stability
                        case EarthNapAm.Gravity: return 4;     // High control
                        case EarthNapAm.Fertility: return 3;   // Medium growth
                        case EarthNapAm.Volcano: return 5;     // High area damage
                        case EarthNapAm.Crystal: return 4;     // High amplification
                        case EarthNapAm.Terra: return 3;       // Medium terrain control
                        default: return 1;
                    }
                    
                default:
                    return 1;
            }
        }
        
        /// <summary>
        /// Get combo bonus when this Nạp Âm is combined with another Nạp Âm
        /// </summary>
        public ComboData GetNapAmCombo(ElementComponent other)
        {
            // Check some common combinations based on the game documents
            
            // Kiếm Băng (Metal.SwordQi + Water.Ice)
            if (Element == ElementType.Metal && _metalNapAm == MetalNapAm.SwordQi &&
                other.Element == ElementType.Water && other._waterNapAm == WaterNapAm.Ice)
            {
                return new ComboData("Kiếm Băng", 10, "Làm chậm đối thủ 2 lượt");
            }
            
            // Thép Linh Hoạt (Metal.Hardness + Water.Adaptation)
            if ((Element == ElementType.Metal && _metalNapAm == MetalNapAm.Hardness &&
                 other.Element == ElementType.Water && other._waterNapAm == WaterNapAm.Adaptation) ||
                (Element == ElementType.Water && _waterNapAm == WaterNapAm.Adaptation &&
                 other.Element == ElementType.Metal && other._metalNapAm == MetalNapAm.Hardness))
            {
                return new ComboData("Thép Linh Hoạt", 15, "Tăng phòng thủ 40%");
            }
            
            // Rừng Cháy (Wood.Growth + Fire.Burning)
            if ((Element == ElementType.Wood && _woodNapAm == WoodNapAm.Growth &&
                 other.Element == ElementType.Fire && other._fireNapAm == FireNapAm.Burning) ||
                (Element == ElementType.Fire && _fireNapAm == FireNapAm.Burning &&
                 other.Element == ElementType.Wood && other._woodNapAm == WoodNapAm.Growth))
            {
                return new ComboData("Rừng Cháy", 20, "Gây cháy và hồi máu");
            }
            
            // Add more combo checks based on your game documents
            
            // Default return if no specific combo found
            return new ComboData("Không Có Combo", 0, "Không có hiệu ứng đặc biệt");
        }
        
        #endregion
    }
    
    /// <summary>
    /// Data structure for Nạp Âm combo effects
    /// </summary>
    public class ComboData
    {
        public string Name { get; private set; }
        public int BonusPoints { get; private set; }
        public string Effect { get; private set; }
        
        public ComboData(string name, int bonusPoints, string effect)
        {
            Name = name;
            BonusPoints = bonusPoints;
            Effect = effect;
        }
    }
}