using UnityEngine;

public class Platform : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Tongue"))
        {
            Rigidbody2D tongueRigid = collision.GetComponent<Rigidbody2D>();

            ResetTongue(tongueRigid);
        }
    }

    public void ResetTongue(Rigidbody2D tongueRigid)
    {
        tongueRigid.linearVelocity = Vector2.zero; // Vector3 → Vector2 로 명확히
        tongueRigid.angularVelocity = 0;

        // 현재 GameObject에 IDynamicPlatform 인터페이스를 구현한 컴포넌트가 없을 때만 Kinematic 설정
        bool hasDynamicPlatform = GetComponent<IDynamicPlatform>() != null;

        if (!hasDynamicPlatform)
        {
            tongueRigid.bodyType = RigidbodyType2D.Kinematic;
        }
    }


}
