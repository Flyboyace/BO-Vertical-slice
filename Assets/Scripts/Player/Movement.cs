using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float fallGravityMultiplier = 3f;
    [SerializeField] private float lowJumpGravityMultiplier = 2f;

    [Header("Speed Boost")]
    [SerializeField] private float timeToSpeedBoost = 2.5f;

    private float runTimer = 0f;
    private float speedMultiplier = 1f;

    private Vector3 inputDirection;
    private Rigidbody rb;
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
        
        Vector3 horizontalVel = inputDirection * movementSpeed * speedMultiplier;

        rb.linearVelocity = new Vector3(
            horizontalVel.x,
            rb.linearVelocity.y,
            horizontalVel.z
        );

        
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

    private void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
