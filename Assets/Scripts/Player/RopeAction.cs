using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class RopeAction : MonoBehaviour
{
    [FoldoutGroup("Attach Ray"), SerializeField] private LayerMask ropeAttachLayer;
    [FoldoutGroup("Attach Ray"), SerializeField] private float raycastDistance;
    [FoldoutGroup("Attach Ray"), SerializeField] private Vector3 hitPosition;

    [FoldoutGroup("Rope"), SerializeField] private float maxTargetDistance;
    [FoldoutGroup("Rope"), SerializeField] private float minTargetDistance;

    [FoldoutGroup("Tongue"), SerializeField] private Transform tongue;
    [FoldoutGroup("Tongue"), SerializeField] private Transform tongueOrigin;

    public UnityEvent OnShot;
    public UnityEvent OnAttached;
    public UnityEvent OnDisableEvent;

    private LineRenderer lineRenderer;
    private Rigidbody2D rigid;
    private SpringJoint2D springJoint;

    private float tongueSpeed;
    private bool isAttached;
    public bool IsAttached => isAttached;

    private float attachDistance;
    private RaycastHit2D hit;

    private bool isFlying;

    private void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        lineRenderer = GetComponent<LineRenderer>();
        springJoint = GetComponent<SpringJoint2D>();
        springJoint.enabled = false;
    }

    private void Update()
    {
        if (isFlying)
        {
            AnimateTongueFlight();
            UpdateLineRenderer();
            UpdatePlayerLookAtTongue();
            return;
        }

        if (!isAttached) return;

        UpdateTonguePosition();
        UpdatePlayerRotation();
        UpdateLineRenderer();
        ClampSpringDistance();
    }


    public void Init(float _tongueSpeed)
    {
        tongueSpeed = _tongueSpeed;
    }

    public void RopeShoot(Vector2 direction)
    {
        OnShot?.Invoke();

        hit = Physics2D.Raycast(tongueOrigin.position, direction, raycastDistance, ropeAttachLayer);

        if (hit.collider == null) return;

        switch (hit.collider.tag)
        {
            case "Land":
                Released();
                break;

            case "Platform":
                hitPosition = hit.point;
                tongue.position = tongueOrigin.position;
                isFlying = true; // ⬅ 혀 날아가는 중으로 상태 설정
                break;

            default:
                Debug.LogWarning($"Unhandled tag: {hit.collider.tag}");
                break;
        }
    }

    private void AnimateTongueFlight()
    {
        tongue.position = Vector3.MoveTowards(tongue.position, hitPosition, tongueSpeed * Time.deltaTime);

        if (Vector3.Distance(tongue.position, hitPosition) < 0.05f)
        {
            isFlying = false;
            Rigidbody2D hitRigid;
            if(hit.transform.TryGetComponent<Rigidbody2D>(out hitRigid))
            {
                AttachToTarget(hitRigid);
            }
        }
    }


    private void AttachToTarget(Rigidbody2D targetBody)
    {
        if (targetBody == null) return;

        springJoint.connectedBody = targetBody;
        springJoint.autoConfigureDistance = false;
        springJoint.enabled = true;

        attachDistance = Vector2.Distance(tongueOrigin.position, hitPosition);
        springJoint.distance = Mathf.Max(attachDistance, minTargetDistance) * 0.8f;

        tongue.position = hitPosition;
        lineRenderer.enabled = true;

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
        tongue.position = transform.position;

        if (springJoint != null)
        {
            springJoint.enabled = false;
        }

        OnDisableEvent?.Invoke();
    }

    private void UpdateTonguePosition()
    {
        if (springJoint.connectedBody != null)
        {
            tongue.position = springJoint.connectedBody.position;
        }
    }

    private void UpdatePlayerRotation()
    {
        if (isAttached)
        {
            // 혀가 부착된 경우, 플레이어가 혀를 바라보도록 회전
            Vector2 direction = tongue.position - transform.position;
            if (direction.sqrMagnitude > Mathf.Epsilon)
            {
                // 혀 방향으로 플레이어를 회전
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
        else
        {
            // 혀가 부착되지 않은 경우, 이동 방향으로 회전
            Vector2 velocity = rigid.linearVelocity;
            if (velocity.sqrMagnitude > Mathf.Epsilon)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }
    }

    private void UpdatePlayerLookAtTongue()
    {
        Vector2 direction = tongue.position - transform.position;
        if (direction.sqrMagnitude > Mathf.Epsilon)
        {
            // 혀 방향으로 플레이어를 회전
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        }
    }

    private void UpdateLineRenderer()
    {
        if (!lineRenderer.enabled) return;
        lineRenderer.SetPositions(new Vector3[] { tongue.position, transform.position });
    }

    private void ClampSpringDistance()
    {
        if (springJoint.distance > maxTargetDistance)
        {
            springJoint.distance = maxTargetDistance;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (tongueOrigin == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(
            tongueOrigin.position,
            tongueOrigin.position + (transform.right * raycastDistance)
        );

        // Optional: 원형 반경으로도 표시하고 싶다면 아래 추가
        Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
        Gizmos.DrawWireSphere(tongueOrigin.position, raycastDistance);
    }

}
