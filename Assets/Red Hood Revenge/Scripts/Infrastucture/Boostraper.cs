using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class Boostraper : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;
    [SerializeField]
    private CinemachineVirtualCamera _virtualCamera;

    void Start()
    {
        InitializeServices();

        GameStateMachine game = new GameStateMachine(_virtualCamera);
        game.StateSwitch<BoostraperState>();
    }

    private void InitializeServices()
    {
        AllServices.Instance.RegisterService(new FactoryPlayer(_playerPrefab));
    }
}
