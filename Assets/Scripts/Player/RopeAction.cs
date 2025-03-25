using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    public UnityEvent OnAttached;

    float tongueSpeed;

    Rigidbody2D rigid;

    [SerializeField]
    float disableTime;
    Coroutine disableCoroutine;

    bool isAttached = false;
    public bool IsAttached { get => isAttached; }

    
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        disableCoroutine = StartCoroutine(DisableCoroutine());
    }

    private void OnDisable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        if(isAttached)
        {
            isAttached = false;
        }

        rigid.linearVelocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Kinematic;

        transform.position = transform.parent.position;
    }
    public void Init(float _tongueSpeed)
    {
        tongueSpeed = _tongueSpeed;
    }

    public void RopeShoot(Vector2 direction)
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.AddForce(direction * tongueSpeed, ForceMode2D.Impulse);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Platform"))
        {

            StopCoroutine(disableCoroutine);

            rigid.bodyType = RigidbodyType2D.Kinematic;
            rigid.linearVelocity = Vector2.zero;

            isAttached = true;
            OnAttached?.Invoke();
        }

        if(collision.CompareTag("Land"))
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
        
    }
}
