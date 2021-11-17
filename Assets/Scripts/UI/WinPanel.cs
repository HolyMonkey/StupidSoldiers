using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WinPanel : MonoBehaviour
{
    [SerializeField] private GameObject _panel;
    [SerializeField] private Button _continueButton;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private TMP_Text _rewardText;
    [SerializeField] private TMP_Text _multiplierText;

    private int _reward;
    private int _multiplier = 1;

    public event UnityAction ContinueButtonClicked;

    private void OnEnable()
    {
        _continueButton.onClick.AddListener(OnContinueButtonClick);
    }

    private void OnDisable()
    {
        _continueButton.onClick.RemoveListener(OnContinueButtonClick);
    }

    private void OnContinueButtonClick()
    {
        ContinueButtonClicked?.Invoke();
    }

    public void ShowPanel(int result,int multiplier)
    {
        _panel.SetActive(true);

        _multiplier = multiplier;
        _reward = result;

        _text.text = "Победа!";

        StartCoroutine(ShowRewardCoins());
    }

    public void ClosePanel()
    {
        _panel.SetActive(false);
    }

    private IEnumerator ShowRewardCoins()
    {
        int currentCoins = 0;

        while (currentCoins < _reward)
        {
            currentCoins++;
            _rewardText.text = "+" + currentCoins.ToString();
            yield return new WaitForSeconds(0.01f);
        }
        _multiplierText.text = "X" + _multiplier;

        yield return new WaitForSeconds(1);

        _multiplierText.text = "";

        _reward *= _multiplier;

        _rewardText.text = "+" + _reward.ToString();

        yield return null;
    }
    
}
