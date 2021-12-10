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
    
    public void Initialize(LeaderboardGetEntriesResponse.Entry leaderboardEntry)
    {
        _rank.text = leaderboardEntry.rank.ToString();
        _name.text = leaderboardEntry.player.publicName;
        _score.text = leaderboardEntry.formattedScore;
    }
}
