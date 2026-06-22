using UnityEngine;

public class BirdAnimController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayFlying()
    {
        if (animator != null)
        {
            animator.SetBool("isFlying", true);
            animator.SetBool("isDiving", false);
        }
    }

    public void PlayDiving()
    {
        if (animator != null)
        {
            animator.SetBool("isDiving", true);
            animator.SetBool("isFlying", false);
        }
    }
}