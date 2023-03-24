using System;
using UnityEngine;

public class GameManager : MonoBehaviour, IService
{
    public GameStateMachine StateMachine { get; private set; }
    public int LivesCount => _livesCount;
    public int Coin => _coin;
    public int Bullet => _bullet;

    public bool IsHaveNotLives;
    public string CurrentGameState;

    [SerializeField]
    private int _defaultCoinCount = 25;
    [SerializeField]
    private int _defaultBulletCount = 15;
    [SerializeField]
    private int _defaultLivesCount = 10;

    private int _coin;
    private int _bullet;
    private int _livesCount;

    public void Awake()
    {
        //AllServices.Instance.RegisterService(this);
        DontDestroyOnLoad(gameObject);
        SetupValues();
    }

    public void Update()
    {
        CurrentGameState = StateMachine.CurrentState;
    }

    public void Initialize(GameStateMachine gameStateMachine)
    {
        StateMachine = gameStateMachine;
    }

    public void StartGame()
    {
        SwichGameState<PlayingState>();
        IsHaveNotLives = false;
    }

    public void GameOver()
    {
        AllServices.Instance.GetService<LevelManager>().PlayerWasKilled();
        AllServices.Instance.GetService<MenuManager>().GameOver();
        SoundManager.PlaySfx(AllServices.Instance.GetService<SoundManager>().SoundGameover);

        if (--_livesCount <= 0)
        {
            // Добавить механику постепенного восстановления жизней
            SetupValues();
            IsHaveNotLives = true;
        }
    }

    public void GameFinish()
    {
        AllServices.Instance.GetService<MenuManager>().GameFinish();
        SoundManager.PlaySfx(AllServices.Instance.GetService<SoundManager>().SoundGamefinish);

        LevelManager levelManager = AllServices.Instance.GetService<LevelManager>();
        _coin += levelManager.Coin;

        // Добавить разблокировку уровней
    }

    public static void SwichGameState<TState>() where TState : IGameState =>
        AllServices.Instance.GetService<GameManager>().StateMachine.StateSwitch<TState>();

    private void SetupValues()
    {
        _coin = _defaultCoinCount;
        _bullet = _defaultBulletCount;
        _livesCount = _defaultLivesCount;
    }
}
