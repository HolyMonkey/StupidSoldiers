using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _lifeTime;
    [SerializeField] private ParticleSystem _collideWithGroundEffect;
    [SerializeField] private ParticleSystem _collideWithWallEffect;
    [SerializeField] private EnemyBodySplat _bodySplat;
    [SerializeField] private AudioClip _sound;
    [SerializeField] private AudioSource _audioSource;
    

    private Rigidbody _rigidbody;

    private void OnEnable()
    {
        _rigidbody = GetComponent<Rigidbody>();
        StartMove();

        _audioSource.clip = _sound;
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

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckCollideWithTarget(collision))
        {
            _audioSource.clip = _sound;
            _audioSource.Play();
            
            if (collision.gameObject.GetComponent<WallGround>())
            {
                var effect = Instantiate(_collideWithWallEffect);
                effect.transform.position = new Vector3(transform.position.x - 0.15f, transform.position.y, transform.position.z);
            }
            else if (collision.gameObject.GetComponent<Ground>())
            {
                var effect = Instantiate(_collideWithGroundEffect);
                effect.transform.position = new Vector3(transform.position.x, collision.gameObject.GetComponent<Ground>().Height, transform.position.z);
            }

            else if (collision.gameObject.GetComponent<Enemy>() )
            {
                collision.gameObject.GetComponent<Enemy>().ShowBodySplat(_bodySplat, collision.contacts[0].point);
            }
            else if (collision.gameObject.GetComponent<SlowMotionTrigger>())
            {
                collision.gameObject.GetComponent<SlowMotionTrigger>().ShowBodySplat(_bodySplat, collision.contacts[0].point);
            }
        }
        if(!collision.gameObject.GetComponent<Weapon>())
        Destroy(gameObject);
    }

    private bool CheckCollideWithTarget(Collision other)
    {
        if (other.gameObject.GetComponent<Multiplier>() | other.gameObject.GetComponent<Barrier>() | other.gameObject.GetComponent<Multiplier>() | other.gameObject.GetComponent<Enemy>() | other.gameObject.GetComponent<SlowMotionTrigger>() | other.gameObject.GetComponent<Ground>()| other.gameObject.GetComponent<WallGround>())
            return true;

        return false;
    }
}