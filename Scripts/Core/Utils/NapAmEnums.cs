// File: Scripts/Core/Utils/NapAmEnums.cs
using System;

namespace Core.Utils
{
    /// <summary>
    /// Nap Am types for Metal element (Kim)
    /// </summary>
    public enum MetalNapAm
    {
        SwordQi,    // Kiếm Khí - Offensive attribute
        Hardness,   // Cương Nghị - Defensive attribute
        Purity,     // Thanh Tịnh - Purification attribute
        Reflection, // Phản Chiếu - Reflection attribute
        Spirit,     // Linh Khí - Spirit attribute
        Calmness    // Trầm Tĩnh - Resistance attribute
    }

    /// <summary>
    /// Nap Am types for Wood element (Mộc)
    /// </summary>
    public enum WoodNapAm
    {
        Growth,      // Sinh Trưởng - Health regeneration attribute
        Flexibility, // Linh Hoạt - Speed and evasion attribute
        Symbiosis,   // Cộng Sinh - Synergy attribute
        Regeneration,// Tái Sinh - Healing attribute
        Toxin,       // Độc Tố - Damage over time attribute
        Shelter      // Che Chắn - Protection attribute
    }

    /// <summary>
    /// Nap Am types for Water element (Thủy)
    /// </summary>
    public enum WaterNapAm
    {
        Adaptation,  // Thích Nghi - Adaptation attribute
        Ice,         // Băng Giá - Freezing attribute
        Flow,        // Dòng Chảy - Flow attribute
        Mist,        // Sương Mù - Mist attribute
        Reflection,  // Phản Ánh - Reflection attribute
        Purification // Thanh Tẩy - Purification attribute
    }

    /// <summary>
    /// Nap Am types for Fire element (Hỏa)
    /// </summary>
    public enum FireNapAm
    {
        Burning,     // Thiêu Đốt - Burning attribute
        Explosion,   // Bùng Nổ - Explosion attribute
        Passion,     // Nhiệt Huyết - Passion attribute
        Light,       // Ánh Sáng - Light attribute
        Forging,     // Rèn Luyện - Forging attribute
        Incineration // Thiêu Rụi - Incineration attribute
    }

    /// <summary>
    /// Nap Am types for Earth element (Thổ)
    /// </summary>
    public enum EarthNapAm
    {
        Solidity,  // Kiên Cố - Solidity attribute
        Gravity,   // Trọng Lực - Gravity attribute
        Fertility, // Màu Mỡ - Fertility attribute
        Volcano,   // Núi Lửa - Volcano attribute
        Crystal,   // Tinh Thể - Crystal attribute
        Terra      // Đại Địa - Terra attribute
    }
}