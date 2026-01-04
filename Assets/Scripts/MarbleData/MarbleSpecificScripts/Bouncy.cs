using UnityEngine;
using System.Collections;

public class Bouncy : MarbleAbility
{
    protected override IEnumerator ActivateAbility()
    {
        // Bouncy doesn't have an active ability in FixedUpdate, only initialization
        yield return null;
    }
}
