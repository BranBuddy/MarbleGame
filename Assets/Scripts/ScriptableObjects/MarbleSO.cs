using UnityEngine;

[CreateAssetMenu(fileName = "New Marble", menuName = "CreateCharacter/Marble", order = 1)]
public class MarbleSO : ScriptableObject
{
    [Header("Marble Info")]
    public string marbleName;
    public string marbleDescription;
    public bool isUnlocked;

    [Header("Marble Stats")]
    public float speed;
    public float acceleration;
    public float handling;
    public float health;
    public float weight;
    public float bounciness;


    [Header("Marble Appearance")]
    public Material marbleMaterial;
    public Sprite marbleSprite;
    
}
