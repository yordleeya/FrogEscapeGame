using UnityEngine;
using System.Collections;


public class GoalTrigger : MonoBehaviour
{
    public GameObject goalTextUI; // "도착했습니다!" UI 오브젝트
    public float displayTime = 2f;

    public GameObject recordPanel; // 기록 패널
    public GameObject timerPanel;  // 타이머 패널
    public GameObject gameUIPanel; // 게임 UI 패널(필요시)

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // 기존 UI 비활성화
            if (goalTextUI != null) goalTextUI.SetActive(false);
            if (timerPanel != null) timerPanel.SetActive(false);
            if (gameUIPanel != null) gameUIPanel.SetActive(false);

            // 기록 패널 활성화
            if (recordPanel != null) recordPanel.SetActive(true);

            // 게임 멈춤
            Time.timeScale = 0f;
        }
    }

    // 기존 코루틴은 사용하지 않으므로 주석 처리
    /*
    IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        goalTextUI.SetActive(false);
    }
    */
}