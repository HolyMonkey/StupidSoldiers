using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using YandexGames;

public class AppStart : MonoBehaviour
{
    private const int GameScene = 1;
    private IEnumerator Start()
    {
        yield return YandexGamesSdk.WaitForInitialization();

        if (!PlayerAccount.IsAuthorized)
            SceneManager.LoadScene(GameScene);
        
        Leaderboard.GetPlayerEntry(YandexGamesConstants.LeaderboardName, OnLeaderboardPlayerEntry, _ => SceneManager.LoadScene(GameScene));
    }
    
    private void OnLeaderboardPlayerEntry(LeaderboardEntryResponse response)
    {
        PlayerPrefs.SetInt(YandexGamesConstants.CoinsKey, response.score);

        var levelNumber = string.IsNullOrEmpty(response.extraData) ? 0 : int.Parse(response.extraData);
        PlayerPrefs.SetInt(YandexGamesConstants.LevelNumberKey, levelNumber);
        
        SceneManager.LoadScene(GameScene);
    }
}
