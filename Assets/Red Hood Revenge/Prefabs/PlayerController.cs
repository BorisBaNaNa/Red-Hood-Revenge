using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.Windows;

public class PlayerController : MonoBehaviour
{
    public bool IsFlaying => _velocity.y > 0f;
    public bool IsFalling => _velocity.y < 0f;
    public bool IsGraund;

    [Header("Ground settings")]
    float maxClimbAngle = 80;
    float maxDescendAngle = 80;
    public LayerMask GraundMask;

    [Header("Move settings")]
    public float MoveSpeed = 3;
    public float accelerationTimeAirborne = .2f;
    public float accelerationTimeGrounded = .1f;

    public float MaxJumpHeight = 5;
    public float MinJumpHeight = 0.5f;

    [Header("Health settings")]
    public float MaxHealth;
    public float Health { get; private set; }

    private float _currentHealth;
    private InputActions _input;
    private Vector2 _velocity;
    private float _velocityXSmoothing;
    private CapsuleCollider2D _capsuleCollider;
    private Rigidbody2D _rb;

    private void Awake()
    {
        Initialize();
        _rb = GetComponent<Rigidbody2D>();

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
        ApplyCollision();

        transform.Translate(_velocity * Time.deltaTime);
    }

    private void FixedUpdate()
    {
        UpdateIsGraund();
    }

    private void Initialize()
    {
        _input = new InputActions();
        _capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    private void UpdateIsGraund()
    {
        float radius = _capsuleCollider.size.x * 0.5f;
        Vector2 circleCenter = new(transform.position.x, transform.position.y + radius - 0.01f);
        Collider2D[] hits = Physics2D.OverlapCircleAll(circleCenter, radius, GraundMask);
        if (hits.Length == 0)
        {
            IsGraund = false;
            return;
        }

        foreach (Collider2D hit in hits)
        {
            float descendAngle = Vector2.Angle(hit.Distance(_capsuleCollider).normal, Vector2.up);
            IsGraund = descendAngle < maxDescendAngle && _velocity.y <= 0;
            if (IsGraund) return;
        }

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
        if (!IsFlaying && IsGraund)
            _velocity.y = 0f;
        else
            _velocity += Physics2D.gravity * 2 * Time.deltaTime;
    }

    private void StartJump()
    {
        if (!IsGraund) return;

        _velocity.y += Mathf.Sqrt(2 * MaxJumpHeight * Mathf.Abs(Physics2D.gravity.y));
    }

    private void StopJump()
    {
        if (!IsFlaying || IsGraund) return;

        float minJumpVelocity = Mathf.Sqrt(2 * MinJumpHeight * Mathf.Abs(Physics2D.gravity.y));
        if (_velocity.y > minJumpVelocity)
            _velocity.y = minJumpVelocity;
    }

    private void ApplyCollision()
    {
        Vector2 CapsuleCenter = new Vector2(transform.position.x, transform.position.y + (_capsuleCollider.size.y * 0.5f));
        Collider2D[] hits = Physics2D.OverlapCapsuleAll(CapsuleCenter, _capsuleCollider.size, CapsuleDirection2D.Vertical, 0);

        foreach (Collider2D hit in hits)
        {
            if (hit == _capsuleCollider) continue;

            ColliderDistance2D colliderDistance = hit.Distance(_capsuleCollider);
            if (colliderDistance.isOverlapped)
                transform.Translate(colliderDistance.pointA - colliderDistance.pointB);
        }
    }

}

class CharacterController2D
{

}
