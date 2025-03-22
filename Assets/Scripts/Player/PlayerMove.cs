using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public UnityEvent OnPlayerMove;
    public UnityEvent OnPlayerJump;
    public float jumpForce = 10f;


    [Required("stat 에셋을 넣지 않으면 플레이어가 움직일 수 없습니다.\n" +
    "PlayerStats형식의 스크립터블 오브젝트를 넣어주세요.")]
    [SerializeField]
    PlayerStats stats;

    public enum JumpType
    {
        MouseRelease,
        EatFly
    }

    private readonly Dictionary<JumpType, float> jumpMultipliers = new()
    {
        { JumpType.MouseRelease, 1f },
        { JumpType.EatFly, 2f }
    };

    Rigidbody2D rigid;
    float speed;
    float jumpPower;
    Vector2 maxVelocity;
    float moveX;
    bool isMoving = false;
    private bool isMovementStopped = false;
    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (rigid == null)
        {
            Debug.LogError("Rigidbody2D가 Player 오브젝트에 할당되지 않았습니다.");
        }
        speed = stats.Speed;
        jumpPower = stats.JumpPower;
        maxVelocity = stats.MaxVelocity;
    }

    private void FixedUpdate()
    {
        if (isMovementStopped)
        {
            rigid.linearVelocity = Vector2.zero; // 이동 멈추기
            return;
        }

        if (isMoving)
        {
            if (Mathf.Abs(rigid.linearVelocityX) < Mathf.Abs(maxVelocity.x))
            {
                rigid.linearVelocityX += moveX;
            }
        }
        else
        {
            rigid.linearVelocityX *= 0.8f;
        }
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            Vector2 dir = context.ReadValue<Vector2>();

            moveX = dir.x * speed;
            isMoving = true;
            OnPlayerMove?.Invoke();
        }
        else
        {
            isMoving = false;
        }

    }
    public void StopMovement()
    {
        isMovementStopped = true; // 이동 멈춤
        rigid.linearVelocity = Vector2.zero; // Rigidbody2D의 속도도 0으로 설정
    }

    // 이동 재개
    public void ResumeMovement()
    {
        isMovementStopped = false; // 이동 재개
    }

    public void Jump(Vector2 direction, JumpType jumpType)
    {

        OnPlayerJump?.Invoke();

        if (jumpMultipliers.TryGetValue(jumpType, out float multiplier))
        {
            rigid.AddForce(direction * jumpPower * multiplier, ForceMode2D.Impulse);
        }
        else
        {
            Debug.LogWarning($"JumpType {jumpType}의 가중치가 설정되지 않았습니다.");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Mushroom"))
        {
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            rigid.linearVelocity = Vector2.zero; // 기존 속도 초기화
            rigid.AddForce(randomDirection * jumpForce, ForceMode2D.Impulse);
            Debug.Log("버섯과 충돌! 랜덤 점프 방향: " + randomDirection);
        }
    }
}



