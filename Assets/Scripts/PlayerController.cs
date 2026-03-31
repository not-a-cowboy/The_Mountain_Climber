using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float frwrd_speed = 5f;
    [Tooltip("How far the player moves sideways with one swipe/keypress")]
    public float hrzntl_speed = 4f;
    public float jump_force = 7f;
    public float lane_limit = 4f;

    [Header("Ground Check Settings")]
    public Transform ground_check;
    public float ground_check_rad = 0.2f;
    public LayerMask ground_layer;

    private Rigidbody rdb;
    private bool is_grounded;
    private float TimeSinceStart;
    private float TimeMilestone;

    // Trigger variables for the snappy movement on key press (simulating a swipe)
    private bool moveLeft;
    private bool moveRight;

    void Start()
    {
        rdb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        is_grounded = Physics.CheckSphere(ground_check.position, ground_check_rad, ground_layer);

        // Jump Input
        if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
        {
            rdb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }


        // GetKeyDown only fires once per press, mimicking a quick screen swipe
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            moveLeft = true;
        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            moveRight = true;
        }

        // Speed Progression
        TimeSinceStart += Time.deltaTime;
        if (TimeSinceStart >= TimeMilestone)
        {
            frwrd_speed += 0.5f;
            TimeMilestone += 10f;
        }
    }

    void FixedUpdate()
    {
        // Continuous forward movement
        Vector3 frwrd_movement = Vector3.forward * frwrd_speed * Time.fixedDeltaTime;

        // --- NEW: Apply Horizontal Swipe ---
        Vector3 hrzntl_movement = Vector3.zero;

        if (moveLeft)
        {

            hrzntl_movement = Vector3.left * hrzntl_speed;
            moveLeft = false; // Reset the trigger
        }
        else if (moveRight)
        {
            hrzntl_movement = Vector3.right * hrzntl_speed;
            moveRight = false; // Reset the trigger
        }


        rdb.MovePosition(rdb.position + frwrd_movement + hrzntl_movement);

        // Clamp to lanes to ensure they don't swipe off the map
        Vector3 clamped_pos = rdb.position;
        clamped_pos.x = Mathf.Clamp(clamped_pos.x, -lane_limit, lane_limit);
        rdb.position = clamped_pos;
    }
}
