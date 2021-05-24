using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOverrider : MonoBehaviour
{
    private Animator animator;
    void Awake()
    {
        animator  = this.gameObject.GetComponent<Animator>();
    }

    public void setAnimation(AnimatorOverrideController overrideController)
    {
        Debug.Log(overrideController);
        animator.runtimeAnimatorController = overrideController;
    }

    public void setTrigger(string trigger)
    {
        animator.SetTrigger(trigger);
    }
}
