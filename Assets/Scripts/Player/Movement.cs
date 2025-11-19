using UnityEngine;

public class Movement : MonoBehaviour
{
    private float movementSpeed = 5f;

    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float holdJumpForce = 4f;
    [SerializeField] private float maxHoldTime = 0.2f;

    private float jumpHoldTimer = 0f;
    private bool isJumping = false;
    private Rigidbody rb;

    
    private float runTimer = 0f;
    private float speedMultiplier = 1f;
    private float timeToSpeedBoost = 2.5f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        
        bool isMoving = horizontalInput != 0 || verticalInput != 0;

        if (isMoving)
        {
            runTimer += Time.deltaTime;

            if (runTimer >= timeToSpeedBoost)
            {
                speedMultiplier = 2f; 
            }
        }
        else
        {
            runTimer = 0f;
            speedMultiplier = 1f; 
        }

        transform.position += new Vector3(
            horizontalInput * movementSpeed * speedMultiplier * Time.deltaTime,
            0,
            verticalInput * movementSpeed * speedMultiplier * Time.deltaTime
        );

        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded())
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {
            if (jumpHoldTimer < maxHoldTime)
            {
                rb.AddForce(Vector3.up * holdJumpForce, ForceMode.Acceleration);
                jumpHoldTimer += Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;
        }

        Debug.Log(transform.position);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
