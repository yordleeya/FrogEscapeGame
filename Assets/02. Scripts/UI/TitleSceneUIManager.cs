using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUIManager : MonoBehaviour
{
    public AudioSource bgmSource; // Inspector에서 오디오 소스 연결

    private void Start()
    {
        Invoke("PlayBGM", 8f); // 7초 뒤에 음악 재생
    }

    private void PlayBGM()
    {
        if (bgmSource != null && !bgmSource.isPlaying)
            bgmSource.Play();
    }

    public void StartButton()
    {
        if (bgmSource != null && bgmSource.isPlaying)
            bgmSource.Stop(); // 음악 정지
        SceneManager.LoadScene("Prologue");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
