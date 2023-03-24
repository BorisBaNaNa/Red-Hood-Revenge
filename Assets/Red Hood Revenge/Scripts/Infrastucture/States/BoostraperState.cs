using UnityEngine;

internal class BoostraperState : IGameState
{
    readonly GameStateMachine _stateMachine;

    public BoostraperState(GameStateMachine stateMachine)
    {
        _stateMachine = stateMachine;
    }

    public void Enter()
    {
        _stateMachine.StateSwitch<MainMenuState>();
    }

    public void Exit()
    {
    }
}
