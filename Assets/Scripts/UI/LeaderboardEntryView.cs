using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Agava.YandexGames;
using UnityEngine.UI;

public class LeaderboardEntryView : MonoBehaviour
{
    [SerializeField] private TMP_Text _scrore;
    [SerializeField] private TMP_Text _name;
    [SerializeField] private Sprite[] _aviableIcons;
    [SerializeField] private Image _icon;

    public void Initialize(LeaderboardEntryResponse data, string playerId)
    {
        if (playerId == data.player.uniqueID)
            _name.color = Color.green;

        _scrore.text = data.score.ToString();
        _name.text = data.player.publicName;
        _icon.sprite = _aviableIcons[UnityEngine.Random.Range(0, _aviableIcons.Length - 1)];
    }
}
