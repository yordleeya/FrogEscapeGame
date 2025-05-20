using UnityEngine;

public class SlippingPlatform : MonoBehaviour, IDynamicPlatform
{
    [SerializeField]
    RopeAction rope;

    [SerializeField]
    float linearDaming;

    Rigidbody2D tongueRigid;
    Rigidbody2D rigid;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        rigid.linearDamping = linearDaming;
    }

    public void OnAttached(Rigidbody2D rigid, RigidbodyType2D bodyType)
    {
    }

    public void OnDettaced(Rigidbody2D rigid)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("혀 감지 됨");

        if(collision.CompareTag("Tongue"))
        {
            tongueRigid = collision.GetComponent<Rigidbody2D>();

            OnAttached(tongueRigid, RigidbodyType2D.Dynamic);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            tongueRigid.linearVelocityX = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tongue") && rope.IsAttached)
        {
            OnDettaced(tongueRigid);
            rope.Released();
        }

    }
}