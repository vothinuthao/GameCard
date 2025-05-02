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
        // Current NapAm for this card
        private ElementType _elementType;
        private int _napAmIndex;
        
        // Constructor
        public NapAmComponent(ElementType elementType, int napAmIndex)
        {
            this._elementType = elementType;
            this._napAmIndex = napAmIndex;
        }
        
        /// <summary>
        /// Get the NapAm name in English
        /// </summary>
        public string GetNapAmName()
        {
            switch (_elementType)
            {
                case ElementType.Metal:
                    return Enum.GetName(typeof(MetalNapAm), _napAmIndex);
                case ElementType.Wood:
                    return Enum.GetName(typeof(WoodNapAm), _napAmIndex);
                case ElementType.Water:
                    return Enum.GetName(typeof(WaterNapAm), _napAmIndex);
                case ElementType.Fire:
                    return Enum.GetName(typeof(FireNapAm), _napAmIndex);
                case ElementType.Earth:
                    return Enum.GetName(typeof(EarthNapAm), _napAmIndex);
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
                    switch ((MetalNapAm)_napAmIndex)
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
                    switch ((WoodNapAm)_napAmIndex)
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
                    switch ((WaterNapAm)_napAmIndex)
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
                    switch ((FireNapAm)_napAmIndex)
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
                    switch ((EarthNapAm)_napAmIndex)
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
        /// Get the NapAm effect power (1-5)
        /// </summary>
        public int GetNapAmPower()
        {
            // This is a simplified implementation. In a full game, each NapAm would have its own power level.
            return (_napAmIndex % 3) + 1; // Values between 1 and 3 for simplicity
        }
        
        /// <summary>
        /// Get the element type for this NapAm
        /// </summary>
        public ElementType GetElementType()
        {
            return _elementType;
        }
        
        /// <summary>
        /// Get the NapAm index
        /// </summary>
        public int GetNapAmIndex()
        {
            return _napAmIndex;
        }
    }
}