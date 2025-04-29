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

    private bool isAttachedToSnake = false;
    private Transform attachedSnakeHead = null;


    [Required("stat 에셋을 넣지 않으면 플레이어가 움직일 수 없습니다.\n" +
    "PlayerStats형식의 스크립터블 오브젝트를 넣어주세요.")]
    [SerializeField]
    PlayerStats stats;

    public enum JumpType
    {
        MouseRelease,
        Attach,
        EatFly,
        Mushroom,
        Move
    }

    // Snake에 부착
    public void AttachToSnake(Transform snakeHead)
    {
        isAttachedToSnake = true;
        attachedSnakeHead = snakeHead;
        rigid.linearVelocity = Vector2.zero; // 속도 초기화
    }

    // Snake에서 해제
    public void DetachFromSnake()
    {
        isAttachedToSnake = false;
        attachedSnakeHead = null;
    }

    private readonly Dictionary<JumpType, float> jumpMultipliers = new()
    {
        { JumpType.MouseRelease, 1f },
        { JumpType.Attach, 0.5f },
        { JumpType.EatFly, 1.5f },
        { JumpType.Mushroom, 2f },
        {JumpType.Move, 1f }
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

        rope.Init(stats.TongueSpeed, stats.TongueShotDistance);
    }

    private void FixedUpdate()
    {
        if (isMoving && rope.IsAttached)
        {
            if (Mathf.Abs(rigid.linearVelocityX) < Mathf.Abs(maxVelocity.x))
            {
                rigid.linearVelocityX = moveX;
            }

            OnPlayerMove?.Invoke();
        }

        if (Physics2D.Raycast(transform.position, Vector2.down, 1f, groundLayer))
        {
            transform.localEulerAngles = Vector3.zero;
        }

        if (isAttachedToSnake && attachedSnakeHead != null)
        {
            // Snake 머리 위치에 플레이어를 고정
            transform.position = attachedSnakeHead.position;
            return; // 이동 입력 무시
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (RhythmManager.IsOnBeat)
        {

            direction = context.ReadValue<Vector2>();

            if (rope.IsAttached && context.performed)
            {
                moveX = direction.x * speed;
            }
            else if (context.started)
            {
                Jump(direction + Vector2.up, JumpType.Move);
            }

            else if (context.canceled)
            {
                isMoving = true;
            }

        }
    }


    public void OnRope(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 worldMousePos = (Vector2)worldMousePos3D;

        Vector2 playerPos = transform.position;

        Vector2 mouseDirection = (worldMousePos - playerPos).normalized;

        if (context.started && RhythmManager.IsOnBeat)
        {
            if (!rope.IsAttached)
            {
                rope.RopeShoot(mouseDirection);
            }
        }
        else if(context.canceled)
        {
            if (rope.IsAttached)
            {
                Jump(mouseDirection, JumpType.MouseRelease);
            }

            rope.Released();
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
    public void SetTransparent(bool isTransparent)
    {
        var sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
        var color = sprite.color;
        color.a = isTransparent ? 0f : 1f;
            sprite.color = color;
        }
    }


}



