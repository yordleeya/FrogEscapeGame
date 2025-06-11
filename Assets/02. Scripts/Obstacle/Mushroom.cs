using UnityEngine;
using System.Collections;

public class Mushroom : MonoBehaviour
{
    PlayerMove playerMove;
    public float bounceForce = 10f;
    public float respawnTime = 5f;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 버섯 비활성화 및 재활성화 코루틴 시작
            StartCoroutine(RespawnCoroutine());

            // 플레이어 점프력 변경 코루틴 시작
            PlayerMove playerMove = collision.transform.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                StartCoroutine(ChangeJumpPowerCoroutine(playerMove));
            }
        }
    }

    IEnumerator RespawnCoroutine()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Collider2D col = GetComponent<Collider2D>();
        if (sr != null) sr.enabled = false;
        if (col != null) col.enabled = false;
        yield return new WaitForSeconds(respawnTime);
        if (sr != null) sr.enabled = true;
        if (col != null) col.enabled = true;
    }

    IEnumerator ChangeJumpPowerCoroutine(PlayerMove playerMove)
    {
        float originalJumpPower = playerMove.JumpPower;
        playerMove.JumpPower = 4f;
        yield return new WaitForSeconds(6f);
        playerMove.JumpPower = originalJumpPower;
    }
}
