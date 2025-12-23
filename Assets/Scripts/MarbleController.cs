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

        Vector3 steeringDir = Vector3.zero;

        if (seekTarget && target != null)
        {
            // Seek behavior: steer toward target
            Vector3 toTarget = target.position - transform.position;
            steeringDir = toTarget.sqrMagnitude > 0.0001f ? toTarget.normalized : Vector3.zero;
        }
        else
        {
            // Constant direction movement
            steeringDir = constantDirection.sqrMagnitude > 0.0001f ? constantDirection.normalized : Vector3.zero;
        }

        movement.SetSteering(steeringDir);
    }
}