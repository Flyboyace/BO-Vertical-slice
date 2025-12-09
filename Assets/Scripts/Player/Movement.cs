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
    private Climb climb;

    private Vector3 input;

    private float runTimer = 0f;
    private bool isBoosted = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
        climb = GetComponent<Climb>();

        rb.freezeRotation = true;
    }

    private void Update()
    {
        if (gp.IsGroundPounding || climb.IsClimbing)
            return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        input = new Vector3(-h, 0, -v).normalized;

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

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        if (gp.IsGroundPounding || climb.IsClimbing)
            return;

        float currentSpeed = isBoosted ? boostedSpeed : speed;

        Vector3 moveDirection =
            transform.right * input.x +
            transform.forward * input.z;

        rb.linearVelocity = new Vector3(
            moveDirection.x * currentSpeed,
            rb.linearVelocity.y,
            moveDirection.z * currentSpeed
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

    public bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
