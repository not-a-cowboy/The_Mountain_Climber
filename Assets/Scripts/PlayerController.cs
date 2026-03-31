using UnityEngine;

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

    void Start()
    {
        rdb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        is_grounded = Physics.CheckSphere(ground_check.position, ground_check_rad, ground_layer);

        if (Input.GetKeyDown(KeyCode.Space) && is_grounded)
        {
            rdb.AddForce(Vector3.up * jump_force, ForceMode.Impulse);
        }
    }

    void FixedUpdate()
    {
        Vector3 frwrd_movement = Vector3.forward * frwrd_speed * Time.fixedDeltaTime;

        float hrzntl_input = Input.GetAxis("Horizontal");
        Vector3 hrzntl_movement = Vector3.right * hrzntl_input * hrzntl_speed * Time.fixedDeltaTime;

        rdb.MovePosition(rdb.position + frwrd_movement + hrzntl_movement);

        Vector3 clamped_pos = rdb.position;
        clamped_pos.x = Mathf.Clamp(clamped_pos.x, -lane_limit, lane_limit);
        rdb.position = clamped_pos;
    }
}