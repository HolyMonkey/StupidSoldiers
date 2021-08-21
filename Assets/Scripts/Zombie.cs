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

    [SerializeField] private ParticleSystem _bodyBloodEffect;
    [SerializeField] private ParticleSystem _headShotBloodEffect;
    [SerializeField] private ParticleSystem _groundBloodEffect;
    [SerializeField] private ParticleSystem _spineBloodEffect;
    [SerializeField] private SlowMotionTrigger _headTrigger;
    [SerializeField] private Animator _animator;

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

            TryDeath();
        }
    }

    private void TryDeath()
    {
        if (_canDeath)
        {
            var groundBlood = Instantiate(_groundBloodEffect);
            groundBlood.transform.position = _groundBloodSpawnPoint.transform.position;
            var spineBloodEffect = Instantiate(_spineBloodEffect);
            spineBloodEffect.transform.position = _spineBloodSpawnPoint.transform.position;                

            _canDeath = false;

            _defaultModel.SetActive(false);
            _ragDollModel.SetActive(true);


            _triggerBodyCapsuleCollider.enabled = false;
            _triggerHeadSphereCollider.enabled = false;
        }
    }

    private void HeadHit()
    {
        _zombiesHead.gameObject.transform.localScale = new Vector3( 0.001f, 0.001f, 0.001f);
        var pieceOfTheBrainEffect = Instantiate(_headShotBloodEffect);
        pieceOfTheBrainEffect.transform.position = _nesk.transform.position;
        TryDeath();
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