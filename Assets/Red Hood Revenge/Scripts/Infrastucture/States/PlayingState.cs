public class PlayingState : IGameState
{
    private IStateSwitcher _stateSwitcher;

    public PlayingState(IStateSwitcher stateSwitcher)
    {
        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {
        AllServices.Instance.GetService<LevelManager>().StartTimer();
        AllServices.Instance.GetService<LevelManager>().Player.Inputs.Player.Enable();
    }

    public void Exit()
    {
        AllServices.Instance.GetService<LevelManager>().Player.Inputs.Player.Disable();
    }
}
