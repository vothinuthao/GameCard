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
        None = 0,
        ElementalCard,
        SupportCard,
        // Monster,
        // SpiritAnimal,
        // Joker
    }
    
    /// <summary>
    /// Support card types
    /// </summary>
    public enum SupportCardType
    {
        DivineBeast,
        Monster,
        // SpiritAnimal,
        Artifact,
        Talisman,
        DivineWeapon
    }
    
    /// <summary>
    /// Activation condition types
    /// </summary>
    public enum ActivationConditionType
    {
        None,
        HealthPercent,
        ElementType,
        ElementCount,
        AllElements,
        AllElementsPlayed,
        Threshold,
        HandSize,
        EffectTargeted,
        DamageDealt,
        ElementCombo,
        TurnStart,
        TurnEnd
    }
    
    /// <summary>
    /// Effect types
    /// </summary>
    public enum EffectType
    {
        None,
        StatBuff,
        DamageOverTime,
        LifeSteal,
        Reflection,
        Harmony,
        Replay,
        ElementUnity,
        ElementReversal,
        Lightning,
        Thunder,
        Flood,
        Charm,
        Complex
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