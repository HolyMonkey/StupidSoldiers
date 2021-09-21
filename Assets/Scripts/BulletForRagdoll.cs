using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BulletForRagdoll : MonoBehaviour
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
        StartCoroutine(DestroyAfterDuration());
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            _rigidbody.velocity = transform.right * _speed;
            yield return null;
        }
    }

    private IEnumerator DestroyAfterDuration()
    {
        yield return new WaitForSeconds(_lifeTime);
        StopAllCoroutines();
        Destroy(gameObject);
    }
}
