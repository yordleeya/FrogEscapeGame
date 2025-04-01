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
        { JumpType.EatFly, 2f },
        { JumpType.Mushroom, 2f }
    };

    Rigidbody2D rigid;
    float speed;
    float airMoveSpeed;
    float jumpPower;
    Vector2 maxVelocity;
    float moveX;
    bool isMoving = false;

    [SerializeField]
    RopeAction rope;

    [SerializeField]
    LayerMask groundLayer;

    private bool isMovementStopped = false;
    bool isJumping;

    RaycastHit2D hit;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            Debug.LogError("Rigidbody2D가 Player 오브젝트에 할당되지 않았습니다.");
        }
        speed = stats.Speed;
        airMoveSpeed = stats.AirMoveSpeed;
        jumpPower = stats.JumpPower;
        maxVelocity = stats.MaxVelocity;

        rope.Init(stats.TongueSpeed);
    }

    private void Update()
    {
        if (Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer))
        {
            isMovementStopped = false;
            isJumping = false;
            transform.localEulerAngles = Vector3.zero;
        }
    }
    private void FixedUpdate()
    {
        if (isMovementStopped)
        {
            return;
        }

        if (isMoving)
        {
            if (Mathf.Abs(rigid.linearVelocityX) < Mathf.Abs(maxVelocity.x))
            {
                rigid.linearVelocityX += moveX;
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
        if (context.started || context.performed)
        {
            Vector2 dir = context.ReadValue<Vector2>();

            if (!isJumping)
            {
                moveX = dir.x * speed;
            }
            else
            {
                moveX = dir.x * airMoveSpeed;
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
        Vector2 worldMousePos = new Vector2(worldMousePos3D.x, worldMousePos3D.y);

        Vector2 playerPos = transform.position;

        Vector2 direction = (worldMousePos - playerPos).normalized;

        if (context.started)
        {
            if (!rope.gameObject.activeSelf)
            {
                rope.gameObject.SetActive(true);
                rope.RopeShoot(direction); // 로프 발사
            }
        }
        else if (context.canceled && rope.gameObject.activeSelf && rope.IsAttached)
        {
            if (Mathf.Abs(rigid.linearVelocity.x) > 1 || Mathf.Abs(rigid.linearVelocityY) > 1)
            {
                direction = rigid.linearVelocity.normalized;
            }

            rope.Released();
            Jump(direction, JumpType.MouseRelease); // 마우스 떼면 점프
        }
    }
    public void StopMovement()
    {
        isMovementStopped = true; // 이동 멈춤
    }

    // 이동 재개
    public void ResumeMovement()
    {
        isMovementStopped = false; // 이동 재개
    }

    public void Jump(Vector2 direction, JumpType jumpType = JumpType.MouseRelease)
    {
        OnPlayerJump?.Invoke();

        rigid.linearVelocityY = 0; // 기존 속도 초기화


        if (jumpMultipliers.TryGetValue(jumpType, out float multiplier))
        {
            rigid.AddForce(direction * jumpPower * multiplier, ForceMode2D.Impulse);

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
        OnPlayerJump?.Invoke();

        rigid.linearVelocityY = 0; // 기존 속도 초기화

        Vector2 direction = Vector2.up;
        JumpType jumpType = JumpType.Attach;

        if (jumpMultipliers.TryGetValue(jumpType, out float multiplier))
        {
            rigid.AddForce(direction * jumpPower * multiplier, ForceMode2D.Impulse);

            Debug.Log(jumpType + "에 의해 " + direction + "방향으로 점프");
        }
        else
        {
            Debug.LogWarning($"JumpType {jumpType}의 가중치가 설정되지 않았습니다.");
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Land") || collision.transform.CompareTag("Platform"))
        {
            isMovementStopped = true;
            isJumping = true;
        }
    }

}



