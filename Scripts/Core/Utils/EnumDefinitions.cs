// EnumDefinitions.cs
namespace Core.Utils
{
    /// <summary>
    /// Element types (Ngũ Hành)
    /// </summary>
    public enum ElementType
    {
        None = 0,
        Metal,  // Kim
        Wood,   // Mộc
        Water,  // Thủy
        Fire,   // Hỏa
        Earth   // Thổ
    }
    
    /// <summary>
    /// Card rarity
    /// </summary>
    public enum Rarity
    {
        Common,     // Phàm
        Rare,       // Linh
        Epic,       // Tiên
        Legendary   // Thần
    }
    
    /// <summary>
    /// Card types
    /// </summary>
    public enum CardType
    {
        ElementalCard,      // Thẻ Nguyên Tố
        DivineBeast,        // Thẻ Thần Thú
        Monster,            // Thẻ Yêu Quái
        SpiritAnimal,       // Thẻ Linh Thú
        Joker               // Thẻ Joker
    }
    
    /// <summary>
    /// NapAm types for Metal element
    /// </summary>
    public enum MetalNapAm
    {
        SwordQi,        // Kiếm Khí
        Hardness,       // Cương Nghị
        Purity,         // Thanh Tịnh
        Reflection,     // Phản Chiếu
        Spirit,         // Linh Khí
        Calmness        // Trầm Tĩnh
    }
    
    /// <summary>
    /// NapAm types for Wood element
    /// </summary>
    public enum WoodNapAm
    {
        Growth,         // Sinh Trưởng
        Flexibility,    // Linh Hoạt
        Symbiosis,      // Cộng Sinh
        Regeneration,   // Tái Sinh
        Toxin,          // Độc Tố
        Shelter         // Che Chắn
    }
    
    /// <summary>
    /// NapAm types for Water element
    /// </summary>
    public enum WaterNapAm
    {
        Adaptation,     // Thích Nghi
        Ice,            // Băng Giá
        Flow,           // Dòng Chảy
        Mist,           // Sương Mù
        Reflection,     // Phản Ánh
        Purification    // Thanh Tẩy
    }
    
    /// <summary>
    /// NapAm types for Fire element
    /// </summary>
    public enum FireNapAm
    {
        Burning,        // Thiêu Đốt
        Explosion,      // Bùng Nổ
        Passion,        // Nhiệt Huyết
        Light,          // Ánh Sáng
        Forging,        // Rèn Luyện
        Incineration    // Thiêu Rụi
    }
    
    /// <summary>
    /// NapAm types for Earth element
    /// </summary>
    public enum EarthNapAm
    {
        Solidity,       // Kiên Cố
        Gravity,        // Trọng Lực
        Fertility,      // Màu Mỡ
        Volcano,        // Núi Lửa
        Crystal,        // Tinh Thể
        Terra           // Đại Địa
    }
    
    /// <summary>
    /// Support card activation types
    /// </summary>
    public enum ActivationType
    {
        OnEntry,        // Kích Hoạt Khi Vào Game
        Persistent,     // Duy Trì Khi Diễn Ra Tính Điểm
        Recurring,      // Lặp Lại Khi Đạt Điều Kiện
        Triggered,      // Kích Hoạt Khi Đạt Điều Kiện
        Reactive,       // Phản Ứng
        Transformative  // Biến Đổi
    }
    
    /// <summary>
    /// Seasons
    /// </summary>
    public enum Season
    {
        Spring,     // Xuân
        Summer,     // Hạ
        Autumn,     // Thu
        Winter      // Đông
    }
    
    /// <summary>
    /// Card state in the game
    /// </summary>
    public enum CardState
    {
        InDeck,
        InHand,
        InPlay,
        InDiscardPile,
        InSupportZone,
        Removed
    }
}