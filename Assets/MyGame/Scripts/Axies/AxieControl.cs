using Spine.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxieControl : MonoBehaviour
{
    #region Inspector
    // [SpineAnimation] attribute allows an Inspector dropdown of Spine animation names coming form SkeletonAnimation.
    //[SpineAnimation]
    public const string runAnimationName = "action/run";
    //[SpineAnimation]
    public const string idleAnimationName = "action/idle/normal";

    public const string buffAnimationName = "battle/get-buff";

    //[SpineAnimation]
    //public string walkAnimationName;

    //[SpineAnimation]
    //public string atkAnimationName_1;

    //[SpineAnimation]
    //public string atkAnimationName_2;

    //[SpineAnimation]
    //public string moveBackAnimationName;

    //[SpineAnimation]
    //public string hitAnimationName;

    //[SpineAnimation]
    //public string deathAnimationName;

    //[SpineAnimation]
    //public string stunAnimationName;

    //[SpineAnimation]
    //public string skillAnimationName_1;
    //[SpineAnimation]
    //public string skillAnimationName_2;
    //[SpineAnimation]
    //public string skillAnimationName_3;

    #endregion

    SkeletonAnimation skeletonAnimation;

    public Spine.AnimationState spineAnimationState;
    public Spine.Skeleton skeleton;

    void Awake()
    {
        skeletonAnimation = GetComponent<SkeletonAnimation>();
        spineAnimationState = skeletonAnimation.AnimationState;
        skeleton = skeletonAnimation.Skeleton;
    }

    public void Buff()
    {
        spineAnimationState.SetAnimation(0, buffAnimationName, false);
        spineAnimationState.AddAnimation(0, idleAnimationName, true, 0);
    }

    public void Run()
    {
        spineAnimationState.SetAnimation(0, runAnimationName, true);
    }

    public void Idle()
    {
        spineAnimationState.SetAnimation(0, idleAnimationName, true);
    }
}
