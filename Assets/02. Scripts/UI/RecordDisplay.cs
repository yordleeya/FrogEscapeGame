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
    public TMP_InputField deleteRankInputField; // 삭제할 순위 입력 필드
    public Button deleteButton;                 // 삭제 버튼

    void OnEnable()
    {
        nameInputField.text = "";
        saveButton.interactable = true;
        if (nameInputField != null) nameInputField.gameObject.SetActive(true);
        if (saveButton != null) saveButton.gameObject.SetActive(true);
        if (restartButton != null) restartButton.gameObject.SetActive(false);
        UpdateRecordList(); // 기록 리스트를 바로 표시
    }

    void Start()
    {
        saveButton.onClick.AddListener(SaveRecord);
        if (restartButton != null)
            restartButton.onClick.AddListener(RestartGame);
        if (deleteButton != null)
            deleteButton.onClick.AddListener(DeleteRecordByRank);
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

    void DeleteRecordByRank()
    {
        int rank;
        if (int.TryParse(deleteRankInputField.text, out rank))
        {
            RecordManager recordManager = FindObjectOfType<RecordManager>();
            var records = recordManager.GetRecords();
            if (rank >= 1 && rank <= records.Count)
            {
                records.RemoveAt(rank - 1); // 0-based index
                // 저장 반영 (private SaveRecords 호출)
                typeof(RecordManager)
                    .GetMethod("SaveRecords", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    .Invoke(recordManager, null);
                UpdateRecordList();
            }
        }
        deleteRankInputField.text = "";
    }
}