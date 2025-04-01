using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    [FoldoutGroup("attach ray")]
    [SerializeField]
    LayerMask ropeAttachLayer;

    [FoldoutGroup("attach ray")]
    [SerializeField]
    float distance;

    [FoldoutGroup("attach ray")]
    [SerializeField]
    Vector2 hitPosition;

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
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, distance, ropeAttachLayer);

        if (hit.collider != null)
        {
            switch(hit.collider.tag)
            {
                case "Land":
                    gameObject.SetActive(false);
                    break;

                case "Platform":
                    hitPosition = hit.point;
                    Attached();
                    break;

                default:
                    Debug.LogWarning(hit.collider.tag + "의 경우는 고려되어있지 않습니다.");
                    break;
            }
        }
    }

    private void Attached()
    {
        transform.position = hitPosition;

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        float distance = Vector2.Distance(transform.position, player.position);

        playerSpringJoint.connectedBody = rigid;
        playerSpringJoint.distance = distance;
        playerSpringJoint.anchor = player.InverseTransformPoint(transform.position);
        playerSpringJoint.connectedAnchor = player.position;
        playerSpringJoint.enabled = true;

        isAttached = true;
        OnAttached?.Invoke();
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