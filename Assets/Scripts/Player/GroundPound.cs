using UnityEngine;

public class Groundpound : MonoBehaviour
{
    [SerializeField] private float groundPoundForce = 20f;
    [SerializeField] private float landingFreezeTime = 0.25f;

    private bool isGroundPounding = false;
    private bool isFreezing = false;
    private float freezeTimer = 0f;

    private Rigidbody rb;
    private Movement movement;

    public bool IsGroundPounding => isGroundPounding;
    public bool IsFrozen => isFreezing;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        // --- Start ground pound ---
        if (!movement.IsGrounded() && !isGroundPounding && Input.GetKeyDown(KeyCode.LeftControl))
        {
            isGroundPounding = true;

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
        }

        // --- Handle freeze timer ---
        if (isFreezing)
        {
            freezeTimer -= Time.deltaTime;

            if (freezeTimer <= 0f)
            {
                isFreezing = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation; // unfreeze movement
            }
        }
    }

    private void FixedUpdate()
    {
        if (isGroundPounding)
        {
            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                -groundPoundForce,
                rb.linearVelocity.z
            );

            // Landing
            if (movement.IsGrounded())
            {
                isGroundPounding = false;
                StartFreeze();
            }
        }
    }

    private void StartFreeze()
    {
        isFreezing = true;
        freezeTimer = landingFreezeTime;

        // Freeze ALL movement (but allow rotation freeze)
        rb.constraints = RigidbodyConstraints.FreezePosition | RigidbodyConstraints.FreezeRotation;
    }
}
