using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RopeAction : MonoBehaviour
{


    [FoldoutGroup("Rope Physics"), Tooltip("로프 최대 길이 (SpringJoint 거리)")]
    [SerializeField] private float maxRopeDistance = 5f;
    [FoldoutGroup("Rope Physics"), Tooltip("로프 최소 길이 (SpringJoint 거리)")]
    [SerializeField] private float minRopeDistance = 0.3f;

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

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        springJoint = GetComponent<SpringJoint2D>();
        playerRigid = GetComponentInParent<Rigidbody2D>();

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



        if (lineRenderer != null) lineRenderer.enabled = false;

        if (springJoint != null)
        {
            springJoint.enabled = false;
            springJoint.connectedBody = null;
        }

        ResetTongueTransform();

        if (wasAttached)
        {
            OnDisableEvent?.Invoke();
        }

        tongueRigidbody.bodyType = RigidbodyType2D.Kinematic;
        tongue.transform.parent = transform;
        tongue.transform.localPosition = Vector3.zero ;

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
}
