// File: Scripts/Components/CardComponents/NapAmComponent.cs
using System;
using Core;
using Core.Utils;

namespace Components
{
    /// <summary>
    /// Represents the NapAm aspect of a card - specialized attribute of elements
    /// </summary>
    public class NapAmComponent : Component
    {
        // Element type of this card
        private ElementType _elementType;
        
        // Nap Am values for each element type
        private MetalNapAm _metalNapAm;
        private WoodNapAm _woodNapAm;
        private WaterNapAm _waterNapAm;
        private FireNapAm _fireNapAm;
        private EarthNapAm _earthNapAm;
        
        /// <summary>
        /// Constructor for Metal Nap Am
        /// </summary>
        public NapAmComponent(ElementType elementType, MetalNapAm napAm)
        {
            _elementType = elementType;
            _metalNapAm = napAm;
        }
        
        /// <summary>
        /// Constructor for Wood Nap Am
        /// </summary>
        public NapAmComponent(ElementType elementType, WoodNapAm napAm)
        {
            _elementType = elementType;
            _woodNapAm = napAm;
        }
        
        /// <summary>
        /// Constructor for Water Nap Am
        /// </summary>
        public NapAmComponent(ElementType elementType, WaterNapAm napAm)
        {
            _elementType = elementType;
            _waterNapAm = napAm;
        }
        
        /// <summary>
        /// Constructor for Fire Nap Am
        /// </summary>
        public NapAmComponent(ElementType elementType, FireNapAm napAm)
        {
            _elementType = elementType;
            _fireNapAm = napAm;
        }
        
        /// <summary>
        /// Constructor for Earth Nap Am
        /// </summary>
        public NapAmComponent(ElementType elementType, EarthNapAm napAm)
        {
            _elementType = elementType;
            _earthNapAm = napAm;
        }
        
        /// <summary>
        /// Legacy constructor using integer index (for backward compatibility)
        /// </summary>
        public NapAmComponent(ElementType elementType, int napAmIndex)
        {
            _elementType = elementType;
            
            // Convert the index to the appropriate enum
            switch (elementType)
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
        /// Get the NapAm name in English
        /// </summary>
        public string GetNapAmName()
        {
            switch (_elementType)
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
            switch (_elementType)
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
        /// Get the specific Nap Am for Metal element
        /// </summary>
        public MetalNapAm GetMetalNapAm()
        {
            return _metalNapAm;
        }
        
        /// <summary>
        /// Get the specific Nap Am for Wood element
        /// </summary>
        public WoodNapAm GetWoodNapAm()
        {
            return _woodNapAm;
        }
        
        /// <summary>
        /// Get the specific Nap Am for Water element
        /// </summary>
        public WaterNapAm GetWaterNapAm()
        {
            return _waterNapAm;
        }
        
        /// <summary>
        /// Get the specific Nap Am for Fire element
        /// </summary>
        public FireNapAm GetFireNapAm()
        {
            return _fireNapAm;
        }
        
        /// <summary>
        /// Get the specific Nap Am for Earth element
        /// </summary>
        public EarthNapAm GetEarthNapAm()
        {
            return _earthNapAm;
        }
        
        /// <summary>
        /// Get the NapAm index for backward compatibility
        /// </summary>
        public int GetNapAmIndex()
        {
            switch (_elementType)
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
        /// Get the NapAm effect power (1-5)
        /// This can be expanded to provide specific effects based on Nap Am type
        /// </summary>
        public int GetNapAmPower()
        {
            // Sample implementation based on the Nap Am type
            switch (_elementType)
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
                    // Similar implementation for Wood Nap Am
                    return ((int)_woodNapAm % 5) + 1;
                case ElementType.Water:
                    // Similar implementation for Water Nap Am
                    return ((int)_waterNapAm % 5) + 1;
                case ElementType.Fire:
                    // Similar implementation for Fire Nap Am
                    return ((int)_fireNapAm % 5) + 1;
                case ElementType.Earth:
                    // Similar implementation for Earth Nap Am
                    return ((int)_earthNapAm % 5) + 1;
                default:
                    return 1;
            }
        }
        
        /// <summary>
        /// Get the element type for this NapAm
        /// </summary>
        public ElementType GetElementType()
        {
            return _elementType;
        }
    }
}