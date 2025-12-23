using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public class Feather : MonoBehaviour
{
    public MarbleSO marbleData { get; set; }
    public Rigidbody rb { get; set; }

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
        if(!onCooldown && startDelayCompleted)
        {
            StartCoroutine(DashAbility(2.0f, marbleData.speed * 10f));
            onCooldown = true;
            StartCoroutine(ResetCooldown(Random.Range(5.0f, 10.0f)));
        }
    }

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
    }

    private IEnumerator StartOfMatchDelay(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        startDelayCompleted = true;
    }
    private IEnumerator DashAbility(float dashDuration, float dashSpeed)
    {
        if(movement == null)
            yield break;

        if (onCooldown)
            yield break;        

        Debug.Log("Dash Activated!");

        float originalSpeed = movement.BaseSpeed;

        movement.speed = dashSpeed;

        Debug.Log("Dashing at speed: " + movement.speed);

        yield return new WaitForSeconds(dashDuration);

        movement.speed = originalSpeed;

        Debug.Log("Dash Ended. Speed reset to: " + movement.speed);
    }

    private IEnumerator ResetCooldown(float cooldownDuration)
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
        Debug.Log("Dash Ready!");
    }
}
