using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float JumpForce = 5f;

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
         rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
        }
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
