using UnityEngine;

public class SnakeHead : MonoBehaviour
{
    public Snake snake; // 에디터에서 Snake 오브젝트 할당

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            var playerMove = other.GetComponent<PlayerMove>();
            if (playerMove != null && snake != null)
            {
                snake.OnPlayerAttachRequest(playerMove);
            }
        }
    }
}