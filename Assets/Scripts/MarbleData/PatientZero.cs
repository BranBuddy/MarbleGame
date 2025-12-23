using System.Collections;
using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public class PatientZero : MonoBehaviour, IMarbles
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

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
    }
    private void FixedUpdate()
    {
        if(!onCooldown && startDelayCompleted)
        {
            StartCoroutine(SideStepAbility(.1f, 10));
            onCooldown = true;
            StartCoroutine(ResetCooldown(Random.Range(5, 10)));
        }
    }

    private IEnumerator StartOfMatchDelay(float delayDuration)
    {
        yield return new WaitForSeconds(delayDuration);
        startDelayCompleted = true;
    }

    private IEnumerator ResetCooldown(float cooldownDuration)
    {
        yield return new WaitForSeconds(cooldownDuration);
        onCooldown = false;
    }

    private IEnumerator SideStepAbility(float duration, float sideStepStrength)
    {
        if(movement == null)
            yield break;

        Rigidbody rb = movement.GetComponent<Rigidbody>();

        Vector3 originalDirection = movement.transform.forward;

        rb.AddForce(movement.transform.right * sideStepStrength, ForceMode.VelocityChange);

        yield return new WaitForSeconds(duration);
    
        rb.AddForce(-movement.transform.right * sideStepStrength, ForceMode.VelocityChange);

        Debug.Log("Side Step Ended!");


        
    }

}
