using UnityEngine;

internal class BoostraperState : IState
{
    IStateSwitcher _stateSwitcher;

    public BoostraperState(IStateSwitcher stateSwitcher)
    {
        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {
        _stateSwitcher.StateSwitch<LoadLevelRecourceState>();
    }

    public void Exit()
    {
    }
}
