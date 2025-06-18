using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;
using UnityEngine.InputSystem;

public class VideoController : MonoBehaviour
{
    VideoPlayer video;

    [SerializeField]
    PLAY_button button;

    private void Awake()
    {
        video = GetComponent<VideoPlayer>();    
    }

    private void Start()
    {
        if (button != null)
        {
            video.loopPointReached += button.Enable;
        }
        else
        {
            video.loopPointReached += PlayScene;
        }
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (button != null)
                button.Enable(video);
            else
                PlayScene(video);
        }
    }

    private void PlayScene(VideoPlayer source)
    {
        SceneManager.LoadScene("PlayScene");
    }
}
