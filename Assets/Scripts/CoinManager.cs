using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    int coinNum;

    public float speed = 5f;
    private Rigidbody2D rigid;

    [SerializeField]
    float createTime = 1f;

    [SerializeField]
    GameObject coin;

    [SerializeField]
    GameObject[] coinArray;

    public GameObject[] CoinArray { get => coinArray; }

    Coroutine createCoinCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject nowCoin;
        coinArray = new GameObject[coinNum];

        for (int i = 0; i < coinNum; i++)
        {
            nowCoin = Instantiate(coin);
            nowCoin.transform.parent = transform;
            nowCoin.SetActive(false);

            coinArray[i] = nowCoin;
        }

        createCoinCoroutine = StartCoroutine(CreateCoin());
    }

    private void OnDisable()
    {
        StopCoroutine(createCoinCoroutine);
    }

    IEnumerator CreateCoin()
    {
        yield return new WaitForSeconds(createTime);

        foreach (GameObject c in coinArray)
        {
            if (!c.activeSelf)
            {
                c.SetActive(true);
                break;
            }
        }

        createCoinCoroutine = StartCoroutine(CreateCoin());

    }
}
