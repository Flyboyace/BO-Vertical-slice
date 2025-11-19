using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float holdJumpForce = 5f;
    [SerializeField] private float maxholdtime = 5f;

    private float jumpHoldTimer = 0f;
    private bool isJumping = false;
    private Rigidbody rb;

     void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
        float horizontalInput = Input.GetAxis("Horizontal");

        float VerticalInput = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3 (horizontalInput, 0, VerticalInput) * movementSpeed;

        transform.position += movement * Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Space) && IsGrounded()) 
        {
         rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            jumpHoldTimer = 0f;
        }

        if (Input.GetKey(KeyCode.Space) && isJumping)
        {


        if (jumpHoldTimer < maxholdtime)
        { 
            rb.AddForce(Vector3.up *  holdJumpForce, ForceMode.Acceleration);
            jumpHoldTimer += Time.deltaTime;
        }

        else 
        {
            isJumping = !isJumping;
        }


        if (Input.GetKeyUp(KeyCode.Space))
        { 
            isJumping |= !isJumping;
        }

      }

    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
