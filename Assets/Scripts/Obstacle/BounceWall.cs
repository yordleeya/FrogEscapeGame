using UnityEngine;

public class BounceWall : MonoBehaviour
{
    [Header("튕김 설정")]
    [SerializeField] private float bounceForce = 10f;    // 튕겨내는 힘의 크기
    [SerializeField] private float bounceAngle = 30f;    // 튕겨내는 각도 (도)

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Player 태그를 가진 오브젝트와 충돌했는지 확인
        if (collision.gameObject.CompareTag("Player"))
        {
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();

            if (playerRb != null)
            {
                // 현재 플레이어의 속도를 0으로 초기화
                playerRb.linearVelocity = Vector2.zero;

                // 튕겨내는 방향 계산 (30도)
                Vector2 bounceDirection = Quaternion.Euler(0, 0, bounceAngle) * -collision.contacts[0].normal;

                // 계산된 방향으로 힘을 가함
                playerRb.AddForce(bounceDirection * bounceForce, ForceMode2D.Impulse);
            }
        }
    }
}