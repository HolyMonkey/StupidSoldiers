    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : MonoBehaviour
{
    [SerializeField] private GameObject _defaultModel;
    [SerializeField] private GameObject _ragDollModel;
    [SerializeField] private GameObject _nesk;
    [SerializeField] private GameObject _zombiesHead;
    [SerializeField] private GameObject _groundBloodSpawnPoint;
    [SerializeField] private GameObject _spineBloodSpawnPoint;
    
    [SerializeField] private ParticleSystem _spineBloodEffectWall;
    [SerializeField] private ParticleSystem _spineBloodEffectGround;
    [SerializeField] private ParticleSystem _neaskBloodEffect;
    [SerializeField] private ParticleSystem _groundBloodEffect;
    [SerializeField] private ParticleSystem _headShotBloodEffect;
    [SerializeField] private ParticleSystem _bodyBloodEffect;
    [SerializeField] private ParticleSystem _groundWallEffect;
    [SerializeField] private SlowMotionTrigger _headTrigger;
    [SerializeField] private Animator _animator;

    [SerializeField] private Transform _bulletCopySpawnPoint;
    [SerializeField] private BulletCopy _bulletTemplate;


    [SerializeField] private CapsuleCollider _triggerBodyCapsuleCollider;
    [SerializeField] private SphereCollider _triggerHeadSphereCollider;

    private bool _canDeath = true;

    private void OnEnable()
    {
        SetRandomIdleAnimation();
        _headTrigger.Hit += HeadHit;
    }

    private void OnDisable()
    {
        _headTrigger.Hit -= HeadHit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>())
        {
            SpawnBloodEffect(other.gameObject.transform);

            Death();
        }
    }

    private void Death()
    {
        _triggerBodyCapsuleCollider.enabled = false;
        _triggerHeadSphereCollider.enabled = false;

        var groundBlood = Instantiate(_groundBloodEffect);
         groundBlood.transform.position = _spineBloodSpawnPoint.transform.position;

        var spineBloodEffectWall = Instantiate(_spineBloodEffectWall);
        spineBloodEffectWall.transform.position = _spineBloodSpawnPoint.transform.position;

        var spineBloodEffectGround = Instantiate(_spineBloodEffectGround);
        spineBloodEffectGround.transform.position = _spineBloodSpawnPoint.transform.position;

        var groundWallEffect = Instantiate(_groundWallEffect);
        groundWallEffect.transform.position = _spineBloodSpawnPoint.transform.position;
        groundWallEffect.Play();

        _canDeath = false;

        _defaultModel.SetActive(false);
        _ragDollModel.SetActive(true);

        var bullet = Instantiate(_bulletTemplate);
        bullet.transform.position = _bulletCopySpawnPoint.transform.position;
    }

    private void HeadHit()
    {
        _zombiesHead.gameObject.transform.localScale = new Vector3( 0.001f, 0.001f, 0.001f);
        //var pieceOfTheBrainEffect = Instantiate(_headShotBloodEffect);
        //pieceOfTheBrainEffect.transform.position = _nesk.transform.position;
        
        var heaskBloodEffect = Instantiate(_neaskBloodEffect,_nesk.transform);
        Death();
    }

    private void SetRandomIdleAnimation()
    {
        int indexAnimation = Random.Range(0, 2);

        if(indexAnimation==0)
            _animator.Play("Idle1");
        if(indexAnimation==1)
            _animator.Play("Idle3");
        else
            _animator.Play("Idle1");
    }

    private void SpawnBloodEffect(Transform hit)
    {
        var hitEffect = Instantiate(_bodyBloodEffect, _ragDollModel.transform);
        hitEffect.transform.position = _triggerBodyCapsuleCollider.transform.position;
    }

}