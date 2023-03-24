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
    public string WalkEventName;

    public AnimationReferenceAsset Idle, Walk, Crouch, Crouching, UnCrouch, Jump, Fall, Land, SlideFaceToWall, SlideBackToWall, 
        MeleeAttackAnim, RangeAttackAnim, TackeDamageAnim, Death, Win, Respawn;

    private EventData eventData;


    public void Start()
    {
        if (SkeletonAnim == null)
            SkeletonAnim = GetComponent<SkeletonAnimation>();

        eventData = SkeletonAnim.Skeleton.Data.FindEvent(WalkEventName);
        SkeletonAnim.AnimationState.Event += HandleAnimationStateEvent;
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
                if (Player.transform.localScale.x == Player.WallDirX)
                    SoftSetAnimation(0, currentAnim, SlideFaceToWall, true);
                else
                    SoftSetAnimation(0, currentAnim, SlideBackToWall, true);
                break;
            default:
                Debug.LogError($"Error type, PlayStableAnimation has not {playerState.GetType().Name} animation type!");
                break;
        }

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
                SoftSetAnimation(0, currentStableAnim, MeleeAttackAnim, false);
                //SoftSetAnimation(1, currentActiveAnim, MeleeAttackAnim, false);
                //SkeletonAnim.AnimationState.AddEmptyAnimation(0, 0.05f, currentStableAnim.Animation.Duration - 0.04f);
                break;
            case "RangeAttackState":
                SoftSetAnimation(0, currentStableAnim, RangeAttackAnim, false, true);
                //SoftSetAnimation(1, currentActiveAnim, RangeAttackAnim, false, true);
                //SkeletonAnim.AnimationState.AddEmptyAnimation(0, 0.02f, currentStableAnim.Animation.Duration - 0.01f);
                break;
            case "TakeDamageState":
                SoftSetAnimation(1, currentActiveAnim, TackeDamageAnim, false);
                break;
            case "DeathState":
                SoftSetAnimation(0, currentStableAnim, Death, false);
                break;
            case "EndGameState":
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

    private void HandleAnimationStateEvent(TrackEntry trackEntry, Spine.Event e)
    {
        // Debug.Log("Event fired! " + e.Data.Name);
        //bool eventMatch = string.Equals(e.Data.Name, eventName, System.StringComparison.Ordinal); // Testing recommendation: String compare.
        bool eventMatch = (eventData == e.Data); // Performance recommendation: Match cached reference instead of string.
        if (eventMatch)
        {
            SoundManager.PlaySfx(Player.WalkSound);
        }
    }

    private void SoftSetAnimation(int trakIndex, TrackEntry cur, Spine.Animation animation, bool loop, bool hardSet = false)
    {
        if (cur == null || cur.Loop || hardSet)
            SkeletonAnim.AnimationState.SetAnimation(trakIndex, animation, loop);
        else
            SkeletonAnim.AnimationState.AddAnimation(trakIndex, animation, loop, 0);
    }

}
