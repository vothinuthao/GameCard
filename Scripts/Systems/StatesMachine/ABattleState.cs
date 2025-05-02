namespace Systems.States
{
    /// <summary>
    /// Base class for battle states
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
}