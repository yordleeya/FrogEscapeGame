using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        if (startButton != null)
        {
            video.loopPointReached += startButton.Enable;
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
