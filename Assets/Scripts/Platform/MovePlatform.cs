using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public float speed = 2f;

    private Vector3 point1Pos;
    private Vector3 point2Pos;
    private Vector3 target;

    void Start()
    {
        // point1, point2의 월드 좌표를 저장
        point1Pos = point1.position;
        point2Pos = point2.position;
        target = point2Pos;
    }

    void Update()
    {
        // point1, point2의 위치를 원래 위치로 고정
        point1.position = point1Pos;
        point2.position = point2Pos;

        // 저장된 월드 좌표로 이동
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            target = (target == point1Pos) ? point2Pos : point1Pos;
        }
    }
}