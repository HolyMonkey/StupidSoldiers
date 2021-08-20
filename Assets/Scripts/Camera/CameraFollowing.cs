using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Weapon _target;
    [SerializeField] private float _durationOfSpeedNormalization;
    [SerializeField] private AnimationCurve _speed“ormalization—urve;
    [SerializeField] private float _speedAfterShoot;
    [SerializeField] private float _delayAfterShoot;

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
        _currentSpeed = 1;
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
        _currentSpeed = _speedAfterShoot;

        StopCoroutine(_normalizeFollowSpeed);
        _normalizeFollowSpeed = NormalizeSpeed();
        StartCoroutine(_normalizeFollowSpeed);
    }

    private IEnumerator NormalizeSpeed()
    {
        yield return new WaitForSeconds(_delayAfterShoot);

        float elapsedTime = 0;
        while(elapsedTime< _durationOfSpeedNormalization)
        {
            float progress = elapsedTime / _durationOfSpeedNormalization;
            _currentSpeed =  Mathf.Lerp(_currentSpeed,_speed“ormalization—urve.Evaluate(progress),0.01f );
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
 
}