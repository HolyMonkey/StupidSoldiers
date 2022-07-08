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
    //[SerializeField] private TMP_Text[] _numbers;
    [SerializeField] private TMP_Text[] _names;
    [SerializeField] private TMP_Text[] _points;
    //[SerializeField] private GameObject[] _pointPanels;
    //[SerializeField] private GameObject[] _numberPanels;
    [SerializeField] private GameObject[] _images;
    //[SerializeField] private GameObject _closeButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private RawImage[] _startButtonsImage;

    private PlayerInput _playerInput;

    private void Awake()
    {
        //_closeButton.SetActive(false);
        _panel.SetActive(false);
        _playerInput = FindObjectOfType<PlayerInput>();
    }

    public void OnLederbordButtonOn()
    {
        if (_panel.activeSelf == true)
        {
            _panel.SetActive(false);
            _playerInput.SetPanelNotActive();
            _shopButton.enabled = true;
            _startButton.enabled = true;

            foreach (var startButtonImage in _startButtonsImage)
                startButtonImage.gameObject.SetActive(true);
        }        
        else
        {
            StartCoroutine(ReciveLederbord());
        }            
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

        //foreach (var number in _numbers)
        //    number.gameObject.SetActive(false);

        foreach (var name in _names)
            name.gameObject.SetActive(false);

        foreach (var point in _points)
            point.gameObject.SetActive(false);

        foreach (var image in _images)
            image.SetActive(false);

        //foreach (var pointPanel in _pointPanels)
        //    pointPanel.SetActive(false);

        //foreach (var numberPanel in _numberPanels)
        //    numberPanel.SetActive(false);

        if (result.entries.Length > 7)
        {
            for (int i = 0; i < 5; i++)
            {
                //_numbers[i].text = result.entries[i].rank.ToString();
                _names[i].text = result.entries[i].player.publicName;
                _points[i].text = result.entries[i].score.ToString();
                _panels[i].SetActive(true);
                //_numbers[i].gameObject.SetActive(true);
                _names[i].gameObject.SetActive(true);
                _points[i].gameObject.SetActive(true);
                _images[i].SetActive(true);
                //_pointPanels[i].SetActive(true);
                //_numberPanels[i].SetActive(true);
            }
        }
        else
        {
            for (int i = 0; i < result.entries.Length; i++)
            {
                //_numbers[i].text = result.entries[i].rank.ToString();
                _names[i].text = result.entries[i].player.publicName;
                _points[i].text = result.entries[i].score.ToString();
                _panels[i].SetActive(true);
                //_numbers[i].gameObject.SetActive(true);
                _names[i].gameObject.SetActive(true);
                _points[i].gameObject.SetActive(true);
                //_pointPanels[i].SetActive(true);
                //_numberPanels[i].SetActive(true);
                _images[i].SetActive(true);
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
            if (_playerInput == null)
            {
                throw new UnityException("Player input is not found");
            }             
            else
            {
                _playerInput.SetPanelActive();
                _panel.SetActive(true);
                _shopButton.enabled = false;
                _startButton.enabled = false;

                foreach (var startButtonImage in _startButtonsImage)
                    startButtonImage.gameObject.SetActive(false);

                //_closeButton.SetActive(true);
            }        
        }

    }
}
