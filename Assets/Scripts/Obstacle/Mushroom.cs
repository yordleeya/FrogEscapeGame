using UnityEngine;

public class Mushroom : MonoBehaviour
{
    PlayerMove playerMove;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if(playerMove == null)
            {
                playerMove = collision.transform.GetComponent<PlayerMove>();
            }
            Vector2 randomDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(0.5f, 1f)).normalized;
            playerMove.Jump(randomDirection, PlayerMove.JumpType.Mushroom);
        }
    }
}
