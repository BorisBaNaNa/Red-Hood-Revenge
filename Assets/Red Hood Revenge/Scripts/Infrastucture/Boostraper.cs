using Cinemachine;
using System;
using System.Collections;
using UnityEngine;

public class Boostraper : MonoBehaviour
{
    [SerializeField]
    private Player _playerPrefab;
    [SerializeField]
    private Projectile _projectilePrefab;
    [SerializeField]
    private SoundManager _soundManagerPrefab;
    [SerializeField]
    private GameManager _gameManagerPrefab;

    private GameStateMachine _stateMachine;
    private GameManager _gameManager;

    public void Awake()
    {
        if (AllServices.Instance.GetService<GameManager>() != null)
        {
            Debug.Log("Resourses already initialized! Boostraper was destroyed...");
            Destroy(gameObject);
            return;
        }

        InitializeServices();
        InitializeGame();
        _stateMachine.StateSwitch<BoostraperState>();
    }

    private void InitializeGame()
    {
        _stateMachine = new GameStateMachine();

        _gameManager = Instantiate(_gameManagerPrefab);
        _gameManager.Initialize(_stateMachine);
        AllServices.Instance.RegisterService(_gameManager);

        Instantiate(_soundManagerPrefab);
    }

    private void InitializeServices()
    {
        AllServices.Instance.RegisterService(new FactoryPlayer(_playerPrefab));
        AllServices.Instance.RegisterService(new FactoryProjectile(_projectilePrefab));
    }
}
