using UnityEngine;
using System.Collections;
public class Coin : MonoBehaviour
{
    public PlayerMove player;
    public float fallSpeed = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            player.StopMovement(); // 코인과 충돌하면 이동 멈춤
            Destroy(gameObject); // 코인 객체 삭제 (옵션)
            StartCoroutine(ResumePlayerMovementAfterDelay()); // 1초 후 이동 재개
        }
    }

    private IEnumerator ResumePlayerMovementAfterDelay()
    {
        yield return new WaitForSeconds(1f); // 1초 대기
        player.ResumeMovement(); // 이동 재개
    }
}
