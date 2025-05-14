using System.Collections.Generic;
using Systems;
using Systems.States;
using Systems.StatesMachine;
using UnityEngine;

/// <summary>
/// Manages the state machine for battles
/// Quản lý máy trạng thái cho các trận đấu
/// </summary>
public class BattleStateMachine
{
    // Trạng thái hiện tại
    // Current state
    private BattleState _currentState;
    
    // Tham chiếu đến hệ thống trận đấu
    // Reference to the battle system
    private BattleSystem _battleSystem;

    // Từ điển tất cả trạng thái khả dụng
    // Dictionary of all available states
    private Dictionary<BattleStateType, BattleState> _states = new Dictionary<BattleStateType, BattleState>();

    // Đại diện sự kiện
    // Event delegates
    public delegate void BattleStateChangedHandler(BattleStateType previousState, BattleStateType newState);
    public event BattleStateChangedHandler OnBattleStateChanged;

    // Constructor
    public BattleStateMachine(BattleSystem battleSystem)
    {
        _battleSystem = battleSystem;
        
        // Khởi tạo tất cả trạng thái trận đấu
        // Initialize all battle states
        InitializeStates();
    }

    /// <summary>
    /// Initialize all the possible battle states
    /// Khởi tạo tất cả các trạng thái trận đấu có thể
    /// </summary>
    private void InitializeStates()
    {
        // Thêm tất cả trạng thái vào từ điển
        // Add all states to the dictionary
        _states.Add(BattleStateType.Initialization, new InitializationState(this));
        _states.Add(BattleStateType.PlayerTurnStart, new PlayerTurnStartState(this));
        _states.Add(BattleStateType.CardSelection, new CardSelectionState(this));
        _states.Add(BattleStateType.CardResolution, new CardResolutionState(this));
        _states.Add(BattleStateType.PlayerTurnEnd, new PlayerTurnEndState(this));
        _states.Add(BattleStateType.EnemyTurnStart, new EnemyTurnStartState(this));
        _states.Add(BattleStateType.EnemyAction, new EnemyActionState(this));
        _states.Add(BattleStateType.EnemyTurnEnd, new EnemyTurnEndState(this));
        _states.Add(BattleStateType.SupportCardCheck, new SupportCardCheckState(this));
        _states.Add(BattleStateType.BattleEnd, new BattleEndState(this));
        _states.Add(BattleStateType.Reward, new RewardState(this));
    }

    /// <summary>
    /// Start the battle state machine
    /// Bắt đầu máy trạng thái trận đấu
    /// </summary>
    public void Start()
    {
        // Bắt đầu với trạng thái khởi tạo
        // Start with the initialization state
        ChangeState(BattleStateType.Initialization);
    }

    /// <summary>
    /// Update the current state
    /// Cập nhật trạng thái hiện tại
    /// </summary>
    public void Update()
    {
        if (_currentState != null)
        {
            _currentState.Update();
        }
    }

    /// <summary>
    /// Change to a new state
    /// Chuyển sang trạng thái mới
    /// </summary>
    public void ChangeState(BattleStateType newStateType)
    {
        // Thoát khỏi trạng thái hiện tại nếu nó tồn tại
        // Exit the current state if it exists
        if (_currentState != null)
        {
            BattleStateType previousStateType = GetCurrentStateType();
            _currentState.Exit();
            
            // Kích hoạt sự kiện thay đổi trạng thái
            // Fire state changed event
            OnBattleStateChanged?.Invoke(previousStateType, newStateType);
        }

        // Vào trạng thái mới
        // Enter the new state
        if (_states.TryGetValue(newStateType, out BattleState newState))
        {
            Debug.Log($"Battle State changing to: {newStateType}");
            _currentState = newState;
            _currentState.Enter();
        }
        else
        {
            Debug.LogError($"No state found for type: {newStateType}");
        }
    }

    /// <summary>
    /// Get the battle system
    /// Lấy hệ thống trận đấu
    /// </summary>
    public BattleSystem GetBattleSystem()
    {
        return _battleSystem;
    }

    /// <summary>
    /// Get the current state type
    /// Lấy loại trạng thái hiện tại
    /// </summary>
    public BattleStateType GetCurrentStateType()
    {
        foreach (var pair in _states)
        {
            if (pair.Value == _currentState)
            {
                return pair.Key;
            }
        }

        return BattleStateType.Initialization; // Default
    }
}

#region Battle State Implementations

/// <summary>
/// Base class for battle states
/// Lớp cơ sở cho các trạng thái trận đấu
/// </summary>
public abstract class BattleState
{
    protected BattleStateMachine StateMachine;

