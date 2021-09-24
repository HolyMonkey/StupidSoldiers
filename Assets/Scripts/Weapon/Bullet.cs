using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;

    private Rigidbody _rigidbody;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        StartMove();
    }

    private void StartMove()
    {
        _rigidbody.velocity = transform.right * _speed;
        StartCoroutine(DestroyAfterDuration());
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(_lifeTime);
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckCollideWithTarget(other))
            Destroy(gameObject);
    }

    private bool CheckCollideWithTarget(Collider other)
    {
        if (other.GetComponent<Barrier>() | other.GetComponent<Multiplier>() | other.GetComponent<Enemy>() | other.GetComponent<SlowMotionTrigger>() | other.GetComponent<Ground>())
            return true;

        return false;
    }
}