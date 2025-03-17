using UnityEngine;
using UnityEngine.InputSystem;

public class RopeAction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    //1. 레이캐스트
    //2. 라인랜더리
    //3. 스프링조인트

    public Transform player;
    Camera cam;
    RaycastHit hit;
    public LayerMask GrapplingObj;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RopeShoot()
    {
        if (Physics.Raycast(player.transform.position, player.transform.forward, out hit, 100f, GrapplingObj))
        {
            print("장애물 검출!");
        }
    }
    public void OnRope(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            RopeShoot();
        }
    }
}
