namespace Systems.StatesMachine
{
    /// <summary>
    /// Defines the possible states for a battle
    /// </summary>
    public enum BattleStateType
    {
        Initialization,     // Initial setup of the battle
        PlayerTurnStart,    // Beginning of player's turn
        CardSelection,      // Player is selecting cards to play
        CardResolution,     // Cards are being resolved
        PlayerTurnEnd,      // End of player's turn
        EnemyTurnStart,     // Beginning of enemy's turn
        EnemyAction,        // Enemy performs actions
        EnemyTurnEnd,       // End of enemy's turn
        SupportCardCheck,   // Check and resolve support card activations
        BattleEnd,          // Battle has ended
        Reward              // Player receives rewards
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
}