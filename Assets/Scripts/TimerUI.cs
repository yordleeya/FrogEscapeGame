using UnityEngine;
using TMPro; // TextMeshProUGUI 사용을 위해 추가

public class TimerUI : MonoBehaviour
{
    public TextMeshProUGUI timerText; // 인스펙터에서 할당
    private float elapsedTime = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        elapsedTime += Time.deltaTime;

        int hours = (int)(elapsedTime / 3600);
        int minutes = (int)((elapsedTime % 3600) / 60);
        int seconds = (int)(elapsedTime % 60);

        timerText.text = string.Format("{0:D2}:{1:D2}:{2:D2}", hours, minutes, seconds);
    }
}
