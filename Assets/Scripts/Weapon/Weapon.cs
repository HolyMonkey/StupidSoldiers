using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerInput))]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private Animator _animator;
    [SerializeField] private Bullet _bulletTemplate;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Transform _recoilDirectionPoint;

    [SerializeField] private float _minRotationSpeed;
    [SerializeField] private float _maxRotationSpeed;
    [SerializeField] private float _speedRotationAfterRecoil;
    [SerializeField] private float _speedChangeRotationToZeroAfterGrounded;

    [SerializeField] private float _delayBeforeChangeRotateDirection;

    [SerializeField] private float _fireLineLiveDuration;
    [SerializeField] private ParticleSystem _fireLine;
    [SerializeField] private ShootEffect _fireEffect;


    [SerializeField] private float _invulnerabilityDurationAfterTouchGround;

    [SerializeField] private float _shootDelay;
    [SerializeField] private float _jumpFromTheGroundForce;
    [SerializeField] private AnimationCurve _gravityAfterShootCurve;
    [SerializeField] private float _recoilMoveDuration;
    [SerializeField] private AnimationCurve _recoilForceCurve;
    [SerializeField] private float _recoilMoveSpeed;
    [SerializeField] private float _gravityForce;
    [SerializeField] private float _speedNormalizeGravity;

    
    private float _modifireRotation = 1;
    private float _currentMoveXSpeed=0;
    private float _currentMoveYSpeed=0;
    private float _currentRotationSpeed;

    private float _rotateSpeedMultiplier=2f;
    private float _moveSpeedMultplier=2f;


    private Vector3 _moveTarget;
    private Rigidbody _rigidbody;
    private ParticleSystem _spawnedFireLine;

    private WeaponAnimator _weaponAnimator;
    private PlayerInput _input;

    private bool _touchedOfGround = false;
    private bool _isInvulnerability = false;
    private bool _canShoot = false;

    private IEnumerator _changeSpeedAfterShoot;
    private IEnumerator _normalizeGravityAfterShoot;
    private IEnumerator _rotationToZero;
    private IEnumerator _shootDelayTime;
    private IEnumerator _jumpOnGround;
    private IEnumerator _invulnerabilityAfterTouchGround;
    private IEnumerator _rotateSpeedNormalization;

    public event UnityAction GameOver;
    public event UnityAction Collided;
    public event UnityAction Shooted;
    public event UnityAction Hit;
    public event UnityAction Death;

    private void Start()
    {

        _audioSource.clip = _shootSound;
        _currentMoveYSpeed = _gravityForce;

        _normalizeGravityAfterShoot = NormalizeGravity();
        _changeSpeedAfterShoot = ChangeSpeedAfterShoot();
        _rotationToZero =SlowRotationToZero();
        _shootDelayTime = ShootDelayTime();
        _jumpOnGround = JumpOnGround();
        _rotateSpeedNormalization = NormalizeRotationSpeed();
        _invulnerabilityAfterTouchGround = InvulnerabilityAfterTouchGround();
    }

    private void OnEnable()
    {
        _input = GetComponent<PlayerInput>();
        _weaponAnimator = GetComponent<WeaponAnimator>();
        _rigidbody = GetComponent<Rigidbody>();

        _input.Touch += OnTouch;
    }

    private void OnDisable()
    {
        _input.Touch -= OnTouch;
    }

    private void Update()
    {
        Rotate();
        Move();
    }

    private void OnTouch()
    {
        if (_canShoot)
            Shoot();
    }

    private void Rotate()
    {
        transform.Rotate(0,0,_currentRotationSpeed* -_modifireRotation*Time.deltaTime);
    }

    private void SpawnButtlet()
    {
        var bullet = Instantiate(_bulletTemplate, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
    }

    private void ShowShootEffect()
    {
        _spawnedFireLine = Instantiate(_fireLine, _bulletSpawnPoint.transform);
        _spawnedFireLine.Play();
        _spawnedFireLine.startLifetime = _fireLineLiveDuration;

        Instantiate(_fireEffect,_bulletSpawnPoint);

        _audioSource.Play();
        _animator.Play("Shoot");

        _weaponAnimator.Shoot();
    }

    private void ChangeRotateSpeed()
    {
        _currentRotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);
        StopCoroutine(_rotateSpeedNormalization);
        _rotateSpeedNormalization = NormalizeRotationSpeed();
        StartCoroutine(_rotateSpeedNormalization);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ground>() & !_isInvulnerability)
        {
            _isInvulnerability = true;
            _touchedOfGround = true;
            Touch();
            _animator.Play("Touch");
        }
        else if (other.GetComponent<DeathZone>())
            Death?.Invoke();
    }
    private void ReversRotateDirection()
    {
        _modifireRotation = -_modifireRotation;        
    }

    private IEnumerator NormalizeRotationSpeed()
    {
        while (_currentRotationSpeed > _speedRotationAfterRecoil)
        {
            _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, _speedRotationAfterRecoil, 0.004f) ;
            yield return null;
        }
    }

    private void Shoot()
    {

        _canShoot = false;
        _shootDelayTime = ShootDelayTime();
        StartCoroutine(_shootDelayTime);

        if (_touchedOfGround)
            Jump();

        Shooted?.Invoke();

        ShowShootEffect();
        SpawnButtlet();

        StartCoroutine(ChangeRotateDirection());
        ChangeRotateSpeed();

        _moveTarget = GetRecoilDirection();

        StopCoroutine(_normalizeGravityAfterShoot);
        StopCoroutine(_changeSpeedAfterShoot);
        _normalizeGravityAfterShoot = NormalizeGravity();
        _changeSpeedAfterShoot = ChangeSpeedAfterShoot();
        StartCoroutine(_changeSpeedAfterShoot);

        bool hit = CheckHit();

        if (hit)
            Hit?.Invoke();
    }       

    private void Move()
    {
        _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity,new Vector3(_currentMoveXSpeed, _currentMoveYSpeed, 0), 0.25f);
    }

    private Vector3 GetRecoilDirection()
    {
        return _recoilDirectionPoint.position - transform.position;
    }

    private void Jump()
    {
        _invulnerabilityAfterTouchGround = InvulnerabilityAfterTouchGround();
        StartCoroutine(_invulnerabilityAfterTouchGround);

        _jumpOnGround = JumpOnGround();
        StartCoroutine(_jumpOnGround);
        _touchedOfGround = false;
        StopCoroutine(_rotationToZero);
        _rotationToZero = SlowRotationToZero();
    }

    private void Touch()
    {
        StopCoroutine(_normalizeGravityAfterShoot);
        StopCoroutine(_changeSpeedAfterShoot);
        _rotationToZero = SlowRotationToZero();
        StartCoroutine(_rotationToZero);
    }

    private bool CheckHit()
    {
        RaycastHit[] hit;
        Ray ray = new Ray(_bulletSpawnPoint.transform.position,transform.right);        
        hit = Physics.RaycastAll(ray,25);

        if (hit != null)
        {
            foreach (var collider in hit)
            {
                if (collider.collider.gameObject.GetComponent<SlowMotionTrigger>()| collider.collider.gameObject.GetComponent<Multiplier>())
                    return true;
            }
        }
        return false;
    }

    private IEnumerator ChangeRotateDirection()
    {
        yield return new WaitForSeconds(_delayBeforeChangeRotateDirection);

        ReversRotateDirection();
    }

    private IEnumerator InvulnerabilityAfterTouchGround()
    {
        yield return new WaitForSeconds(_invulnerabilityDurationAfterTouchGround);
        _isInvulnerability = false;
    }

    private IEnumerator ShootDelayTime()
    {
        yield return new WaitForSeconds(_shootDelay);
        _canShoot = true;
    }

    private IEnumerator JumpOnGround()
    {
        _rigidbody.AddForce(Vector3.up* _jumpFromTheGroundForce);
        yield return new WaitForSeconds(1.5f);
        _currentMoveYSpeed = _gravityForce;
        yield return null;
    }

    private IEnumerator ChangeSpeedAfterShoot()
    {
        StartCoroutine(_normalizeGravityAfterShoot);
        float elapsedTime = 0;
        while (elapsedTime < _recoilMoveDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _recoilMoveDuration;

            _currentMoveXSpeed = _moveTarget.x * _recoilForceCurve.Evaluate(progress) * _recoilMoveSpeed;
            _currentMoveYSpeed = _moveTarget.y * _recoilForceCurve.Evaluate(progress) * _recoilMoveSpeed + _gravityAfterShootCurve.Evaluate(progress) * _gravityForce;
        }
    }

    private IEnumerator NormalizeGravity()
    {
        while(_currentMoveYSpeed< _gravityForce | _currentMoveYSpeed > _gravityForce)
        {
            _currentMoveYSpeed = Mathf.Lerp(_currentMoveYSpeed, _gravityForce, _speedNormalizeGravity);
            yield return null;
        }
    }

    private IEnumerator SlowRotationToZero()
    {
        while (_currentRotationSpeed>0)
        {
            _currentRotationSpeed = Mathf.Lerp(_currentRotationSpeed, 0, _speedChangeRotationToZeroAfterGrounded);
            _currentMoveYSpeed = -1;
            _currentMoveXSpeed = 0;
            yield return null;
        }
            _currentMoveYSpeed = 0;
    }

    public void CanShoot()
    {
        _canShoot = true;
    }
    public void DisableShoot()
    {
        _canShoot = false;
    }
}