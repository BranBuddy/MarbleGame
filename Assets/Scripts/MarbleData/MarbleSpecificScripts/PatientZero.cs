using System.Collections;
using UnityEngine;

public class PatientZero : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(SideStepAbility(.1f, 10));
        yield return StartCoroutine(ResetCooldown(Random.Range(5, 10)));
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
