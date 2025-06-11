using DG.Tweening;
using UnityEngine;

public class SlippingPlatform : MonoBehaviour, IDynamicPlatform
{
    [SerializeField]
    RopeAction rope;

    [SerializeField]
    float slippingSpeed;

    [SerializeField]
    float slipTime = 1f;

    float defaultSlipTime;

    void Awake()
    {
        defaultSlipTime = slipTime;
    }
    public void OnAttached(Rigidbody2D rigid, RigidbodyType2D bodyType)
    {
        rigid.bodyType = bodyType;
    }

    public void OnDettaced(Rigidbody2D rigid)
    {
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            OnAttached(collision.GetComponent<Rigidbody2D>(), RigidbodyType2D.Kinematic);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            collision.transform.position += Vector3.down * slippingSpeed * Time.fixedDeltaTime;


            slipTime -= Time.fixedDeltaTime;

            if(slipTime<=0)
            {
                if (rope != null)
                {
                    rope.Released();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            slipTime = defaultSlipTime;
            if (rope != null)
            {
                rope.Released();
            }
        }
    }

}