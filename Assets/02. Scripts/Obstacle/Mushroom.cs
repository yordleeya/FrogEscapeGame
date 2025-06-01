using UnityEngine;

public class Mushroom : MonoBehaviour
{
    PlayerMove playerMove;
    public float bounceForce = 10f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(playerMove == null)
            {
                playerMove = collision.transform.GetComponent<PlayerMove>();
            }
            // 플레이어와 버섯의 위치 차이(방향) 계산
            Vector2 direction = (collision.transform.position - transform.position).normalized;
            float angle = 45f * Mathf.Deg2Rad; // 45도를 라디안으로 변환
            float cos = Mathf.Cos(angle);
            float sin = Mathf.Sin(angle);
            Vector2 rotatedDirection = new Vector2(
                direction.x * cos - direction.y * sin,
                direction.x * sin + direction.y * cos
            );
            Vector2 bounceDirection = rotatedDirection * bounceForce;
            playerMove.Jump(bounceDirection, PlayerMove.JumpType.Mushroom);
        }
    }
}
