using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleSceneUIManager : MonoBehaviour
{
    public void StartButton()
    {
        SceneManager.LoadScene("Prologue");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
