using System.Collections.Generic;
using UnityEngine;

public class Mario3DWorldCamera : MonoBehaviour
{
    [Header("Targets")]
    public List<Transform> targets;

    [Header("Camera Offsets")]
    public float distance = 12f;
    public float height = 8f;
    public float tiltAngle = 20f;

    [Header("Framing")]
    public float minZoom = 40f;
    public float maxZoom = 65f;
    public float zoomLimiter = 12f;

    [Header("Movement")]
    public float followSmoothTime = 0.25f;

    private Vector3 velocity;
    private Camera cam;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (targets == null || targets.Count == 0)
            return;

        Move();
        Zoom();
        Tilt();
    }

    void Move()
    {
        Vector3 centerPoint = GetCenterPoint();

        // Offset back and above players (rail-like)
        Vector3 desiredPosition =
            centerPoint
            - transform.forward * distance
            + Vector3.up * height;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            followSmoothTime
        );
    }

    void Zoom()
    {
        float greatestDistance = GetGreatestDistance();
        float newZoom = Mathf.Lerp(maxZoom, minZoom, greatestDistance / zoomLimiter);

        cam.fieldOfView = Mathf.Lerp(
            cam.fieldOfView,
            newZoom,
            Time.deltaTime * 3f
        );
    }

    void Tilt()
    {
        Quaternion targetRot =
            Quaternion.Euler(tiltAngle, transform.eulerAngles.y, 0f);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRot,
            Time.deltaTime * 3f
        );
    }

    Vector3 GetCenterPoint()
    {
        if (targets.Count == 1)
            return targets[0].position;

        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (var t in targets)
            bounds.Encapsulate(t.position);

        return bounds.center;
    }

    float GetGreatestDistance()
    {
        var bounds = new Bounds(targets[0].position, Vector3.zero);
        foreach (var t in targets)
            bounds.Encapsulate(t.position);

        return Mathf.Max(bounds.size.x, bounds.size.z);
    }
}
