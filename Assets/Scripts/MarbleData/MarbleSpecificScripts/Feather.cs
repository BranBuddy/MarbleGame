using System.Collections;
using UnityEngine;

public class Feather : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(DashAbility(2.0f, marbleData.speed * 8f));
        yield return StartCoroutine(ResetCooldown(Random.Range(8.0f, 10.0f)));
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
}
