using System.Collections;
using UnityEngine;

public class EnemyAnimator : MonoBehaviour
{
    [SerializeField] private float _minAnimationDelay;
    [SerializeField] private float _maxAnimationDelay;
    [SerializeField] private Animator _animator;
    private bool _isKilled=false;

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
        _isKilled = true;
        _animator.enabled = false;
    }

    public void ShowIdleAnimation()
    {    
        if (!_isKilled)
        {
        _animator.enabled = true;
        _animator.Play("Idle");
        }
    }
    public void StopShowIdleAnimation()
    {
        _animator.enabled = false;
    }
}