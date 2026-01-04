using System.Collections;
using UnityEngine;

public class BigBoy : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(GrowthAbility(5.0f, 1.5f));
        yield return StartCoroutine(ResetCooldown(Random.Range(10.0f, 15.0f)));
    }

    private IEnumerator GrowthAbility(float growthDuration, float sizeMultiplier)
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
}
