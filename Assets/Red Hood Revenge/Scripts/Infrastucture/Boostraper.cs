using System;
using System.Collections;
using UnityEngine;

public class Boostraper : MonoBehaviour
{
    [SerializeField]
    private GameObject _playerPrefab;

    void Start()
    {
        InitializeServices();

        GameStateMachine game = new(AllServices.Instance.GetService<FactoryPlayer>());
        game.StateSwitch<BoostraperState>();
    }

    private void InitializeServices()
    {
        AllServices.Instance.RegisterService(new FactoryPlayer(_playerPrefab));
    }
}
