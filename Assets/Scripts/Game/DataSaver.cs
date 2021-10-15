using System;
using System.IO;
using UnityEngine;

public class DataSaver : MonoBehaviour
{

    [SerializeField] private Wallet _wallet;
    [SerializeField] private Game _game;

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

    public void SaveData()
    {
       // Amplitude.Instance.setUserProperty("level_last", _game.LevelNumber);

        _save.CoinsCount =  (uint)_wallet.Coins;
        _save.CurrentLevelNumber = _game.LevelNumber+1;
//        SetLastLevel();


        File.WriteAllText(_path, JsonUtility.ToJson(_save));

    }

    public void DownloadSave()
    {

#if UNITY_ANDROID && !UNITY_EDITOR
        _path = Path.Combine(Application.persistentDataPath,SaveName);
#else
        _path = Path.Combine(Application.dataPath, SaveName);
#endif

        if (File.Exists(_path))
        {
            _save = JsonUtility.FromJson<Save>(File.ReadAllText(_path));
          //  SetDaysAfter();
          //  SetSessioinID();
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

        string time = DateTime.Now.ToString("dd/MM/yy");

//        Amplitude.Instance.addUserProperty("session_id", 1);
  //      Amplitude.Instance.addUserProperty("reg_day", time);
  //      Amplitude.Instance.addUserProperty("days_after", 0);
   //     Amplitude.Instance.addUserProperty("level_last", 1);
    }

    public uint GetCoinsCount()
    {
        return _save.CoinsCount;
    }

    public uint GetCurrentLevelNumber()
    {
        return _save.CurrentLevelNumber;
    }
}

[Serializable]
public class Save
{
    public uint CoinsCount;
    public uint CurrentLevelNumber;

    public int SessionsID;
    public int LastLevel;
    public int DaysAfter;
    public string RegDay;
}