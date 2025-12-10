using UnityEngine;

public class CatWallClimb : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 3f;
    public float slideSpeed = 2f;
    public float wallCheckDistance = 0.6f;

    public LayerMask climbableMask;

    private Rigidbody rb;
    private GroundPound gp;
    private CatDive dive;
    private Movement move;

    private bool isClimbing = false;
    private Vector3 wallNormal;

    public bool IsClimbing => isClimbing;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        gp = GetComponent<GroundPound>();
        dive = GetComponent<CatDive>();
        move = GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        // disable climbing if diving or ground pounding
        if ((gp != null && gp.IsGroundPounding) ||
            (dive != null && dive.IsDiving))
        {
            if (isClimbing)
                StopClimbing(false);
            return;
        }

        if (!isClimbing)
            TryStartClimb();
        else
            ClimbingLogic();
    }

    void TryStartClimb()
    {
        // ray from chest height forward
        Vector3 origin = transform.position + Vector3.up * 1f;

        // Try forward first
        Vector3 dir = transform.forward;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, wallCheckDistance, climbableMask))
        {
            StartClimbing(hit.normal);
            return;
        }

        // ALSO detect climbing based on movement direction (Mario-like)
        if (rb.linearVelocity.magnitude > 0.1f)
        {
            Vector3 velDir = rb.linearVelocity.normalized;
            if (Physics.Raycast(origin, velDir, out hit, wallCheckDistance, climbableMask))
            {
                StartClimbing(hit.normal);
            }
        }
    }

    void StartClimbing(Vector3 normal)
    {
        isClimbing = true;
        wallNormal = normal;

        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;

        // disable movement so climbing takes control
        if (move != null) move.enabled = false;
        if (dive != null) dive.enabled = false;
    }

    void ClimbingLogic()
    {
        float v = Input.GetAxisRaw("Vertical");

        if (v > 0.1f)
            rb.linearVelocity = Vector3.up * climbSpeed;
        else if (v < -0.1f)
            rb.linearVelocity = Vector3.down * climbSpeed;
        else
            rb.linearVelocity = Vector3.down * slideSpeed;

        // keep player stuck to wall
        rb.position -= wallNormal * 0.03f;

        // jump off
        if (Input.GetKeyDown(KeyCode.Space))
            StopClimbing(true);
    }

    void StopClimbing(bool jumpOff)
    {
        isClimbing = false;
        rb.useGravity = true;

        if (move != null) move.enabled = true;
        if (dive != null) dive.enabled = true;

        if (jumpOff)
            rb.AddForce((wallNormal + Vector3.up) * 6f, ForceMode.Impulse);
    }
}
