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
            isOnBeat = true;

            try
            {
                OnBeatEnter?.Invoke();
            }
            catch (Exception ex)
            {
                Debug.LogError($"OnBeatEnter.Invoke() ���� �߻�: {ex}");
            }

            // OffsetDelay�� ���������� ���� (isOnBeat�� offset �Ŀ� false�� �����)
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
            // ����
        }
    }

}
