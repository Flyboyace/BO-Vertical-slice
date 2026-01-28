using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4f;
    public float runSpeed = 7f;
    public float timeToRun = 0.4f;
    public float deceleration = 40f;

    [Header("Rotation")]
    public float rotationSpeed = 15f;

    [Header("Jumping")]
    public float jumpForce = 7f;
    public float maxJumpHoldTime = 0.25f;
    public float jumpHoldForce = 20f;
    public float fallGravityMultiplier = 2.5f;
    public float lowJumpGravityMultiplier = 3.5f;

    private Rigidbody rb;
    private GroundPound gp;
    private CatWallClimb climb;
    private CatDive dive;

    private Animator animator;

    private Vector3 input;
    private Vector3 moveDirection;

    private float moveTimer;
    private bool isJumping;
    private float jumpHoldTimer;

    private bool isRunButtonHeld;

    public Vector3 LastMoveDirection { get; private set; }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
        climb = GetComponent<CatWallClimb>();
        dive = GetComponent<CatDive>();
        animator = GetComponent<Animator>();

        rb.freezeRotation = true;
        rb.linearDamping = 0f;
    }

    private void Update()
    {
        bool grounded = IsGrounded();

        if (gp.IsGroundPounding ||
            climb.IsClimbing ||
            (dive != null && (dive.IsFreezing || dive.IsDiving)))
        {
            input = Vector3.zero;
            moveTimer = 0f;

            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsGrounded", grounded);
            animator.SetBool("IsJumping", !grounded);
            return;
        }

        float h = 0f;
        float v = 0f;

        if (Input.GetKey(KeyCode.A)) h = 1f;
        if (Input.GetKey(KeyCode.D)) h = -1f;
        if (Input.GetKey(KeyCode.W)) v = -1f;
        if (Input.GetKey(KeyCode.S)) v = 1f;

        input = new Vector3(h, 0f, v).normalized;

        isRunButtonHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && grounded)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            isJumping = true;
            jumpHoldTimer = 0f;

            animator.SetTrigger("Jump");
            animator.SetBool("IsJumping", true);
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        // Animator states
        animator.SetBool("IsGrounded", grounded);
        animator.SetBool("IsFalling", rb.linearVelocity.y < -0.1f);

        if (grounded && rb.linearVelocity.y <= 0f)
        {
            animator.SetBool("IsJumping", false);
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
            moveDirection = new Vector3(input.x, 0f, input.z);

            float angle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            angle = Mathf.Round(angle / 45f) * 45f;
            moveDirection = Quaternion.Euler(0f, angle, 0f) * Vector3.forward;

            if (Vector3.Dot(moveDirection, LastMoveDirection) < 0.9f)
                moveTimer = 0f;

            LastMoveDirection = moveDirection;
            moveTimer += Time.fixedDeltaTime;

            float targetSpeed = isRunButtonHeld
                ? runSpeed
                : (moveTimer >= timeToRun ? runSpeed : walkSpeed);

            Vector3 targetVelocity = moveDirection * targetSpeed;

            horizontalVel = Vector3.MoveTowards(
                horizontalVel,
                targetVelocity,
                deceleration * Time.fixedDeltaTime
            );

            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            rb.MoveRotation(
                Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime)
            );

            animator.SetFloat("Speed", horizontalVel.magnitude);
        }
        else
        {
            moveTimer = 0f;

            horizontalVel = Vector3.MoveTowards(
                horizontalVel,
                Vector3.zero,
                deceleration * Time.fixedDeltaTime
            );

            animator.SetFloat("Speed", 0f);
        }

        rb.linearVelocity = new Vector3(horizontalVel.x, rb.linearVelocity.y, horizontalVel.z);

        // Variable jump height
        if (isJumping && Input.GetKey(KeyCode.Space))
        {
            jumpHoldTimer += Time.fixedDeltaTime;
            if (jumpHoldTimer < maxJumpHoldTime)
            {
                rb.AddForce(Vector3.up * jumpHoldForce, ForceMode.Acceleration);
            }
        }

        // Gravity tweaks
        if (rb.linearVelocity.y < 0f)
        {
            rb.AddForce(Vector3.up * Physics.gravity.y * (fallGravityMultiplier - 1f),
                ForceMode.Acceleration);
        }
        else if (rb.linearVelocity.y > 0f && !Input.GetKey(KeyCode.Space))
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
