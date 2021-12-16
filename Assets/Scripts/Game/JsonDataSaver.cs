using System;
using System.IO;
using UnityEngine;

public class JsonDataSaver : DataSaver
{
    private string _path;
    private Save _save = new Save();

    private const string SaveName = "Save.json";

#if UNITY_ANDROID && !UNITY_EDITOR
    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            File.WriteAllText(_path, JsonUtility.ToJson(_save));
        }
    }
#else
    private void OnApplicationQuit()
    {
        File.WriteAllText(_path, JsonUtility.ToJson(_save));
    }
#endif

    public override void SaveData(int coins, int levelNumber)
    {
        _save.CoinsCount =  coins;
        _save.CurrentLevelNumber = levelNumber;
        
        File.WriteAllText(_path, JsonUtility.ToJson(_save));
    }

    public override void DownloadSave()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        _path = Path.Combine(Application.persistentDataPath,SaveName);
#else
        _path = Path.Combine(Application.dataPath, SaveName);
#endif

        if (File.Exists(_path))
        {
            _save = JsonUtility.FromJson<Save>(File.ReadAllText(_path));
            SetSessionID();
        }
        else
        {
            CreateNewSave();
        }
    }

    private void CreateNewSave()
    {
        _save = new Save();
        _save.CurrentLevelNumber = 1;
        _save.CoinsCount = 0;

        _save.RegDay = DateTime.Now.ToString("dd/MM/yy");
        _save.LastLevel = 1;
        _save.DaysAfter = 1;
        _save.SessionsID = 1;
    }
    
    private void SetSessionID()
    {
        _save.SessionsID += 1;
    }
    
    public override int GetCoinsCount()
    {
        return _save.CoinsCount;
    }

    public override int GetCurrentLevelNumber()
    {
        return _save.CurrentLevelNumber;
    }
}

[Serializable]
public class Save
{
    public int CoinsCount;
    public int CurrentLevelNumber;

    public int SessionsID;
    public int LastLevel;
    public int DaysAfter;
    public string RegDay;
}