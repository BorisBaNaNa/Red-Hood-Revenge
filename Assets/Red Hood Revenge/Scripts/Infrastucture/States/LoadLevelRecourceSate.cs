using UnityEngine;

internal class LoadLevelRecourceSate : IState
{
    private IStateSwitcher _stateSwitcher;
    private FactoryPlayer _factoryPlayer;

    public LoadLevelRecourceSate(IStateSwitcher stateSwitcher, FactoryPlayer factoryPlayer)
    {
        _stateSwitcher = stateSwitcher;
        _factoryPlayer = factoryPlayer;
    }

    public void Enter()
    {
        CreatePlayer();
    }

    public void Exit()
    {
    }

    private void CreatePlayer()
    {
        Vector3 spawnPoint = GameObject.FindGameObjectWithTag("LevelStart").transform.position;
        _factoryPlayer.BuildPlayer(spawnPoint);
    }
}
