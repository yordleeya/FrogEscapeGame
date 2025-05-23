using UnityEngine;

public class TongueAttach : MonoBehaviour
{
    [SerializeField]
    RopeAction rope;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision != null)
        {
            switch (collision.tag)
            {
                case "Platform":
                    rope.ConnectSpringJoint();
                    rope.IsAttached = true;

                    transform.parent = collision.transform; // HEAD 코드 유지
                    break;
                case "Ground":
                    rope.ResetRopeState();
                    break;
                case "Object":
                    break;
            }
        }
    }
}
