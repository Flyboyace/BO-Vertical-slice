using UnityEngine;

public class GroundPound : MonoBehaviour
{
    public float slamSpeed = -20f;
    public KeyCode groundPoundKey = KeyCode.LeftShift;

    public bool IsGroundPounding { get; private set; }

    Rigidbody rb;
    Movement move;
    Climb climb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        move = GetComponent<Movement>();
        climb = GetComponent<Climb>();
    }

    private void Update()
    {
        if (IsGroundPounding)
            return;

        if (Input.GetKeyDown(groundPoundKey) && !move.IsGrounded() && !climb.IsClimbing)
        {
            StartGroundPound();
        }
    }

    void StartGroundPound()
    {
        IsGroundPounding = true;
        rb.linearVelocity = new Vector3(0, slamSpeed, 0);
    }

    private void FixedUpdate()
    {
        if (IsGroundPounding)
        {
            if (move.IsGrounded())
            {
                IsGroundPounding = false;
            }
        }
    }
}
