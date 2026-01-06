using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleCollision))]
public class MarbleMovement : MonoBehaviour
{
    [Tooltip("Maximum speed in m/s")]
    [SerializeField] internal float speed;
    [Tooltip("Acceleration applied along forward when steering is active")]
    [SerializeField] internal float acceleration;
    [Tooltip("Turn rate in degrees per second")]
    [SerializeField] internal float handling;
    [Tooltip("Bounciness of the marble")]
    [SerializeField] internal float bounciness;
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

    // Navigation settings (from MarbleController)
    [Tooltip("Target transform for the marble to move toward")]
    [SerializeField] private Transform target;
    [Tooltip("If no target, move in this constant direction")]
    [SerializeField] private Vector3 constantDirection = Vector3.forward;
    [Tooltip("If true, move toward target; if false, move in constant direction")]
    [SerializeField] private bool seekTarget = false;

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
        bounciness = marbleData.bounciness;

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

        // Update steering direction based on navigation logic (formerly MarbleController)
        UpdateSteeringDirection();

        // Apply physics
        ApplyDrag();
        ApplyAcceleration();
        EnforceSpeedLimit();
    }

    private void UpdateSteeringDirection()
    {
        // Check for walls FIRST, before setting steering
        Vector3 wallAvoidance = GetWallAvoidanceDirection();
        
        if (wallAvoidance.sqrMagnitude > 0.0001f)
        {
            steeringDir = wallAvoidance;
        }
        else if (seekTarget && target != null)
        {
            Vector3 toTarget = target.position - transform.position;
            steeringDir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector3.zero;
        }
        else
        {
            steeringDir = constantDirection.sqrMagnitude > 0.0001f ? constantDirection.normalized : Vector3.zero;
        }
    }

    private Vector3 GetWallAvoidanceDirection()
    {
        if(Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1.0f))
        {
            if(hit.collider.CompareTag("Wall"))
            {
                DrawDebugRay(hit.point, Color.red);
                return -transform.forward;
            }
        }
        return Vector3.zero;
    }

    private void DrawDebugRay(Vector3 hitPoint, Color color)
    {
        Debug.DrawRay(transform.position, hitPoint - transform.position, color);
    }

    private void ApplyDrag()
    {
        float velocityMag = rb.linearVelocity.magnitude;
        if (velocityMag > 0.01f)
        {
            float v2 = velocityMag * velocityMag;
            float dragMagnitude = 0.5f * airDensity * dragCoefficient * frontalArea * v2;
            Vector3 dragForce = -rb.linearVelocity.normalized * dragMagnitude;
            rb.AddForce(dragForce, ForceMode.Force);
        }
    }

    private void ApplyAcceleration()
    {
        float velocityMag = rb.linearVelocity.magnitude;
        float effSpeed = speed * speedMultiplier;
        float effAccel = acceleration * accelerationMultiplier;

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

        // Continuously maintain forward momentum if moving below speed cap
        if (velocityMag > 0.01f && velocityMag < effSpeed)
        {
            float speedDeficit = effSpeed - velocityMag;
            if (thrustUsesMass)
                rb.AddForce(rb.linearVelocity.normalized * speedDeficit * 10f, ForceMode.Force);
            else
                rb.AddForce(rb.linearVelocity.normalized * speedDeficit * 10f, ForceMode.Acceleration);
        }
    }

    private void EnforceSpeedLimit()
    {
        float effSpeed = speed * speedMultiplier;
        if (enforceSpeedCap)
        {
            var v = rb.linearVelocity;
            var horizontal = new Vector3(v.x, 0f, v.z);
            float hMag = horizontal.magnitude;
            if (hMag > effSpeed)
            {
                var clampedHorizontal = horizontal.normalized * effSpeed;
                rb.linearVelocity = new Vector3(clampedHorizontal.x, v.y, clampedHorizontal.z);
            }
        }
    }

    private void OnDestroy()
    {
        if(this.transform.position.y < -50f)
        {
            Debug.Log("Marble fell out of bounds and was destroyed.");
        }
    }
}