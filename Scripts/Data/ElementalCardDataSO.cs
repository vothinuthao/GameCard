// File: Scripts/Data/ElementalCardDataSO.cs
using Core.Utils;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Scriptable object for elemental cards with Nap Am specialization
    /// </summary>
    [CreateAssetMenu(fileName = "New Elemental Card", menuName = "NguhanhGame/Elemental Card Data")]
    public class ElementalCardDataSO : CardDataSO
    {
        [Header("Nạp Âm Configuration")]
        [Tooltip("The element type of this card")]
        public ElementType elementType = ElementType.None;

        [Tooltip("The Metal Nap Am type (only used when elementType is Metal)")]
        public MetalNapAm metalNapAm = MetalNapAm.SwordQi;

        [Tooltip("The Wood Nap Am type (only used when elementType is Wood)")]
        public WoodNapAm woodNapAm = WoodNapAm.Growth;

        [Tooltip("The Water Nap Am type (only used when elementType is Water)")]
        public WaterNapAm waterNapAm = WaterNapAm.Adaptation;

        [Tooltip("The Fire Nap Am type (only used when elementType is Fire)")]
        public FireNapAm fireNapAm = FireNapAm.Burning;

        [Tooltip("The Earth Nap Am type (only used when elementType is Earth)")]
        public EarthNapAm earthNapAm = EarthNapAm.Solidity;

        /// <summary>
        /// Get the appropriate Nap Am index based on element type
        /// </summary>
        public int GetNapAmIndex()
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    return (int)metalNapAm;
                case ElementType.Wood:
                    return (int)woodNapAm;
                case ElementType.Water:
                    return (int)waterNapAm;
                case ElementType.Fire:
                    return (int)fireNapAm;
                case ElementType.Earth:
                    return (int)earthNapAm;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Get the Vietnamese name of the Nap Am
        /// </summary>
        public string GetNapAmVietnameseName()
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    switch (metalNapAm)
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
                    switch (woodNapAm)
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
                    switch (waterNapAm)
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
                    switch (fireNapAm)
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
                    switch (earthNapAm)
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
        /// Get the English name of the Nap Am
        /// </summary>
        public string GetNapAmEnglishName()
        {
            switch (elementType)
            {
                case ElementType.Metal:
                    return metalNapAm.ToString();
                case ElementType.Wood:
                    return woodNapAm.ToString();
                case ElementType.Water:
                    return waterNapAm.ToString();
                case ElementType.Fire:
                    return fireNapAm.ToString();
                case ElementType.Earth:
                    return earthNapAm.ToString();
                default:
                    return "Unknown";
            }
        }
    }
}