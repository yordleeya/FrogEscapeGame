using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField]
    CoinStats stats;

    PlayerMove playerMove;

    Rigidbody2D rigid;

    float fallSpeed;
    float disableTime;
    float stunTime;

    Coroutine disableCoroutine;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

    }

    private void Start()
    {
        disableTime = stats.DisableTime;
        stunTime = stats.StunTime;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if(playerMove == null)
            {
                playerMove = collision.GetComponent<PlayerMove>();
            }
            playerMove.Stun(stunTime);
        }
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        rigid.linearVelocity = Vector2.zero;
        rigid.linearVelocityY = fallSpeed;
        disableCoroutine = StartCoroutine(Disable());
    }
    private void OnDisable()
    {
        disableCoroutine = null;
    }

    public void Init(float _disableTime, float _fallSpeed)
    {
        disableTime = _disableTime;
        fallSpeed = _fallSpeed;
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(disableTime);

        gameObject.SetActive(false);
    }
}
