using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    public UnityEvent OnShot;
    public UnityEvent OnAttached;
    public UnityEvent OnDisableEvent;

    [SerializeField] private float disableTime;
    [SerializeField] private LineRenderer lineRenderer;
    private Coroutine disableCoroutine;

    private Rigidbody2D rigid;
    private Transform player;
    private SpringJoint2D playerSpringJoint;

    private float tongueSpeed;
    private bool isAttached;
    public bool IsAttached => isAttached;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = transform.parent;
        playerSpringJoint = player.GetComponent<SpringJoint2D>();
    }

    private void OnEnable()
    {
        OnShot?.Invoke();
        disableCoroutine = StartCoroutine(DisableCoroutine());
        lineRenderer.enabled = true;
    }

    private void OnDisable()
    {
        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        ResetRope();
        OnDisableEvent?.Invoke();
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPositions(new Vector3[] { player.position, transform.position });
        }
    }

    public void Init(float _tongueSpeed)
    {
        tongueSpeed = _tongueSpeed;
    }

    public void RopeShoot(Vector2 direction)
    {
        rigid.bodyType = RigidbodyType2D.Dynamic;
        rigid.linearVelocity = direction * tongueSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Platform"))
        {
            Attached();
        }
        else if (collision.CompareTag("Land"))
        {
            gameObject.SetActive(false);
        }
    }

    private void Attached()
    {
        transform.parent = null;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.linearVelocity = Vector2.zero;

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        AttachToPlayer();
        isAttached = true;
        OnAttached?.Invoke();
    }

    private void AttachToPlayer()
    {
        Vector2 attachPoint = transform.position;
        float distance = Vector2.Distance(attachPoint, player.position) * 0.8f;

        playerSpringJoint.connectedBody = rigid;
        playerSpringJoint.distance = distance;
        playerSpringJoint.connectedAnchor = attachPoint;
        playerSpringJoint.enabled = true;
    }

    public void Released()
    {
        if (!isAttached) return;

        playerSpringJoint.enabled = false;
        playerSpringJoint.connectedBody = null;

        ResetRope();
    }

    private void ResetRope()
    {
        isAttached = false;
        transform.parent = player;
        rigid.bodyType = RigidbodyType2D.Kinematic;
        rigid.linearVelocity = Vector2.zero;
        transform.position = player.position;
        lineRenderer.enabled = false;
        gameObject.SetActive(false);
    }

    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}