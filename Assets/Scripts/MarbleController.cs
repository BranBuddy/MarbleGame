using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MarbleMovement))]
public class MarbleController : MonoBehaviour
{
    [Tooltip("Target transform for the marble to move toward")]
    [SerializeField] private Transform target;
    [Tooltip("If no target, move in this constant direction")]
    [SerializeField] private Vector3 constantDirection = Vector3.forward;
    [Tooltip("If true, move toward target; if false, move in constant direction")]
    [SerializeField] private bool seekTarget = false;

    private MarbleMovement movement;

    private void Awake()
    {
        movement = GetComponent<MarbleMovement>();
    }

    private void FixedUpdate()
    {
        if (movement == null)
            return;

        // Check for walls FIRST, before setting steering
        Vector3 steeringDir = GetWallAvoidanceDirection();
        
        // If no wall detected, use normal steering
        if (steeringDir.sqrMagnitude < 0.0001f)
        {
            if (seekTarget && target != null)
            {
                Vector3 toTarget = target.position - transform.position;
                steeringDir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector3.zero;
            }
            else
            {
                steeringDir = constantDirection.sqrMagnitude > 0.0001f ? constantDirection.normalized : Vector3.zero;
            }
        }

        movement.SetSteering(steeringDir);
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

    private void OnDestroy()
    {
        if(this.transform.position.y < -50f)
        {
            Debug.Log("Marble fell out of bounds and was destroyed.");
        }
    }
}