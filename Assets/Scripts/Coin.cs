using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField]
    PlayerMove playerMove;

    Rigidbody2D rigid;

    [SerializeField]
    float fallSpeed = 1f;

    [SerializeField]
    float disableTime = 3f;

    Coroutine disableCoroutine;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();

    }

    private void OnEnable()
    {
        rigid.linearVelocityY = fallSpeed;
        disableCoroutine = StartCoroutine(Disable());
    }
    private void OnDisable()
    {
        rigid.linearVelocity = Vector2.zero;
        disableCoroutine = null;
    }

    IEnumerator Disable()
    {
        yield return new WaitForSeconds(disableTime);

        gameObject.SetActive(false);
    }
}
