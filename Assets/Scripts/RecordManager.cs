using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[System.Serializable]
public class Record
{
    public string playerName;
    public float time;

    public Record(string name, float time)
    {
        this.playerName = name;
        this.time = time;
    }
}

public class RecordManager : MonoBehaviour
{
    private const int MaxRecords = 10;
    private List<Record> records = new List<Record>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        LoadRecords();
    }

    public void AddRecord(string name, float time)
    {
        records.Add(new Record(name, time));
        records = records.OrderBy(r => r.time).ToList();
        if (records.Count > MaxRecords)
            records.RemoveAt(records.Count - 1);
        SaveRecords();
    }

    public List<Record> GetRecords()
    {
        return records;
    }

    private void SaveRecords()
    {
        for (int i = 0; i < records.Count; i++)
        {
            PlayerPrefs.SetString($"RecordName{i}", records[i].playerName);
            PlayerPrefs.SetFloat($"RecordTime{i}", records[i].time);
        }
        PlayerPrefs.SetInt("RecordCount", records.Count);
        PlayerPrefs.Save();
    }

    private void LoadRecords()
    {
        records.Clear();
        int count = PlayerPrefs.GetInt("RecordCount", 0);
        for (int i = 0; i < count; i++)
        {
            string name = PlayerPrefs.GetString($"RecordName{i}", "");
            float time = PlayerPrefs.GetFloat($"RecordTime{i}", 0f);
            records.Add(new Record(name, time));
        }
    }
}
