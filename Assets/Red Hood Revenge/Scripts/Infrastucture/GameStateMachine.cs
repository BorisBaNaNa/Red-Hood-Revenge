using System.Collections.Generic;
using System.Linq;

public class GameStateMachine : IStateSwitcher  
{
    private readonly List<IState> _states;
    IState _currentState;
    public GameStateMachine(FactoryPlayer factoryPlayer)
    {
        _states = new List<IState>
        {
            new BoostraperState(this),
            new LoadLevelRecourceSate(this, factoryPlayer),
        };
    }

    public void StateSwitch<TState>() where TState : IState
    {
        _currentState?.Exit();
        _currentState = _states.FirstOrDefault(state => state is TState);
        _currentState.Enter();
    }
}
