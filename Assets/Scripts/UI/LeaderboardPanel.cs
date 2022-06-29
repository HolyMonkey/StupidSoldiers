using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Agava.YandexGames;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private LeaderboardEntryView _template;
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TMP_Text[] _numbers;
    [SerializeField] private TMP_Text[] _names;
    [SerializeField] private TMP_Text[] _points;
    [SerializeField] private GameObject[] _pointPanels;
    [SerializeField] private GameObject[] _numberPanels;
    [SerializeField] private GameObject _closeButton;

    private void Awake()
    {
        _panel.SetActive(false);
        _closeButton.SetActive(false);
    }

    public void OnLederbordButtonOn()
    {
        StartCoroutine(ReciveLederbord());
        _closeButton.SetActive(true);
    }

    private IEnumerator ReciveLederbord()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        yield break;
#endif

        yield return YandexGamesSdk.WaitForInitialization();

        if (!PlayerAccount.IsAuthorized)
            PlayerAccount.Authorize();
        //yield break;

        Leaderboard.GetEntries(YandexGamesConstants.LeaderboardName, OnLeaderboardReceived);
    }

    private void OnLeaderboardReceived(LeaderboardGetEntriesResponse result)
    {
        if (result.entries.Length == 0)
            return;

        foreach (var panel in _panels)
            panel.SetActive(false);

        foreach (var number in _numbers)
            number.gameObject.SetActive(false);

        foreach (var name in _names)
            name.gameObject.SetActive(false);

        foreach (var point in _points)
            point.gameObject.SetActive(false);

        foreach (var pointPanel in _pointPanels)
            pointPanel.SetActive(false);

        foreach (var numberPanel in _numberPanels)
            numberPanel.SetActive(false);

        if (result.entries.Length > 7)
        {
            for (int i = 0; i < 7; i++)
            {
                _numbers[i].text = result.entries[i].rank.ToString();
                _names[i].text = result.entries[i].player.publicName;
                _points[i].text = result.entries[i].score.ToString();
                _panels[i].SetActive(true);
                _numbers[i].gameObject.SetActive(true);
                _names[i].gameObject.SetActive(true);
                _points[i].gameObject.SetActive(true);
                _pointPanels[i].SetActive(true);
                _numberPanels[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < result.entries.Length; i++)
            {
                _numbers[i].text = result.entries[i].rank.ToString();
                _names[i].text = result.entries[i].player.publicName;
                _points[i].text = result.entries[i].score.ToString();
                _panels[i].SetActive(true);
                _numbers[i].gameObject.SetActive(true);
                _names[i].gameObject.SetActive(true);
                _points[i].gameObject.SetActive(true);
                _pointPanels[i].SetActive(true);
                _numberPanels[i].SetActive(true);
            }
        }
        //foreach (LeaderboardEntryResponse leaderboardEntry in result.entries)
        //{
        //    var leaderboardEntryView = Instantiate(_template,_panel.transform);
        //    //var ledeborbEntryViewRect = leaderboardEntryView.GetComponent<RectTransform>();
        //    //ledeborbEntryViewRect.anchorMin = new Vector2(ledeborbEntryViewRect.anchorMin.x, ledeborbEntryViewRect.anchorMin.y + 2);
        //    //ledeborbEntryViewRect.anchorMax = new Vector2(ledeborbEntryViewRect.anchorMax.x, ledeborbEntryViewRect.anchorMax.y + 2);
        //    leaderboardEntryView.Initialize(leaderboardEntry);
        //}

        if (_panel.activeSelf == false)
        {
            _panel.SetActive(true);
        }

    }
}
