using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    int coinNum;

    [SerializeField]
    float createTime = 1f;

    [SerializeField]
    GameObject coin;

    [SerializeField]
    GameObject[] coinArray;

    public GameObject[] CoinArray { get => coinArray; }

    Coroutine createCoinCoroutine;

    [SerializeField]
    Vector2 spawnPoint;

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
                c.transform.position = spawnPoint;
                c.SetActive(true);
                break;
            }
        }

        createCoinCoroutine = StartCoroutine(CreateCoin());

    }
}
