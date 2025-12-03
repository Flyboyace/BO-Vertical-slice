using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float holdJumpForce = 4f;
    [SerializeField] private float maxHoldTime = 0.2f;

    private float jumpHoldTimer = 0f;
    private bool isJumping = false;
    private Rigidbody rb;

    private float runTimer = 0f;
    private float speedMultiplier = 1f;
    private float timeToSpeedBoost = 2.5f;

    private Vector3 inputDirection;

    private Groundpound groundpound;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        groundpound = GetComponent<Groundpound>();

        rb.freezeRotation = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
    }

    private void Update()
    {
        if (groundpound != null && groundpound.IsFrozen)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        inputDirection = new Vector3(-h, 0, -v).normalized;

        if (inputDirection.sqrMagnitude > 0.01f)
        {
            runTimer += Time.deltaTime;
            if (runTimer >= timeToSpeedBoost)
                speedMultiplier = 2f;
        }
        else
        {
            runTimer = 0f;
            speedMultiplier = 1f;
        }

        HandleJumpInput();
    }

    private void FixedUpdate()
    {
        Vector3 horizontalVelocity = inputDirection * movementSpeed * speedMultiplier;
        Vector3 verticalVelocity = new Vector3(0, rb.linearVelocity.y, 0);

        rb.linearVelocity = horizontalVelocity + verticalVelocity;
    }

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpHoldTimer < maxHoldTime)
            {
                rb.AddForce(Vector3.up * holdJumpForce, ForceMode.Acceleration);
                jumpHoldTimer += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
