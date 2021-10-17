using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ProgressBar))]
public class PlayPanel : MonoBehaviour
{

    [SerializeField] private GameObject _panel;
    [SerializeField] private Slider _progress;
    [SerializeField] private TMP_Text _coins;
    [SerializeField] private Wallet _playerWallet;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _currentLevel;
    [SerializeField] private TMP_Text _nextLevel;
    [SerializeField] private float _changeCoinsDelay;

    private ProgressBar _progressBar;
    private IEnumerator _showProgress;
    private IEnumerator _changeCoins;
    private int _targetCoinsCount = 0;
        private int _currentCoinsCount ;

    private void OnEnable()
    {
        _progressBar = GetComponent<ProgressBar>();
        _playerWallet.ChangeCoinsCount += OnCoinsCountChanged;
        _showProgress = ShowProgress();
        _changeCoins = ChangeCoinsCount();
    }

    private void OnDisable()
    {
        _playerWallet.ChangeCoinsCount -= OnCoinsCountChanged;
    }

    private void OnCoinsCountChanged(int currentCount)
    {
        _targetCoinsCount = currentCount;
        StopCoroutine(_changeCoins);
        _changeCoins = ChangeCoinsCount();
        StartCoroutine(_changeCoins);
    }

    public void ShowPanel()
    {
        _currentCoinsCount = _playerWallet.Coins;
        _coins.text = _playerWallet.Coins.ToString()+"$";
        StartCoroutine(VisiblePanel());

        _panel.SetActive(true);
        StartCoroutine(_showProgress);
    }

    public void ClosePanel()
    {
        _panel.SetActive(false);
        StopCoroutine(_showProgress);
        _showProgress = ShowProgress();
    }

    public void SetCurrentLevel(int number)
    {
        _currentLevel.text = number.ToString();
        _nextLevel.text = (number+1).ToString();
    }

    private IEnumerator ShowProgress()
    {
        while (true)
        {
            _progress.value = _progressBar.Progress;
            yield return null;
        }
    }

    private IEnumerator VisiblePanel()
    {
        _canvasGroup.alpha = 0;
        float elapsedTime = 0;
        while (elapsedTime < 0.2f)
        {
            elapsedTime += Time.deltaTime;
            _canvasGroup.alpha = elapsedTime/0.2f;

            yield return null;
        }
        _canvasGroup.alpha = 1;
    }

    private IEnumerator ChangeCoinsCount()
    {
        yield return new WaitForSeconds(0.7f);

        while (_currentCoinsCount < _targetCoinsCount)
        {
            _currentCoinsCount++;
            _coins.text = _currentCoinsCount.ToString() + "$";
            yield return new WaitForSeconds(_changeCoinsDelay);
        }
    }
}