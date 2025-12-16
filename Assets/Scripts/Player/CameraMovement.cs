using UnityEngine;
public class CameraMovement : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Positioning")]
    public Vector3 offset = new Vector3(-50f, 67f, 67f);
    public float smoothTime = 0.25f;

    [Header("Look Ahead")]
    public float lookAheadDistance = 2f;
    public float lookAheadSmoothTime = 0.2f;

    [Header("Bounds (Optional)")]
    public bool useBounds = false;
    public Vector2 minBounds;
    public Vector2 maxBounds;

    private Vector3 velocity;
    private Vector3 lookAheadVelocity;
    private Vector3 currentLookAhead;

    private Vector3 lastTargetPosition;
    [Header("References")]
    [SerializeField] private Transform playerPos;
    [Header("Attributes")]
    [SerializeField] private bool isOnCamEdge;

    void Start()
    {
        if (!target) return;
        lastTargetPosition = target.position;
        isOnCamEdge = false;
    }

    void LateUpdate()
    {
        if (!target) return;

        Vector3 movementDelta = target.position - lastTargetPosition;
        lastTargetPosition = target.position;

        Vector3 lookDir = new Vector3(movementDelta.x, 0, movementDelta.z).normalized;
        Vector3 targetLookAhead = lookDir * lookAheadDistance;

        currentLookAhead = Vector3.SmoothDamp(
            currentLookAhead,
            targetLookAhead,
            ref lookAheadVelocity,
            lookAheadSmoothTime
        );
        Vector3 desiredPosition = target.position + offset + currentLookAhead;
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime
        );

        if (useBounds)
            if (isOnCamEdge)
            {
                smoothedPosition.x = Mathf.Clamp(smoothedPosition.x, minBounds.x, maxBounds.x);
                smoothedPosition.z = Mathf.Clamp(smoothedPosition.z, minBounds.y, maxBounds.y);
            }

        transform.position = smoothedPosition;

        transform.LookAt(target.position + Vector3.up * 1.5f);
    }
}