using UnityEngine;

/// <summary>
/// Holds a reference to the MarbleSO for this marble prefab.
/// Attach this component to each marble prefab.
/// </summary>
public class MarbleDataReference : MonoBehaviour
{
    [SerializeField] public MarbleSO marbleSO;
}
