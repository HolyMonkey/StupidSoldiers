using System.Collections;
using UnityEngine;

[RequireComponent(typeof(EnemyAnimator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _neck;
    [SerializeField] private GameObject _headModel;
    [SerializeField] private GameObject _spineBloodSpawnPoint;
    [SerializeField] private EnemyRagDollModel _ragDoll;

    [SerializeField] private ParticleSystem _spineBloodEffectWall;
    [SerializeField] private ParticleSystem _spineBloodEffectGround;
    [SerializeField] private ParticleSystem _neaskBloodEffect;
    [SerializeField] private ParticleSystem _groundBloodEffect;
    [SerializeField] private ParticleSystem _bodyBloodEffect;
    [SerializeField] private ParticleSystem _groundWallEffect;
    [SerializeField] private ParticleSystem _groundHeadColorSplat;

    [SerializeField] private SlowMotionTrigger _headTrigger;

    [SerializeField] private CapsuleCollider _triggerBodyCapsuleCollider;
    [SerializeField] private SphereCollider _triggerHeadSphereCollider;

    [SerializeField] private Transform _bulletSpawnPoint;

    private EnemyAnimator _enemyAnimator;

    private void OnEnable()
    {
        _headTrigger.Hit += HeadHit;
        _enemyAnimator = GetComponent<EnemyAnimator>();
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
            _ragDoll.SetCollisionWithBulletPoint(other.transform.position);
            Death();
        }
    }

    private void Death()
    {
        _triggerBodyCapsuleCollider.enabled = false;
        _triggerHeadSphereCollider.enabled = false;

        ShowBloodEffects();

        _enemyAnimator.StopPlayingAnimation();
        _ragDoll.StartFall();
        
    }

    private void ShowBloodEffects()
    {
        var groundBlood = Instantiate(_groundBloodEffect);
        groundBlood.transform.position = _spineBloodSpawnPoint.transform.position;

        var spineBloodEffectWall = Instantiate(_spineBloodEffectWall);
        spineBloodEffectWall.transform.position = _spineBloodSpawnPoint.transform.position;

        var spineBloodEffectGround = Instantiate(_spineBloodEffectGround);
        spineBloodEffectGround.transform.position = _spineBloodSpawnPoint.transform.position;

        var groundWallEffect = Instantiate(_groundWallEffect);
        groundWallEffect.transform.position = _spineBloodSpawnPoint.transform.position;
        groundWallEffect.Play();

        var headGroundSplat = Instantiate(_groundHeadColorSplat);
        headGroundSplat.transform.position = _headModel.transform.position;
        StartCoroutine(Follow(headGroundSplat));
    }

    private void HeadHit()
    {
        _headModel.gameObject.transform.localScale = new Vector3( 0.001f, 0.001f, 0.001f);
        _neck.gameObject.transform.localScale = new Vector3( 0.001f, 0.001f, 0.001f);
        
        var heaskBloodEffect = Instantiate(_neaskBloodEffect,_neck.transform);
        Death();
    }

    private void SpawnBloodEffect(Transform hit)
    {
        var hitEffect = Instantiate(_bodyBloodEffect, transform);
        hitEffect.transform.position = hit.position;
    }

    private IEnumerator Follow(ParticleSystem ps)
    {
        float elapsedTime = 0;
        while (elapsedTime < 100)
        {
            ps.transform.position = _headModel.transform.position;
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
}