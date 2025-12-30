using System.Collections;
using UnityEngine;

public interface IMarbles
{
    public bool isUnlocked {get; set;}
    public bool isGameOver {get; set;}
    public MarbleSO marbleData {get; set;}
    public Rigidbody rb {get; set;}
    public void SetSteering(Vector3 dir);
    public IEnumerator ResetCooldown(float cooldownDuration);
    public IEnumerator StartOfMatchDelay(float delayDuration);
}
