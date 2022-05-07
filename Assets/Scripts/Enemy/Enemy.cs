using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EnemyAnimator))]
public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _neck;
    [SerializeField] private GameObject _headModel;
    [SerializeField] private GameObject _spineBloodSpawnPoint;
    [SerializeField] private EnemyRagDollModel _ragDoll;
    
    [SerializeField] private ParticleSystem _bodyBloodEffect;
    [SerializeField] private ParticleSystem _coinEffect;
    [SerializeField] private ParticleSystem _bang;

    [SerializeField] private SlowMotionTrigger _headTrigger;

    [SerializeField] private CapsuleCollider _triggerBodyCapsuleCollider;
    [SerializeField] private SphereCollider _triggerHeadSphereCollider;
    [SerializeField] private EnemyOptimization _optimization;

    [SerializeField] private float _slowMotionCooldown;

    [SerializeField] private Game _game;

    private bool _canSlowMotion = true;
    
    private EnemyAnimator _enemyAnimator;

    public event UnityAction Killed;
    
    private void OnEnable()
    {
        _headTrigger.Hit += HeadHit;
        _enemyAnimator = GetComponent<EnemyAnimator>();

        _optimization.Visible += OnVisible;
        _optimization.Invisible += OnInvisible;
    }

    private void OnDisable()
    {
        _headTrigger.Hit -= HeadHit;

        _optimization.Visible += OnVisible;
        _optimization.Invisible += OnInvisible;
    }

    public void ShowBodySplat(EnemyBodySplat splat, Vector3 position)
    {
        if(_canSlowMotion == true)
        {
            var spawnedSplat = Instantiate(splat);
            spawnedSplat.gameObject.transform.SetParent(_ragDoll.GetMainBody().gameObject.transform);
            spawnedSplat.transform.position = position;
            spawnedSplat.SetJoint(_ragDoll.GetMainBody());
            spawnedSplat.SetGravityVelocity(_ragDoll.GetYVelocity());
            StartCoroutine(StartSlowMotionCooldown(_slowMotionCooldown));
            _canSlowMotion = false;
        }      
    }

    private void OnVisible()
    {
        _model.SetActive(true);
        _enemyAnimator.ShowIdleAnimation();
    }

    private void OnInvisible()
    {
        _model.SetActive(false);
        _enemyAnimator.StopShowIdleAnimation();
    }


    private void OnCollisionEnter(Collision other)
    {
        if (!other.gameObject.GetComponent<Bullet>()) 
            return;
        
        SpawnBloodEffect(other.gameObject.transform);
        Death();
    }

    private void Death()
    {
        _triggerBodyCapsuleCollider.enabled = false;
        _triggerHeadSphereCollider.enabled = false;

        ShowBloodEffects();

        _enemyAnimator.StopPlayingAnimation();
        _ragDoll.StartFall();
        Killed?.Invoke();
        //_game.DescreaseEnemyCount();
    }

    private void ShowBloodEffects()
    {
        SpawnEffect(_coinEffect, new Vector3(_spineBloodSpawnPoint.transform.position.x-1, _spineBloodSpawnPoint.transform.position.y, _spineBloodSpawnPoint.transform.position.z));
        SpawnEffect(_bang, new Vector3(_headModel.transform.position.x, _headModel.transform.position.y + 3, _headModel.transform.position.z));
    }

    private void SpawnEffect(ParticleSystem effect, Vector3 position)
    {
        var spawnedEffect = Instantiate(effect);
        spawnedEffect.transform.position = position;
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

    private IEnumerator StartSlowMotionCooldown(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        _canSlowMotion = true;
        StopCoroutine(StartSlowMotionCooldown(cooldown));
    }
}