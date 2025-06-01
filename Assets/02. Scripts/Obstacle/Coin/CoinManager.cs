using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    [SerializeField]
    int coinNum;

    [SerializeField]
    float createTime = 1f;

    [FoldoutGroup("Coin data")]
    [SerializeField]
    float coinDisableTime;

    [FoldoutGroup("Coin data")]
    [SerializeField]
    float coinFallSpeed;

    [FoldoutGroup("Coin data")]
    [SerializeField]
    GameObject coin;

    [FoldoutGroup("Coin data")]
    [SerializeField]
    GameObject[] coinArray;

    [FoldoutGroup("Coin data")]
    public GameObject[] CoinArray { get => coinArray; }

    Coroutine createCoinCoroutine;

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
        if (createCoinCoroutine != null)
        {
            StopCoroutine(createCoinCoroutine);
            createCoinCoroutine = null;
        }
    }

    Coin coinScript;

    IEnumerator CreateCoin()
    {
        yield return new WaitForSeconds(createTime);

        foreach (GameObject c in coinArray)
        {
            if (!c.activeSelf)
            {
                coinScript = c.GetComponent<Coin>();
                c.SetActive(true);
                c.transform.position = transform.position;
                coinScript.Init(coinDisableTime, coinFallSpeed);
                break;
            }
        }

        createCoinCoroutine = StartCoroutine(CreateCoin());

    }
}
