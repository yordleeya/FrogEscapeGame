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

    private CancellationTokenSource cts;

    private void OnEnable()
    {
        cts = new CancellationTokenSource();
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

            await OffsetDelay(token);

            await UniTask.Delay(TimeSpan.FromSeconds(interval), ignoreTimeScale: true, cancellationToken: token);
        }
    }

    private async UniTask OffsetDelay(CancellationToken token)
    {
        try
        {
            await UniTask.Delay(TimeSpan.FromSeconds(offset), ignoreTimeScale: true, cancellationToken: token);
            isOnBeat = false;

            try
            {
                OnBeatExit?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"OnBeatExit.Invoke() 예외 발생: {ex}");
            }
        }
        catch (OperationCanceledException)
        {
            // 취소 무시
        }
        catch (Exception ex)
        {
            Debug.LogError($"OffsetDelay 예외: {ex}");
        }
    }
}
