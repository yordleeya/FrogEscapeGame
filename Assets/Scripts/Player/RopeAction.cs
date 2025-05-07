using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RopeAction : MonoBehaviour
{
    [FoldoutGroup("Attach Ray Settings"), Tooltip("로프가 부착될 수 있는 오브젝트의 레이어 마스크")]
    [SerializeField] private LayerMask ropeAttachLayer = default;
    [FoldoutGroup("Attach Ray Settings"), Tooltip("로프 발사 최대 거리")]
    [SerializeField] private float maxTongueShotDistance = 20f;
    private readonly float tongueShotDistance = 0;

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


    private bool isFlying = false;

    private bool isSlipping = false;
    private SlippingPlatform currentSlippingPlatform = null;
    private float currentSlipDistance = 0f;
    private Vector3 initialAttachPosition;


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

        ResetTongueTransform();
    }

    private void Update()
    {
        if(isShooting)
        {
            UpdateLineRenderer();
        }
    }

    public void Init(float _tongueSpeed)
    {
        if (!enabled) return;

        tongueSpeed = _tongueSpeed;
    }

    Vector2 mouseDirection;

    bool isShooting = false;

    public bool IsAttached { get => isAttached; set => isAttached = value; }

    public void RopeShoot(Vector2 direction)
    {
        if (isAttached || isFlying) return;

        mouseDirection = direction;

        tongue.position = tongueOrigin.position;

        tongueRigidbody.bodyType = RigidbodyType2D.Dynamic;

        tongueRigidbody.linearVelocity = Vector2.zero;
        tongueRigidbody.AddForce(direction * tongueSpeed, ForceMode2D.Impulse);

        Debug.Log(direction);
        OnShot?.Invoke();
        isShooting = true;
    }

    public void ConnectSpringJoint()
    {
        if (springJoint == null || tongueRigidbody == null || playerRigid == null) return;

        springJoint.connectedBody = tongueRigidbody;
        float currentDistance = Vector2.Distance(playerRigid.position, tongueRigidbody.position);
        springJoint.distance = Mathf.Clamp(currentDistance, minRopeDistance, maxRopeDistance);

        springJoint.enabled = true;
        Debug.Log($"[RopeAction] SpringJoint connected. Initial distance: {springJoint.distance}");
    }

    public void Released()
    {
        if (!enabled) return;

        ResetRopeState();
    }

    public void ResetRopeState()
    {
        Debug.LogWarning("ResetRopeState() 호출됨! 혀가 해제됨!");
        if (!enabled) return;

        bool wasAttached = isAttached;

        isShooting = false;
        isAttached = false;
        isFlying = false;
        isSlipping = false;
        currentSlippingPlatform = null;
        currentSlipDistance = 0f;

        if (lineRenderer != null) lineRenderer.enabled = false;

        if (springJoint != null)
        {
            springJoint.enabled = false;
            springJoint.connectedBody = null;
        }

        ResetTongue();
        ResetTongueTransform();

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
        }
    }

    private void UpdateLineRenderer()
    {
        if (playerRigid == null || tongue == null) return;

        lineRenderer.enabled = true;

        lineRenderer.SetPosition(0, playerRigid.position);
        lineRenderer.SetPosition(1, tongue.position);
    }



    public void ResetTongue()
    {
        if (tongueRigidbody != null)
        {
            tongueRigidbody.linearVelocity = Vector3.zero;
            tongueRigidbody.bodyType = RigidbodyType2D.Kinematic;
            tongueRigidbody.angularVelocity = 0;
        }
    }
}
