using UnityEngine;

public class GoldenBall : MonoBehaviour
{
    [SerializeField]
    Transform goldenBallPosition;

    Rigidbody2D rigid;

    [SerializeField]
    LayerMask excludeLayersWhenAttached;
    LayerMask excludeLayersWhenDetached;

    [SerializeField]
    bool isAttached = false;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        excludeLayersWhenDetached = rigid.excludeLayers;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Collider2D collider = collision.collider;
        Debug.Log(collider.tag);

        switch(collider.tag)
        {
            case "Player":
            case "Tongue":
                transform.parent = goldenBallPosition;

                isAttached = true;

                rigid.excludeLayers = excludeLayersWhenAttached;
                rigid.bodyType = RigidbodyType2D.Kinematic;

                transform.localPosition = Vector3.zero;

                break;
            default:
                Debug.Log("À¸¾Ç");

                if (isAttached)
                {
                    Debug.Log("¶³¾î¶ß·Á¶ó");

                    transform.parent = null;
                    rigid.excludeLayers = excludeLayersWhenDetached;
                    rigid.bodyType = RigidbodyType2D.Dynamic;

                    isAttached = false;
                }
                break;
        }
    }
}
