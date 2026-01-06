using System.Collections;
using UnityEngine;

public class PatientZero : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        yield return StartCoroutine(Intangibility(5f));
        yield return StartCoroutine(ResetCooldown(Random.Range(5, 10)));
    }

    private IEnumerator Intangibility(float duration)
    {
        if(movement == null)
            yield break;

        Material originalMaterial = GetComponent<Renderer>().material;

        Rigidbody rb = movement.GetComponent<Rigidbody>();

        Debug.Log("Intangibility Activated!");
        int marbleLayer = LayerMask.NameToLayer("Marble");
        if (marbleLayer == -1)
        {
            Debug.LogError("PatientZero: Layer 'Marble' not found. Please create this layer.");
            yield break;
        }

        movement.handling *= 1.5f; // Increase handling while intangible
        movement.acceleration *= 1.5f; // Increase acceleration while intangible

        Material intangibleMaterial = new Material(originalMaterial);
        
        // Set the alpha value for transparency
        intangibleMaterial.color = new Color(214f/255f, 80f/255f, 80f/255f, 0.5f);
        intangibleMaterial.SetFloat("_Glossiness", 3); // Transparent mode
    
        GetComponent<Renderer>().material = intangibleMaterial;
        Physics.IgnoreLayerCollision(gameObject.layer, marbleLayer, true);
        yield return new WaitForSeconds(duration);
        GetComponent<Renderer>().material = originalMaterial;
        Physics.IgnoreLayerCollision(gameObject.layer, marbleLayer, false);
        Debug.Log("Intangibility Ended!");
    }
}
