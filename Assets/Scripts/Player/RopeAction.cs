using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    public UnityEvent OnShot;
    public UnityEvent OnAttached;
    public UnityEvent OnDisableEvent;

    float tongueSpeed;
    Rigidbody2D rigid;

    [SerializeField]
    float disableTime;
    Coroutine disableCoroutine;

    bool isAttached = false;
    public bool IsAttached { get => isAttached; }

    Transform player;
    SpringJoint2D playerJoint;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = transform.parent;

        // SpringJoint2D 가져오기
        playerJoint = player.GetComponent<SpringJoint2D>();

        if (playerJoint == null)
        {
            playerJoint = player.gameObject.AddComponent<SpringJoint2D>();
            playerJoint.autoConfigureDistance = false;
            playerJoint.dampingRatio = 0.2f;
            playerJoint.frequency = 1.5f;
            playerJoint.enabled = false; // 기본적으로 비활성화
        }
    }

    private void OnEnable()
    {

        OnShot?.Invoke();
        disableCoroutine = StartCoroutine(DisableCoroutine());
    }

    private void OnDisable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        if (isAttached)
        {
            isAttached = false;
        }

        rigid.linearVelocity = Vector2.zero;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        transform.position = player.position;

        OnDisableEvent?.Invoke();
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
        if (collision.CompareTag("Platform"))
        {
            Attached();
        }

        if (collision.CompareTag("Land"))
        {
            gameObject.SetActive(false);
        }
    }

    public void Attached()
    {
        transform.parent = null;

        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.linearVelocity = Vector2.zero;

        StopCoroutine(disableCoroutine);

        // 플레이어에 연결
        playerJoint.connectedBody = rigid;
        playerJoint.distance = Vector2.Distance(transform.position, player.position);
        playerJoint.enabled = true; // 활성화

        isAttached = true;
        OnAttached?.Invoke();
    }

    public void Released()
    {
        if (playerJoint == null)
            return;

        playerJoint.enabled = false; // 비활성화
        playerJoint.connectedBody = null;

        isAttached = false;

        transform.parent = player;
        gameObject.SetActive(false);
    }

    IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}
