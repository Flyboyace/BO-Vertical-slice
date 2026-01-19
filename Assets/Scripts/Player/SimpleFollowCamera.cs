using UnityEngine;

public class AdjustableTargetCamera : MonoBehaviour
{
    [Header("Target")]
    public Transform player;   // drag your player here

    [Header("Camera Controls")]
    public float distance = 10f;     // how far from the player
    public float yaw = 45f;          // horizontal angle around player
    public float pitch = 20f;        // tilt up/down

    [Header("Smoothing")]
    public float moveSmooth = 5f;    // position smoothing
    public float rotateSmooth = 5f;  // rotation smoothing

    void LateUpdate()
    {
        if (player == null)
            return;

        // Convert angles to a rotation
        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

        // Desired camera position in orbit around player
        Vector3 desiredPosition =
            player.position - (rotation * Vector3.forward * distance);

        // Smooth position
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            moveSmooth * Time.deltaTime
        );

        // Smooth rotation to look at the player
        Quaternion desiredRot = Quaternion.LookRotation(player.position - transform.position);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            desiredRot,
            rotateSmooth * Time.deltaTime
        );
    }
}
