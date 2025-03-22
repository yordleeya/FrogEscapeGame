using UnityEngine;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{

    [SerializeField]
    PlayerStats stat;

    float tongueSpeed;

    [SerializeField]
    LayerMask GrapplingObj;

    [SerializeField]
    Rigidbody2D tongueEndRigid;

    PlayerMove move;

    private void Awake()
    {
        move = GetComponent<PlayerMove>();

        tongueSpeed = stat.TongueSpeed;
    }

    void RopeShoot(Vector2 direction)
    {
        tongueEndRigid.AddForce(direction * tongueSpeed, ForceMode2D.Impulse);
    }

    public void OnRope(InputAction.CallbackContext context)
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        // 마우스 위치를 월드 좌표로 변환 (Vector3 → Vector2 변환)
        Vector3 worldMousePos3D = Camera.main.ScreenToWorldPoint(new Vector3(mousePosition.x, mousePosition.y, Camera.main.nearClipPlane));
        Vector2 worldMousePos = new Vector2(worldMousePos3D.x, worldMousePos3D.y);

        // 현재 플레이어 위치
        Vector2 playerPos = transform.position;

        // 방향 벡터 계산 (목표 지점 - 현재 위치)
        Vector2 direction = (worldMousePos - playerPos).normalized;

        if (context.started)
        {
            RopeShoot(direction); // 로프 발사
        }
        else if (context.canceled)
        {
            move.Jump(direction, PlayerMove.JumpType.MouseRelease); // 마우스 떼면 점프
        }
    }

}
