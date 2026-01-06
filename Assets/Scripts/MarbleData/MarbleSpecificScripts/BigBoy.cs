using System.Collections;
using UnityEngine;

public class BigBoy : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(GrowthAbility(5.0f, 1.5f));
        yield return StartCoroutine(ResetCooldown(Random.Range(5.0f, 10.0f)));
    }

    private IEnumerator GrowthAbility(float growthDuration, float sizeMultiplier)
    {
        if(movement == null)
            yield break;

        Debug.Log("Growth Activated!");

        Rigidbody rb = movement.GetComponent<Rigidbody>();
        float originalMass = rb.mass;
        float originalAcceleration = movement.BaseAcceleration;

        movement.acceleration = originalAcceleration * sizeMultiplier;
        Vector3 originalScale = transform.localScale;
        transform.localScale = originalScale * sizeMultiplier;
        rb.mass *= sizeMultiplier * sizeMultiplier * sizeMultiplier;
        yield return new WaitForSeconds(growthDuration);

        transform.localScale = originalScale;

        Debug.Log("Growth Ended!");

        movement.acceleration = originalAcceleration;
        rb.mass = originalMass;
    }
}
