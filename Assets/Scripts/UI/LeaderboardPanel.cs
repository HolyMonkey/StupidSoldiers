using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Agava.YandexGames;

public class LeaderboardPanel : MonoBehaviour
{
#if YANDEX_GAMES
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject[] _panels;
    [SerializeField] private TMP_Text[] _names;
    [SerializeField] private TMP_Text[] _points;
    [SerializeField] private Image[] _images;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private RawImage[] _startButtonsImage;
    [SerializeField] private Sprite[] _icons;
    [SerializeField] private GameObject _leaderbordButton;

    [SerializeField] private GameObject _template;
    [SerializeField] private GameObject _fartherPanel;
    [SerializeField] private int _maxLederbordEntriesCount;

    private PlayerInput _playerInput;
    private string _name;
    private int _countOfEntries;
    private List<GameObject> _leaderBordEntryVievs = new List<GameObject>();

    private void Awake()
    {

        _panel.SetActive(false);
        _playerInput = FindObjectOfType<PlayerInput>();
        ReciveCountOfEntries();
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

    private void ReciveCountOfEntries()
    {
        Leaderboard.GetEntries(YandexGamesConstants.LeaderboardName, OnLeaderbordInStartRecived); 
      
    }

    private void OnLeaderbordInStartRecived(LeaderboardGetEntriesResponse result)
    {
        if (result.entries.Length == 0)
        {
            _leaderbordButton.SetActive(false);           
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

        PlayerAccount.GetProfileData((responseresult) =>
        {
            _name = responseresult.uniqueID;
        });

        Leaderboard.GetEntries(YandexGamesConstants.LeaderboardName, OnLeaderboardReceived);
    }

    private void OnLeaderboardReceived(LeaderboardGetEntriesResponse result)
    {     
        foreach (var lederbordEntry in _leaderBordEntryVievs)
            Destroy(lederbordEntry);

        _leaderBordEntryVievs.Clear();

        if (result.entries.Length > _maxLederbordEntriesCount)
        {
            for (int i = 0; i < _maxLederbordEntriesCount; i++)
            {
                InstatiateLederbordData(result.entries[i]);
            }
        }               
        else
        {
            for (int i = 0; i < result.entries.Length; i++)
            {
                InstatiateLederbordData(result.entries[i]);
            }
        }                      

        Debug.Log(result.entries.Length);
        _panel.SetActive(true);

        if (_playerInput == null)
        {
            throw new UnityException("Player input is not found");
        }
        else
        {
            _playerInput.SetPanelActive();
            _shopButton.enabled = false;
            _startButton.enabled = false;

            foreach (var startButtonImage in _startButtonsImage)
                startButtonImage.gameObject.SetActive(false);
        }
    }

    private void InstatiateLederbordData(LeaderboardEntryResponse data)
    {
        var leaderboardEntryView = Instantiate(_template, _fartherPanel.transform);
        leaderboardEntryView.TryGetComponent(out LeaderboardEntryView entryViev);
        _leaderBordEntryVievs.Add(leaderboardEntryView);
        entryViev.Initialize(data, data.player.uniqueID);
    }
#endif

#if VK_GAMES
    [SerializeField] private Wallet _wallet;

    private void Awake()
    {
        _wallet = FindObjectOfType<Wallet>();
        StartCoroutine(InitializeVKSdk());
    }

    public void OnLederbordButtonOn()
    {
        Agava.VKGames.Leaderboard.ShowLeaderboard(_wallet.Coins);
    }

    private IEnumerator InitializeVKSdk()
    {
        yield return Agava.VKGames.VKGamesSdk.Initialize();
    }
#endif
}
