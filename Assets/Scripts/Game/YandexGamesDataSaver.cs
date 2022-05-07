
using System;
using System.Collections;
using UnityEngine;
using YandexGames;

public class YandexGamesDataSaver : DataSaver
{
    private int _coins;
    private int _levelNumber;
    
    public override void DownloadSave()
    {
        _coins = PlayerPrefs.GetInt(YandexGamesConstants.CoinsKey);
        _levelNumber = PlayerPrefs.GetInt(YandexGamesConstants.LevelNumberKey);
     
    }

    public override int GetCoinsCount()
    {
        return _coins;
    }

    public override int GetCurrentLevelNumber()
    {
        return _levelNumber;
    }

    public override void SaveData(int coins, int levelNumber)
    {
        PlayerPrefs.SetInt(YandexGamesConstants.CoinsKey, coins);
        PlayerPrefs.SetInt(YandexGamesConstants.LevelNumberKey, levelNumber);

        if (!PlayerAccount.IsAuthorized)
            return;
        
        if (!PlayerAccount.HasPersonalProfileDataPermission)
            PlayerAccount.RequestPersonalProfileDataPermission(() => Leaderboard.SetScore(YandexGamesConstants.LeaderboardName, coins, extraData: levelNumber.ToString()));
        else
            Leaderboard.SetScore(YandexGamesConstants.LeaderboardName, coins, extraData: levelNumber.ToString());
    }
}
