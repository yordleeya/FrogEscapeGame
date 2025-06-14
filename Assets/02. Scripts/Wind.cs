using UnityEngine;

public class Wind : MonoBehaviour
{
    public Vector2 windDirection = Vector2.right; // 바람 방향
    public float baseWindForce = 10f; // 기본 바람 세기
    public float windForceVariation = 5f; // 바람 세기 변화폭
    public float windChangeSpeed = 1f; // 바람 변화 속도

    public AudioClip windSound; // 바람 소리 클립
    private AudioSource audioSource;

    public Vector2 windRange = new Vector2(2f, 10f); // 바람 효과 범위 (X, Y)
    public Vector2 soundRange = new Vector2(2f, 10f); // 소리 효과 범위 (X, Y)

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // AudioSource 세팅
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.clip = windSound;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            Vector2 windCenter = transform.position;
            Vector2 playerPos = player.transform.position;

            // 바람 범위 체크
            if (IsInRange(windCenter, playerPos, windRange))
            {
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    float dynamicWindForce = baseWindForce + Mathf.Sin(Time.time * windChangeSpeed) * windForceVariation;
                    rb.AddForce(windDirection.normalized * dynamicWindForce, ForceMode2D.Force);
                }
            }

            // 소리 범위 체크
            if (IsInRange(windCenter, playerPos, soundRange))
            {
                if (!audioSource.isPlaying && windSound != null)
                    audioSource.Play();
            }
            else
            {
                if (audioSource.isPlaying)
                    audioSource.Pause();
            }
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Pause();
        }
    }

    // X, Y 범위 내에 있는지 체크 (X: 양쪽, Y: 위쪽만)
    bool IsInRange(Vector2 center, Vector2 target, Vector2 range)
    {
        Vector2 diff = target - center;
        return Mathf.Abs(diff.x) <= range.x * 0.5f && diff.y >= 0 && diff.y <= range.y;
    }

    // Scene 뷰에서 감지 범위 시각화
    void OnDrawGizmosSelected()
    {
        // 바람 감지 범위(파란색)
        Gizmos.color = new Color(0f, 0.5f, 1f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(windRange.x, windRange.y, 0.1f));

        // 소리 감지 범위(초록색)
        Gizmos.color = new Color(0f, 1f, 0.2f, 0.3f);
        Gizmos.DrawWireCube(transform.position, new Vector3(soundRange.x, soundRange.y, 0.1f));
    }
}
