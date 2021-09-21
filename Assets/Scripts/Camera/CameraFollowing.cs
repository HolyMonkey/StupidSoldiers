using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Weapon _target;

    [SerializeField] private float _delayAfterShoot;
    [SerializeField] private float _smoothSpeed;
    [SerializeField] private float _smoothSpeedRemoved;

    [SerializeField] private float _slowMotionDuration;
    [SerializeField] private AnimationCurve _slowMotionCurve;

    private Vector3 _offset;
    private IEnumerator _slowMotion;
    private IEnumerator _shootDelay;

    private const float FixedDeltaTimeMultiplier = 0.02f;
    private const float DefaultSmoothSpeed = 0.125f;

    private void WaitShootDelay()
    {
        StopCoroutine(_shootDelay);
        _shootDelay = ShootDelay();
        StartCoroutine(_shootDelay);
    }

    private void OnEnable()
    {
        _slowMotion = ShowSlowMotion();
        _offset = _target.transform.position - transform.position;

        _target.Shooted += WaitShootDelay;
        _target.Hit += OnBulletHitSlowMotionTarget;

        _shootDelay = ShootDelay();
    }

    private void OnDisable()
    {
        _target.Hit -= OnBulletHitSlowMotionTarget;
        _target.Shooted -= WaitShootDelay;
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = _target.transform.position - _offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed);
    }

    private void OnBulletHitSlowMotionTarget()
    {
        StopCoroutine(_slowMotion);
        _slowMotion = ShowSlowMotion();
        StartCoroutine(_slowMotion);
    }

    private IEnumerator ShowSlowMotion()
    {
        float elapsedTime = 0;

        while (elapsedTime < _slowMotionDuration)
        {
            elapsedTime += Time.deltaTime;
            Time.timeScale = _slowMotionCurve.Evaluate(elapsedTime / _slowMotionDuration);
            Time.fixedDeltaTime = Time.timeScale * FixedDeltaTimeMultiplier;
            yield return null;
        }
        Time.timeScale = 1;
    }

    private IEnumerator ShootDelay()
    {
        while (_smoothSpeed > 0)
        {
            _smoothSpeed = Mathf.MoveTowards(_smoothSpeed, 0, _smoothSpeedRemoved);
            yield return null;
        }

        yield return new WaitForSeconds(_delayAfterShoot);

        while (_smoothSpeed < DefaultSmoothSpeed)
        {
            _smoothSpeed = Mathf.MoveTowards(_smoothSpeed, DefaultSmoothSpeed, _smoothSpeedRemoved);
            yield return null;
        }
        _smoothSpeed = DefaultSmoothSpeed;
    }
}