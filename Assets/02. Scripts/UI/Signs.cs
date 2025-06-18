using UnityEngine;

public class Signs : MonoBehaviour
{
    public GameObject SignsPanel; // Inspector에서 패널 오브젝트 연결

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (SignsPanel != null)
                SignsPanel.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (SignsPanel != null)
                SignsPanel.SetActive(false);
        }
    }
}
