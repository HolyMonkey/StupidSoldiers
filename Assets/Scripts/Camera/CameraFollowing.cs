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
    [SerializeField] private AnimationCurve _slowMotionForce;

    private Vector3 _offset;
    private IEnumerator _slowMotion;
    private IEnumerator _shootDelay;


    public event UnityAction EffectEnded;


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
        _target.Hit += OnWeaponHitInTarget;

        _shootDelay = ShootDelay();
    }

    private void OnDisable()
    {
        _target.Hit -= OnWeaponHitInTarget;
        _target.Shooted -= WaitShootDelay;
    }

    private void FixedUpdate()
    {
        Vector3 desiredPosition = _target.transform.position - _offset;

        transform.position = Vector3.Lerp(transform.position, desiredPosition,_smoothSpeed);
    }

    private void OnWeaponHitInTarget()
    {
        _slowMotion = ShowSlowMotion();
        StartCoroutine(_slowMotion);
    }

    private IEnumerator ShowSlowMotion()
    {
        float elapsedTime = 0;

        while (elapsedTime < _slowMotionDuration)
        {
            elapsedTime += Time.deltaTime;
            Time.timeScale = _slowMotionForce.Evaluate(elapsedTime / _slowMotionDuration);
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
            yield return null;
        }
        Time.timeScale = 1;
    }
    private IEnumerator ShootDelay()
    {

        _smoothSpeed = 0;
        yield return new WaitForSeconds(_delayAfterShoot);

        while (_smoothSpeed < 0.125f)
        {
            _smoothSpeed = Mathf.MoveTowards(_smoothSpeed, 0.125f, _smoothSpeedRemoved);
            yield return null;
        }
        _smoothSpeed = 0.125f;
    }
}