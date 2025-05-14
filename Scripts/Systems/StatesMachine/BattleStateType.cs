namespace Systems.StatesMachine
{
    /// <summary>
    /// Defines the possible states for a battle
    /// Định nghĩa các trạng thái có thể cho một trận đấu
    /// </summary>
    public enum BattleStateType
    {
        Initialization,     // Initial setup of the battle - Thiết lập ban đầu của trận đấu
        PlayerTurnStart,    // Beginning of player's turn - Bắt đầu lượt của người chơi
        CardSelection,      // Player is selecting cards to play - Người chơi đang chọn thẻ để chơi
        CardResolution,     // Cards are being resolved - Các thẻ đang được giải quyết
        PlayerTurnEnd,      // End of player's turn - Kết thúc lượt của người chơi
        EnemyTurnStart,     // Beginning of enemy's turn - Bắt đầu lượt của kẻ địch
        EnemyAction,        // Enemy performs actions - Kẻ địch thực hiện hành động
        EnemyTurnEnd,       // End of enemy's turn - Kết thúc lượt của kẻ địch
        SupportCardCheck,   // Check and resolve support card activations - Kiểm tra và giải quyết kích hoạt thẻ hỗ trợ
        BattleEnd,          // Battle has ended - Trận đấu đã kết thúc
        Reward              // Player receives rewards - Người chơi nhận phần thưởng
    }
    
    /// <summary>
    /// Support card activation types
    /// Các loại kích hoạt thẻ hỗ trợ
    /// </summary>
    public enum ActivationType
    {
        OnEntry,        // Kích Hoạt Khi Vào Game - Activate when entering battle
        Persistent,     // Duy Trì Khi Diễn Ra Tính Điểm - Persistent during score calculation
        Recurring,      // Lặp Lại Khi Đạt Điều Kiện - Repeat when condition is met
        Triggered,      // Kích Hoạt Khi Đạt Điều Kiện - Activate when condition is met
        Reactive,       // Phản Ứng - Reactive
        Transformative  // Biến Đổi - Transformative
    }
}