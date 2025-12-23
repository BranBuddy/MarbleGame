using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleCollision))]
public class MarbleMovement : MonoBehaviour
{
    [Tooltip("Maximum speed in m/s")]
    [SerializeField] private float speed = 10f;
    [Tooltip("Acceleration applied along forward when steering is active")]
    [SerializeField] private float acceleration = 5f;
    [Tooltip("Turn rate in degrees per second")]
    [SerializeField] private float handling = 90f;
    [Tooltip("Air density in kg/m³ (set low to reduce drag)")]
    [SerializeField] private float airDensity = 0.02f;
    [Tooltip("Drag coefficient (0.47 for sphere)")]
    [SerializeField] private float dragCoefficient = 0.05f;
    [Tooltip("Frontal area in m²")]
    [SerializeField] private float frontalArea = 0.05f;
    [Tooltip("Global speed multiplier for quick tuning")]
    [SerializeField] private float speedMultiplier = 1.5f;
    [Tooltip("Global acceleration multiplier for quick tuning")]
    [SerializeField] private float accelerationMultiplier = 1.5f;
    [Tooltip("Clamp velocity to speed value")]
    [SerializeField] private bool enforceSpeedCap = true;
    [Tooltip("If true, thrust is mass-dependent (Force); if false, mass-agnostic (Acceleration)")]
    [SerializeField] private bool thrustUsesMass = false;

    private Vector3 steeringDir = Vector3.zero;
    private Rigidbody rb;
    private float health;

    // Debug/read-only properties for HUD and tooling
    public float BaseSpeed => speed;
    public float BaseAcceleration => acceleration;
    public float SpeedMultiplier => speedMultiplier;
    public float AccelerationMultiplier => accelerationMultiplier;
    public float EffSpeed => speed * speedMultiplier;
    public float EffAcceleration => acceleration * accelerationMultiplier;
    public float CurrentSpeed => rb != null ? rb.linearVelocity.magnitude : 0f;
    public bool EnforceSpeedCap => enforceSpeedCap;
    public bool ThrustUsesMass => thrustUsesMass;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void ApplyMarbleData(MarbleSO marbleData)
    {
        if (marbleData == null || rb == null)
            return;

        rb.linearDamping = 0f;
        rb.mass = marbleData.weight;

        // Store base stats; multipliers applied at runtime
        speed = marbleData.speed;
        acceleration = marbleData.acceleration;
        handling = marbleData.handling;
        health = marbleData.health;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && marbleData.marbleMaterial != null)
            renderer.material = marbleData.marbleMaterial;
    }

    public void SetSteering(Vector3 dir)
    {
        steeringDir = dir.sqrMagnitude > 0.0001f ? dir.normalized : Vector3.zero;
    }

    private void FixedUpdate()
    {
        if (rb == null)
            return;

        float velocityMag = rb.linearVelocity.magnitude;
        float effSpeed = speed * speedMultiplier;
        float effAccel = acceleration * accelerationMultiplier;

        if (velocityMag > 0.01f)
        {
            float v2 = velocityMag * velocityMag;
            float dragMagnitude = 0.5f * airDensity * dragCoefficient * frontalArea * v2;
            Vector3 dragForce = -rb.linearVelocity.normalized * dragMagnitude;
            rb.AddForce(dragForce, ForceMode.Force);
        }

        if (steeringDir.sqrMagnitude > 0.0001f && (!enforceSpeedCap || velocityMag < effSpeed))
        {
            Quaternion targetRot = Quaternion.LookRotation(steeringDir, Vector3.up);
            float maxAngle = handling * Time.fixedDeltaTime;
            Quaternion newRot = Quaternion.RotateTowards(rb.rotation, targetRot, maxAngle);
            rb.MoveRotation(newRot);

            if (thrustUsesMass)
                rb.AddForce(transform.forward * effAccel, ForceMode.Force);
            else
                rb.AddForce(transform.forward * effAccel, ForceMode.Acceleration);
        }

        if (enforceSpeedCap && rb.linearVelocity.magnitude > effSpeed)
            rb.linearVelocity = rb.linearVelocity.normalized * effSpeed;
    }
}