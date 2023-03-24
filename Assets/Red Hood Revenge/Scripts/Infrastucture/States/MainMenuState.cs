public class MainMenuState : IGameState
{
    private IStateSwitcher _stateSwitcher;

    public MainMenuState(IStateSwitcher stateSwitcher)
    {
        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {

    }

    public void Exit()
    {

    }
}
