using UnityEngine;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //1. 레이캐스트
    //2. 라인랜더리
    //3. 스프링조인트

    RaycastHit hit;

    [SerializeField]
    LayerMask GrapplingObj;

    void RopeShoot(Vector2 mousePosition)
    {
        if (Physics.Raycast(transform.position, mousePosition, out hit, 100f, GrapplingObj))
        {
            Debug.Log("장애물 검출!");
        }
    }
    public void OnRope(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            RopeShoot(mousePosition);
        }
    }
}
