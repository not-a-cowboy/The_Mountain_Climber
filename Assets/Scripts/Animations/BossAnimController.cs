using UnityEngine;

public class BossAnimController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();

        // As soon as the boss spawns, it should be in the climbing state
        if (animator != null)
        {
            animator.SetBool("isClimbing", true);
        }
    }

    // This is a public method that other scripts can call to trigger the kick
    public void PlayKickAnimation()
    {
        if (animator != null)
        {
            animator.SetTrigger("kick");
        }
    }

    // This is a public method that other scripts can call when the boss dies
    public void PlayDefeatAnimation()
    {
        if (animator != null)
        {
            animator.SetBool("isClimbing", false);
            animator.SetTrigger("defeat");
        }
    }
}