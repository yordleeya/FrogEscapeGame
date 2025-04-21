using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    [FoldoutGroup("Attach Ray Settings"), Tooltip("로프가 부착될 수 있는 오브젝트의 레이어 마스크")]
    [SerializeField] private LayerMask ropeAttachLayer = default;
    [FoldoutGroup("Attach Ray Settings"), Tooltip("로프 발사 최대 거리")]
    [SerializeField] private float maxTongueShotDistance = 20f;
    private float tongueShotDistance = 0;

    [FoldoutGroup("Rope Physics"), Tooltip("로프 최대 길이 (SpringJoint 거리)")]
    [SerializeField] private float maxRopeDistance = 18f;
    [FoldoutGroup("Rope Physics"), Tooltip("로프 최소 길이 (SpringJoint 거리)")]
    [SerializeField] private float minRopeDistance = 1f;

    [FoldoutGroup("Tongue References"), Tooltip("혀(로프 끝) 역할을 하는 오브젝트의 Transform")]
    [Required(InfoMessageType.Error), SerializeField] private Transform tongue = null;
    [FoldoutGroup("Tongue References"), Tooltip("혀가 발사 시작되는 위치 Transform")]
    [Required(InfoMessageType.Error), SerializeField] private Transform tongueOrigin = null;
    [FoldoutGroup("Tongue References"), Tooltip("혀 오브젝트에 부착된 Rigidbody2D 컴포넌트")]
    [Required(InfoMessageType.Error), SerializeField] private Rigidbody2D tongueRigidbody = null;

    [FoldoutGroup("Events"), Tooltip("로프 발사 시 호출되는 이벤트")]
    public UnityEvent OnShot;
    [FoldoutGroup("Events"), Tooltip("로프가 부착되었을 때 호출되는 이벤트")]
    public UnityEvent OnAttached;
    [FoldoutGroup("Events"), Tooltip("로프가 해제되었을 때 호출되는 이벤트")]
    public UnityEvent OnDisableEvent;

    private LineRenderer lineRenderer;
    private SpringJoint2D springJoint;
    private Rigidbody2D playerRigid;

    private float tongueSpeed;
    private bool isAttached = false;
    public bool IsAttached => isAttached;
    
    // Handle 부착 여부를 확인하는 프로퍼티 추가
    public bool IsAttachedToHandle => isAttached && hitInfo.collider != null && hitInfo.collider.GetComponent<HandleTrigger>() != null;

    private bool isFlying = false;
    private RaycastHit2D hitInfo;

    private RigidbodyType2D initialTongueBodyType;
    private bool isSlipping = false;
    private SlippingPlatform currentSlippingPlatform = null;
    private float currentSlipDistance = 0f;
    private Vector3 initialAttachPosition;


    [SerializeField]
    PlayerStats stat;
    float expandSpeed;

    [SerializeField]
    RectTransform circleTransform;

    bool isHolding;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        springJoint = GetComponent<SpringJoint2D>();
        playerRigid = GetComponentInParent<Rigidbody2D>();

        bool setupError = false;
        if (playerRigid == null)
        {
            Debug.LogError("[RopeAction] Player Rigidbody2D not found in parent or self!", this);
            setupError = true;
        }
        if (tongue == null)
        {
            Debug.LogError("[RopeAction] 'Tongue' Transform is not assigned in the Inspector!", this);
            setupError = true;
        }
        if (tongueOrigin == null)
        {
            Debug.LogError("[RopeAction] 'Tongue Origin' Transform is not assigned in the Inspector!", this);
            setupError = true;
        }
        if (tongueRigidbody == null)
        {
            Debug.LogError("[RopeAction] 'Tongue Rigidbody' is not assigned in the Inspector!", this);
            setupError = true;
        }
        if (ropeAttachLayer == 0)
        {
            Debug.LogWarning("[RopeAction] 'Rope Attach Layer' is not set. Rope might not attach to anything.", this);
        }

        if (setupError)
        {
            Debug.LogError("[RopeAction] Setup is incomplete. Disabling component to prevent errors.", this);
            enabled = false;
            return;
        }

        lineRenderer.positionCount = 2;
        lineRenderer.enabled = false;

        springJoint.enabled = false;
        springJoint.autoConfigureDistance = false;
        springJoint.enableCollision = true;
    }

    private void Start()
    {
        if (!enabled) return;

        initialTongueBodyType = tongueRigidbody.bodyType;
        tongueRigidbody.simulated = false;
        ResetTongueTransform();
    }

    public void Init(float _tongueSpeed, float _expandSpeed, float _maxTongueDistance)
    {
        if (!enabled) return;

        tongueSpeed = _tongueSpeed;
        expandSpeed = _expandSpeed;
        maxTongueShotDistance = _maxTongueDistance;
    }

    public void OnInput(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            circleTransform.gameObject.SetActive(true);
            isHolding = true;
        }
        else if (context.canceled || isAttached)
        {
            circleTransform.gameObject.SetActive(false);
            tongueShotDistance = 0;
            isHolding = false;
        }
    }

    private void Update()
    {
        if (!enabled) return;

        if (isFlying)
        {
            FlyTongue();
            UpdateLineRenderer();
            return;
        }

        if (isAttached)
        {
            UpdateLineRenderer();

            if (isSlipping && currentSlippingPlatform != null)
            {
                HandleSlipping();
            }
        }
        else
        {
            if (tongueRigidbody != null && !tongueRigidbody.simulated)
            {
                ResetTongueTransform();
            }
        }

        if (isHolding && tongueShotDistance < maxTongueShotDistance)
        {
            float delta = expandSpeed * Time.deltaTime;
            tongueShotDistance = Mathf.Clamp(tongueShotDistance + delta, 0, maxTongueShotDistance);
            circleTransform.sizeDelta = new Vector2(tongueShotDistance * 200, tongueShotDistance * 200);
        }
    }

    public void RopeShoot(Vector2 direction)
    {
        if (!enabled || isAttached || isFlying) return;


        hitInfo = Physics2D.Raycast(tongueOrigin.position, direction.normalized, tongueShotDistance, ropeAttachLayer);

        if (hitInfo.collider == null)
        {
            Debug.Log("[RopeAction] Rope missed.");
            return;
        }

        OnShot?.Invoke();

        Vector3 hitPoint = hitInfo.point;
        Collider2D hitCollider = hitInfo.collider;
        currentSlippingPlatform = null;
        bool attachSuccess = false;

        if (hitCollider.CompareTag("Platform"))
        {
            currentSlippingPlatform = hitCollider.GetComponent<SlippingPlatform>();
            attachSuccess = true;
            Debug.Log($"[RopeAction] Hit Platform: '{hitCollider.name}'. Slipping: {currentSlippingPlatform != null}");
        }
        else if (hitCollider.CompareTag("Land"))
        {
            Debug.Log($"[RopeAction] Hit Land ('{hitCollider.name}'). Cannot attach.");
            return;
        }
        else
        {
            Debug.LogWarning($"[RopeAction] Hit object with unhandled tag: Tag='{hitCollider.tag}', Name='{hitCollider.name}'");
            return;
        }

        if (attachSuccess)
        {
            ResetTongueTransform();
            tongueRigidbody.simulated = true;
            tongueRigidbody.bodyType = RigidbodyType2D.Kinematic;
            tongueRigidbody.linearVelocity = Vector2.zero;
            tongueRigidbody.angularVelocity = 0f;

            isFlying = true;
            lineRenderer.enabled = true;
        }
    }

    private void FlyTongue()
    {
        Vector2 currentPos = tongueRigidbody.position;
        Vector2 targetPos = hitInfo.point;
        Vector2 newPos = Vector2.MoveTowards(currentPos, targetPos, tongueSpeed * Time.deltaTime);
        tongueRigidbody.MovePosition(newPos);

        if (Vector2.Distance(newPos, targetPos) < 0.05f)
        {
            isFlying = false;
            // 👇 추가된 코드: 붙을 때 법선 방향으로 살짝 밀어 넣기
            Vector2 offset = -hitInfo.normal * 0.01f; // 법선 방향으로 0.05만큼 안쪽으로
            Vector2 adjustedPos = hitInfo.point + offset;

            tongueRigidbody.MovePosition(adjustedPos); // 수정된 위치로 혀를 이동

            tongueRigidbody.linearVelocity = Vector2.zero;


            if (hitInfo.collider == null || !hitInfo.collider.CompareTag("Platform"))
            {
                 Debug.LogWarning("[RopeAction] Target platform disappeared or changed tag during flight.");
                 ResetRopeState();
                 return;
            }

            if (currentSlippingPlatform != null)
            {
                AttachToSlippingPlatform();
            }
            else
            {
                AttachToStaticPoint();
            }
        }
    }

    private void AttachToSlippingPlatform()
    {
        Debug.Log($"[RopeAction] Attaching to Slipping Platform: {currentSlippingPlatform.gameObject.name}");
        isSlipping = true;
        currentSlipDistance = 0f;
        initialAttachPosition = tongueRigidbody.position;

        tongueRigidbody.bodyType = RigidbodyType2D.Kinematic;
        tongueRigidbody.simulated = true;

        ConnectSpringJoint();

        isAttached = true;
        OnAttached?.Invoke();
    }

    private void AttachToStaticPoint()
    {
        if (hitInfo.collider == null) return;
        Debug.Log($"[RopeAction] Attaching to Static Point: {hitInfo.collider.name}");

        // Handle에 부착될 때는 Kinematic으로 설정
        if (hitInfo.collider.GetComponent<HandleTrigger>())
        {
            tongueRigidbody.bodyType = RigidbodyType2D.Kinematic;
        }
        else
        {
            tongueRigidbody.bodyType = RigidbodyType2D.Static;
        }
        
        tongueRigidbody.simulated = true;

        ConnectSpringJoint();

        isAttached = true;
        
        // Handle에 부착될 때는 점프하지 않도록 수정
        if (!hitInfo.collider.GetComponent<HandleTrigger>())
        {
            OnAttached?.Invoke();
        }
    }

    private void ConnectSpringJoint()
    {
        if (springJoint == null || tongueRigidbody == null || playerRigid == null) return;

        springJoint.connectedBody = tongueRigidbody;
        float currentDistance = Vector2.Distance(playerRigid.position, tongueRigidbody.position);
        springJoint.distance = Mathf.Clamp(currentDistance, minRopeDistance, maxRopeDistance);

        springJoint.enabled = true;
        Debug.Log($"[RopeAction] SpringJoint connected. Initial distance: {springJoint.distance}");
    }

    private void HandleSlipping()
    {
        if (tongueRigidbody.bodyType != RigidbodyType2D.Kinematic || currentSlippingPlatform == null)
        {
            return;
        }

        float speed = currentSlippingPlatform.slipSpeed;
        float maxDistance = currentSlippingPlatform.maxSlipDistance;

        Vector2 slipDelta = Vector2.down * speed * Time.deltaTime;
        Vector2 newPosition = tongueRigidbody.position + slipDelta;

        tongueRigidbody.MovePosition(newPosition);

        currentSlipDistance = Vector2.Distance(initialAttachPosition, newPosition);
        if (currentSlipDistance >= maxDistance)
        {
            Debug.Log($"[RopeAction] Max slip distance ({maxDistance}) reached. Releasing rope.");
            ResetRopeState();
        }
    }

    public void Released()
    {
        if (!enabled) return;
        if (isAttached || isFlying)
        {
            ResetRopeState();
        }
    }

    private void ResetRopeState()
    {
        Debug.LogWarning("ResetRopeState() 호출됨! 혀가 해제됨!");
        if (!enabled) return;

        Debug.Log("[RopeAction] Resetting rope state...");
        bool wasAttached = isAttached;

        isAttached = false;
        isFlying = false;
        isSlipping = false;
        currentSlippingPlatform = null;
        currentSlipDistance = 0f;
        hitInfo = default;

        if (lineRenderer != null) lineRenderer.enabled = false;

        if (springJoint != null)
        {
            springJoint.enabled = false;
            springJoint.connectedBody = null;
        }

        if (tongueRigidbody != null)
        {
            // 먼저 bodyType 되돌리기 (Static → Dynamic 등)
            tongueRigidbody.bodyType = initialTongueBodyType;
            // 그 다음 속도 초기화
            tongueRigidbody.linearVelocity = Vector2.zero;
            tongueRigidbody.angularVelocity = 0f;
            //마지막에 비활성화
            tongueRigidbody.simulated = false;

            ResetTongueTransform();
        }

        if (wasAttached)
        {
            OnDisableEvent?.Invoke();
        }
    }

    private void ResetTongueTransform()
    {
        if (tongue != null && tongueOrigin != null)
        {
            tongue.position = tongueOrigin.position;
            tongue.rotation = tongueOrigin.rotation;
        }
    }

    private void UpdateLineRenderer()
    {
        if (!lineRenderer.enabled || playerRigid == null || tongue == null) return;

        lineRenderer.SetPosition(0, playerRigid.position);
        lineRenderer.SetPosition(1, tongue.position);
    }

    private void OnDrawGizmosSelected()
    {
        if (tongueOrigin != null)
        {
            Gizmos.color = Color.cyan;
            Vector3 gizmoDirection = transform.right;
            Gizmos.DrawLine(tongueOrigin.position, tongueOrigin.position + gizmoDirection * tongueShotDistance);
            Gizmos.color = new Color(0f, 1f, 1f, 0.1f);
            Gizmos.DrawWireSphere(tongueOrigin.position, tongueShotDistance);
        }

        if (!Application.isPlaying) return;

        if (tongue != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(tongue.position, 0.3f);
        }

        if (isSlipping)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(initialAttachPosition, 0.15f);
            if (tongue != null) Gizmos.DrawLine(initialAttachPosition, tongue.position);
        }
    }
}