    public BattleState(BattleStateMachine stateMachine)
    {
        StateMachine = stateMachine;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}

/// <summary>
/// Battle initialization state
/// Trạng thái khởi tạo trận đấu
/// </summary>
public class InitializationState : BattleState
{
    public InitializationState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering InitializationState");
        
        // Khởi tạo trận đấu, rút bài, v.v.
        // Initialize the battle, draw cards, etc.
        StateMachine.GetBattleSystem().InitializeBattle();
        
        // Chuyển sang lượt của người chơi sau khi khởi tạo
        // Transition to player's turn after initialization
        StateMachine.ChangeState(BattleStateType.PlayerTurnStart);
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này vì nó chuyển ngay lập tức
        // Nothing to update in this state as it immediately transitions
    }

    public override void Exit()
    {
        Debug.Log("Exiting InitializationState");
    }
}

/// <summary>
/// Player turn start state
/// Trạng thái bắt đầu lượt người chơi
/// </summary>
public class PlayerTurnStartState : BattleState
{
    public PlayerTurnStartState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering PlayerTurnStartState");
        
        // Bắt đầu lượt người chơi - rút bài, đặt lại năng lượng, v.v.
        // Start player's turn - draw cards, reset energy, etc.
        StateMachine.GetBattleSystem().StartPlayerTurn();
        
        // Kiểm tra thẻ hỗ trợ kích hoạt khi bắt đầu lượt người chơi
        // Check for support cards that activate at the start of player's turn
        StateMachine.ChangeState(BattleStateType.SupportCardCheck);
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting PlayerTurnStartState");
    }
}

/// <summary>
/// Support card check state
/// Trạng thái kiểm tra thẻ hỗ trợ
/// </summary>
public class SupportCardCheckState : BattleState
{
    public SupportCardCheckState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering SupportCardCheckState");
        
        // Kiểm tra và kích hoạt bất kỳ thẻ hỗ trợ nào dựa trên điều kiện hiện tại
        // Check and activate any support cards based on current conditions
        StateMachine.GetBattleSystem().CheckSupportCards();
        
        // Chuyển sang lựa chọn thẻ sau khi kiểm tra thẻ hỗ trợ
        // Transition to card selection after support card checks
        BattleStateType currentState = StateMachine.GetCurrentStateType();
        
        // Nếu đến từ bắt đầu lượt người chơi, chuyển sang lựa chọn thẻ
        // If we came from player turn start, go to card selection
        if (currentState == BattleStateType.SupportCardCheck)
        {
            StateMachine.ChangeState(BattleStateType.CardSelection);
        }
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting SupportCardCheckState");
    }
}

/// <summary>
/// Card selection state
/// Trạng thái lựa chọn thẻ
/// </summary>
public class CardSelectionState : BattleState
{
    public CardSelectionState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering CardSelectionState");
        
        // Kích hoạt UI cho lựa chọn thẻ
        // Enable UI for card selection
        // Điều này thường được xử lý bởi trình quản lý UI
        // This would typically be handled by a UI manager
    }

    public override void Update()
    {
        // Kiểm tra xem người chơi đã chọn thẻ chưa
        // Check if player has selected cards
        // Điều này sẽ được kích hoạt bởi UI thông qua hệ thống trận đấu
        // This will be triggered by the UI through the battle system
        
        // Đối với hiện tại, chúng tôi sẽ chuyển thủ công sau một thời gian ngắn (để kiểm tra)
        // For now, we'll manually transition after a short time (for testing)
        // Trong triển khai thực tế, điều này sẽ được kích hoạt bởi lựa chọn thẻ của người chơi
        // In the real implementation, this would be triggered by the player's card selection
    }

    public override void Exit()
    {
        Debug.Log("Exiting CardSelectionState");
    }
}

/// <summary>
/// Card resolution state
/// Trạng thái giải quyết thẻ
/// </summary>
public class CardResolutionState : BattleState
{
    public CardResolutionState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering CardResolutionState");
        
        // Giải quyết các thẻ đã chọn
        // Resolve the selected cards
        StateMachine.GetBattleSystem().ResolveSelectedCards();
        
        // Kiểm tra xem trận đấu đã kết thúc chưa
        // Check if battle is over
        if (StateMachine.GetBattleSystem().IsBattleOver())
        {
            StateMachine.ChangeState(BattleStateType.BattleEnd);
        }
        else
        {
            // Nếu trận đấu chưa kết thúc, kết thúc lượt người chơi
            // If battle is not over, end player turn
            StateMachine.ChangeState(BattleStateType.PlayerTurnEnd);
        }
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting CardResolutionState");
    }
}

