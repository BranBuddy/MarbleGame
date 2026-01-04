using UnityEngine;
using System.Collections;

public class Wizard : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(TeleportAbility(20f));
        yield return StartCoroutine(ResetCooldown(Random.Range(10.0f, 15.0f)));
    }

    private IEnumerator TeleportAbility(float teleportDistance)
    {
        if (movement == null)
            yield break;

        Debug.Log("Teleport Activated!");

        FindPlayerToTeleport();

        yield return null;
    }

    private void FindPlayerToTeleport()
    {
        Physics.SphereCast(transform.position, 50f, Vector3.up, out RaycastHit hitInfo);

        if(hitInfo.collider != null && hitInfo.collider.CompareTag("Marble"))
        {
            Vector3 directionToPlayer = (hitInfo.collider.transform.position - transform.position).normalized;
            Vector3 teleportPosition = hitInfo.collider.transform.position - directionToPlayer * 5f;

            transform.position = teleportPosition;
        }

        Debug.DrawRay(transform.position, Vector3.up * 50f, Color.red, 2.0f);
    }
}
