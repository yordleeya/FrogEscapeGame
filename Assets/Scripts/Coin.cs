using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    [SerializeField]
    PlayerMove playerMove;

    Rigidbody2D rigid;

    float fallSpeed;

    float disableTime;

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
