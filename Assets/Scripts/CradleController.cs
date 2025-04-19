using UnityEngine;
using System.Collections;
using DG.Tweening;

public class CradleController : MonoBehaviour
{
    [Header("그릇 이동 관련")]
    public Transform downTargetPosition;
    public Transform launchTargetPosition;
    public Transform initialPosition;
    public float descentTime = 2.5f;
    public float launchTime = 0.3f;
    public float resetTime = 2f;

    [Header("플레이어 발사 설정")]
    public float launchForceX = 30f;
    public float launchForceY = 5f;
    public float dragForce = 0f; // 공기저항 제거

    [Header("Handle 제어")]
    public HandleTrigger handleTrigger;

    private bool canLoadPlayer = false;
    private bool launched = false;
    private GameObject attachedPlayer = null;
    private Vector3 playerLocalScale;
    private bool isResetting = false;

    // 위치 저장용 변수들ㅌ
    private Vector3 savedDownPosition;
    private Vector3 savedLaunchPosition;
    private Vector3 savedInitialPosition;

    private void Start()
    {
        // 시작할 때 모든 위치 저장
        if (downTargetPosition != null)
        {
            savedDownPosition = downTargetPosition.position;
        }
        else
        {
            Debug.LogError("Down Target Position이 설정되지 않았습니다!");
        }

        if (launchTargetPosition != null)
        {
            savedLaunchPosition = launchTargetPosition.position;
        }
        else
        {
            Debug.LogError("Launch Target Position이 설정되지 않았습니다!");
        }

        // initialPosition이 없을 경우 현재 위치를 저장
        if (initialPosition == null)
        {
            savedInitialPosition = transform.position;
        }
        else
        {
            savedInitialPosition = initialPosition.position;
        }

        if (handleTrigger == null)
        {
            Debug.LogWarning("HandleTrigger가 설정되지 않았습니다. Inspector에서 설정해주세요.");
        }

        // 시작할 때 -90도로 회전
        transform.rotation = Quaternion.Euler(0, 0, -90f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Player 탑승 로직
        if (other.CompareTag("Player") && canLoadPlayer && !launched && attachedPlayer == null && !isResetting)
        {
            AttachPlayer(other.gameObject);
        }
    }

    private void AttachPlayer(GameObject player)
    {
        if (player == null) return;

        attachedPlayer = player;
        
        playerLocalScale = player.transform.localScale;
        
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            playerRb.simulated = false;
        }

        player.transform.SetParent(transform);
        player.transform.localPosition = Vector3.zero;
        player.transform.localScale = playerLocalScale;
        
        // 플레이어 부착 후 바로 LaunchSequence 호출하지 않음
        StartCoroutine(StartLaunchAfterDelay());
    }

    private IEnumerator StartLaunchAfterDelay()
    {
        // 잠시 대기하여 플레이어가 Cradle에 안정적으로 부착되도록 함
        yield return new WaitForSeconds(0.1f);
        LaunchSequence();
    }

    private void LaunchSequence()
    {
        if (attachedPlayer == null) return;

        Vector3 targetPos = savedLaunchPosition;

        // 투석기 움직임 시퀀스
        Sequence launchSequence = DOTween.Sequence();

        // 이동과 회전
        launchSequence.Append(transform.DOMove(targetPos, launchTime)
            .SetEase(Ease.InQuad)
            .OnUpdate(() => {
                // 목표 지점까지의 거리 계산
                float distanceToTarget = Vector3.Distance(transform.position, targetPos);
                
                // 목표 지점에 매우 근접했을 때 (약 5% 거리 남았을 때) 플레이어 발사
                if (distanceToTarget < 0.2f && attachedPlayer != null)
                {
                    LaunchPlayer();
                }
            }));
            
        launchSequence.Join(transform.DORotate(new Vector3(0, 0, -90f), launchTime)
            .SetEase(Ease.InQuad));

        launched = true;
    }

