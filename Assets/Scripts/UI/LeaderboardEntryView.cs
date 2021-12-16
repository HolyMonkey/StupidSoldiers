using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using YandexGames;

public class LeaderboardEntryView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _rank;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private TextMeshProUGUI _score;
    
    public void Initialize(LeaderboardEntryResponse leaderboardEntry)
    {
        _rank.text = leaderboardEntry.rank.ToString();
        _name.text = FormatName(leaderboardEntry);
        _score.text = leaderboardEntry.formattedScore;
    }

    private string FormatName(LeaderboardEntryResponse leaderboardEntry)
    {
        return string.IsNullOrEmpty(leaderboardEntry.player.publicName) 
            ? "Anon"
            : leaderboardEntry.player.publicName;
    }
}
