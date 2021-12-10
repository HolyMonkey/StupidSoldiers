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
    
    private const string LeaderboardName = "1";

    private IEnumerator Start()
    {
        _panel.SetActive(false);
        
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif
        
        yield return YandexGamesSdk.WaitForInitialization();
        
        if (!PlayerAccount.IsAuthorized)
            yield break;
        
        yield return Leaderboard.WaitForInitialization();
        Leaderboard.GetEntries(LeaderboardName, OnLeaderboardReceived);
    }
    
    private void OnLeaderboardReceived(LeaderboardGetEntriesResponse result)
    {
        foreach (LeaderboardGetEntriesResponse.Entry leaderboardEntry in result.entries)
        {
            var leaderboardEntryView = Instantiate(_template, _panel.transform);
            leaderboardEntryView.Initialize(leaderboardEntry);
        }
        
        _panel.SetActive(true);
    }
}
