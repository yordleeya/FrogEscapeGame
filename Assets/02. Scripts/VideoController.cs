using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

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

    private void PlayScene(VideoPlayer source)
    {
        SceneManager.LoadScene("PlayScene");
    }

}
