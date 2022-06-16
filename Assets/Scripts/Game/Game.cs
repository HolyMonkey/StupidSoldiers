using UnityEngine;
using UnityEngine.SceneManagement;
using Agava.YandexGames;

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
    [SerializeField] private GameObject _shopButton;

    private int _levelNumber;
    private int _killedEnemy;

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
        _weapon.CanShoot();
        _ui.ShowPlayPanel();
        _shopButton.SetActive(false);
    }

    private void OnRestartButtonClick()
    {
        SceneManager.LoadScene(_currentSceneIndex);
    }

    private void OnContinueButtonClick()
    {
        _dataSaver.SaveData(_wallet.Coins, _levelNumber + 1);
        InterestialAd.Show();
        SceneManager.LoadScene(_nextSceneIndex);
    }
}