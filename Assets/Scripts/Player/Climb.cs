using UnityEngine;

public class Climb : MonoBehaviour
{
    public float climbSpeed = 4f;
    public float climbCheckDistance = 0.6f;
    public LayerMask climbableMask;

    public bool IsClimbing { get; private set; }

    private Rigidbody rb;
    private GroundPound gp;

    float verticalInput;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
    }

    private void Update()
    {
        verticalInput = Input.GetAxisRaw("Vertical"); // read input from Update
    }

    private void FixedUpdate()
    {
        // Ground pound cancels climb
        if (gp != null && gp.IsGroundPounding)
        {
            StopClimbing();
            return;
        }

        CheckClimb();

        if (IsClimbing)
        {
            rb.useGravity = false;

            // KEEP horizontal movement from your movement script
            Vector3 vel = rb.linearVelocity;

            // Only REPLACE vertical movement
            vel.y = verticalInput * climbSpeed;

            rb.linearVelocity = vel;
        }
    }

    void CheckClimb()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;
        Vector3 forward = transform.forward;

        Debug.DrawRay(origin, forward * climbCheckDistance, Color.green);

        if (Physics.Raycast(origin, forward, climbCheckDistance, climbableMask))
        {
            if (!IsClimbing)
            {
                IsClimbing = true;

                // Stop vertical motion ONLY. (Do NOT stop horizontal.)
                Vector3 vel = rb.linearVelocity;
                vel.y = 0f;
                rb.linearVelocity = vel;
            }
        }
        else
        {
            StopClimbing();
        }
    }

    void StopClimbing()
    {
        if (!IsClimbing) return;

        IsClimbing = false;
        rb.useGravity = true;
    }
}
