using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;

public class Wizard : MarbleAbility
{

    private List<GameObject> playersInRange = new List<GameObject>();
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

        TeleportToRandomPlayer();

        yield return null;
    }

    private void TeleportToRandomPlayer()
    {
        if(playersInRange.Count == 0)
            return;

        int randomIndex = Random.Range(0, playersInRange.Count);
        GameObject targetPlayer = playersInRange[randomIndex];

        transform.position = targetPlayer.transform.position + Vector3.forward * 2f; // Teleport above the player
        targetPlayer.transform.position = transform.position - Vector3.forward * .5f; // Teleport player above the wizard

        playersInRange.Clear();
    }

    private void AddPlayerInRange(GameObject player)
    {
        if(playersInRange.Contains(player))
            return;

        playersInRange.Add(player);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Marble") && other.gameObject != this.gameObject && !onCooldown)
        {
            AddPlayerInRange(other.gameObject);
        }
    }   
}
