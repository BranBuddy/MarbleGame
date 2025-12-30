using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public class Bouncy : MonoBehaviour, IMarbles
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

    void Start()
    {
        if (marbleData != null && movement != null)
            movement.ApplyMarbleData(marbleData);

    }
    public bool AmIUnlocked()
    {
        StartLineManager.Instance.poolOfMarbles.TryGetValue(this.gameObject, out bool unlockedStatus);
        return unlockedStatus;
    }

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
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
}
