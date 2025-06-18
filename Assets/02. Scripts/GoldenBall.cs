using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class GoldenBall : MonoBehaviour
{
    private bool isAttached = false;
    private Rigidbody2D rigid;
    private Collider2D col;
    private bool isInvincible = false; // 무적 상태
    private bool isRegenerateCooldown = false; // Regenerate 쿨타임 플래그

    public UnityEvent OnBallAttached;
    public UnityEvent OnBallDettached;

    public bool IsAttached => isAttached;

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
                OnBallAttached?.Invoke();

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
            OnBallDettached?.Invoke();

            DetachFromPoketPoint();
        }

        // Regenerate에 닿았을 때 (플레이어에 부착 중이면 무시)
        if (!isAttached && !isRegenerateCooldown && collision.gameObject.name.StartsWith("Regenerate"))
        {
            // Regenerate 이름에서 번호 추출 (예: Regenerate1 → 1)
            string regenName = collision.gameObject.name;
            string number = regenName.Replace("Regenerate", ""); // "1", "2" 등

            string resetName = "goldenballReset" + number; // goldenballReset1, goldenballReset2 등

            Transform resetPoint = collision.transform.Find(resetName);
            if (resetPoint != null)
            {
                transform.position = resetPoint.position;
                StartCoroutine(RegenerateCooldown(1f));
            }
            else
            {
                Debug.LogWarning($"{resetName} 오브젝트를 찾을 수 없습니다.");
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

    // Regenerate 쿨타임 코루틴
    private IEnumerator RegenerateCooldown(float seconds)
    {
        isRegenerateCooldown = true;
        yield return new WaitForSeconds(seconds);
        isRegenerateCooldown = false;
    }
}