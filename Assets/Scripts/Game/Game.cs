using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    [SerializeField] private Weapon _currentWeapon;
    [SerializeField] private CameraFollowing _camera;
    [SerializeField] private Multiplier[] _multipliers;
    [SerializeField] private Barrier[] _barriers;
    //[SerializeField] private UI _uiPanel;

    public event UnityAction GameStarted;

    private void OnEnable()
    {
        foreach(var mulitiplier in _multipliers)
        {
            mulitiplier.MultiplierHit += OnMultiplierHit;
        }

        foreach(var barrier in _barriers)
        {
            barrier.Hit += OnBarrierHit;
        }

        _currentWeapon.GameOver += GameOver;

        StartGame();
    }

    private void OnDisable()
    {
        _currentWeapon.GameOver -= GameOver;

        foreach (var mulitiplier in _multipliers)
        {
            mulitiplier.MultiplierHit -= OnMultiplierHit;
        }

        foreach (var barrier in _barriers)
        {
            barrier.Hit -= OnBarrierHit;
        }
    }

    private void OnMultiplierHit(Multiplier multiplier)
    {
        //_camera.ShowResultEffect();
        _currentWeapon.DisableShooting();
        _currentWeapon.DisableShooting();
        foreach (var mulitiplier in _multipliers)
        {
            mulitiplier.SetDestroy();
        }
    }

    private void OnBarrierHit()
    {
       // _uiPanel.ShowPraise();
    }

    private void StartGame()
    {
        _camera.StartFollow();
        GameStarted?.Invoke();
        _currentWeapon.enabled = true;
    }

    private void GameOver()
    {
        Restart();
    }

    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}