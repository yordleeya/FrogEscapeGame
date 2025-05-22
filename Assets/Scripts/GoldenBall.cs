using UnityEngine;
using System.Collections;

public class GoldenBall : MonoBehaviour
{
    private bool isAttached = false;
    private Rigidbody2D rigid;
    private Collider2D col;
    private bool isInvincible = false; // ���� ���� �÷���

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Player�� �ε����� PoketPoint�� ��� �����̵� �� ����
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
                StartCoroutine(InvincibleForSeconds(1f)); // 1�ʰ� ����
            }
        }
        // Land�� Platform�� �ε����� PoketPoint���� ���� ����(�и�) �� �ٴ����� ������
        else if (isAttached && !isInvincible && (collision.CompareTag("Land") || collision.CompareTag("Platform")))
        {
            DetachFromPoketPoint();
        }


        if (collision.gameObject.name == "Regenerate")
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
    }

    // PoketPoint���� �и��Ǿ� �ٴ����� �������� ����� �Լ�
    private void DetachFromPoketPoint()
    {
        col.isTrigger = false;
        isAttached = false;
        transform.SetParent(null);
        rigid.bodyType = RigidbodyType2D.Dynamic;
    }

    // ���� �ð� ���� Land/Platform�� ��Ƶ� �������� �ʰ� �ϴ� ���� �ڷ�ƾ
    private IEnumerator InvincibleForSeconds(float seconds)
    {
        isInvincible = true;
        yield return new WaitForSeconds(seconds);
        isInvincible = false;
    }

}