using UnityEngine;

public class CatWallClimb : MonoBehaviour
{
    [Header("Climbing Settings")]
    public float climbSpeed = 4f;
    public float wallCheckDistance = 0.6f;
    public LayerMask climbableMask;

    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    public float staminaDrain = 20f;   // per second when climbing up
    public float staminaRegen = 25f;   // per second when not climbing

    private float currentStamina;

    private Rigidbody rb;
    private bool isClimbing = false;
    private Vector3 wallNormal;

    public bool IsClimbing => isClimbing;
    public float Stamina => currentStamina;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        // Regenerate stamina when NOT climbing
        if (!isClimbing)
        {
            currentStamina = Mathf.Min(
                currentStamina + staminaRegen * Time.deltaTime,
                maxStamina
            );
        }

        // Try to start climbing
        if (!isClimbing)
            TryStartClimbing();
    }

    private void FixedUpdate()
    {
        if (isClimbing)
            HandleClimbing();
    }

    void TryStartClimbing()
    {
        RaycastHit hit;

        if (Physics.Raycast(
            transform.position + Vector3.up * 1f,
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

        // 🚪 NO INPUT → DETACH
        if (Mathf.Abs(v) < 0.1f)
        {
            StopClimbing();
            return;
        }

        // ⬆ CLIMB UP (needs stamina)
        if (v > 0.1f)
        {
            if (currentStamina <= 0f)
            {
                StopClimbing();
                return;
            }

            currentStamina -= staminaDrain * Time.fixedDeltaTime;
            currentStamina = Mathf.Max(currentStamina, 0f);

            rb.linearVelocity = Vector3.up * climbSpeed;
            rb.position -= wallNormal * 0.03f;
        }
        // ⬇ CLIMB DOWN
        else if (v < -0.1f)
        {
            rb.linearVelocity = Vector3.down * climbSpeed;
        }

        Vector3 rayOrigin = transform.position + Vector3.up * 1f;

        // Wall lost → detach
        if (!Physics.Raycast(rayOrigin, -wallNormal, wallCheckDistance, climbableMask))
        {
            StopClimbing();
        }
    }

    void StopClimbing()
    {
        isClimbing = false;
        rb.useGravity = true;

        // Push away slightly to avoid sticking
        rb.AddForce(wallNormal * 2f, ForceMode.VelocityChange);
    }
}
