using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName ="Player Stat", menuName = "ScriptableObjects/Player/PlayerStats", order = 1)]

public class PlayerStats : ScriptableObject
{

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

    [FoldoutGroup("Tongue")]
    [SerializeField]
    float tongueSpeed;

    [FoldoutGroup("Tongue")]
    [SerializeField]
    float maxTongueDistance;

    [FoldoutGroup("Tongue")]
    [SerializeField]
    float tongueDelay;


    public float Speed { get => speed;}
    public float JumpPower { get => jumpPower; }
    public float PowerGainSpeed { get => powerGainSpeed;}
    public Vector2 MaxVelocity { get => maxVelocity; }
    public float TongueSpeed { get => tongueSpeed;}
    public float MaxTongueDistance { get => maxTongueDistance;}
    public float TongueDelay { get => tongueDelay;}
}
