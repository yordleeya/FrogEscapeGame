using UnityEngine;

public class DisappearPlatform : MonoBehaviour
{
    public float interval = 2f;
    private float timer = 0f;
    private bool isVisible = true;
    private Renderer rend;
    private Collider2D col;

    void Start()
    {
        rend = GetComponent<Renderer>();
        col = GetComponent<Collider2D>();
        SetVisible(true);
    }

    void Update()
    {
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