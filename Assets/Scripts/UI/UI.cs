using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class UI : MonoBehaviour
{
    [SerializeField] private WinPanel _winPanel;
    [SerializeField] private LosePanel _losePanel;
    [SerializeField] private PlayPanel _playPanel;
    [SerializeField] private StartPanel _startPanel;

    public event UnityAction RestartButtonClicked;
    public event UnityAction StartButtonClicked;
    public event UnityAction ContinueButtonClicked;

    private void OnEnable()
    {
        _startPanel.ShowPanel();

        _startPanel.StartButtonClicked += OnStartButtonClicked;
        _winPanel.ContinueButtonClicked += OnContinueButtonClicked;
        _losePanel.RestartButtonClicked += OnRestartButtonClicked;
    }

    private void OnDisable()
    {
        _startPanel.StartButtonClicked -= OnStartButtonClicked;
        _winPanel.ContinueButtonClicked -= OnContinueButtonClicked;
        _losePanel.RestartButtonClicked -= OnRestartButtonClicked;
    }


    private void OnRestartButtonClicked()
    {
        RestartButtonClicked?.Invoke();
    }

    private void OnStartButtonClicked()
    {
        StartButtonClicked?.Invoke();
        _startPanel.ClosePanel();
    }

    private void OnContinueButtonClicked()
    {
        ContinueButtonClicked?.Invoke();
    }

    public void ShowResultPanel(float result)
    {
        _winPanel.ShowPanel(result);
    }

    public void ShowLosePanel()
    {
        _losePanel.ShowPanel();
    }

    public void ShowPlayPanel()
    {

    }
}