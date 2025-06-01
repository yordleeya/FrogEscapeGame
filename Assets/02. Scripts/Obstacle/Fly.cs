using UnityEngine;

public class Fly : MonoBehaviour
{
    PlayerMove playerMove;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMove= collision.GetComponent<PlayerMove>();
            playerMove.Jump(playerMove.Direction+Vector2.up, PlayerMove.JumpType.EatFly);
        }
        
    }

}
