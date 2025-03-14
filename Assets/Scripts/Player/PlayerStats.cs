using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName ="Player Stat", menuName = "ScriptableObjects/Player/PlayerStats", order = 1)]

public class PlayerStats : ScriptableObject
{
    [FoldoutGroup("Default")]
    [SerializeField]
    int maxStamina;

    [FoldoutGroup("MoveMent")]
    [SerializeField]
    float speed;

    [FoldoutGroup("MoveMent")]
    [SerializeField]
    float jumpPower;

    [FoldoutGroup("MoveMent")]
    [SerializeField]
    float powerGainSpeed;

    [FoldoutGroup("MoveMent")]
    [SerializeField]
    Vector2 maxVelocity;

    public int Stamina {  get => maxStamina; }
    public float Speed { get => speed;}
    public float JumpPower { get => jumpPower; }
    public float PowerGainSpeed { get => powerGainSpeed;}
    public Vector2 MaxVelocity { get => maxVelocity; }
}
