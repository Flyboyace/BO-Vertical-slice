using UnityEngine;

public class Climb : MonoBehaviour
{
    public float climbSpeed = 3f;
    public float checkDistance = 1f;
    public LayerMask climbLayer;

    private Rigidbody rb;
    private bool isClimbing = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position + Vector3.up * 1f,
                            transform.forward,
                            out hit,
                            checkDistance,
                            climbLayer))
        {
            if (Input.GetKey(KeyCode.Space))
            {
                StartClimbing();
            }
        }
        else
        {
            StopClimbing();
        }

        if (isClimbing)
        {
            ClimbMovement();
        }
    }

    void StartClimbing()
    {
        isClimbing = true;
        rb.useGravity = false;
        rb.linearVelocity = Vector3.zero;
    }

    void StopClimbing()
    {
        if (isClimbing)
        {
            isClimbing = false;
            rb.useGravity = true;
        }
    }

    void ClimbMovement()
    {
        transform.position += Vector3.up * climbSpeed * Time.deltaTime;
    }
}

