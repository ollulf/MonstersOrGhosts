using UnityEngine;
using NaughtyAttributes;

public class GetAndSetAnimationSpeed : MonoBehaviour
{
    [SerializeField] private int animationInYears;

    [ShowNonSerializedField] private float animationInSeconds;
    private Animator anim;

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
