using UnityEngine;

public class SlippingPlatform : MonoBehaviour, IDynamicPlatform
{
    [SerializeField]
    RopeAction rope;

    Rigidbody2D rigid;


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
            rigid = collision.GetComponent<Rigidbody2D>();

            OnAttached(rigid, RigidbodyType2D.Dynamic);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            rigid.linearVelocityX = 0;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tongue") && rope.IsAttached)
        {
            OnDettaced(rigid);
            rope.Released();
        }

    }
}