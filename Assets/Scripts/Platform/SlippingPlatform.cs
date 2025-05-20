using UnityEngine;

public class SlippingPlatform : MonoBehaviour
{
    [SerializeField]
    RopeAction rope;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            Rigidbody2D rigid = collision.GetComponent<Rigidbody2D>();

            rigid.bodyType = RigidbodyType2D.Dynamic;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Tongue") && rope.IsAttached)
        {
            rope.Released();
        }

    }
}