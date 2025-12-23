using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(MarbleMovement))]
public class PatientZero : MonoBehaviour, IMarbles
{
    public MarbleSO marbleData { get; set; }
    public Rigidbody rb { get; set; }

    [SerializeField] private MarbleSO marbleDataSO;
    [SerializeField] private MarbleMovement movement;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        movement = GetComponent<MarbleMovement>();

        if (marbleData == null)
            marbleData = marbleDataSO;
    }

    private void Start()
    {
        if (marbleData != null && movement != null)
            movement.ApplyMarbleData(marbleData);
    }

    public void SetSteering(Vector3 dir)
    {
        movement?.SetSteering(dir);
    }

}
