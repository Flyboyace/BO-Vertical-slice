using UnityEngine;

public class CatClimb : MonoBehaviour
{
    [Header("Climb Settings")]
    public float climbUpSpeed = 3f;
    public float climbDownSpeed = 2f;
    public float slideDownSpeed = 1.2f;
    public float wallCheckDistance = 0.6f;
    public float detachPushForce = 5f;

    [Header("Controls")]
    public KeyCode jumpOffKey = KeyCode.Space;

    public bool IsClimbing { get; private set; }

    Rigidbody rb;
    Movement movement;
    CatDive dive;
    GroundPound gp;

    Vector3 wallNormal;
    Vector3 wallPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        dive = GetComponent<CatDive>();
        gp = GetComponent<GroundPound>();
    }

    private void Update()
    {
        // Jumping off wall
        if (IsClimbing && Input.GetKeyDown(jumpOffKey))
        {
            ExitClimb(true);
        }
    }

    private void FixedUpdate()
    {
        if (gp.IsGroundPounding || (dive != null && dive.IsDiving))
        {
            if (IsClimbing)
                ExitClimb(false);

            return;
        }

        if (IsClimbing)
        {
            HandleClimbing();
        }
        else
        {
            CheckForWall();
        }
    }

    // ---------------------------------------------------------
    // CHECK FOR WALL
    // ---------------------------------------------------------
    void CheckForWall()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 dir = transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, wallCheckDistance))
        {
            // Must be tagged "Climbable"
            if (!hit.collider.CompareTag("Climbable"))
                return;

            // Only start climbing while in the air
            if (movement != null && movement.IsGrounded())
                return;

            StartClimbing(hit);
        }
    }

    // ---------------------------------------------------------
    // BEGIN CLIMB
    // ---------------------------------------------------------
    void StartClimbing(RaycastHit hit)
    {
        IsClimbing = true;

        wallNormal = hit.normal;
        wallPoint = hit.point;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        if (movement != null) movement.enabled = false;
        if (dive != null) dive.enabled = false;
    }

    // ---------------------------------------------------------
    // CLIMB LOOP
    // ---------------------------------------------------------
    void HandleClimbing()
    {
        float v = Input.GetAxisRaw("Vertical");

        // Move up
        if (v > 0.1f)
        {
            rb.linearVelocity = Vector3.up * climbUpSpeed;
        }
        // Move down
        else if (v < -0.1f)
        {
            rb.linearVelocity = Vector3.down * climbDownSpeed;
        }
        // Sliding down automatically
        else
        {
            rb.linearVelocity = Vector3.down * slideDownSpeed;
        }

        // Stick to the wall
        KeepAttachedToWall();

        // Player pushes away → detach
        float h = Input.GetAxisRaw("Horizontal");
        if (Mathf.Abs(h) > 0.3f)
            ExitClimb(true);
    }

    // ---------------------------------------------------------
    // KEEP CHARACTER ON WALL
    // ---------------------------------------------------------
    void KeepAttachedToWall()
    {
        // Push player slightly toward wall
        Vector3 push = -wallNormal * 0.2f;
        rb.position = Vector3.Lerp(rb.position, wallPoint + push, 0.4f);
    }

    // ---------------------------------------------------------
    // EXIT CLIMB
    // ---------------------------------------------------------
    void ExitClimb(bool pushOff)
    {
        IsClimbing = false;

        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        if (movement != null) movement.enabled = true;
        if (dive != null) dive.enabled = true;

        // Jump away from wall
        if (pushOff)
        {
            rb.AddForce((wallNormal + Vector3.up) * detachPushForce, ForceMode.Impulse);
        }
    }
}
