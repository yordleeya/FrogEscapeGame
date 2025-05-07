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
        if(Vector3.Distance(player.position,transform.position) > maxTongueDistance)
        {
            Debug.Log("너무 멀어서 비활성화");
            ropeAction.Released();
        }
    }
}
