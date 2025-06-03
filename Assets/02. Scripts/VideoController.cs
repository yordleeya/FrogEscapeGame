using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    VideoPlayer video;

    [SerializeField]
    PLAY_button startButton;

    private void Awake()
    {
        video = GetComponent<VideoPlayer>();    
    }

    private void Start()
    {
        video.loopPointReached += startButton.Enable;
    }

}
