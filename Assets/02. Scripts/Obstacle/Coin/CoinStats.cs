using Sirenix.OdinInspector;
using UnityEngine;
[CreateAssetMenu(fileName ="Coin Stat", menuName = "ScriptableObjects/Obstacles/CoinStats")]

public class CoinStats : ScriptableObject
{
    [FoldoutGroup("Default")]
    [SerializeField]
    float fallSpeed;

    [FoldoutGroup("Default")]
    [SerializeField]
    float disableTime;

    [FoldoutGroup("Coin")]
    [SerializeField]
    float stunTime;

    public float FallSpeed { get => fallSpeed;}
    public float DisableTime { get => disableTime;}
    public float StunTime { get => stunTime;}
}
