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

    [Header("Variable Jump Height")]
    public float maxJumpHoldTime = 0.25f;   // how long you can hold jump
    public float jumpHoldForce = 20f;       // upward assist force

    private Rigidbody rb;
    private GroundPound gp;
    private CatWallClimb climb;
    private CatDive dive;

    private Vector3 input;
    private float runTimer = 0f;
    private bool isBoosted = false;

    // For variable jump height
    private bool isJumping = false;
    private float jumpHoldTimer = 0f;

    // Used for your diagonal dive direction
    public Vector3 LastMoveDirection { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
        climb = GetComponent<CatWallClimb>();
        dive = GetComponent<CatDive>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        // Stop movement if special actions are active
        if (gp.IsGroundPounding ||
            climb.IsClimbing ||
            (dive != null && (dive.IsFreezing || dive.IsDiving)))
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // Your inverted movement input
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

        // Start jump
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isJumping = true;
            jumpHoldTimer = 0f;
        }

        // Stop jump early
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    private void FixedUpdate()
    {
        // Disable normal movement during special states
        if (gp.IsGroundPounding || climb.IsClimbing)
            return;

        if (dive != null && (dive.IsFreezing || dive.IsDiving))
            return; // Dive overrides velocity

        float currentSpeed = isBoosted ? boostedSpeed : speed;

        // Convert local input to world direction
        Vector3 moveDirection =
            transform.right * input.x +
            transform.forward * input.z;

        // Store last movement direction (for dive)
        if (moveDirection.sqrMagnitude > 0.01f)
            LastMoveDirection = moveDirection.normalized;

        // Apply horizontal movement
        rb.linearVelocity = new Vector3(
            moveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            moveDirection.z * currentSpeed
        );

        // VARIABLE JUMP HEIGHT – hold to jump higher
        if (isJumping)
        {
            if (Input.GetKey(KeyCode.Space))
            {
                jumpHoldTimer += Time.fixedDeltaTime;

                if (jumpHoldTimer < maxJumpHoldTime)
                {
                    rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Acceleration);
                }
            }
            else
            {
                isJumping = false;
            }
        }

        // Gravity adjustments
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
