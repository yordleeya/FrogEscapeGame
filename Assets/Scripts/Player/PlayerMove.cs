using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;

public class PlayerMove : MonoBehaviour
{
    public UnityEvent OnPlayerMove;
    public UnityEvent OnPlayerJump;


    [Required("stat 에셋을 넣지 않으면 플레이어가 움직일 수 없습니다.\n" +
    "PlayerStats형식의 스크립터블 오브젝트를 넣어주세요.")]
    [SerializeField]
    PlayerStats stats;

    public enum JumpType
    {
        MouseRelease,
        Attach,
        EatFly,
        Mushroom
    }

    private readonly Dictionary<JumpType, float> jumpMultipliers = new()
    {
        { JumpType.MouseRelease, 1f },
        { JumpType.Attach, 0.5f },
        { JumpType.EatFly, 1.5f },
        { JumpType.Mushroom, 2f }
    };

    Rigidbody2D rigid;
    float speed;
    float jumpPower;
    Vector2 maxVelocity;
    float moveX;
    bool isMoving = false;

    [SerializeField]
    RopeAction rope;

    [SerializeField]
    LayerMask groundLayer;


    RaycastHit2D hit;
    Vector2 direction = Vector2.right;
    SpriteRenderer spriteRenderer;

    public Vector2 Direction { get => direction;}

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            Debug.LogError("Rigidbody2D가 Player 오브젝트에 할당되지 않았습니다.");
        }
        speed = stats.Speed;
        jumpPower = stats.JumpPower;
        maxVelocity = stats.MaxVelocity;

        rope.Init(stats.TongueSpeed, stats.TongueRangeExpandSpeed, stats.MaxTongueShotDistance);
    }

    private void Update()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer))
        {
            transform.localEulerAngles = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        if (isMoving)
        {
            if (Mathf.Abs(rigid.linearVelocityX) < Mathf.Abs(maxVelocity.x))
            {
                rigid.linearVelocityX = moveX;

            }


            OnPlayerMove?.Invoke();
        }
        else
        {
            rigid.linearVelocityX *= 0.8f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if ((context.started || context.performed))
        {
            direction = context.ReadValue<Vector2>();

            if (rope.IsAttached)
            {
                moveX = direction.x * speed;
            }
            else
            {
                moveX = direction.x * speed * 0.5f;
            }

            isMoving = true;
        }
        else
        {
            isMoving = false;
        }
    }

    public void OnRope(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 worldMousePos = (Vector2)worldMousePos3D;

        Vector2 playerPos = transform.position;

        Vector2 direction = (worldMousePos - playerPos).normalized;

        if (context.started)// 로프 발사
        {
            rope.RopeShoot(direction); 
        }
        else if (context.canceled && rope.IsAttached) // 마우스 떼면 점프
        {
            // Handle에 부착된 경우는 점프하지 않음
            if (rope.IsAttachedToHandle)
            {
                rope.Released();
                return;
            }

            if (Mathf.Abs(rigid.linearVelocity.x) > 1 || Mathf.Abs(rigid.linearVelocityY) > 1)
            {
                direction = rigid.linearVelocity.normalized;
            }

            rope.Released();
            Jump(direction, JumpType.MouseRelease);
        }
    }

    public void Jump(Vector2 direction, JumpType jumpType = JumpType.MouseRelease)
    {
        OnPlayerJump?.Invoke();

        rigid.linearVelocityY = 0; // 기존 속도 초기화


        if (jumpMultipliers.TryGetValue(jumpType, out float multiplier))
        {
            rigid.AddForce(jumpPower * multiplier * direction, ForceMode2D.Impulse);

            Debug.Log(jumpType + "에 의해 " + direction + "방향으로 점프");
        }
        else
        {
            Debug.LogWarning($"JumpType {jumpType}의 가중치가 설정되지 않았습니다.");
        }
    }
    /// <summary>
    /// attach 상태일떄만 사용되는 함수, 그 외의 경우에는 사용 금지
    /// </summary>
    /// 
    public void Jump()
    {
        if(rigid.linearVelocityY <= 0)
        {
            return;
        }

        OnPlayerJump?.Invoke();

        Vector2 direction = Vector2.up;
        JumpType jumpType = JumpType.Attach;

        if (jumpMultipliers.TryGetValue(jumpType, out float multiplier))
        {
            rigid.AddForce(jumpPower * multiplier * direction, ForceMode2D.Impulse);

            Debug.Log(jumpType + "에 의해 " + direction + "방향으로 점프");
        }
        else
        {
            Debug.LogWarning($"JumpType {jumpType}의 가중치가 설정되지 않았습니다.");
        }
    }

}



