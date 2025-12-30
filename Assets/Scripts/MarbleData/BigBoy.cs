using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public class BigBoy : MonoBehaviour, IMarbles
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

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
    }

    private void FixedUpdate()
    {
        if(!onCooldown && startDelayCompleted && !isGameOver)
        {
            StartCoroutine(GrowthAbility(5.0f, 1.5f));
            onCooldown = true;
            StartCoroutine(ResetCooldown(Random.Range(10.0f, 15.0f)));
        }
    }

    public bool AmIUnlocked()
    {
        StartLineManager.Instance.poolOfMarbles.TryGetValue(this.gameObject, out bool unlockedStatus);
        return unlockedStatus;
    }

    public IEnumerator StartOfMatchDelay(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        startDelayCompleted = true;
    }

    public IEnumerator GrowthAbility(float growthDuration, float sizeMultiplier)
    {
        if(movement == null)
            yield break;

        Debug.Log("Growth Activated!");

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        float originalMass = rb.mass;

        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * sizeMultiplier;
        rb.mass *= sizeMultiplier * sizeMultiplier * sizeMultiplier;
        yield return new WaitForSeconds(growthDuration);

        transform.localScale = originalScale;

        Debug.Log("Growth Ended!");

        rb.mass = originalMass;
    }

    public IEnumerator ResetCooldown(float cooldownDuration)
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }
}
