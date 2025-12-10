using UnityEngine;

public class GroundPound : MonoBehaviour
{
    [Header("Ground Pound Settings")]
    public float hoverTime = 0.2f;          // Time frozen before slam
    public float slamSpeed = -20f;          // Slam velocity (down)
    public float impactFreezeTime = 0.15f;  // Freeze on impact
    public KeyCode groundPoundKey = KeyCode.LeftShift;

    public bool IsGroundPounding { get; private set; }

    private Rigidbody rb;
    private Movement move;
    private CatClimb climb;

    private bool isHovering = false;
    private bool isImpactFreezing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        move = GetComponent<Movement>();
        climb = GetComponent<CatClimb>();
    }

    private void Update()
    {
        if (IsGroundPounding || isHovering || isImpactFreezing)
            return;

        if (Input.GetKeyDown(groundPoundKey) && !move.IsGrounded() && !climb.IsClimbing)
        {
            StartCoroutine(GroundPoundRoutine());
        }
    }

    private System.Collections.IEnumerator GroundPoundRoutine()
    {
        IsGroundPounding = true;

        // Hover (freeze in air)
        isHovering = true;
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        yield return new WaitForSeconds(hoverTime);

        // Slam
        isHovering = false;
        rb.useGravity = true;
        rb.linearVelocity = new Vector3(0, slamSpeed, 0);
    }

    private void FixedUpdate()
    {
        if (IsGroundPounding && !isImpactFreezing)
        {
            if (move.IsGrounded())
            {
                StartCoroutine(ImpactFreezeRoutine());
            }
        }
    }

    private System.Collections.IEnumerator ImpactFreezeRoutine()
    {
        isImpactFreezing = true;

        // Freeze
        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;

        yield return new WaitForSeconds(impactFreezeTime);

        // Restore movement
        rb.useGravity = true;
        isImpactFreezing = false;
        IsGroundPounding = false;
    }
}
