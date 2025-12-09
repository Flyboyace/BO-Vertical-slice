using UnityEngine;

public class Climb : MonoBehaviour
{
    public float climbSpeed = 4f;
    public float climbCheckDistance = 0.6f;
    public LayerMask climbableMask;

    public bool IsClimbing { get; private set; }

    Rigidbody rb;
    GroundPound gp;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
    }

    private void FixedUpdate()
    {
        if (gp.IsGroundPounding)
        {
            StopClimbing();
            return;
        }

        CheckClimb();

        if (IsClimbing)
        {
            rb.useGravity = false;

            float v = Input.GetAxisRaw("Vertical");
            rb.linearVelocity = Vector3.up * v * climbSpeed;
        }
    }

    void CheckClimb()
    {
        Vector3 origin = transform.position + Vector3.up * 1f;

        if (Physics.Raycast(origin, transform.forward, climbCheckDistance, climbableMask))
        {
            if (!IsClimbing)
            {
                IsClimbing = true;
                rb.linearVelocity = Vector3.zero;
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
