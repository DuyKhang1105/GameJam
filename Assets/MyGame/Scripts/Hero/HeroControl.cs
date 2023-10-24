using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroControl : MonoBehaviour
{
    #region Inspector
    // [SpineAnimation] attribute allows an Inspector dropdown of Spine animation names coming form SkeletonAnimation.
    [SpineAnimation]
    public string runAnimationName;

    [SpineAnimation]
    public string idleAnimationName;

    [SpineAnimation]
    public string walkAnimationName;

    [SpineAnimation]
    public string atkAnimationName_1;

    [SpineAnimation]
    public string atkAnimationName_2;

    [SpineAnimation]
    public string jumpAnimationName;

    [SpineAnimation]
    public string hitAnimationName;

    [SpineAnimation]
    public string deathAnimationName;

    [SpineAnimation]
    public string stunAnimationName;

    [SpineAnimation]
    public string skillAnimationName_1;
    [SpineAnimation]
    public string skillAnimationName_2;
    [SpineAnimation]
    public string skillAnimationName_3;

    #endregion

    SkeletonAnimation skeletonAnimation;

    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    void Start()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;
    }

    public void OneAttack()
    {
        spineAnimationState.SetAnimation(0, atkAnimationName_1, false);
        spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
    }

    public void Critical()
    {
        spineAnimationState.SetAnimation(0, atkAnimationName_2, false);
        spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
    }

    public void Heal()
    {
        spineAnimationState.SetAnimation(0, skillAnimationName_2, false);
        spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
    }

    public void OneHit()
    {
        spineAnimationState.SetAnimation(0, hitAnimationName, false);
        spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
    }

    public void Dead()
    {
        spineAnimationState.SetAnimation(0, deathAnimationName, false);
    }

    public void Run()
    {
        spineAnimationState.SetAnimation(0, runAnimationName, true);
    }

    public void Idle()
    {
        spineAnimationState.SetAnimation(0, idleAnimationName, true);
    }

    public void Dodge()
    {
        spineAnimationState.SetAnimation(0, atkAnimationName_2, false);
        spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
    }

    public void Stun()
    {
        spineAnimationState.SetAnimation(0, stunAnimationName, true);
    }
}
