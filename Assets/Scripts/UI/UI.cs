using System.Collections;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TMP_Text _praise;
    [SerializeField] private CanvasGroup _praiseCanvasGroup;
    [SerializeField] private float _showPraiseDuration;
    [SerializeField] private AnimationCurve _praiseCurve;

    private IEnumerator _changePraiseImage;
    private const string PraiseValue = "Headshot";

    private void OnEnable()
    {
        _changePraiseImage = ChangePraiseImage();
        _praise.text = PraiseValue;
    }

    public void ShowPraise()
    {
        StopCoroutine(_changePraiseImage);
        _changePraiseImage = ChangePraiseImage();
        StartCoroutine(_changePraiseImage);
    }

    private IEnumerator ChangePraiseImage()
    {
        float elapsedTime = 0;
        
        while (elapsedTime < _showPraiseDuration)
        {
            float progress = elapsedTime / _showPraiseDuration;
            elapsedTime += Time.deltaTime;
            _praiseCanvasGroup.alpha = _praiseCurve.Evaluate(progress);

            yield return null;
        }
    }

}