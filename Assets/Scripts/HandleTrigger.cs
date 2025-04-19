using UnityEngine;
using System.Collections;
using DG.Tweening;

public class HandleTrigger : MonoBehaviour
{
    [Tooltip("혀가 붙어 있어야 하는 시간 (초)")]
    public float requiredAttachTime = 2f;

    [Tooltip("CradleController를 인스펙터에서 드래그해서 연결하세요")]
    public CradleController cradle;

    private float attachTimer = 0f;
    private bool isTriggered = false;

    private void Start()
    {
        if (cradle == null)
        {
            Debug.LogError("Cradle이 설정되지 않았습니다. Inspector에서 설정해주세요.");
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        // Player 태그는 무시
        if (other.CompareTag("Player")) return;
        
        // Tongue 태그만 처리
        if (other.CompareTag("Tongue") && !isTriggered)
        {
            attachTimer += Time.deltaTime;
            Debug.Log($"혀가 붙어있는 중: {attachTimer:F2}초 / {requiredAttachTime}초");

            if (attachTimer >= requiredAttachTime)
            {
                isTriggered = true;
                Debug.Log("조건 만족! Cradle 내려간다!");

                if (cradle == null)
                {
                    Debug.LogError("Cradle이 연결되지 않았습니다!");
                }
                else
                {
                    cradle.StartDescent();
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Tongue") && !isTriggered)
        {
            Debug.Log("혀가 빠졌습니다. 타이머 초기화!");
            attachTimer = 0f;
        }
    }

    public bool IsTriggered()
    {
        return isTriggered;
    }

    public void ResetHandle()
    {
        attachTimer = 0f;
        isTriggered = false;
        Debug.Log("Handle 상태가 초기화되었습니다!");
    }
}