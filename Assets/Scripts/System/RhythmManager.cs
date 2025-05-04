using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using System.Threading;

public class RhythmManager : MonoBehaviour
{
    public UnityEvent OnBeatEnter;
    public UnityEvent OnBeatExit;

    [SerializeField]
    private float bpm = 120;
    public float Bpm => bpm;

    [SerializeField]
    private float offset;

    private static bool isOnBeat = false;
    public static bool IsOnBeat => isOnBeat;

    private bool isRunning = false;
    private CancellationTokenSource cts;

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
        isRunning = true;
        StartRhythmLoop(cts.Token).Forget();
    }

    private void OnDisable()
    {
        isRunning = false;
        cts?.Cancel();
        cts?.Dispose();
        cts = null;
    }

    private async UniTaskVoid StartRhythmLoop(CancellationToken token)
    {
        float interval = 60f / bpm;

        while (isRunning && !token.IsCancellationRequested)
        {
            isOnBeat = true;
            OnBeatEnter?.Invoke();

            await OffsetDelay(token); // 반드시 실행되게 await로 연결

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
            // 무시: 해제 중 취소될 수 있음
        }
    }
}
