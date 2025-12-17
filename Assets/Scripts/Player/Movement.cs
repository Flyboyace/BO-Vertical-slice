using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float acceleration = 25f;
    public float deceleration = 40f;

    [Header("Rotation")]
    public float rotationSpeed = 15f;

    [Header("Jumping")]
    public float jumpForce = 7f;

    private Rigidbody rb;
    private GroundPound gp;
    private CatWallClimb climb;
    private CatDive dive;

    private Vector3 input;
    private Vector3 moveDirection;

    public Vector3 LastMoveDirection { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
        climb = GetComponent<CatWallClimb>();
        dive = GetComponent<CatDive>();

        rb.freezeRotation = true;
        rb.linearDamping = 0f; // we handle deceleration manually
    }

    private void Update()
    {
        if (gp.IsGroundPounding ||
            climb.IsClimbing ||
            (dive != null && (dive.IsFreezing || dive.IsDiving)))
        {
            input = Vector3.zero;
            return;
        }

        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = -1f;
        if (Input.GetKey(KeyCode.D)) h = 1f;
        if (Input.GetKey(KeyCode.W)) v = 1f;
        if (Input.GetKey(KeyCode.S)) v = -1f;

        input = new Vector3(h, 0f, v).normalized;

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (gp.IsGroundPounding || climb.IsClimbing)
            return;

        if (dive != null && (dive.IsFreezing || dive.IsDiving))
            return;

        Vector3 velocity = rb.linearVelocity;
        Vector3 horizontalVel = new Vector3(velocity.x, 0f, velocity.z);

        if (input.sqrMagnitude > 0.01f)
        {
            // WORLD SPACE movement
            moveDirection = new Vector3(input.x, 0f, input.z);

            // Snap to 8 directions
            float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 45f) * 45f;
            moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            LastMoveDirection = moveDirection;

            Vector3 targetVelocity = moveDirection * speed;

            // Accelerate toward target
            horizontalVel = Vector3.MoveTowards(
                horizontalVel,
                targetVelocity,
                acceleration * Time.fixedDeltaTime
            );

            // Rotate ONLY when walking
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime)
            );
        }
        else
        {
            // Strong ground deceleration (removes ice feeling)
            horizontalVel = Vector3.MoveTowards(
                horizontalVel,
                Vector3.zero,
                deceleration * Time.fixedDeltaTime
            );
        }

        rb.linearVelocity = new Vector3(
            horizontalVel.x,
            rb.linearVelocity.y,
            horizontalVel.z
        );
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
