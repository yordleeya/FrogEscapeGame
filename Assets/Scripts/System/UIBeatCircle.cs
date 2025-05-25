using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UIBeatCircle : MonoBehaviour
{
    [SerializeField] private float maxScale = 1.5f; // 퍼질 때 최대 크기
    [SerializeField] private float minScale = 0.5f; // 쪼그라들 때 최소 크기
    [SerializeField] private float duration = 0.2f; // 애니메이션 시간

    [SerializeField] RhythmManager rm;

    private void Awake()
    {
        transform.localScale = Vector3.one * maxScale;
    }

    private void Start()
    {
        // 테스트용: 1초 후 OnBeat() 호출
        Invoke("OnBeat", 1f);
    }

    public void OnBeat()
    {
        Debug.Log("OnBeat 호출됨");
        float interval = 60f / rm.Bpm;

        // 이전 Tween이 남아있으면 종료
        transform.DOKill();

        // scale을 1로 초기화 (다시 '생겨나는' 효과)
        transform.localScale = Vector3.one * maxScale;

        // 0으로 줄어드는 애니메이션
        transform
            .DOScale(minScale, interval)
            .SetEase(Ease.OutQuad);
    }
}