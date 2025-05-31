using UnityEngine;

public class MovePlatform1 : MonoBehaviour
{
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public Transform point4;
    public float speed = 2f;

    private Vector3 point1Pos;
    private Vector3 point2Pos;
    private Vector3 point3Pos;
    private Vector3 point4Pos;
    private Vector3[] points;
    private int targetIndex;
    private Vector3 target;

    void Start()
    {
        // point1, point2, point3, point4의 월드 좌표를 저장
        point1Pos = point1.position;
        point2Pos = point2.position;
        point3Pos = point3.position;
        point4Pos = point4.position;
        points = new Vector3[] { point1Pos, point2Pos, point3Pos, point4Pos };
        targetIndex = 1; // 처음에는 point2로 이동
        target = points[targetIndex];
    }

    void Update()
    {
        // point1~4의 위치를 원래 위치로 고정
        point1.position = point1Pos;
        point2.position = point2Pos;
        point3.position = point3Pos;
        point4.position = point4Pos;

        // 저장된 월드 좌표로 이동
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            targetIndex = (targetIndex + 1) % points.Length;
            target = points[targetIndex];
        }
    }
}