using UnityEngine;
//using UnityEngine.InputSystem;   removing it as it's causing problems

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float frwrd_speed = 5f;
    public float hrzntl_speed = 5f;
    public float jump_force = 7f;
    public float lane_limit = 4f;

    [Header("Ground Check Settings")]
    public Transform ground_check;
    public float ground_check_rad = 0.2f;
    public LayerMask ground_layer;

    private Rigidbody rdb;
    private bool is_grounded;

    // New Input System variables
    //private Vector2 moveInput;    

    void Start()
    {
        rdb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        is_grounded = Physics.CheckSphere(ground_check.position, ground_check_rad, ground_layer);

        Debug.Log("Is Grounded: " + is_grounded);
        Debug.Log("Horizontal Input: " + Input.GetAxis("Horizontal")); // just checking if inputs were working

        if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
        {
            rdb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }
    }

    // Called automatically if PlayerInput is set to "Send Messages"
    /*public void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    } */  // Commenting out as it's causing issues, but you can re-enable if you set up the new input system

    /* public void OnJump()   // or OnJump(InputValue value) if you want to check phase
     {
         if (is_grounded)
         {
             rdb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
         }
     } */  // Commenting out as it's causing issues, but you can re-enable if you set up the new input system

    void FixedUpdate()
    {
        Vector3 frwrd_movement = Vector3.forward * frwrd_speed * Time.fixedDeltaTime;

        // New input system horizontal movement
        float hrzntl_input = Input.GetAxis("Horizontal"); // Replace with moveInput.x if using new input system
        Vector3 hrzntl_movement = Vector3.right * hrzntl_input * hrzntl_speed * Time.fixedDeltaTime;

        rdb.MovePosition(rdb.position + frwrd_movement + hrzntl_movement);

        // Clamp
        Vector3 clamped_pos = rdb.position;
        clamped_pos.x = Mathf.Clamp(clamped_pos.x, -lane_limit, lane_limit);
        rdb.position = clamped_pos;
    }
}