using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private ParticleSystem _collideWithGroundEffect;
    [SerializeField] private ParticleSystem _collideWithWallEffect;

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
        {
            Destroy(gameObject);

            if (other.GetComponent<WallGround>())
            {
                var effect = Instantiate(_collideWithWallEffect);
                effect.transform.position = new Vector3(transform.position.x, transform.position.y + 0.15f, transform.position.z);
            }
            else if (other.GetComponent<Ground>())
            {
                var effect = Instantiate(_collideWithGroundEffect);
                effect.transform.position = new Vector3(transform.position.x, transform.position.y+0.23f, transform.position.z);
            }

        }
    }

    private bool CheckCollideWithTarget(Collider other)
    {
        if (other.GetComponent<Barrier>() | other.GetComponent<Multiplier>() | other.GetComponent<Enemy>() | other.GetComponent<SlowMotionTrigger>() | other.GetComponent<Ground>()| other.GetComponent<WallGround>())
            return true;

        return false;
    }

}