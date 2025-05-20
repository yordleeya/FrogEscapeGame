using UnityEngine;

public class MovePlatform : MonoBehaviour, IFunctionalPlatform
{
    [SerializeField] private Transform[] pointArray;
    [SerializeField] private float moveSpeed = 0.1f;
    [SerializeField] Vector2[] pointPositionArray;
   
    private bool isMovingToFirst = true;  // 첫 번째 포인트로 이동 중인지 확인하는 변수

    void IFunctionalPlatform.OnDettaced(Rigidbody2D rigid)
    {
        throw new System.NotImplementedException();
    }

    void IFunctionalPlatform.OnAttached(Rigidbody2D rigid, RigidbodyType2D bodyType)
    {
        throw new System.NotImplementedException();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pointArray = new Transform[transform.childCount+1];
        pointPositionArray = new Vector2[pointArray.Length];

        for (int i= 0;i<pointArray.Length;i++)
        {
            pointArray[i] = transform.GetChild(0).GetChild(i);
        }

        int index = 0;

        foreach (var point in pointArray)
        {
            pointPositionArray[index] = point.position;
            point.gameObject.SetActive(false);
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMovingToFirst)
        {
            if (transform.position != (Vector3)pointPositionArray[0])
            {
                transform.position = Vector2.MoveTowards(transform.position, pointPositionArray[0], moveSpeed * Time.deltaTime);
            }
            else
            {
                isMovingToFirst = false;
            }
        }
        else
        {
            if (transform.position != (Vector3)pointPositionArray[1])
            {
                transform.position = Vector2.MoveTowards(transform.position, pointPositionArray[1], moveSpeed * Time.deltaTime);
            }
            else
            {
                isMovingToFirst = true;
            }
        }
    }
}
