using UnityEngine;

public class Climb : MonoBehaviour
{
    [Header("Climb Settings")]
    public float climbSpeed = 3f;
    public float checkDistance = 1f;
    public LayerMask climbLayer;
    public KeyCode climbKey = KeyCode.Space;

    [Header("Ray Settings")]
    public float rayHeight = 1.2f;
    public float rayForwardOffset = 0.3f;

    private Rigidbody rb;
    private bool isClimbing = false;
    private bool wallDetected = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        DetectWall();

        if (wallDetected && Input.GetKey(climbKey))
            StartClimbing();
        else if (!Input.GetKey(climbKey))
            StopClimbing();
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            Vector3 next = rb.position + Vector3.up * climbSpeed * Time.fixedDeltaTime;
            rb.MovePosition(next);
        }
    }

    void DetectWall()
    {
        Vector3 origin = transform.position +
                         Vector3.up * rayHeight +
                         transform.forward * rayForwardOffset;

        wallDetected = Physics.Raycast(origin, transform.forward, out RaycastHit hit, checkDistance, climbLayer);

        Debug.DrawRay(origin, transform.forward * checkDistance, wallDetected ? Color.green : Color.red);
    }

    void StartClimbing()
    {
        if (isClimbing) return;

        isClimbing = true;

        rb.linearVelocity = Vector3.zero;

        rb.useGravity = false;
        rb.isKinematic = false; 
    }

    void StopClimbing()
    {
        if (!isClimbing) return;

        isClimbing = false;

        rb.useGravity = true;
    }
}
