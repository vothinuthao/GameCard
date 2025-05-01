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
        /// Get the NapAm name
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
                    }
                    break;
                case ElementType.Wood:
                    switch ((WoodNapAm)_napAmIndex)
                    {
                        case WoodNapAm.Growth: return "Sinh Trưởng";
                        case WoodNapAm.Flexibility: return "Linh Hoạt";
                        case WoodNapAm.Symbiosis: return "Cộng Sinh";
                        case WoodNapAm.Regeneration: return "Tái Sinh";
                        case WoodNapAm.Toxin: return "Độc Tố";
                        case WoodNapAm.Shelter: return "Che Chắn";
                    }
                    break;
                // Similar cases for other elements...
            }
            return "Unknown";
        }
        
        /// <summary>
        /// Get the NapAm effect power (1-5)
        /// </summary>
        public int GetNapAmPower()
        {
            // This is a simplified implementation. In a full game, each NapAm would have its own power level.
            return (_napAmIndex % 3) + 1; // Values between 1 and 3 for simplicity
        }
    }
}