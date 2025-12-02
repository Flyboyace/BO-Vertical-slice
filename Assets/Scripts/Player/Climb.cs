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
        {
            StartClimbing();
        }
        else
        {
            StopClimbing();
        }
    }

    void FixedUpdate()
    {
        if (isClimbing)
        {
            transform.position += Vector3.up * climbSpeed * Time.fixedDeltaTime;
        }
    }

    void DetectWall()
    {
        Vector3 origin = transform.position +
                         Vector3.up * rayHeight +
                         transform.forward * rayForwardOffset;

        RaycastHit hit;
        wallDetected = Physics.Raycast(origin, transform.forward, out hit, checkDistance, climbLayer);

        Debug.DrawRay(origin, transform.forward * checkDistance, wallDetected ? Color.green : Color.red);
    }

    void StartClimbing()
    {
        if (!isClimbing)
        {
            isClimbing = true;
            rb.linearVelocity = Vector3.zero;
            rb.useGravity = false;
            rb.isKinematic = true;
        }
    }

    void StopClimbing()
    {
        if (isClimbing)
        {
            isClimbing = false;
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}
