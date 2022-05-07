using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YandexGames;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private LeaderboardEntryView _template;
    [SerializeField] private GameObject _panel;
    
    private IEnumerator Start()
    {
        _panel.SetActive(false);
        
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        
        yield return YandexGamesSdk.WaitForInitialization();

        if (!PlayerAccount.IsAuthorized)
            yield break;

        Debug.Log(PlayerAccount.IsAuthorized);

        Leaderboard.GetEntries(YandexGamesConstants.LeaderboardName, OnLeaderboardReceived);
    }
    
    private void OnLeaderboardReceived(LeaderboardGetEntriesResponse result)
    {
        if (result.entries.Length == 0)
            return;
        
        foreach (LeaderboardEntryResponse leaderboardEntry in result.entries)
        {
            var leaderboardEntryView = Instantiate(_template, _panel.transform);
            leaderboardEntryView.Initialize(leaderboardEntry);
        }
        
        _panel.SetActive(true);
    }
}
