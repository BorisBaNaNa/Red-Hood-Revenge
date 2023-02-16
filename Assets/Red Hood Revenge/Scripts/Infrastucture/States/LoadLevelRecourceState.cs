using UnityEngine;
using Cinemachine;

internal class LoadLevelRecourceState : IState
{
    private IStateSwitcher _stateSwitcher;
    private FactoryPlayer _factoryPlayer;
    CinemachineVirtualCamera _virtualCamera;

    public LoadLevelRecourceState(IStateSwitcher stateSwitcher, CinemachineVirtualCamera virtualCamera)
    {
        _factoryPlayer = AllServices.Instance.GetService<FactoryPlayer>();

        _virtualCamera = virtualCamera;
        _stateSwitcher = stateSwitcher;
    }

    public void Enter()
    {
        GameObject player = CreatePlayer();

        _virtualCamera.Follow = player.transform;
        _virtualCamera.LookAt = player.transform;
    }

    public void Exit()
    {
    }

    private GameObject CreatePlayer()
    {
        Vector3 spawnPoint = GameObject.FindGameObjectWithTag("LevelStart").transform.position;
        return _factoryPlayer.BuildPlayer(spawnPoint);
    }
}
