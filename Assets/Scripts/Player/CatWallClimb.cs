using UnityEngine;

public class CatWallClimb : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 4f;
    public float slideSpeed = 2f;
    public float wallCheckDistance = 0.6f;
    public LayerMask climbableMask;

    private Rigidbody rb;
    private bool isClimbing = false;

    private Vector3 wallNormal;

    public bool IsClimbing => isClimbing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Start climbing
        if (!isClimbing)
            TryStartClimbing();

        // Stop climbing manually (jump off)
        if (isClimbing && Input.GetKeyDown(KeyCode.W))
        {
            StopClimbing();
        }
    }

    private void FixedUpdate()
    {
        if (isClimbing)
            HandleClimbing();
    }

    void TryStartClimbing()
    {
        RaycastHit hit;

        // Check in front of player
        if (Physics.Raycast(transform.position + Vector3.up * 1f,
                            transform.forward,
                            out hit,
                            wallCheckDistance,
                            climbableMask))
        {
            wallNormal = hit.normal;
            StartClimbing();
        }
    }

    void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    void HandleClimbing()
    {
        float v = Input.GetAxisRaw("Vertical");

        // Move UP
        if (v > 0.1f)
            rb.linearVelocity = Vector3.up * climbSpeed;

        // Move DOWN
        else if (v < -0.1f)
            rb.linearVelocity = Vector3.down * climbSpeed;

        // Sliding motion when no input
        else
            rb.linearVelocity = Vector3.down * slideSpeed;

        rb.position -= wallNormal * 0.03f;

        
        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        // Raycast *towards* the wall. If it misses, we reached the top.
        if (!Physics.Raycast(rayOrigin, -wallNormal, wallCheckDistance, climbableMask))
        {
            StopClimbing();
            return;
        }
    }

    void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;
    }
}
