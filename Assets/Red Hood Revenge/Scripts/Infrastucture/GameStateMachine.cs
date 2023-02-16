using Cinemachine;
using System.Collections.Generic;
using System.Linq;

public class GameStateMachine : IStateSwitcher  
{
    private readonly List<IState> _states;
    private IState _currentState;
    public GameStateMachine(CinemachineVirtualCamera virtualCamera)
    {
        _states = new List<IState>
        {
            new BoostraperState(this),
            new LoadLevelRecourceState(this, virtualCamera),
            new MenuState(this),
            new PlayingState(this),
            new DeadState(this),
            new FinishState(this),
        };
    }

    public void StateSwitch<TState>() where TState : IState
    {
        _currentState?.Exit();
        _currentState = _states.FirstOrDefault(state => state is TState);
        _currentState.Enter();
    }
}