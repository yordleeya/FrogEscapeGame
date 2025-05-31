using UnityEngine;

public class Tongue : MonoBehaviour
{
    [SerializeField]
    PlayerStats stats;

    [SerializeField]
    Transform player;
    RopeAction ropeAction;
    float maxTongueDistance;

    private void Awake()
    {
        maxTongueDistance = stats.MaxTongueDistance;

        ropeAction = player.GetComponent<RopeAction>();
    }
    private void LateUpdate()
    {
        float distance = Vector3.Distance(player.position, transform.position);
        if (distance > maxTongueDistance && !ropeAction.IsAttached)
        {
            ropeAction.Released();
        }
    }
}
