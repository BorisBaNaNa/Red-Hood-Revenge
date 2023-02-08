using UnityEngine;

public class PlayerController : CharacterController2D
{
    [Header("Health settings")]
    public float MaxHealth;
    public float Health { get; private set; }

    private float _currentHealth;

    protected override void Awake()
    {
        base.Awake();
        Initialize();
    }

    protected void Update()
    {
        Vector2 dir = _input.Player.Move.ReadValue<Vector2>();
        CharacterMove(dir);
    }

    private void Initialize() { }
}