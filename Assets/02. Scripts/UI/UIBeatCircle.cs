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

    private void OnEnable()
    {
        if (rm != null)
            rm.OnBeatEnter.AddListener(OnBeat);
    }

    private void OnDisable()
    {
        if (rm != null)
            rm.OnBeatEnter.RemoveListener(OnBeat);
    }

    private void Start()
    {
        // RhythmManager의 이벤트를 통해 OnBeat가 호출되므로 별도 처리 불필요
    }

    public void OnBeat()
    {
        float interval = 60f / rm.Bpm;
        float tweenDuration = interval * 0.9f; // interval보다 약간 짧게

        transform.DOKill();

        // scale을 minScale로 초기화 (다시 '생겨나는' 효과)
        transform.localScale = Vector3.one * minScale;

        // maxScale로 커지는 애니메이션
        transform
            .DOScale(maxScale, tweenDuration)
            .SetEase(Ease.OutQuad);
    }
}