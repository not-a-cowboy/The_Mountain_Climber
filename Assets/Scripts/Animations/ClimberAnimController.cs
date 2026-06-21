using UnityEngine;

public class ClimberAnimController : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // --- BASE STATE ---
        animator.SetBool("isClimbing", true);

        // --- LANE MOVEMENT LOGIC ---

        if (Input.GetKey(KeyCode.A))
        {
            animator.SetBool("isMovingLeft", true);
        }
        else
        {
            animator.SetBool("isMovingLeft", false);
        }

        if (Input.GetKey(KeyCode.D))
        {
            animator.SetBool("isMovingRight", true);
        }
        else
        {
            animator.SetBool("isMovingRight", false);
        }

        // --- JUMP LOGIC ---
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.SetTrigger("jump");
        }

        // --- CROUCH LOGIC ---

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C))
        {
            animator.SetBool("Crouch", true);
        }
        else
        {
            animator.SetBool("Crouch", false);
        }
    }
}