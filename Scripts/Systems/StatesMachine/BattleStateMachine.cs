using System.Collections.Generic;
using Systems;
using Systems.States;
using UnityEngine;

/// <summary>
    /// Manages the state machine for battles
    /// </summary>
    public class BattleStateMachine
    {
        // Current state
        private BattleState _currentState;
        
        // Reference to the battle system
        private BattleSystem _battleSystem;

        // Dictionary of all available states
        private Dictionary<BattleStateType, BattleState> _states = new Dictionary<BattleStateType, BattleState>();

        // Event delegates
        public delegate void BattleStateChangedHandler(BattleStateType previousState, BattleStateType newState);
        public event BattleStateChangedHandler OnBattleStateChanged;

        // Constructor
        public BattleStateMachine(BattleSystem battleSystem)
        {
            _battleSystem = battleSystem;
            
            // Initialize all battle states
            InitializeStates();
        }

        /// <summary>
        /// Initialize all the possible battle states
        /// </summary>
        private void InitializeStates()
        {
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
        /// </summary>
        public void Start()
        {
            // Start with the initialization state
            ChangeState(BattleStateType.Initialization);
        }

        /// <summary>
        /// Update the current state
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
        /// </summary>
        public void ChangeState(BattleStateType newStateType)
        {
            // Exit the current state if it exists
            if (_currentState != null)
            {
                BattleStateType previousStateType = GetCurrentStateType();
                _currentState.Exit();
                
                // Fire state changed event
                OnBattleStateChanged?.Invoke(previousStateType, newStateType);
            }

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
        /// </summary>
        public BattleSystem GetBattleSystem()
        {
            return _battleSystem;
        }

        /// <summary>
        /// Get the current state type
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
    /// Battle initialization state
    /// </summary>
    public class InitializationState : BattleState
    {
        public InitializationState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering InitializationState");
            
            // Initialize the battle, draw cards, etc.
            // This would typically be handled by the battle system
            StateMachine.GetBattleSystem().InitializeBattle();
            
            // Transition to player's turn after initialization
            StateMachine.ChangeState(BattleStateType.PlayerTurnStart);
        }

        public override void Update()
        {
            // Nothing to update in this state as it immediately transitions
        }

        public override void Exit()
        {
            Debug.Log("Exiting InitializationState");
        }
    }

    /// <summary>
    /// Player turn start state
    /// </summary>
    public class PlayerTurnStartState : BattleState
    {
        public PlayerTurnStartState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering PlayerTurnStartState");
            
            // Start player's turn - draw cards, reset energy, etc.
            StateMachine.GetBattleSystem().StartPlayerTurn();
            
            // Check for support cards that activate at the start of player's turn
            StateMachine.ChangeState(BattleStateType.SupportCardCheck);
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting PlayerTurnStartState");
        }
    }

    /// <summary>
    /// Support card check state
    /// </summary>
    public class SupportCardCheckState : BattleState
    {
        public SupportCardCheckState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering SupportCardCheckState");
            
            // Check and activate any support cards based on current conditions
            StateMachine.GetBattleSystem().CheckSupportCards();
            
            // Transition to card selection after support card checks
            BattleStateType currentState = StateMachine.GetCurrentStateType();
            
            // If we came from player turn start, go to card selection
            if (currentState == BattleStateType.SupportCardCheck)
            {
                StateMachine.ChangeState(BattleStateType.CardSelection);
            }
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting SupportCardCheckState");
        }
    }

    /// <summary>
    /// Card selection state
    /// </summary>
    public class CardSelectionState : BattleState
    {
        public CardSelectionState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering CardSelectionState");
            
            // Enable UI for card selection
            // This would typically be handled by a UI manager
        }

        public override void Update()
        {
            // Check if player has selected cards
            // This will be triggered by the UI through the battle system
            
            // For now, we'll manually transition after a short time (for testing)
            // In the real implementation, this would be triggered by the player's card selection
        }

        public override void Exit()
        {
            Debug.Log("Exiting CardSelectionState");
        }
    }

    /// <summary>
    /// Card resolution state
    /// </summary>
    public class CardResolutionState : BattleState
    {
        public CardResolutionState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering CardResolutionState");
            
            // Resolve the selected cards
            StateMachine.GetBattleSystem().ResolveSelectedCards();
            
            // Check if battle is over
            if (StateMachine.GetBattleSystem().IsBattleOver())
            {
                StateMachine.ChangeState(BattleStateType.BattleEnd);
            }
            else
            {
                // If battle is not over, end player turn
                StateMachine.ChangeState(BattleStateType.PlayerTurnEnd);
            }
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting CardResolutionState");
        }
    }

    /// <summary>
    /// Player turn end state
    /// </summary>
    public class PlayerTurnEndState : BattleState
    {
        public PlayerTurnEndState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering PlayerTurnEndState");
            
            // End player's turn - discard played cards, etc.
            StateMachine.GetBattleSystem().EndPlayerTurn();
            
            // Transition to enemy's turn
            StateMachine.ChangeState(BattleStateType.EnemyTurnStart);
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting PlayerTurnEndState");
        }
    }

    /// <summary>
    /// Enemy turn start state
    /// </summary>
    public class EnemyTurnStartState : BattleState
    {
        public EnemyTurnStartState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering EnemyTurnStartState");
            
            // Start enemy's turn
            StateMachine.GetBattleSystem().StartEnemyTurn();
            
            // Transition to enemy action
            StateMachine.ChangeState(BattleStateType.EnemyAction);
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting EnemyTurnStartState");
        }
    }

    /// <summary>
    /// Enemy action state
    /// </summary>
    public class EnemyActionState : BattleState
    {
        public EnemyActionState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering EnemyActionState");
            
            // Enemy performs its action
            StateMachine.GetBattleSystem().PerformEnemyAction();
            
            // Check if battle is over
            if (StateMachine.GetBattleSystem().IsBattleOver())
            {
                StateMachine.ChangeState(BattleStateType.BattleEnd);
            }
            else
            {
                // If battle is not over, end enemy turn
                StateMachine.ChangeState(BattleStateType.EnemyTurnEnd);
            }
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting EnemyActionState");
        }
    }

    /// <summary>
    /// Enemy turn end state
    /// </summary>
    public class EnemyTurnEndState : BattleState
    {
        public EnemyTurnEndState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering EnemyTurnEndState");
            
            // End enemy's turn
            StateMachine.GetBattleSystem().EndEnemyTurn();
            
            // Transition back to player's turn
            StateMachine.ChangeState(BattleStateType.PlayerTurnStart);
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting EnemyTurnEndState");
        }
    }

    /// <summary>
    /// Battle end state
    /// </summary>
    public class BattleEndState : BattleState
    {
        public BattleEndState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering BattleEndState");
            
            // Handle battle end logic
            StateMachine.GetBattleSystem().EndBattle();
            
            // If player won, transition to reward state
            if (StateMachine.GetBattleSystem().GetWinner() == StateMachine.GetBattleSystem().GetPlayerEntity())
            {
                StateMachine.ChangeState(BattleStateType.Reward);
            }
            // Otherwise, the game would handle defeat logic (not part of the state machine)
        }

        public override void Update()
        {
            // Nothing to update in this state
        }

        public override void Exit()
        {
            Debug.Log("Exiting BattleEndState");
        }
    }

    /// <summary>
    /// Reward state
    /// </summary>
    public class RewardState : BattleState
    {
        public RewardState(BattleStateMachine stateMachine) : base(stateMachine) { }

        public override void Enter()
        {
            Debug.Log("Entering RewardState");
            
            // Handle reward logic
            StateMachine.GetBattleSystem().GiveRewards();
            
            // The player would typically choose rewards, and then the game would continue
            // Not handled within the state machine
        }

        public override void Update()
        {
            // Check if player has chosen rewards
            // This would be triggered by the UI
        }

        public override void Exit()
        {
            Debug.Log("Exiting RewardState");
        }
    }

    #endregion
