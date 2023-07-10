using Spine;
using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimController : MonoBehaviour
{
    public AnimationReferenceAsset RangeAttack, MeleeAttack, Death;
    private Spine.Animation _baseAnim;

    [SerializeField]
    private SkeletonAnimation _skeletonAnimation;

    private void Awake()
    {
        _skeletonAnimation = _skeletonAnimation != null ? _skeletonAnimation : GetComponent<SkeletonAnimation>();
        _baseAnim = _skeletonAnimation.AnimationState.GetCurrent(0).Animation;
    }

    public void PlayRangeAttackAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0, RangeAttack, false);
        BackToWalkAnim(0.2f);
    }

    public void PlayMeleeAttackAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0, MeleeAttack, false);
        BackToWalkAnim(0.4f);
    }

    public void PlayDeathAnim()
    {
        _skeletonAnimation.AnimationState.SetAnimation(0, Death, false);
    }

    public void BackToWalkAnim(float delay = 0f)
        => _skeletonAnimation.AnimationState.AddAnimation(0, _baseAnim, true, delay);
}
