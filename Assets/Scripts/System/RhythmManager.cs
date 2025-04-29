using System;
using UnityEngine;
using UnityEngine.Events;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;

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

    private void OnEnable()
    {
        isRunning = true;
        StartRhythmLoop().Forget();  // UniTask를 fire-and-forget 형태로 실행
    }

    private void OnDisable()
    {
        isRunning = false;
    }

    private async UniTaskVoid StartRhythmLoop()
    {
        float interval = 60f / bpm;

        while (isRunning)
        {
            isOnBeat = true;
            OnBeatEnter?.Invoke();

            _ = OffsetDelay(); // offset 이후에 beat 종료 알림

            await UniTask.Delay(TimeSpan.FromSeconds(interval), ignoreTimeScale: true);
        }
    }

    private async UniTaskVoid OffsetDelay()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(offset), ignoreTimeScale: true);
        isOnBeat = false;
        OnBeatExit?.Invoke();
    }
}
