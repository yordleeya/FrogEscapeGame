using UnityEngine;
using TMPro; // TextMeshProUGUI를 사용하려면 필요
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RecordDisplay : MonoBehaviour
{
    public TextMeshProUGUI recordListText; // 인스펙터에서 연결
    public TMP_InputField nameInputField; // 이름 입력 필드
    public Button saveButton;             // 저장 버튼
    public TimerUI timerUI;               // TimerUI 스크립트 참조
    public Button restartButton;          // 재시작 버튼

    void OnEnable()
    {
        nameInputField.text = "";
        saveButton.interactable = true;
        if (nameInputField != null) nameInputField.gameObject.SetActive(true);
        if (saveButton != null) saveButton.gameObject.SetActive(true);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        if (recordListText != null) recordListText.text = ""; // 기록 등록 전에는 기록 리스트를 비움
    }

    void Start()
    {
        saveButton.onClick.AddListener(SaveRecord);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
    }

    void SaveRecord()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrEmpty(playerName)) return;

        float time = timerUI != null ? timerUI.GetElapsedTime() : 0f;
        RecordManager recordManager = FindObjectOfType<RecordManager>();
        recordManager.AddRecord(playerName, time);

        UpdateRecordList();
        saveButton.interactable = false; // 중복 저장 방지
        if (nameInputField != null) nameInputField.gameObject.SetActive(false);
        if (saveButton != null) saveButton.gameObject.SetActive(false);
        if (restartButton != null) restartButton.gameObject.SetActive(true);
    }

    void RestartGame()
    {
        Time.timeScale = 1f; // 게임 시간 재개
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // 현재 씬 다시 로드
    }

    void UpdateRecordList()
    {
        RecordManager recordManager = FindObjectOfType<RecordManager>();
        var records = recordManager.GetRecords();
        string result = "";

        for (int i = 0; i < records.Count; i++)
        {
            var rec = records[i];
            int minutes = (int)(rec.time / 60);
            int seconds = (int)(rec.time % 60);
            int millis = (int)((rec.time * 1000) % 1000);
            result += $"{i + 1}. {rec.playerName} - {minutes:D2}:{seconds:D2}.{millis:D3}\n";
        }

        if (records.Count == 0)
            result = "";

        recordListText.text = result;
    }
}