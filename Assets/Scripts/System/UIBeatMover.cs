using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIBeatMover : MonoBehaviour
{
    private Vector3 startPosition;
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float moveDuration = 0.2f;

    private RectTransform rectTransform;

    [SerializeField]
    RhythmManager rm;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.anchoredPosition = startPosition;
    }

    private void Start()
    {
        startPosition =  new Vector3(0, 150, 0);
    }

    private void OnDisable()
    {
        rectTransform.anchoredPosition = startPosition;
    }

    public void OnBeat()
    {
        float interval = 60f / rm.Bpm;
        rectTransform.anchoredPosition = startPosition;

        rectTransform
            .DOAnchorPos(targetPosition, interval);
    }
}
