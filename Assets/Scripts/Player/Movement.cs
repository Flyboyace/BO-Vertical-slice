using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 6f;
    public float boostedSpeed = 12f;
    public float timeToSpeedBoost = 2.5f;

    [Header("Jumping")]
    public float jumpForce = 7f;
    public float fallGravityMultiplier = 3f;
    public float lowJumpGravityMultiplier = 2f;

    private Rigidbody rb;
    private GroundPound gp;
    private CatClimb climb;
    private CatDive dive;

    private Vector3 input;

    private float runTimer = 0f;
    private bool isBoosted = false;

    // ★ NEW — dive direction support
    public Vector3 LastMoveDirection { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
        climb = GetComponent<CatClimb>();
        dive = GetComponent<CatDive>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Disable movement while special actions run
        if (gp.IsGroundPounding || climb.IsClimbing || (dive != null && dive.IsFreezing) || (dive != null && dive.IsDiving))
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Your inverted controls
        input = new Vector3(-h, 0, -v).normalized;

        // Boost logic
        if (input.sqrMagnitude > 0.01f)
        {
            runTimer += Time.deltaTime;

            if (runTimer >= timeToSpeedBoost)
                isBoosted = true;
        }
        else
        {
            runTimer = 0f;
            isBoosted = false;
        }

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        // DO NOT MOVE DURING SPECIAL STATES
        if (gp.IsGroundPounding || climb.IsClimbing)
            return;

        if (dive != null && (dive.IsFreezing || dive.IsDiving))
            return; // CatDive handles velocity

        float currentSpeed = isBoosted ? boostedSpeed : speed;

        // Convert local input to world movement
        Vector3 moveDirection =
            transform.right * input.x +
            transform.forward * input.z;

        // ★ NEW — store last movement direction
        if (moveDirection.sqrMagnitude > 0.01f)
        {
            LastMoveDirection = moveDirection.normalized;
        }

        // Apply horizontal movement
        rb.linearVelocity = new Vector3(
            moveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            moveDirection.z * currentSpeed
        );

        // Better jumping / falling
        if (rb.linearVelocity.y < 0)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f),
                ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * (lowJumpGravityMultiplier - 1f),
                ForceMode.Acceleration);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
