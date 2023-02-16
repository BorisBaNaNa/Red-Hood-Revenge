using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.Playables;
using static Player;

public class AnimationController : MonoBehaviour
{
    [Header("Components")]
    public Player Player;
    public SkeletonAnimation SkeletonAnim;

    public AnimationReferenceAsset Idle, Walk, Crouch, Crouching, UnCrouch, Jump, Fall, Land, Slide, 
        MeleeAttackAnim, RangeAttackAnim, TackeDamageAnim, Death, Win, Respawn;
    public EventDataReferenceAsset FootstepEvent, FireEvent;

    private void Start()
    {
        if (SkeletonAnim == null)
            SkeletonAnim = GetComponent<SkeletonAnimation>();
    }

    public void PlayStableAnimation(IState playerState)
    {
        TrackEntry currentAnim = SkeletonAnim.AnimationState.GetCurrent(0);

        switch (playerState.GetType().Name)
        {
            case "IdleState":
                SoftSetAnimation(0, currentAnim, Idle, true);
                break;
            case "WalkState":
                SoftSetAnimation(0, currentAnim, Walk, true);
                break;
            case "CrouchState":
                SkeletonAnim.AnimationState.SetAnimation(0, Crouch, false);
                SkeletonAnim.AnimationState.AddAnimation(0, Crouching, true, 0);
                break;
            case "FallState":
                SoftSetAnimation(0, currentAnim, Fall, true);
                break;
            case "SlideState":
                SoftSetAnimation(0, currentAnim, Slide, true);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }

    }

    private void SoftSetAnimation(int trakIndex, TrackEntry cur, Spine.Animation animation, bool loop)
    {
        if (cur == null || cur.Loop)
            SkeletonAnim.AnimationState.SetAnimation(trakIndex, animation, loop);
        else
            SkeletonAnim.AnimationState.AddAnimation(trakIndex, animation, loop, 0);
    }

    public void PlayExitStateAnimation(IState playerState)
    {
        switch (playerState.GetType().Name)
        {
            case "CrouchState":
                SkeletonAnim.AnimationState.SetAnimation(0, UnCrouch, false);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }
    }

    public void PlayOneTimeAnimation(IState playerState)
    {
        TrackEntry currentStableAnim = SkeletonAnim.AnimationState.GetCurrent(0);
        TrackEntry currentActiveAnim = SkeletonAnim.AnimationState.GetCurrent(1);
        switch (playerState.GetType().Name)
        {
            case "LandState":
                SoftSetAnimation(0, currentStableAnim, Land, false);
                break;
            case "JumpState":
                SoftSetAnimation(0, currentStableAnim, Jump, false);
                break;
            case "MelleAttackState":
                SoftSetAnimation(1, currentActiveAnim, MeleeAttackAnim, false);
                SkeletonAnim.AnimationState.AddEmptyAnimation(1, 0.1f, 0);
                break;
            case "RangeAttackState":
                SoftSetAnimation(1, currentActiveAnim, RangeAttackAnim, false);
                SkeletonAnim.AnimationState.AddEmptyAnimation(1, 0.1f, 0);
                break;
            case "TakeDamageState":
                SoftSetAnimation(1, currentActiveAnim, TackeDamageAnim, false);
                break;
            case "DeathState":
                SoftSetAnimation(0, currentStableAnim, Death, false);
                break;
            case "FinishState":
                SoftSetAnimation(0, currentStableAnim, Win, false);
                break;
            case "RespawnState":
                SoftSetAnimation(0, currentStableAnim, Respawn, false);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }
    }

}
