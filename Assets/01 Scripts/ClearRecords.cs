using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class ClearRecords : MonoBehaviour
{
    /*
     `シングルトーンパターンでクリア時間を記録する
     */
    public static ClearRecords inst;
    private Dictionary<string, float> clearRecords = new Dictionary<string, float>();

    private string jsonPath;
    private void Awake()
    {//重複を避けるため・
        if (ClearRecords.inst == null)
        {
            ClearRecords.inst = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else 
        {
            Destroy(this.gameObject);
        }
    }

    public void AddClearTime(float time) 
    {
        string nowStage = SceneManager.GetActiveScene().name;
        if (clearRecords.ContainsKey(nowStage))
        {
            if (clearRecords[nowStage] > time) 
            {
                clearRecords[nowStage] = time;
            }
        }
        else
        {
            clearRecords.Add(nowStage, time);
        }
        foreach (var record in clearRecords) 
        {
            Debug.Log("Saved time: " + record);
        }
    }
    /*
    public void RecordClearTime()
    {
        string jsonData = DictionaryJsonUtility.ToJson(clearRecords, true);
        if (!Directory.Exists(jsonPath))
        {
            Directory.CreateDirectory(jsonPath);
        }
        System.IO.File.WriteAllText(jsonPath + "/ClearRecord.json", jsonData);
    }
    */
  
    public string LoadClearTIme() 
    {
        string records = "";
        foreach (var record in clearRecords) 
        {
            records += record.Key + ": " + record.Value.ToString("0.00") + "\n";
        }
        return records;
    }
    
}