/// <summary>
/// Player turn end state
/// Trạng thái kết thúc lượt người chơi
/// </summary>
public class PlayerTurnEndState : BattleState
{
    public PlayerTurnEndState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering PlayerTurnEndState");
        
        // Kết thúc lượt người chơi - bỏ thẻ đã chơi, v.v.
        // End player's turn - discard played cards, etc.
        StateMachine.GetBattleSystem().EndPlayerTurn();
        
        // Chuyển sang lượt của kẻ địch
        // Transition to enemy's turn
        StateMachine.ChangeState(BattleStateType.EnemyTurnStart);
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting PlayerTurnEndState");
    }
}

/// <summary>
/// Enemy turn start state
/// Trạng thái bắt đầu lượt kẻ địch
/// </summary>
public class EnemyTurnStartState : BattleState
{
    public EnemyTurnStartState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering EnemyTurnStartState");
        
        // Bắt đầu lượt kẻ địch
        // Start enemy's turn
        StateMachine.GetBattleSystem().StartEnemyTurn();
        
        // Chuyển sang hành động kẻ địch
        // Transition to enemy action
        StateMachine.ChangeState(BattleStateType.EnemyAction);
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting EnemyTurnStartState");
    }
}

/// <summary>
/// Enemy action state
/// Trạng thái hành động kẻ địch
/// </summary>
public class EnemyActionState : BattleState
{
    public EnemyActionState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering EnemyActionState");
        
        // Kẻ địch thực hiện hành động của nó
        // Enemy performs its action
        StateMachine.GetBattleSystem().PerformEnemyAction();
        
        // Kiểm tra xem trận đấu đã kết thúc chưa
        // Check if battle is over
        if (StateMachine.GetBattleSystem().IsBattleOver())
        {
            StateMachine.ChangeState(BattleStateType.BattleEnd);
        }
        else
        {
            // Nếu trận đấu chưa kết thúc, kết thúc lượt kẻ địch
            // If battle is not over, end enemy turn
            StateMachine.ChangeState(BattleStateType.EnemyTurnEnd);
        }
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting EnemyActionState");
    }
}

/// <summary>
/// Enemy turn end state
/// Trạng thái kết thúc lượt kẻ địch
/// </summary>
public class EnemyTurnEndState : BattleState
{
    public EnemyTurnEndState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering EnemyTurnEndState");
        
        // Kết thúc lượt kẻ địch
        // End enemy's turn
        StateMachine.GetBattleSystem().EndEnemyTurn();
        
        // Chuyển lại sang lượt người chơi
        // Transition back to player's turn
        StateMachine.ChangeState(BattleStateType.PlayerTurnStart);
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting EnemyTurnEndState");
    }
}

/// <summary>
/// Battle end state
/// Trạng thái kết thúc trận đấu
/// </summary>
public class BattleEndState : BattleState
{
    public BattleEndState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering BattleEndState");
        
        // Xử lý logic kết thúc trận đấu
        // Handle battle end logic
        StateMachine.GetBattleSystem().EndBattle();
        
        // Nếu người chơi thắng, chuyển sang trạng thái phần thưởng
        // If player won, transition to reward state
        if (StateMachine.GetBattleSystem().GetWinner() == StateMachine.GetBattleSystem().GetPlayerEntity())
        {
            StateMachine.ChangeState(BattleStateType.Reward);
        }
        // Nếu không, trò chơi sẽ xử lý logic thất bại (không phải là một phần của máy trạng thái)
        // Otherwise, the game would handle defeat logic (not part of the state machine)
    }

    public override void Update()
    {
        // Không có gì để cập nhật trong trạng thái này
        // Nothing to update in this state
    }

    public override void Exit()
    {
        Debug.Log("Exiting BattleEndState");
    }
}

/// <summary>
/// Reward state
/// Trạng thái phần thưởng
/// </summary>
public class RewardState : BattleState
{
    public RewardState(BattleStateMachine stateMachine) : base(stateMachine) { }

    public override void Enter()
    {
        Debug.Log("Entering RewardState");
        
        // Xử lý logic phần thưởng
        // Handle reward logic
        StateMachine.GetBattleSystem().GiveRewards();
        
        // Người chơi thường sẽ chọn phần thưởng, sau đó trò chơi sẽ tiếp tục
        // The player would typically choose rewards, and then the game would continue
        // Không được xử lý trong máy trạng thái
        // Not handled within the state machine
    }

    public override void Update()
    {
        // Kiểm tra xem người chơi đã chọn phần thưởng chưa
        // Check if player has chosen rewards
        // Điều này sẽ được kích hoạt bởi UI
        // This would be triggered by the UI
    }

    public override void Exit()
    {
        Debug.Log("Exiting RewardState");
    }
}

#endregion