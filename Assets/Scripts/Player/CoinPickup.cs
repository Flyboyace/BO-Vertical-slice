using UnityEngine;

public class Pickup : MonoBehaviour
{
    [Header("Pickup Settings")]
    public int value = 1;
    public bool rotate = true;
    public float rotateSpeed = 60f;

    [Header("Effects")]
    public AudioClip pickupSound;
    public GameObject pickupEffect;

    private static int totalPickedUp = 0;

    void Update()
    {
        if (rotate)
        {
            transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            totalPickedUp += value;

            Debug.Log("Picked up: " + value + " | Total: " + totalPickedUp);

            if (pickupSound != null)
            {
                AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            }

            if (pickupEffect != null)
            {
                Instantiate(pickupEffect, transform.position, Quaternion.identity);
            }

            Destroy(gameObject);
        }
    }
}
