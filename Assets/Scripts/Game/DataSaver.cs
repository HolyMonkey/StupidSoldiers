
using UnityEngine;

public abstract class DataSaver : MonoBehaviour
{
    public abstract void DownloadSave();

    public abstract int GetCoinsCount();

    public abstract int GetCurrentLevelNumber();

    public abstract void SaveData(int coins, int levelNumber);
}
