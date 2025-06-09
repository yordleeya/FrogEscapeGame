using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIBeatCircle : MonoBehaviour
{
    [SerializeField] private float maxScale = 1.5f; // 퍼질 때 최대 크기
    [SerializeField] private float minScale = 0.5f; // 쪼그라들 때 최소 크기
    [SerializeField] private float syncOffset = 0f; // 초 단위, Inspector에서 조정
    [SerializeField] private float timingOffset = 0f; // 초 단위, Inspector에서 조정
    [SerializeField] private float tweenDuration = 1.0f; // Inspector에서 직접 조정

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
        // 첫 비트 전에 미리 한 번 애니메이션 시작
        OnBeat();
    }

    public void OnBeat()
    {
        if (tweenDuration < 0.01f) tweenDuration = 0.01f; // 음수 방지

        transform.DOKill();
        transform.localScale = Vector3.one * minScale;

        transform
            .DOScale(maxScale, tweenDuration)
            .SetEase(Ease.Linear);
    }
}