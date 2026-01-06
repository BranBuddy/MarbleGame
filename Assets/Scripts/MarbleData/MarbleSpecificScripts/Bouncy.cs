using UnityEngine;
using System.Collections;

public class Bouncy : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(JumpBoost());
        yield return StartCoroutine(ResetCooldown(Random.Range(7.0f, 15.0f)));
    }

    private IEnumerator JumpBoost()
    {
        if (movement == null)
            yield break;

        Debug.Log("Bouncy Jump Activated!");

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 10f, ForceMode.VelocityChange); // Apply an upward velocity change for mass-agnostic jump
        rb.AddForce(Vector3.forward * 5f, ForceMode.VelocityChange); // Slight forward boost
        yield return null;
    }
}
