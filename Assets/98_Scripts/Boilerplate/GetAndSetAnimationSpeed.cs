using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class GetAndSetAnimationSpeed : MonoBehaviour
{
    [SerializeField] private int animationInYears;

    [ShowNonSerializedField] private float animationInSeconds;
    private Animator anim;
    private Animation anim2;

    // Start is called before the first frame update
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        SetSpeed();

    }

    private void SetSpeed()
    {
        AnimationClip[] clips = anim.runtimeAnimatorController.animationClips;
        foreach(AnimationClip clip in clips)
        {
            float oneYearInSeconds;
            animationInSeconds = clip.length;
            oneYearInSeconds = animationInSeconds / animationInYears;
            anim.SetFloat("speed",oneYearInSeconds/ 5);
        }
    }
}
