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

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        OnGameStart?.Invoke();
        StartRhythmLoop(cts.Token).Forget();
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
            isOnBeat = true;

            try
            {
                OnBeatEnter?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"OnBeatEnter.Invoke() 예외 발생: {ex}");
            }

            // OffsetDelay는 병렬적으로 실행 (isOnBeat를 offset 후에 false로 만들기)
            _ = OffsetDelay(token); // fire-and-forget

            await UniTask.Delay(TimeSpan.FromSeconds(interval), ignoreTimeScale: true, cancellationToken: token);
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
            // 무시
        }
    }

}
