using UnityEngine;

public class SlippingPlatform : MonoBehaviour
{
    [SerializeField] private float slipFactor = 0.1f; // �̲������� ���� (�÷��̾� �ӵ��� ������)
    [SerializeField] private float maxSlipDistance = 2f; // �ִ� �̲������� �Ÿ�

    private SpringJoint2D playerSpringJoint;
    private Rigidbody2D playerRigidbody;
    private Vector2 initialLocalAnchor; // ó�� ������ ���� ��ġ
    private Vector2 currentLocalAnchor; // ���� ������ ���� ��ġ
    private bool isTongueAttached = false;

    // RopeAction���� ���� ������ �� ȣ��˴ϴ�.
    public void AttachTongue(SpringJoint2D joint, Rigidbody2D playerRb)
    {
        playerSpringJoint = joint;
        playerRigidbody = playerRb;
        // connectedAnchor�� Rigidbody�� ���� ��ǥ �����Դϴ�.
        initialLocalAnchor = joint.connectedAnchor;
        currentLocalAnchor = initialLocalAnchor;
        isTongueAttached = true;
    }

    // RopeAction���� ���� �и��� �� ȣ��˴ϴ�.
    public void DetachTongue()
    {
        isTongueAttached = false;
        playerSpringJoint = null;
        playerRigidbody = null;
    }

    private void FixedUpdate()
    {
        if (isTongueAttached && playerSpringJoint != null && playerRigidbody != null)
        {
            // �÷��̾��� ���� �ӵ��� �����ɴϴ�.
            float horizontalVelocity = playerRigidbody.linearVelocity.x;

            // �̲����� ������ ��� (���� X�� ��������)
            float slipAmount = horizontalVelocity * slipFactor * Time.fixedDeltaTime;
            Vector2 slipOffset = new Vector2(slipAmount, 0);

            // ��ǥ ���� ��Ŀ ��ġ ���
            Vector2 targetLocalAnchor = currentLocalAnchor + slipOffset;

            // �ʱ� ��ġ�κ����� �Ÿ� ����
            float slipDistance = Vector2.Distance(targetLocalAnchor, initialLocalAnchor);
            if (slipDistance > maxSlipDistance)
            {
                // �ִ� �Ÿ��� ���� �ʵ��� ���� ���͸� �����մϴ�.
                Vector2 directionFromInitial = (targetLocalAnchor - initialLocalAnchor).normalized;
                targetLocalAnchor = initialLocalAnchor + directionFromInitial * maxSlipDistance;
            }

            // ���� ���� ��Ŀ ������Ʈ �� ������ ����Ʈ�� ����
            currentLocalAnchor = targetLocalAnchor;
            playerSpringJoint.connectedAnchor = currentLocalAnchor;
        }
    }
}