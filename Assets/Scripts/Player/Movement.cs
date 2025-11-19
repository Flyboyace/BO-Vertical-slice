using System.Runtime.CompilerServices;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float movementSpeed = 5f;
    private Vector3 movement;

    void Update()
    {
        
        float horizontalInput = Input.GetAxis("Horizontal");
        
        float verticalInput = Input.GetAxis("Vertical");

        movement = new Vector3(1, 0, 1);

        transform.position = transform.position + new Vector3(horizontalInput * movementSpeed * Time.deltaTime, 0 , verticalInput * movementSpeed * Time.deltaTime);

        Debug.Log(transform.position);
    }
}
