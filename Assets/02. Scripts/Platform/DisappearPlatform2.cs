using UnityEngine;

public class DisappearPlatform2 : MonoBehaviour
{
    public float appearDelay = 2f;
    public float interval = 2f;
    private float timer = 0f;
    private bool isVisible = false;
    private bool started = false;
    private Renderer rend;
    private Collider2D col;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider2D>();
        SetVisible(false);
        Invoke("AppearAndStartCycle", appearDelay);
    }

    void AppearAndStartCycle()
    {
        SetVisible(true);
        isVisible = true;
        timer = 0f;
        started = true;
    }

    void Update()
    {
        if (!started) return;
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            isVisible = !isVisible;
            SetVisible(isVisible);
            timer = 0f;
        }
    }

    void SetVisible(bool visible)
    {
        if (rend != null) rend.enabled = visible;
        if (col != null) col.enabled = visible;
    }
}