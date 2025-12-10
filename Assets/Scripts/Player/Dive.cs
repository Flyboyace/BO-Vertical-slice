using UnityEngine;

public class CatDive : MonoBehaviour
{
    [Header("Cat Dive Settings")]
    public float diveSpeed = 18f;
    public float downwardStrength = 2.2f;
    public float recoveryTime = 0.25f;

    [Header("Freeze Before Dive")]
    public float freezeTime = 0.12f;

    public KeyCode diveKey = KeyCode.LeftControl;

    public bool IsDiving { get; private set; }
    public bool IsRecovering { get; private set; }
    public bool IsFreezing { get; private set; }

    Rigidbody rb;
    Movement movement;
    GroundPound gp;
    Climb climb;

    float freezeTimer;
    float recoveryTimer;

    bool savedMovementEnabled;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
        gp = GetComponent<GroundPound>();
        climb = GetComponent<Climb>();
    }

    private void Update()
    {
        if (IsFreezing || IsDiving || IsRecovering)
            return;

        if (gp != null && gp.IsGroundPounding) return;
        if (climb != null && climb.IsClimbing) return;

        if (!movement.IsGrounded())
        {
            if (Input.GetKeyDown(diveKey))
                StartFreeze();
        }
    }

    // -----------------------------------------------------
    // FREEZE BEFORE DIVE
    // -----------------------------------------------------
    void StartFreeze()
    {
        IsFreezing = true;
        freezeTimer = freezeTime;

        savedMovementEnabled = movement.enabled;
        movement.enabled = false;

        rb.linearVelocity = Vector3.zero;
        rb.useGravity = false;
    }

    // -----------------------------------------------------
    // DIVE START
    // -----------------------------------------------------
    void StartDive()
    {
        IsFreezing = false;
        IsDiving = true;

        Vector3 diveForward = GetDiveDirection();
        Vector3 diveDir = (diveForward + Vector3.down * downwardStrength).normalized;

        rb.linearVelocity = diveDir * diveSpeed;
    }

    // -----------------------------------------------------
    // FIXED UPDATE
    // -----------------------------------------------------
    private void FixedUpdate()
    {
        // FREEZE
        if (IsFreezing)
        {
            freezeTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = Vector3.zero;

            if (freezeTimer <= 0f)
                StartDive();

            return;
        }

        // DIVING
        if (IsDiving)
        {
            Vector3 diveForward = GetDiveDirection();
            Vector3 diveDir = (diveForward + Vector3.down * downwardStrength).normalized;

            rb.linearVelocity = diveDir * diveSpeed;

            if (movement.IsGrounded())
                Landed();

            return;
        }

        // RECOVERY
        if (IsRecovering)
        {
            recoveryTimer -= Time.fixedDeltaTime;

            if (recoveryTimer <= 0f)
            {
                IsRecovering = false;
                movement.enabled = savedMovementEnabled;
            }
        }
    }

    // -----------------------------------------------------
    // LAND
    // -----------------------------------------------------
    void Landed()
    {
        IsDiving = false;
        IsRecovering = true;

        rb.useGravity = true;
        rb.linearVelocity = Vector3.zero;

        recoveryTimer = recoveryTime;
    }

    // -----------------------------------------------------
    // MAIN DIVE DIRECTION LOGIC
    // -----------------------------------------------------
    Vector3 GetDiveDirection()
    {
        // ALWAYS use the direction you moved
        if (movement.LastMoveDirection.sqrMagnitude > 0.01f)
        {
            Vector3 d = movement.LastMoveDirection;
            d.y = 0;
            return d.normalized;
        }

        // fallback
        Vector3 fallback = transform.forward;
        fallback.y = 0;
        return fallback.normalized;
    }
}
