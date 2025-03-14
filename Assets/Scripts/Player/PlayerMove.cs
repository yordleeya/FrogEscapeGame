using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class PlayerMove : MonoBehaviour
{
    public UnityEvent OnPlayerMove;

    Rigidbody2D rigid;

    [SerializeField]
    PlayerStats stats;

    float speed;

    Vector2 maxVelocity;

    float moveX;

    bool isMoving = false;

    [Required("stat 에셋을 넣지 않으면 플레이어가 움직일 수 없습니다.\n" +
        "PlayerStats형식의 스크립터블 오브젝트를 넣어주세요.")]


    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

        speed = stats.Speed;
        maxVelocity = stats.MaxVelocity;
    }

    private void FixedUpdate()
    {
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

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Debug.Log("점프");
        }
    }
}
