using UnityEngine;

public class Groundpound : MonoBehaviour
{
    [SerializeField] private float groundPoundForce = 20f;

    private bool isGroundPounding = false;
    private Rigidbody rb;
    private Movement movement;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<Movement>();
    }

    void Update()
    {
        if (!movement.IsGrounded() && !isGroundPounding && Input.GetKeyDown(KeyCode.LeftControl))
        {
            isGroundPounding = true;

            rb.linearVelocity = new Vector3(
                rb.linearVelocity.x,
                0f,
                rb.linearVelocity.z
            );
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

            if (movement.IsGrounded())
            {
                isGroundPounding = false;
            }
        }
    }
}
