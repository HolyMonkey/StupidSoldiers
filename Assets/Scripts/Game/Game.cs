using UnityEngine;
using UnityEngine.SceneManagement;
using Agava.YandexGames;
using System;
using GameAnalyticsSDK;

public class Game : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private UI _ui;
    [SerializeField] private Multiplier[] _multipliers;
    [SerializeField] private int _currentSceneIndex;
    [SerializeField] private int _nextSceneIndex;
    [SerializeField] private Wallet _wallet;
    [SerializeField] private DataSaver _dataSaver;
    [SerializeField] private int _enemyCount;
    [SerializeField] private GameObject _levelCompleteText;
    [SerializeField] private GameObject _leaderbordButton;
    [SerializeField] private GameObject _shopButton;
    [SerializeField] private GameObject _revardVideoButton;
    [SerializeField] private GameObject _revardedVideoButton;

    private int _levelNumber;
    private int _killedEnemy;
    private PlayerInput _playerInput;
    private Action _adOpened;
    private Action<bool> _adClosed;
    private Action _adOfline;
    private Action<string> _adError;
    private int _levelTimeSpent;
    private int _totalPlayedTime;

    public int EnemyCount => _enemyCount;
    public int KilledEnemy => _killedEnemy;
    public int LevelNumber => _levelNumber;

    //public void DescreaseEnemyCount()
    //{
    //    _enemyCount--;
    //    _killedEnemy++;
    //    if(_enemyCount == 0)
    //    {
    //        _levelCompleteText.SetActive(true);
    //        foreach(var multiplier in _multipliers)
    //        {
    //            multiplier.ChangeCanDestroyStatus();
    //        }
    //    }
    //}

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _totalPlayedTime = PlayerPrefs.GetInt("TotalPlayedTime");
        _levelTimeSpent = 0;
        PlayerPrefs.SetInt("TotalPlayedTime", _totalPlayedTime);
        GameAnalitic.TotalPlayedTime(_totalPlayedTime);
    }



    private void OnEnable()
    {
        _killedEnemy = 0;

        _dataSaver.DownloadSave();

        _wallet.SetCoins(_dataSaver.GetCoinsCount());
        _levelNumber = _dataSaver.GetCurrentLevelNumber();

        Time.timeScale = 1;
        _weapon.Death += OnWeaponDead;

        _ui.StartButtonClicked += OnStartButtonClick;
        _ui.ContinueButtonClicked += OnContinueButtonClick;
        _ui.RestartButtonClicked += OnRestartButtonClick;

        foreach (var multiplier in _multipliers)
        {
            multiplier.MultiplierHit += OnMultiplierHit;
        }

        _ui.SetCurrentLevel(_levelNumber);

        _levelCompleteText.SetActive(false);

        _adOpened += OnAdOpen;
        _adClosed += OnAdClose;
        _adOfline += OnAdOfline;
        _adError += OnAdError;
    }

    private void OnDisable()
    {
        _weapon.Death -= OnWeaponDead;

        _ui.StartButtonClicked -= OnStartButtonClick;
        _ui.ContinueButtonClicked += OnContinueButtonClick;
        _ui.RestartButtonClicked += OnRestartButtonClick;

        foreach (var multiplier in _multipliers)
        {
            multiplier.MultiplierHit += OnMultiplierHit;
        }

        _adOpened -= OnAdOpen;
        _adClosed -= OnAdClose;
        _adOfline -= OnAdOfline;
        _adError -= OnAdError;
    }

#if VK_GAMES
    private void Update()
    {
        _totalPlayedTime++;     
        _levelTimeSpent++;
#endif
    }

    private void OnWeaponDead()
    {
        Time.timeScale = 0;
        _weapon.DisableShoot();
        _ui.ShowLosePanel();
    }

    private void OnMultiplierHit(Multiplier multiplier)
    {
        _weapon.DisableShoot();

        _ui.ClosePlayPanel();

        _ui.ShowResultPanel(_wallet.Increase, multiplier.MultiplierValue);

        _wallet.SetMultiplier(multiplier.MultiplierValue);

        foreach (var mult in _multipliers)
        {
            if (mult != multiplier)
                mult.Disable();
        }

    }

    private void OnStartButtonClick()
    {
        if (_playerInput == null)
        {
            throw new UnityException("Player input is not found");
        }
        else
        {
            if (_playerInput.IsPanelOpen == false)
            {
                _weapon.CanShoot();
                _ui.ShowPlayPanel();
                _leaderbordButton.SetActive(false);
                _shopButton.SetActive(false);
                _revardedVideoButton.SetActive(false);
                _revardVideoButton.SetActive(false);
                _playerInput.StartGame();
#if VK_GAMES
                GameAnalitic.StartLevel(_currentSceneIndex);
                PlayerPrefs.SetInt("TotalPlayedTime", _totalPlayedTime);
                GameAnalitic.TotalPlayedTime(_totalPlayedTime);
#endif
            }
        }
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(_currentSceneIndex);
#if VK_GAMES
        GameAnalitic.RestartLevel(_currentSceneIndex, _levelTimeSpent);
        PlayerPrefs.SetInt("TotalPlayedTime", _totalPlayedTime);
        GameAnalitic.TotalPlayedTime(_totalPlayedTime);
#endif
    }

    private void OnContinueButtonClick()
    {
        if (_levelNumber == 18)
            _levelNumber = 0;
        _dataSaver.SaveData(_wallet.Coins, _levelNumber + 1);
#if YANDEX_GAMES
        InterestialAd.Show(_adOpened, _adClosed, _adError, _adOfline);
#endif
#if VK_GAMES
        Agava.VKGames.Interstitial.Show();
        GameAnalitic.AddsStart(GameAnalyticsSDK.GAAdType.Interstitial);
        GameAnalitic.AdsComplete(GameAnalyticsSDK.GAAdType.Interstitial);
        GameAnalitic.EndLevel(_currentSceneIndex, _levelTimeSpent);
        PlayerPrefs.SetInt("TotalPlayedTime", _totalPlayedTime);
        GameAnalitic.TotalPlayedTime(_totalPlayedTime);
        SceneManager.LoadScene(_nextSceneIndex);
#endif

    }

    private void OnAdError(string eror)
    {
        AudioListener.pause = false;
        AudioListener.volume = 0.5f;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    private void OnAdOfline()
    {
        AudioListener.pause = false;
        AudioListener.volume = 0.5f;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    private void OnAdClose(bool boolean)
    {
        AudioListener.pause = false;
        AudioListener.volume = 0.5f;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    private void OnAdOpen()
    {
        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }
}