using UnityEngine;


public class MarbleCollision : MonoBehaviour
{
    [Tooltip("0 = inelastic, 1 = perfectly elastic")]
    [SerializeField, Range(0f, 1.5f)] private float bounciness = 0.6f;

    private void OnCollisionEnter(Collision collision)
    {
        Rigidbody otherRb = collision.rigidbody;
        Rigidbody thisRb = GetComponent<Rigidbody>();

        if (otherRb == null || thisRb == null)
            return;

        ContactPoint contact = collision.GetContact(0);
        Vector3 normal = contact.normal; // points from other to this

        Vector3 relativeVelocity = thisRb.linearVelocity - otherRb.linearVelocity;
        float closingSpeed = Vector3.Dot(relativeVelocity, normal); // only along contact normal

        if (closingSpeed <= 0f)
            return; // already separating, no corrective impulse needed

        // True restitution-based impulse (elastic collision along the normal)
        float mA = thisRb.mass;
        float mB = otherRb.mass;
        float e = Mathf.Clamp01(bounciness); // coefficient of restitution

        // j = -(1+e) * closingSpeed / (1/mA + 1/mB)
        float denom = (1f / Mathf.Max(mA, 0.0001f)) + (1f / Mathf.Max(mB, 0.0001f));
        float j = (1f + e) * closingSpeed / denom;
        Vector3 impulse = normal * j;

        thisRb.AddForce(-impulse, ForceMode.Impulse);

        if (collision.gameObject.CompareTag("Marble"))
            otherRb.AddForce(impulse, ForceMode.Impulse);
        
    }
}
