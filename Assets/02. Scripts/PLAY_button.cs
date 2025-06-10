using UnityEngine;
using UnityEngine.SceneManagement;

public class PLAY_button : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void StartButton()
    {
        SceneManager.LoadScene("Prologue");
    }

    public void Enable(UnityEngine.Video.VideoPlayer video)
    {
        gameObject.SetActive(true);
    }
}
