using UnityEngine;
using UnityEngine.SceneManagement;
using YandexGames;

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

    private int _levelNumber;

    public void DescreaseEnemyCount()
    {
        _enemyCount--;
    }

    private void OnEnable()
    {
        _dataSaver.DownloadSave();

        _wallet.SetCoins(_dataSaver.GetCoinsCount());
        _levelNumber = _dataSaver.GetCurrentLevelNumber();

        Time.timeScale = 1;
        _weapon.Death += OnWeaponDead;

        _ui.StartButtonClicked += OnStartButtonClick;
        _ui.ContinueButtonClicked += OnContinueButtonClick;
        _ui.RestartButtonClicked += OnRestartButtonClick;

        foreach(var multiplier in _multipliers)
        {
            multiplier.MultiplierHit += OnMultiplierHit;
        }

        _ui.SetCurrentLevel(_levelNumber);
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
        if (_enemyCount <= 0)
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
    }

    private void OnStartButtonClick()
    {
        _weapon.CanShoot();
        _ui.ShowPlayPanel();
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