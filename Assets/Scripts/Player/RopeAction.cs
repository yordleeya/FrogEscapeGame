using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    [FoldoutGroup("attach ray")]
    [SerializeField] LayerMask ropeAttachLayer;

    [FoldoutGroup("attach ray")]
    [SerializeField] float distance;

    [FoldoutGroup("attach ray")]
    [SerializeField] Vector2 hitPosition;

    public UnityEvent OnShot;
    public UnityEvent OnAttached;
    public UnityEvent OnDisableEvent;

    [SerializeField] private Transform player;
    private LineRenderer lineRenderer;

    [SerializeField] private float disableTime;
    private Coroutine disableCoroutine;

    private Rigidbody2D rigid;
    private SpringJoint2D springJoint;

    private float tongueSpeed;
    private bool isAttached;
    public bool IsAttached => isAttached;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        lineRenderer = player.GetComponent<LineRenderer>();
        springJoint = player.GetComponent<SpringJoint2D>();
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

        ResetRope();
        OnDisableEvent?.Invoke();
    }

    private void Update()
    {
        if (isAttached)
        {
            springJoint.connectedAnchor = player.position;

            if (lineRenderer.enabled)
            {
                lineRenderer.SetPositions(new Vector3[] { player.position, transform.position });
            }
        }
    }

    public void Init(float _tongueSpeed)
    {
        tongueSpeed = _tongueSpeed;
    }

    public void RopeShoot(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(player.position, direction, distance, ropeAttachLayer);

        if (hit.collider != null)
        {
            switch (hit.collider.tag)
            {
                case "Land":
                    Released();
                    break;

                case "Platform":
                    hitPosition = hit.point;
                    Attached();
                    break;

                default:
                    Debug.LogWarning($"Unhandled tag: {hit.collider.tag}");
                    break;
            }
        }
    }

    private void Attached()
    {

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
            disableCoroutine = null;
        }

        transform.position = hitPosition;

        springJoint.anchor = transform.position;

        lineRenderer.enabled = true;

        float attachDistance = Vector2.Distance(transform.position, player.position);

        springJoint.distance = attachDistance;
        springJoint.enabled = true;

        isAttached = true;
        OnAttached?.Invoke();
    }

    public void Released()
    {
        if (!isAttached) return;

        ResetRope();
    }

    private void ResetRope()
    {
        isAttached = false;
        lineRenderer.enabled = false;
        transform.position = player.position;
        gameObject.SetActive(false);

        if (springJoint != null)
        {
            springJoint.enabled = false;
        }
    }

    private IEnumerator DisableCoroutine()
    {
        yield return new WaitForSeconds(disableTime);
        gameObject.SetActive(false);
    }
}
