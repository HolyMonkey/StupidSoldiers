using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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

    [SerializeField] private ParticleSystem _bang;

    [SerializeField] private SlowMotionTrigger _headTrigger;

    [SerializeField] private CapsuleCollider _triggerBodyCapsuleCollider;
    [SerializeField] private SphereCollider _triggerHeadSphereCollider;

    private EnemyAnimator _enemyAnimator;

    public event UnityAction Killed;

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
        Killed?.Invoke();
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

        var bang= Instantiate(_bang);
        bang.transform.position = new Vector3(_headModel.transform.position.x, _headModel.transform.position.y+3, _headModel.transform.position.z);
    }

    private void HeadHit()
    {        

        StartCoroutine(DestroyHead());
        Death();
    }

    private void SpawnBloodEffect(Transform hit)
    {
        var hitEffect = Instantiate(_bodyBloodEffect, transform);
        hitEffect.transform.position = hit.position;
    }
    
    private IEnumerator DestroyHead()
    {
        float duration = 0.1f;
        float elapsedTime = 0;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            _headModel.transform.localScale = Vector3.Lerp(_headModel.transform.localScale, new Vector3(1 - elapsedTime / duration , 1 - elapsedTime / duration , 1 - elapsedTime / duration),0.009f);
            yield return null;
        }
        _headModel.transform.localScale = new Vector3(0, 0, 0);
        _neck.transform.localScale = new Vector3(0, 0, 0);
    }
}