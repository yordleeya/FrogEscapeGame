using UnityEngine;

public class GoldenballArrow : MonoBehaviour
{
    [SerializeField]
    Transform target;

    [SerializeField]
    Transform player;

    [SerializeField]
    Vector3 offset;

    public void SetPosition()
    {
        if (player == null) return;

        transform.position = player.position + offset;
    }
    // Update is called once per frame
    void Update()
    {
        SetPosition();

        if (target == null) return;

        Vector2 direction = target.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
