using UnityEngine;
using System.Collections;

public class GoldenBall : MonoBehaviour
{
    private bool isAttached = false;
    private Rigidbody2D rigid;
    private Collider2D col;
    private bool isInvincible = false; // 무적 상태

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player에 닿았을 때 PoketPoint에 공을 붙임
        if (!isAttached && collision.CompareTag("Player"))
        {
            Transform poketPoint = collision.transform.Find("PoketPoint");
            if (poketPoint != null)
            {
                isAttached = true;
                transform.SetParent(poketPoint);
                transform.position = poketPoint.position;
                transform.rotation = poketPoint.rotation;
                transform.localPosition = Vector3.zero;
                transform.localRotation = Quaternion.identity;
                rigid.bodyType = RigidbodyType2D.Kinematic;
                rigid.linearVelocity = Vector2.zero;
                col.isTrigger = true;
                StartCoroutine(InvincibleForSeconds(1f)); // 1초간 무적
            }
        }
        // Land나 Platform에 닿았을 때 PoketPoint에서 분리
        else if (isAttached && !isInvincible && (collision.CompareTag("Land") || collision.CompareTag("Platform")))
        {
            DetachFromPoketPoint();
        }

        // Regenerate에 닿았을 때 (플레이어에 부착 중이면 무시)
        if (!isAttached && collision.gameObject.name == "Regenerate")
        {
            Transform resetPoint = collision.transform.Find("goldenballReset");
            if (resetPoint != null)
            {
                transform.position = resetPoint.position;
            }
            else
            {
                Debug.LogWarning("goldenballReset 오브젝트를 찾을 수 없습니다.");
            }
        }

        // GoldenBallPoint에 닿았을 때 (플레이어에 부착 중이면 무시)
        if (!isAttached && collision.gameObject.name == "GoldenBallPoint")
        {
            Transform savePoint = collision.transform.Find("saveGoldenBall");
            if (savePoint != null)
            {
                transform.position = savePoint.position;
            }
            else
            {
                Debug.LogWarning("saveGoldenBall 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    // PoketPoint에서 분리하는 함수
    private void DetachFromPoketPoint()
    {
        col.isTrigger = false;
        isAttached = false;
        transform.SetParent(null);
        rigid.bodyType = RigidbodyType2D.Dynamic;
    }

    // 무적 상태를 유지할 코루틴
    private IEnumerator InvincibleForSeconds(float seconds)
    {
        isInvincible = true;
        yield return new WaitForSeconds(seconds);
        isInvincible = false;
    }

}