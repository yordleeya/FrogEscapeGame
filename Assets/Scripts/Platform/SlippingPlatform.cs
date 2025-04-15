// c:\Users\lwjin\OneDrive\바탕 화면\capstone\Assets\Scripts\Platform\SlippingPlatform.cs
using UnityEngine;

public class SlippingPlatform : MonoBehaviour
{
    [Tooltip("혀가 이 플랫폼에 붙었을 때 아래로 미끄러지는 속도")]
    public float slipSpeed = 0.5f;

    [Tooltip("혀가 최대로 미끄러질 수 있는 거리 (이 거리 도달 시 로프 해제)")]
    public float maxSlipDistance = 0.1f;

    // 특별한 로직은 필요 없음. RopeAction에서 이 컴포넌트와 값들을 참조함.
}