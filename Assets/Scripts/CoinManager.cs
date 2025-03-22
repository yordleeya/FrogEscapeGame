using UnityEngine;

public class CoinManager : MonoBehaviour
{


    public float speed = 5f;
    private Rigidbody2D rigid;
    float currentTIme;
    public float createTime = 1f;
    public GameObject CoinFactory;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
     
    }

    // Update is called once per frame
    void Update()
    {
        currentTIme += Time.deltaTime;
        if (currentTIme > createTime)
        {
            GameObject Coin = Instantiate(CoinFactory);
            Coin.transform.position = transform.position;
            currentTIme = 0;
        }
    }
}
