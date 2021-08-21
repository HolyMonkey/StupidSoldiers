using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CameraFollowing : MonoBehaviour
{
    [SerializeField] private Weapon _target;

    [SerializeField] private float _delayAfterShoot;
    [SerializeField] private float _smoothSpeed;

    [SerializeField] private float _slowMotionDuration;
    [SerializeField] private AnimationCurve _slowMotionForce;

    private Vector3 _offset;
    private IEnumerator _slowMotion;


    public event UnityAction EffectEnded;

    private void OnEnable()
    {
        _slowMotion = ShowSlowMotion();
        _offset = _target.transform.position - transform.position;

        _target.Hit += OnWeaponHitInTarget;
    }

    private void OnDisable()
    {
        _target.Hit -= OnWeaponHitInTarget;

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
}