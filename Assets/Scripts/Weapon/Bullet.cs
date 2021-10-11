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

    private void OnCollisionEnter(Collision collision)
    {
        if (CheckCollideWithTarget(collision))
        {
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
                Enemy enemy = collision.gameObject.GetComponent<Enemy>();
                enemy.SetCollidePosition(collision.contacts[0].point);

                var splat = Instantiate(_bodySplat);
                splat.transform.position = enemy.GetModelTransform().position;
                splat.transform.SetParent(enemy.GetModelTransform());
                splat.StartFollow(enemy.GetModelTransform(), enemy.GetModelRigidbody() );



            }
            else if (collision.gameObject.GetComponent<SlowMotionTrigger>())
            {
                SlowMotionTrigger slowMotionTrigger = collision.gameObject.GetComponent<SlowMotionTrigger>();
                slowMotionTrigger.SetCollidePosition();

                var splat = Instantiate(_bodySplat);
                splat.transform.SetParent(slowMotionTrigger.GetModelTransform());
                splat.transform.position = slowMotionTrigger.GetModelTransform().position;
                splat.StartFollow(slowMotionTrigger.GetModelTransform(), slowMotionTrigger.GetRigidbody());

                splat.transform.rotation = new Quaternion(0, 0, -90, 0);
            }
            
        }
        if(!collision.gameObject.GetComponent<Weapon>())
        Destroy(gameObject);

    }
    

    private bool CheckCollideWithTarget(Collision other)
    {
        if (other.gameObject.GetComponent<Barrier>() | other.gameObject.GetComponent<Multiplier>() | other.gameObject.GetComponent<Enemy>() | other.gameObject.GetComponent<SlowMotionTrigger>() | other.gameObject.GetComponent<Ground>()| other.gameObject.GetComponent<WallGround>())
            return true;

        return false;
    }

}