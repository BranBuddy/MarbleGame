using System.Collections;
using UnityEngine;

/// <summary>
/// Base class for marble abilities. Consolidates common functionality across all marble types.
/// Inheriting classes only need to implement ActivateAbility() and optional setup.
/// </summary>
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public abstract class MarbleAbility : MonoBehaviour, IMarbles
{
    [SerializeField] protected MarbleSO marbleDataSO;
    
    protected MarbleMovement movement;
    protected bool onCooldown = false;
    protected bool startDelayCompleted = false;

    // IMarbles properties
    public bool isUnlocked { get; set; }
    public MarbleSO marbleData { get; set; }
    public Rigidbody rb { get; set; }
    public bool isGameOver { get; set; }

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<MarbleMovement>();

        if (marbleData == null)
            marbleData = marbleDataSO;
    }

    protected virtual void Start()
    {
        if (marbleData != null && movement != null)
            movement.ApplyMarbleData(marbleData);

        StartCoroutine(StartOfMatchDelay(3.0f));
    }

    protected virtual void FixedUpdate()
    {
        if (!onCooldown && startDelayCompleted && !isGameOver)
        {
            StartCoroutine(ActivateAbility());
            onCooldown = true;
        }
    }

    /// <summary>
    /// Override this method to implement the marble's unique ability.
    /// Must call ResetCooldown() with appropriate duration when done.
    /// </summary>
    protected abstract IEnumerator ActivateAbility();

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
    }

    public bool AmIUnlocked()
    {
        MarbleManager.Instance.poolOfMarbles.TryGetValue(this.gameObject, out bool unlockedStatus);
        return unlockedStatus;
    }

    public IEnumerator StartOfMatchDelay(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        startDelayCompleted = true;
    }

    public IEnumerator ResetCooldown(float cooldownDuration)
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }
}
