using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private float _minAnimationDelay;
    [SerializeField] private float _maxAnimationDelay;
    [SerializeField] private Animator _animator;

    private void OnEnable()
    {
        StartCoroutine(SetAnimation());   
    }

    private IEnumerator SetAnimation()
    {
        float delay = Random.Range(_minAnimationDelay, _maxAnimationDelay);

        yield return new WaitForSeconds(delay);

            _animator.Play("Idle");
    }

    public void StopPlayingAnimation()
    {
        _animator.enabled = false;

    }
}