    private void LaunchPlayer()
    {
        if (attachedPlayer == null) return;

        try
        {
            // 플레이어의 현재 월드 위치와 스케일 저장
            Vector3 worldPos = attachedPlayer.transform.position;
            Vector3 currentScale = attachedPlayer.transform.localScale;
            
            // 플레이어를 완전히 분리
            attachedPlayer.transform.SetParent(null);
            attachedPlayer.transform.position = worldPos;
            attachedPlayer.transform.localScale = currentScale;
            attachedPlayer.transform.rotation = Quaternion.Euler(0, 0, 30f);
            
            // Rigidbody2D 설정
            Rigidbody2D playerRb = attachedPlayer.GetComponent<Rigidbody2D>();
            if (playerRb != null)
            {
                // 물리 시뮬레이션 활성화
                playerRb.simulated = true;
                playerRb.bodyType = RigidbodyType2D.Dynamic;
                playerRb.interpolation = RigidbodyInterpolation2D.Interpolate;
                playerRb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                
                // 초기 상태 설정
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;
                playerRb.gravityScale = 1f;
                playerRb.mass = 1f;
                playerRb.linearDamping = 0f; // 공기 저항 제거
                playerRb.angularDamping = 0.05f; // 회전 저항 감소
                
                // 발사!
                Vector2 launchForce = new Vector2(launchForceX, launchForceY);
                playerRb.AddForce(launchForce, ForceMode2D.Impulse);
                
                Debug.Log($"💨 플레이어 발사! 힘: ({launchForceX}, {launchForceY}), 중력: {playerRb.gravityScale}, 감속: {playerRb.linearDamping}");
            }

            // 플레이어 참조 해제
            attachedPlayer = null;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"플레이어 발사 중 오류 발생: {e.Message}");
            if (attachedPlayer != null)
            {
                attachedPlayer = null;
            }
        }
        finally
        {
            // Cradle만 리셋
            StartCoroutine(ResetCradle());
        }
    }

    private IEnumerator ResetCradle()
    {
        isResetting = true;
        
        // Handle 먼저 리셋
        if (handleTrigger != null)
        {
            handleTrigger.ResetHandle();
            Debug.Log("Handle 리셋 시작!");
        }
        
        // 약간의 대기 시간 추가
        yield return new WaitForSeconds(0.1f);
        
        Sequence resetSequence = DOTween.Sequence();
        
        resetSequence.Append(transform.DOMove(savedInitialPosition, resetTime)
            .SetEase(Ease.InOutSine));
            
        resetSequence.OnComplete(() =>
        {
            canLoadPlayer = false;
            launched = false;
            isResetting = false;
            Debug.Log("✅ Cradle 리셋 완료!");
        });

        yield return resetSequence.WaitForCompletion();
        
        // 리셋이 완료된 후 -90도로 회전
        transform.rotation = Quaternion.Euler(0, 0, -90f);
    }

    public void StartDescent()
    {
        if (isResetting) return;
        
        Debug.Log("🔥 Cradle StartDescent() 호출됨");
        
        // 저장된 위치로 이동하면서 0도로 회전
        Sequence descentSequence = DOTween.Sequence();
        
        // 위치 이동과 회전을 동시에 실행
        descentSequence.Join(transform.DOMove(savedDownPosition, descentTime)
            .SetEase(Ease.InOutSine));
            
        descentSequence.Join(transform.DORotate(Vector3.zero, descentTime)
            .SetEase(Ease.InOutSine));
            
        descentSequence.OnComplete(() =>
        {
            Debug.Log("✅ Cradle 내려오기 완료");
            canLoadPlayer = true;
        });
    }

    // 위치가 변경되었을 때 수동으로 호출할 수 있는 업데이트 메서드
    public void UpdatePositions()
    {
        if (downTargetPosition != null)
        {
            savedDownPosition = downTargetPosition.position;
            Debug.Log($"Down Position 업데이트: {savedDownPosition}");
        }
        
        if (launchTargetPosition != null)
        {
            savedLaunchPosition = launchTargetPosition.position;
            Debug.Log($"Launch Position 업데이트: {savedLaunchPosition}");
        }
        
        if (initialPosition != null)
        {
            savedInitialPosition = initialPosition.position;
            Debug.Log($"Initial Position 업데이트: {savedInitialPosition}");
        }
    }

    private void OnValidate()
    {
        // Inspector에서 값이 변경될 때 위치 업데이트
        if (Application.isPlaying) return; // 플레이 모드에서는 실행하지 않음
        
        UpdatePositions();
    }

    // Handle의 Tongue이 닿았을 때만 호출되는 메서드
    public void OnTongueContact()
    {
        // 이미 리셋 중이거나, 발사된 상태거나, 플레이어가 탑승 중이면 무시
        if (isResetting || launched || attachedPlayer != null) return;

        StartDescent();
    }
}