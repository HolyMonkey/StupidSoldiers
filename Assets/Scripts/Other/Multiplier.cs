using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Multiplier : MonoBehaviour
{
    [SerializeField] private int _multiplier;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private ParticleSystem _hitEffect;
    [SerializeField] private AnimationCurve _destroyCurve;

    [SerializeField] private float _scaleDuration;

    private bool _canDestroy = true;
    public int MultiplierValue => _multiplier;

    private void OnEnable()
    {
        _text.text = "x"+_multiplier.ToString();
    }

    public event UnityAction<Multiplier> MultiplierHit;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<Bullet>() & _canDestroy)
        {
            MultiplierHit?.Invoke(this);
            Destroy();
        }
    }

    public void Destroy()
    {
        if (_canDestroy)
        {
            _canDestroy = false;
            Instantiate(_hitEffect,transform);
            StartCoroutine(ChangeScale());
        }
    }

    public void Disable()
    {
        _canDestroy = false;
    }

    private IEnumerator ChangeScale()
    {
        float elapsedTime = 0;

        while (elapsedTime < _scaleDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _scaleDuration;

            transform.localScale = new Vector3(transform.localScale.x * _destroyCurve.Evaluate(progress), transform.localScale.y * _destroyCurve.Evaluate(progress), transform.localScale.z * _destroyCurve.Evaluate(progress));
            yield return null;
        }
        gameObject.SetActive(false);
    }
}