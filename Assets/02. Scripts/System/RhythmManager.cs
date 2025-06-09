using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;
using System.Collections;

public class RhythmManager : MonoBehaviour
{
    public UnityEvent OnGameStart;
    public UnityEvent OnBeatEnter;
    public UnityEvent OnBeatExit;

    [SerializeField]
    private float bpm = 120;
    public float Bpm => bpm;

    [SerializeField]
    private float offset;

    private static bool isOnBeat = false;
    public static bool IsOnBeat => isOnBeat;

    public float Offset { get => offset;}

    private CancellationTokenSource cts;

    [SerializeField, Tooltip("UIBeatCircle의 tweenDuration과 동일하게 맞추세요!")]
    private float beatVisualDelay = 1.0f; // Inspector에서 tweenDuration과 동일하게 설정

    [SerializeField, Tooltip("비트 입력 허용 범위(초)")]
    private float beatJudgeWindow = 0.2f; // Inspector에서 조정

    void Start()
    {
        StartRhythm();
    }
    
    public void StartRhythm()
    {
        if (cts != null)
        {
            cts.Cancel();
            cts.Dispose();
        }
        cts = new CancellationTokenSource();
        OnGameStart?.Invoke();
        StartRhythmLoop(cts.Token).Forget();
    }

    private void OnEnable()
    {
        // 자동 시작 제거 (StartRhythm을 외부에서 명시적으로 호출)
    }

    private void OnDisable()
    {
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async UniTaskVoid StartRhythmLoop(CancellationToken token)
    {
        float interval = 60f / bpm;

        while (!token.IsCancellationRequested)
        {
            isOnBeat = false;
            OnBeatEnter?.Invoke();

            // 애니메이션 끝까지 대기
            await UniTask.Delay(TimeSpan.FromSeconds(beatVisualDelay), ignoreTimeScale: true, cancellationToken: token);

            // 판정 윈도우만큼 isOnBeat을 true로 유지
            isOnBeat = true;
            await UniTask.Delay(TimeSpan.FromSeconds(beatJudgeWindow), ignoreTimeScale: true, cancellationToken: token);

            isOnBeat = false;

            // 남은 시간 대기
            float remain = Mathf.Max(0.01f, interval - beatVisualDelay - beatJudgeWindow);
            await UniTask.Delay(TimeSpan.FromSeconds(remain), ignoreTimeScale: true, cancellationToken: token);

            OnBeatExit?.Invoke();
        }
    }

    private async UniTask OffsetDelay(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(offset), ignoreTimeScale: true, cancellationToken: token);
            isOnBeat = false;
            OnBeatExit?.Invoke();
        }
        catch (OperationCanceledException)
        {
            // ����
        }
    }

}
