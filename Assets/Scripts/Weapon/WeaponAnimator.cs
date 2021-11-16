using System.Collections;
using UnityEngine;

public class WeaponAnimator : MonoBehaviour
{
    [SerializeField] private GameObject _body;
    [SerializeField] private float _delayAfterShoot;
    [SerializeField] private AnimationCurve _scaleCurve;
    [SerializeField] private float _scaleChangeDuration;
    [SerializeField] private float _scaleChangeSpeed;

    private IEnumerator _changeBodyScale;

    private void OnEnable()
    {
        _changeBodyScale = ChangeBodyScale();
    }

    public void Shoot()
    {
        StopCoroutine(_changeBodyScale);
        _changeBodyScale = ChangeBodyScale();
        StartCoroutine(_changeBodyScale);
    }


    private IEnumerator ChangeBodyScale()
    {
        yield return new WaitForSeconds(_delayAfterShoot);

        float elapsedTime = 0;

        while (elapsedTime < _scaleChangeDuration)
        {
            elapsedTime += Time.deltaTime;
            var progress = elapsedTime / _scaleChangeDuration;

            _body.transform.localScale = new Vector3(_scaleCurve.Evaluate(progress)*_scaleChangeSpeed, _scaleCurve.Evaluate(progress) * _scaleChangeSpeed, _scaleCurve.Evaluate(progress) * _scaleChangeSpeed);
            yield return null;
        }
    }
}
