using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBeatCircle : MonoBehaviour
{
    [SerializeField] private float maxScale = 1.5f; // 퍼질 때 최대 크기
    [SerializeField] private float minScale = 0.5f; // 쪼그라들 때 최소 크기

    [SerializeField] RhythmManager rm;

    private Coroutine beatCoroutine;

    private void Awake()
    {
        transform.localScale = Vector3.one * minScale; // 최소 크기로 시작
    }

    private void Start()
    {
        if (beatCoroutine != null) StopCoroutine(beatCoroutine);
        beatCoroutine = StartCoroutine(BeatRoutine());
    }

    private System.Collections.IEnumerator BeatRoutine()
    {
        while (true)
        {
            OnBeat();
            float interval = 60f / rm.Bpm;
            yield return new WaitForSeconds(interval);
        }
    }

    public void OnBeat()
    {
        Debug.Log("OnBeat 호출됨");
        float interval = 60f / rm.Bpm;

        // 이전 Tween이 남아있으면 종료
        transform.DOKill();

        // scale을 minScale로 초기화 (다시 '생겨나는' 효과)
        transform.localScale = Vector3.one * minScale;

        // maxScale로 커지는 애니메이션
        transform
            .DOScale(maxScale, interval)
            .SetEase(Ease.OutQuad);
    }
}