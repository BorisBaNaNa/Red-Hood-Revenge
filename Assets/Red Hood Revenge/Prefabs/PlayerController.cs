using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    public bool IsGraund;

    public LayerMask GraundMask;
    public float MoveSpeed = 3;
    [Range(0, 1)]
    public float inverseDrag = 0.9f;

    public float JumpHeight = 20;

    public float MaxHeath;
    public float Heath { get; private set; }

    private float _currentHeath;
    private InputActions _input;
    private CapsuleCollider2D _capsuleCollider;
    private Rigidbody2D _rigidbody;

    private void Awake()
    {
        Initialize();
        InitializeInput();

    }

    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void FixedUpdate()
    {
        Vector2 dir = _input.Player.Move.ReadValue<Vector2>();

        MoveAndApplyDrag(dir);

        UpdateIsGraund();
    }

    private void Initialize()
    {
        _input = new InputActions();
        _rigidbody = GetComponent<Rigidbody2D>();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void InitializeInput()
    {
        _input.Player.Jump.performed += _ => StartJump();
        _input.Player.Jump.canceled += _ => StopJump();
    }

    private void MoveAndApplyDrag(Vector2 dir)
    {
        var velocity = _rigidbody.velocity;
        velocity.x += dir.x * MoveSpeed * Time.fixedDeltaTime;
        velocity.x *= inverseDrag;
        _rigidbody.velocity = velocity;
    }

    private void UpdateIsGraund()
    {
        float radius = _capsuleCollider.size.x * 0.5f;
        Vector2 circleCenter = new(transform.position.x, transform.position.y + radius - 0.01f);
        IsGraund = Physics2D.OverlapCircle(circleCenter, radius, GraundMask);
    }

    private void StartJump()
    {
        if (!IsGraund) return;

        _rigidbody.AddForce(new Vector2(0, JumpHeight), ForceMode2D.Impulse);
    }

    private void StopJump() { }
}
