using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    public bool IsJumping => _velocity.y > 0f;
    public bool IsFalling => _velocity.y < 0f;
    public bool IsGraund;

    public LayerMask GraundMask;
    public float MoveSpeed = 3;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float MaxJumpHeight = 5;
    public float MinJumpHeight = 0.5f;

    public float MaxHeath;
    public float Heath { get; private set; }

    private float _currentHeath;
    private InputActions _input;
    private Vector2 _velocity;
    private float _velocityXSmoothing;
    private CapsuleCollider2D _capsuleCollider;

    private void Awake()
    {
        Initialize();

        _input.Player.Jump.performed += _ => StartJump(); 
        _input.Player.Jump.canceled += _ => StopJump(); 
    }

    private void OnEnable()
    {
        _input.Player.Enable();
    }

    private void OnDisable()
    {
        _input.Player.Disable();
    }

    private void Update()
    {
        Vector2 dir = _input.Player.Move.ReadValue<Vector2>();

        ApplySmooth(dir);
        ApplyGravity();

        transform.Translate(_velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        UpdateIsGraund();
    }

    private void UpdateIsGraund()
    {
        float radius = _capsuleCollider.size.x * 0.5f;
        Vector2 circleCenter = new(transform.position.x, transform.position.y + radius - 0.01f);
        IsGraund = Physics2D.OverlapCircle(circleCenter, radius, GraundMask);

        //Vector2 circleCenter = transform.TransformPoint(new Vector3(0, _capsuleCollider.size.x));
        //IsGraund = Physics2D.CircleCast(circleCenter, _capsuleCollider.size.x, -Vector2.up, 0.01f);
    }

    private void ApplySmooth(Vector2 dir)
    {
        float targetVelocityX = dir.x * MoveSpeed;
        _velocity.x = Mathf.SmoothDamp(_velocity.x, targetVelocityX, ref _velocityXSmoothing, IsGraund ? accelerationTimeGrounded : accelerationTimeAirborne);
    }

    private void ApplyGravity()
    {
        if (!IsJumping && !IsGraund)
            _velocity.y = 0f;
        else
            _velocity += Physics2D.gravity * 2 * Time.deltaTime;
    }

    private void StartJump()
    {
        if (!IsGraund) return;

        _velocity.y += Mathf.Sqrt(MaxJumpHeight * -Physics2D.gravity.y);
    }

    private void StopJump()
    {
        if (!IsJumping || IsGraund) return;

        _velocity.y = 0;
    }

    private void Initialize()
    {
        _input = new InputActions();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();

    }
}
