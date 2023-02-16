#define NOT_IS_DEL_TRASH

using UnityEngine;
using System.Collections;
using Unity.Collections;
using static UnityEditor.Experimental.GraphView.GraphView;
using System.Runtime.CompilerServices;

[RequireComponent(typeof(Controller2D)),]
public class Player : MonoBehaviour, ICanTakeDamage
{
    #region PlayerStates
    public class IdleState : IState
    {
        private PlayerStateMachine _stateMachine;

        public IdleState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Player.AnimController.PlayStableAnimation(this);
        }

        public void Exit() { }
    }

    public class WalkState : IState
    {
        private PlayerStateMachine _stateMachine;

        public WalkState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Player.AnimController.PlayStableAnimation(this);
        }

        public void Exit() { }
    }

    public class CrouchState : IState
    {
        private PlayerStateMachine _stateMachine;

        public CrouchState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Player.controller.Crouch(true);
            _stateMachine.Player.AnimController.PlayStableAnimation(this);
        }

        public void Exit()
        {
            _stateMachine.Player.controller.Crouch(false);
            _stateMachine.Player.AnimController.PlayExitStateAnimation(this);
        }
    }

    public class JumpState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;


        public JumpState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            _player.AnimController.PlayOneTimeAnimation(this);

            if (_player.IsSliding)
            {
                JumpSliding();
            }
            else if (_player.controller.collisions.below)
            {
                Jump(_player.maxJumpVelocity);
                _player.numberOfJumpLeft = _player.numberOfJumpMax;
            }
            else if (--_player.numberOfJumpLeft > 0)
            {
                Jump(_player.minJumpVelocity);
            }
        }

        public void Exit() { }

        private void Jump(float jumpVelocity)
        {
            _player.velocity.y = jumpVelocity;

            if (_player.JumpEffect != null)
                Instantiate(_player.JumpEffect, _player.transform.position, _player.transform.rotation);
            SoundManager.PlaySfx(_player.jumpSound, _player.jumpSoundVolume);
        }

        private void JumpSliding()
        {
            if (_player.wallDirX == _player._moveDir.x)
            {
                _player.velocity.x = -_player.wallDirX * _player.wallJumpClimb.x;
                _player.velocity.y = _player.wallJumpClimb.y;
            }
            else if (_player._moveDir.x == 0)
            {
                _player.velocity.x = -_player.wallDirX * _player.wallJumpOff.x;
                _player.velocity.y = _player.wallJumpOff.y;
                _player.Flip();
            }
            else
            {
                _player.velocity.x = -_player.wallDirX * _player.wallLeap.x;
                _player.velocity.y = _player.wallLeap.y;
                _player.Flip();
            }
            SoundManager.PlaySfx(_player.jumpSound, _player.jumpSoundVolume);
        }

    }

    public class FallState : IState
    {
        private PlayerStateMachine _stateMachine;

        public FallState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Player.AnimController.PlayStableAnimation(this);
        }

        public void Exit() { }
    }

    public class LandState : IState //add hard land
    {
        private PlayerStateMachine _stateMachine;

        public LandState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
            // Start sound
        }

        public void Exit() { }
    }

    public class SlideState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;
        private float timeToWallUnstick;

        public SlideState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            if (!_player.soundFx.isPlaying)
                _player.soundFx.Play();

            _stateMachine.Player.AnimController.PlayStableAnimation(this);

            _stateMachine.CurrentAction += Sliding;
        }

        public void Exit()
        {
            if (_player.soundFx.isPlaying)
                _player.soundFx.Stop();

            _stateMachine.CurrentAction -= Sliding;
        }

        private void Sliding()
        {
            _player.wallDirX = (_player.controller.collisions.left) ? -1 : 1;


            if (_player.velocity.y < -_player.wallSlideSpeedMax)
                _player.velocity.y = -_player.wallSlideSpeedMax;

            if (timeToWallUnstick > 0)
            {
                _player.velocityXSmoothing = 0;
                _player.velocity.x = 0;

                if (_player._moveDir.x != 0 && _player._moveDir.x != _player.wallDirX)
                {
                    timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    timeToWallUnstick = _player.wallStickTime;
                }
            }
            else
            {
                timeToWallUnstick = _player.wallStickTime;
            }

        }
    }

    public class MelleAttackState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;

        public MelleAttackState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            if (_player.meleeAttack.Attack())
            {
                _stateMachine.ThisStateIsActive = true;

                _player.AnimController.PlayOneTimeAnimation(this);
                SoundManager.PlaySfx(_player.meleeAttackSound, _player.meleeAttackSoundVolume);

                _player.StartCoroutine(_player.SwichActiveStateRuotime(_player.AnimController.MeleeAttackAnim.Animation.Duration));
            }
        }

        public void Exit() { }
    }

    public class RangeAttackState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;

        public RangeAttackState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            _stateMachine.ThisStateIsActive = true;

            if (_player.rangeAttack.Fire())
            {
                _player.AnimController.PlayOneTimeAnimation(this);
                SoundManager.PlaySfx(_player.rangeAttackSound, _player.rangeAttackSoundVolume);
            }

            _player.StartCoroutine(_player.SwichActiveStateRuotime(_player.AnimController.RangeAttackAnim.Animation.Duration));
        }

        public void Exit() { }
    }

    public class TakeDamageState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;

        public TakeDamageState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            _stateMachine.ThisStateIsActive = true;
            SoundManager.PlaySfx(_player.hurtSound, _player.hurtSoundVolume);

            if (_player.HurtEffect != null)
                Instantiate(_player.HurtEffect, _player._lastInstigator.transform.position, Quaternion.identity);

            _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
            // Debug.LogWarning("Player enter in take damage state");
            _player.StartCoroutine(_player.SwichActiveStateRuotime());
        }

        public void Exit() { }
    }

    public class DeathState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;

        public DeathState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            _stateMachine.Player._inputs.Player.Disable();
            SoundManager.PlaySfx(_player.deadSound, _player.deadSoundVolume);
            _player.soundFx.Stop(); //stop the sliding wall sound if it's playing

            _player.SetForce(new Vector2(0, 7f));
            _player.Health = 0;
            //_stateMachine.Player.controller.HandlePhysic = false;

            _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
        }

        public void Exit() { }
    }

    public class FinishState : IState
    {
        private PlayerStateMachine _stateMachine;

        public FinishState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
        }

        public void Enter()
        {
            _stateMachine.Player._inputs.Player.Disable();
            _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);
        }

        public void Exit() { }
    }

    public class RespawnState : IState
    {
        private PlayerStateMachine _stateMachine;
        private Player _player;

        public RespawnState(PlayerStateMachine stateMachine)
        {
            _stateMachine = stateMachine;
            _player = _stateMachine.Player;
        }

        public void Enter()
        {
            _player._inputs.Enable();
            _player.Health = _player.maxHealth;

            var boxCo = _player.GetComponents<BoxCollider2D>();
            foreach (var box in boxCo)
            {
                box.enabled = true;
            }
            var CirCo = _player.GetComponents<CircleCollider2D>();
            foreach (var cir in CirCo)
            {
                cir.enabled = true;
            }

            _player.controller.HandlePhysic = true;

            _stateMachine.Player.AnimController.PlayOneTimeAnimation(this);

            _stateMachine.StateSwitch<IdleState>();
        }

        public void Exit() { }
    }

    private IEnumerator SwichActiveStateRuotime(float time = -1)
    {
        if (time < 0) time = MinActiveStateTime;

        yield return new WaitForSeconds(time);
        _stateMachine.ThisStateIsActive = false;
    }

    #endregion

    #region Inspector
    public bool GodMode;
    public string CurrentState;
    [Header("Animations")]
    public AnimationController AnimController;

    [Header("Moving")]
    public float moveSpeed = 3;

    private float accelerationTimeAirborne = .2f;
    private float accelerationTimeGrounded = .1f;

    [Header("Jump")]
    public float maxJumpHeight = 3;
    public float minJumpHeight = 1;
    public float timeToJumpApex = .4f;
    public int numberOfJumpMax = 1;
    public GameObject JumpEffect;

    private int numberOfJumpLeft;


    [Header("Wall Slide")]
    public Vector2 wallJumpClimb;
    public Vector2 wallJumpOff;
    public Vector2 wallLeap;
    public float wallSlideSpeedMax = 3;
    public float wallStickTime = .25f;

    [Header("Health")]
    public int maxHealth;
    public int Health { get; private set; }
    public GameObject HurtEffect;

    [Header("Sound")]
    public AudioClip jumpSound;
    [Range(0, 1)]
    public float jumpSoundVolume = 0.5f;

    public AudioClip landSound;
    [Range(0, 1)]
    public float landSoundVolume = 0.5f;

    public AudioClip wallSlideSound;
    [Range(0, 1)]
    public float wallSlideSoundVolume = 0.5f;

    public AudioClip hurtSound;
    [Range(0, 1)]
    public float hurtSoundVolume = 0.5f;

    public AudioClip deadSound;
    [Range(0, 1)]
    public float deadSoundVolume = 0.5f;

    public AudioClip rangeAttackSound;
    [Range(0, 1)]
    public float rangeAttackSoundVolume = 0.5f;

    public AudioClip meleeAttackSound;
    [Range(0, 1)]
    public float meleeAttackSoundVolume = 0.5f;

    private bool isPlayedLandSound;

    [Header("Option")]
    public bool allowMeleeAttack;
    public bool allowRangeAttack;
    public bool allowSlideWall;
    [Range(0.01f, 1)]
    public float MinActiveStateTime = 0.1f;
    public float MinVelosityForLand = 3;
    #endregion

    public bool IsSliding
    {
        get
        {
            bool nearWall = controller.collisions.left || controller.collisions.right;
            bool isMoveDown = !controller.collisions.below && velocity.y < 0;
            return allowSlideWall && nearWall && isMoveDown;
        }
    }
    public bool WasGrounded => _wasGrounded;
    public bool IsGrounded => controller.collisions.below;
    public bool IsHardLand => _isHardLand;
    public Controller2D Controller => controller;
    public Vector2 MoveDir => _moveDir;
    public Vector2 Velocity => velocity;

    protected RangeAttack rangeAttack;
    protected MeleeAttack meleeAttack;

    private AudioSource soundFx;

    private float gravity;
    private float maxJumpVelocity;
    private float minJumpVelocity;
    private float velocityXSmoothing;
    private bool _wasGrounded;
    private bool _isHardLand;

    private int wallDirX;

    private Vector2 velocity;
    private Vector2 _moveDir;

    private Controller2D controller;

    public bool isPlaying { get; private set; }
    public bool isFinish { get; set; }

    private InputActions _inputs;
    private PlayerStateMachine _stateMachine;
    private GameObject _lastInstigator;

    void Awake()
    {
        Initialize();
    }

    void Start()
    {
        InitInputs();
        TurnGravityAndJump();
        TurningFields();

        isPlaying = true;
    }

    void Update()
    {
        HandleInput();

        velocity.x = ApplySmoothing();
        velocity.y += gravity * Time.deltaTime;

        _isHardLand = Mathf.Abs(velocity.y) > MinVelosityForLand;
        _wasGrounded = IsGrounded;

        if (_stateMachine.CurrentAction != null)
            _stateMachine.CurrentAction();

        controller.Move(velocity * Time.deltaTime, _moveDir);
        if (controller.collisions.above || IsGrounded)
            velocity.y = 0;

        _stateMachine.StateControl();
#if NOT_IS_DEL_TRASH
        PlayWalkSound();
#endif
    }

    private void OnEnable()
    {
        _inputs.Player.Enable();
    }

    private void OnDisable()
    {
        _inputs.Player.Disable();
    }

    #region Init
    private void Initialize()
    {
        controller = GetComponent<Controller2D>();
        rangeAttack = GetComponent<RangeAttack>();
        meleeAttack = GetComponent<MeleeAttack>();
        _inputs = new InputActions();

        _stateMachine = new PlayerStateMachine(this);
        _stateMachine.StateSwitch<RespawnState>();
    }

    private void InitInputs()
    {
        _inputs.Player.Jump.performed += _ => Jump();
        _inputs.Player.Jump.canceled += _ => JumpOff();
        _inputs.Player.RangeAttack.performed += _ => RangeAttack();
        _inputs.Player.MeleeAttack.performed += _ => MeleeAttack();
    }

    private void TurningFields()
    {
        Health = maxHealth;

        soundFx = gameObject.AddComponent<AudioSource>();
        soundFx.loop = true;
        soundFx.playOnAwake = false;
        soundFx.clip = wallSlideSound;
        soundFx.volume = wallSlideSoundVolume;
    }

    private void TurnGravityAndJump()
    {
        gravity = -(2 * maxJumpHeight) / Mathf.Pow(timeToJumpApex, 2);
        maxJumpVelocity = Mathf.Abs(gravity) * timeToJumpApex;
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);
        //		print ("Gravity: " + gravity + "  Jump Velocity: " + maxJumpVelocity);

        numberOfJumpLeft = numberOfJumpMax;
    }
    #endregion

    #region API
    public void Jump()
    {
        if (_moveDir.y >= 0)
            _stateMachine.StateSwitch<JumpState>();
    }

    public void JumpOff()
    {
        if (velocity.y > minJumpVelocity)
        {
            velocity.y = minJumpVelocity;
        }
    }

    public void MeleeAttack()
    {
        if (allowMeleeAttack && meleeAttack != null)
            _stateMachine.StateSwitch<MelleAttackState>();
    }

    public void RangeAttack()
    {
        if (allowRangeAttack && rangeAttack != null)
            _stateMachine.StateSwitch<RangeAttackState>();
    }

    public void SetForce(Vector2 force)
    {
        velocity = (Vector3)force;
        //		controller.SetForce(forceDir);
    }

    public void AddForce(Vector2 force)
    {
        velocity += force;
    }

    public void RespawnAt(Vector2 pos)
    {
        transform.position = pos;

        _stateMachine.StateSwitch<RespawnState>();
    }

    public void TakeDamage(float damage, Vector2 forceDir, GameObject instigator)
    {
        _lastInstigator = instigator;
        _stateMachine.StateSwitch<TakeDamageState>();

        if (GodMode)
            return;

        Health -= (int)damage;

        if (Health <= 0)
            AllServices.Instance.GetService<LevelManager>().KillPlayer();

        if (forceDir.x == 0 && forceDir.y == 0)
            return;

        //set forceDir to player
        var facingDirectionX = Mathf.Sign(transform.position.x - instigator.transform.position.x);
        var facingDirectionY = Mathf.Sign(velocity.y);

        SetForce(new Vector2(Mathf.Clamp(Mathf.Abs(velocity.x), 10, 15) * facingDirectionX,
            Mathf.Clamp(Mathf.Abs(velocity.y), 5, 15) * -facingDirectionY));
    }

    public void GiveHealth(int hearthToGive, GameObject instigator)
    {
        Health = Mathf.Min(Health + hearthToGive, maxHealth);
        //GameManager.Instance.ShowFloatingText("+" + hearthToGive, transform.position, Color.red);
    }

    public void Kill()
    {
        _stateMachine.StateSwitch<DeathState>();
    }
    #endregion

    private void HandleInput()
    {
        _moveDir = _inputs.Player.Move.ReadValue<Vector2>();
        if (_moveDir.x != 0 && Mathf.Sign(transform.localScale.x) != _moveDir.x)
            Flip();
    }

    private void Flip()
    {
        transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
    }

    private float ApplySmoothing()
    {
        float targetVelocityX = _moveDir.x * moveSpeed;
        float smoothTime = (controller.collisions.below) ? accelerationTimeGrounded : accelerationTimeAirborne;
        return Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, smoothTime);
    }

#if NOT_IS_DEL_TRASH
    private void PlayWalkSound()
    {
        //check to play land sound
        if (controller.collisions.below && !isPlayedLandSound)
        {
            isPlayedLandSound = true;
            SoundManager.PlaySfx(landSound, landSoundVolume);
        }
        else if (!controller.collisions.below && isPlayedLandSound)
            isPlayedLandSound = false;
    }
#endif
}
