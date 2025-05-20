using UnityEngine;
using System.Collections;


public class GoalTrigger : MonoBehaviour
{
    public GameObject goalTextUI; // "도착했습니다!" UI 오브젝트
    public float displayTime = 2f;

    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            goalTextUI.SetActive(true);
            StartCoroutine(HideTextAfterDelay());
        }
    }
      IEnumerator HideTextAfterDelay()
    {
        yield return new WaitForSeconds(displayTime);
        goalTextUI.SetActive(false);
    }
}