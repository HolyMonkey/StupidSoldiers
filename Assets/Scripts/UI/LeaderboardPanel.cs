using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Agava.YandexGames;

public class LeaderboardPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private GameObject[] _panels;
    //[SerializeField] private TMP_Text[] _numbers;
    [SerializeField] private TMP_Text[] _names;
    [SerializeField] private TMP_Text[] _points;
    //[SerializeField] private GameObject[] _pointPanels;
    //[SerializeField] private GameObject[] _numberPanels;
    [SerializeField] private Image[] _images;
    //[SerializeField] private GameObject _closeButton;
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
        //_closeButton.SetActive(false);
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
        //yield break;

        PlayerAccount.GetProfileData((responseresult) =>
        {
            _name = responseresult.uniqueID;
        });

        Leaderboard.GetEntries(YandexGamesConstants.LeaderboardName, OnLeaderboardReceived);
    }

    private void OnLeaderboardReceived(LeaderboardGetEntriesResponse result)
    {
        //foreach (var panel in _panels)
        //    panel.SetActive(false);

        //foreach (var name in _names)
        //    name.gameObject.SetActive(false);

        //foreach (var point in _points)
        //    point.gameObject.SetActive(false);

        //foreach (var image in _images)
        //    image.gameObject.SetActive(false);

        //if (result.entries.Length > 6)
        //{
        //    for (int i = 0; i < 5; i++)
        //    {
        //        Debug.Log(_name + " in for");
        //        Debug.Log(result.entries[i].player.uniqueID + "name in bord");
        //        Debug.Log(result.entries[i].player.publicName);
        //        _names[i].text = result.entries[i].player.publicName;
        //        _points[i].text = result.entries[i].score.ToString();
        //        _panels[i].SetActive(true);
        //        _names[i].gameObject.SetActive(true);
        //        _points[i].gameObject.SetActive(true);
        //        var random = UnityEngine.Random.Range(0, _icons.Length - 1);
        //        _images[i].sprite = _icons[random];
        //        _images[i].gameObject.SetActive(true);

        //        if (_name == result.entries[i].player.uniqueID)
        //        {
        //            Debug.Log(_names[i].color + "before change");
        //            _names[i].color = Color.green;
        //            Debug.Log("Color mast change");
        //            Debug.Log(_names[i].color + "after change");
        //        }
        //    }
        //}
        //else
        //{
        //    for (int i = 0; i < result.entries.Length - 1; i++)
        //    {
        //        Debug.Log(_name + " in for");
        //        Debug.Log(result.entries[i].player.uniqueID + "name in bord");
        //        Debug.Log(result.entries[i].player.publicName);
        //        _names[i].text = result.entries[i].player.publicName;
        //        _points[i].text = result.entries[i].score.ToString();
        //        _panels[i].SetActive(true);
        //        _names[i].gameObject.SetActive(true);
        //        _points[i].gameObject.SetActive(true);
        //        var random = UnityEngine.Random.Range(0, _icons.Length - 1);
        //        _images[i].sprite = _icons[random];
        //        _images[i].gameObject.SetActive(true);

        //        if (_name == result.entries[i].player.uniqueID)
        //        {
        //            Debug.Log(_names[i].color + "before change");
        //            _names[i].color = Color.green;
        //            Debug.Log("Color mast change");
        //            Debug.Log(_names[i].color + "after change");
        //        }
        //    }
        //}

        //if (_panel.activeSelf == false)
        //{
        //    if (_playerInput == null)
        //    {
        //        throw new UnityException("Player input is not found");
        //    }
        //    else
        //    {
        //        _playerInput.SetPanelActive();
        //        _panel.SetActive(true);
        //        _shopButton.enabled = false;
        //        _startButton.enabled = false;

        //        foreach (var startButtonImage in _startButtonsImage)
        //            startButtonImage.gameObject.SetActive(false);
        //    }
        //}

        

        foreach (var lederbordEntry in _leaderBordEntryVievs)
            Destroy(lederbordEntry);

        _leaderBordEntryVievs.Clear();

        if (result.entries.Length > _maxLederbordEntriesCount)
        {
            Debug.Log(result.entries.Length + "test228");
            for (int i = 0; i < _maxLederbordEntriesCount; i++)
            {
                Debug.Log(i + "test1");
                Debug.Log(result.entries[i].player.publicName + "test11");
                InstatiateLederbordData(result.entries[i]);
                Debug.Log(i + "test2");
                Debug.Log(result.entries[i].player.publicName + "test22");
            }
        }               
        else
        {
            Debug.Log(result.entries.Length + "test322");
            for (int i = 0; i < result.entries.Length; i++)
            {
                Debug.Log(i + "test1");
                Debug.Log(result.entries[i].player.publicName + "test11");
                InstatiateLederbordData(result.entries[i]);
                Debug.Log(i + "test2");
                Debug.Log(result.entries[i].player.publicName + "test22");
            }
        }                      

        Debug.Log("test");
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
        Debug.Log("spawn");
        var leaderboardEntryView = Instantiate(_template, _fartherPanel.transform);
        leaderboardEntryView.TryGetComponent(out LeaderboardEntryView entryViev);
        _leaderBordEntryVievs.Add(leaderboardEntryView);
        entryViev.Initialize(data, data.player.uniqueID);

    }
}
