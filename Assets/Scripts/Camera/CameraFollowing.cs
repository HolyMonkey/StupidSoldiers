using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Weapon _target;
    [SerializeField] private float _speedChangeDuration;
    [SerializeField] private float _defaultFollowSpeed;
    [SerializeField] private AnimationCurve _followSpeed;
    [SerializeField] private float _speedChange;

    [SerializeField] private float _effectDuration;
    [SerializeField] private ParticleSystem _endEffect;
    [SerializeField] private Weapon _weapon;

    [SerializeField] private float _slowMotionDuration;
    [SerializeField] private AnimationCurve _slowMotionForce;

    private float _currentSpeed=1;
    private bool _followMode = false;
    private Vector3 _targetDistance;
    private IEnumerator _slowMotion;
    private IEnumerator _normalizeFollowSpeed;


    public event UnityAction EffectEnded;

    private void OnEnable()
    {
        _slowMotion = ShowSlowMotion();
        _normalizeFollowSpeed = NormalizeSpeed();
        _currentSpeed = _defaultFollowSpeed;
        _targetDistance = _target.transform.position - transform.position;
        _target.Shooted += Shoot;
        _target.Hit += OnWeaponHitInTarget;
    }

    private void OnDisable()
    {
        _target.Shooted -= Shoot;
        _target.Hit -= OnWeaponHitInTarget;

    }

    private void Update()
    {
        if (_followMode)
        {
            Vector3 targetPosition = new Vector3(_target.transform.position.x - _targetDistance.x, _target.transform.position.y - _targetDistance.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, _currentSpeed);
        }
    }

    private void OnWeaponHitInTarget()
    {
        _slowMotion = ShowSlowMotion();
        StartCoroutine(_slowMotion);
    }

    private void Shoot()
    {
        _currentSpeed = 0.01f;
        StopCoroutine(_normalizeFollowSpeed);

        _normalizeFollowSpeed = NormalizeSpeed();
        StartCoroutine(_normalizeFollowSpeed);
    }

    private IEnumerator NormalizeSpeed()
    {
        float elapsedTime = 0;
        while(elapsedTime< _speedChangeDuration)
        {
            float progress = elapsedTime / _speedChangeDuration;
            _currentSpeed =  _followSpeed.Evaluate(progress);
            elapsedTime += Time.deltaTime;
            yield return null;
        }        
    }

    private IEnumerator ShowSlowMotion()
    {
        float elapsedTime = 0;

        while (elapsedTime < _slowMotionDuration)
        {
            elapsedTime += Time.deltaTime;
            Time.timeScale = _slowMotionForce.Evaluate(elapsedTime / _slowMotionDuration);
            yield return null;
        }
        Time.timeScale = 1;
    }

    public void StartFollow()
    {
        _followMode = true;
    }
 
    public void ShowResultEffect()
    {
        _endEffect.gameObject.SetActive(true);
        _endEffect.Play();
    }
}