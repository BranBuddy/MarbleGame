using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public class Wizard : MonoBehaviour, IMarbles
{
    public bool isUnlocked { get; set; }
    public MarbleSO marbleData { get; set; }
    public Rigidbody rb { get; set; }
    public bool isGameOver { get; set; }

    [SerializeField] private MarbleSO marbleDataSO;
    [SerializeField] private MarbleMovement movement;

    private bool onCooldown = false;
    private bool startDelayCompleted = false;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<MarbleMovement>();

        if (marbleData == null)
            marbleData = marbleDataSO;
    }
    private void Start()
    {
        if (marbleData != null && movement != null)
            movement.ApplyMarbleData(marbleData);

        StartCoroutine(StartOfMatchDelay(3.0f));
    }

    void FixedUpdate()
    {
        if(!onCooldown && startDelayCompleted && !isGameOver)
        {
            StartCoroutine(TeleportAbility(20f));
            onCooldown = true;
            StartCoroutine(ResetCooldown(Random.Range(10.0f, 15.0f)));
        }
    }

    public bool AmIUnlocked()
    {
        StartLineManager.Instance.poolOfMarbles.TryGetValue(this.gameObject, out bool unlockedStatus);
        return unlockedStatus;
    }

    public IEnumerator ResetCooldown(float cooldownDuration)
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }

    public IEnumerator StartOfMatchDelay(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        startDelayCompleted = true;
    }

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
    }

    public IEnumerator TeleportAbility(float teleportDistance)
    {
        if (movement == null)
            yield break;

        Debug.Log("Teleport Activated!");

        FindPlayerToTeleport();

        yield return null;
    }

    public void FindPlayerToTeleport()
    {
        Physics.SphereCast(transform.position, 50f, Vector3.up, out RaycastHit hitInfo);

        if(hitInfo.collider != null && hitInfo.collider.CompareTag("Marble"))
        {
            Vector3 directionToPlayer = (hitInfo.collider.transform.position - transform.position).normalized;
            Vector3 teleportPosition = hitInfo.collider.transform.position - directionToPlayer * 5f;

            transform.position = teleportPosition;
        }

        Debug.DrawRay(transform.position, Vector3.up * 50f, Color.red, 2.0f);
        
    }

}
