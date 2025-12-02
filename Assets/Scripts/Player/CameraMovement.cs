using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform playerPos;
    [Header("Attributes")]
    [SerializeField] private bool isOnCamEdge;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isOnCamEdge)
        {

        }
    }
}
