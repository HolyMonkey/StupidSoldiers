using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class StartPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _tapToStart;
    [SerializeField] private Button _startButton;
    [SerializeField] private float _flashDuration;
    [SerializeField] private float _flashSpeed;
    [SerializeField] private AnimationCurve _flashXCurve;
    [SerializeField] private AnimationCurve _flashYCurve;

    private IEnumerator _showTextEnumerator;

    public event UnityAction StartButtonClicked;

    private void OnEnable()
    {
        _startButton.onClick.AddListener(OnStartButtonClicked);
    }

    private void OnDisable()
    {
        _startButton.onClick.RemoveListener(OnStartButtonClicked);
    }

    private void OnStartButtonClicked()
    {
        StartButtonClicked?.Invoke();
    }

    public void ShowPanel()
    {
        _panel.SetActive(true);
        _showTextEnumerator = ShowText();
        StartCoroutine(_showTextEnumerator);
    }

    public void ClosePanel()
    {
        StopCoroutine(_showTextEnumerator);
        _panel.SetActive(false);
    }

    private IEnumerator ShowText()
    {
        Vector3 defaultScale = _tapToStart.transform.localScale;

        while (true)
        {
            float elapsedTime = 0;
            while (elapsedTime < _flashDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / _flashDuration;
                _tapToStart.transform.localScale = new Vector3(defaultScale.x * _flashXCurve.Evaluate(progress), defaultScale.y * _flashYCurve.Evaluate(progress));
                yield return null;
            }
        }
    }

}
