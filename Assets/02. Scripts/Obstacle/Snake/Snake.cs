using UnityEngine;
using System.Collections; 

public class Snake : MonoBehaviour
{
    public Transform tailPoint;
    public Transform headPoint;
    public float teleportDelay = 2.0f;

    private Animator animator;
    private bool isPlayerAttached = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPlayerAttachRequest(PlayerMove playerMove)
    {
        if (isPlayerAttached) return; // 이미 부착 중이면 무시

        isPlayerAttached = true;

        // Eat 애니메이션 파라미터 true (한 번만)
        if (animator != null)
            animator.SetBool("Eat", true);

        playerMove.AttachToSnake(headPoint);
        playerMove.SetTransparent(true); // 부착 시 투명하게
        StartCoroutine(TeleportAfterDelay(playerMove.transform, playerMove));
    }

    private IEnumerator TeleportAfterDelay(Transform player, PlayerMove playerMove)
    {
        yield return new WaitForSeconds(teleportDelay);
        player.position = tailPoint.position;
        if (playerMove != null)
        {
            playerMove.DetachFromSnake();
            playerMove.SetTransparent(false); // 해제 시 다시 보이게
        }

        // Eat 애니메이션 파라미터 false (Idle로 복귀)
        if (animator != null)
            animator.SetBool("Eat", false);

        isPlayerAttached = false; // 부착 해제
    }
}