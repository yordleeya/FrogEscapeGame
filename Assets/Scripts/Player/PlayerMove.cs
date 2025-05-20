using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections;
using System.Linq.Expressions;

public class PlayerMove : MonoBehaviour
{
    public UnityEvent OnRopeDelayStart;
    public UnityEvent OnRopeDelayEnd;

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

    [SerializeField]
    private readonly Dictionary<JumpType, float> jumpMultipliers = new()
    {
        { JumpType.Attach, 0.5f },
        { JumpType.EatFly, 1.5f },
        { JumpType.Mushroom, 2f },
        { JumpType.Move, 0.5f },
        { JumpType.MouseRelease, 1.5f}
    };

    Rigidbody2D rigid;
    float speed;
    float jumpPower;
    Vector2 maxVelocity;
    float moveX;
    bool isMoving = false;

    bool isOnGround = false;

    float tongueDelay;


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
        tongueDelay = stats.TongueDelay;

        rope.Init(stats.TongueSpeed);
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

        if (Physics2D.Raycast(transform.position, Vector2.down, 0.51f, groundLayer))
        {
            transform.localEulerAngles = Vector3.zero;
            isOnGround = true;
        }
        else
        {
            isOnGround = false;
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
        direction = context.ReadValue<Vector2>();
        if(context.started)
        {
            transform.localScale = new Vector3(-Mathf.Sign(direction.x), 1, 1);
        }
        else if (context.performed)
        {
            if (rope.IsAttached)
            {
                isMoving = true;

                moveX = direction.x * speed;
                rigid.linearVelocityX += moveX;
            }

        }
        else if (context.canceled)
        {
            isMoving = false;

            if(rope.IsAttached)
            {
                rigid.linearVelocityX *= 0.6f;
            }
        }

         if (context.started && isOnGround)
         {
            if (RhythmManager.IsOnBeat)
            {
                Jump(direction + Vector2.up, JumpType.Move);
            }
            else
            {
                rigid.linearVelocityX *= 0.5f;
            }
         }
    }


    bool isDelayed = false;



    public void OnRope(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 worldMousePos = (Vector2)worldMousePos3D;

        Vector2 playerPos = transform.position;

        Vector2 mouseDirection = (worldMousePos - playerPos).normalized;

        if (context.started)
        {
            if (RhythmManager.IsOnBeat)
            {
                if (!isDelayed)
                {
                    rope.RopeShoot(mouseDirection);
                }
            }
            else
            {
                delayCoroutine = StartCoroutine(RopeDelay());
            }
        }
        else if(context.canceled)
        {
            if (rope.IsAttached)
            {
                if(Mathf.Abs(rigid.linearVelocityX) > 0.5f)
                Jump(rigid.linearVelocity.normalized, JumpType.MouseRelease);
            }

            rope.Released();
        }
    }

    Coroutine delayCoroutine = null;

    IEnumerator RopeDelay()
    {
        if (delayCoroutine != null)
        {
            StopCoroutine(delayCoroutine);
        }

        OnRopeDelayStart?.Invoke();
        isDelayed = true;

        yield return new WaitForSeconds(tongueDelay);

        Debug.Log("딜레이 종료");
        isDelayed = false;

        OnRopeDelayEnd?.Invoke();

        delayCoroutine = null;
    }

    public void Jump(Vector2 direction, JumpType jumpType = JumpType.MouseRelease)
    {
        OnPlayerJump?.Invoke();

        rigid.linearVelocityY = 0; // 기존 속도 초기화

        Debug.Log("점프");

        if (jumpMultipliers.TryGetValue(jumpType, out float multiplier))
        {
            rigid.AddForce(jumpPower * multiplier * direction, ForceMode2D.Impulse);

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

        Debug.Log("점프");

         rigid.AddForce(jumpPower * jumpMultipliers[JumpType.MouseRelease] * direction, ForceMode2D.Impulse);

    }

    public void SetTransparent(bool isTransparent)
    {
        if (TryGetComponent<SpriteRenderer>(out var sprite))
        {
            var color = sprite.color;
            color.a = isTransparent ? 0f : 1f;
            sprite.color = color;
        }
    }


}



