using UnityEngine;
using UnityEngine.SceneManagement;
using Agava.YandexGames;
using System;

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

    public int EnemyCount => _enemyCount;
    public int KilledEnemy => _killedEnemy;

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
            }           
        }
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(_currentSceneIndex);
    }

    private void OnContinueButtonClick()
    {
        if (_levelNumber == 18)
            _levelNumber = 0;
        _dataSaver.SaveData(_wallet.Coins, _levelNumber + 1);
        InterestialAd.Show(_adOpened, _adClosed, _adError, _adOfline);
    }

    private void OnAdError(string eror)
    {
        Debug.Log("VideoClose");
        AudioListener.pause = false;
        AudioListener.volume = 1f;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    private void OnAdOfline()
    {
        Debug.Log("VideoClose");
        AudioListener.pause = false;
        AudioListener.volume = 1f;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    private void OnAdClose(bool boolean)
    {
        Debug.Log("VideoClose");
        AudioListener.pause = false;
        AudioListener.volume = 1f;
        SceneManager.LoadScene(_nextSceneIndex);
    }

    private void OnAdOpen()
    {
        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }
}