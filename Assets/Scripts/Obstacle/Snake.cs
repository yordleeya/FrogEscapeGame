using UnityEngine;
using System.Collections;

public class Snake : MonoBehaviour
{
    public Transform tailPoint; // TailPoint를 에디터에서 할당
    public float teleportDelay = 1.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerMove = other.GetComponent<PlayerMove>();
            if (playerMove != null)
            {
                playerMove.AttachToSnake(this.transform);
                StartCoroutine(TeleportAfterDelay(other.transform, playerMove));
            }
        }
    }

    private IEnumerator TeleportAfterDelay(Transform player, PlayerMove playerMove)
    {
        yield return new WaitForSeconds(teleportDelay);
        player.position = tailPoint.position; // 꼬리 위치로 이동
        if (playerMove != null)
            playerMove.DetachFromSnake();
    }
}