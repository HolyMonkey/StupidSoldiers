using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody))]
public abstract class Weapon : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootSound;
    [SerializeField] private Animator _animator;
    [SerializeField] private Bullet _bulletTemplate;
    [SerializeField] private Transform _bulletSpawnPoint;
    [SerializeField] private Transform _recoilDirectionPoint;
    [SerializeField] private LineRenderer _lineRenderer;
    [SerializeField] private ParticleSystem _shootEffect;

    [SerializeField] private ParticleSystem _hitEffect;

    [SerializeField] private float _minRotationSpeed;
    [SerializeField] private float _maxRotationSpeed;
    [SerializeField] private float _fallRotationSpeed;
    [SerializeField] private float _speedReducedRotation;

    [SerializeField] private float _shootDelay;
    [SerializeField] private float _jumpForce;
    [SerializeField] private AnimationCurve _gravityCurve;
    [SerializeField] private float _fallDuration;
    [SerializeField] private float _recoilForceDuration;
    [SerializeField] private AnimationCurve _recoilForceCurve;
    [SerializeField] private float _recoilMoveSpeed;
    [SerializeField] private float _gravityFallSpeed;

    [SerializeField] private float _speedChangeY;
    [SerializeField] private float _speedChangeRotationToZero;
    


    private float _modifireRotation = 1;
    private float _currentMoveXSpeed=0;
    private float _currentMoveYSpeed=0;
    private float _currentRotationSpeed;
    private Rigidbody _rigidbody;

    private bool _touchedOfGround = false;
    private bool _canShoot = true;
    private Vector3 _moveTarget;

    private IEnumerator _changeSpeedAfterShoot;
    private IEnumerator _normalizeGravityAfterShoot;
    private IEnumerator _rotationToZero;
    private IEnumerator _shootDelayTime;
    private IEnumerator _jumpOnGround;

    private const float _laserRenderDistance = 50;

    public event UnityAction GameOver;
    public event UnityAction Collided;
    public event UnityAction Shooted;
    public event UnityAction Hit;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _audioSource.clip = _shootSound;
        _shootEffect.gameObject.SetActive(true);
        _currentMoveYSpeed = _gravityFallSpeed;

        _normalizeGravityAfterShoot = NormalizeGravity();
        _changeSpeedAfterShoot = ChangeSpeedAfterShoot();
        _rotationToZero =SlowRotationToZero();
        _shootDelayTime = ShootDelayTime();
        _jumpOnGround = JumpOnGround();
    }

    private void Update()
    {
         LaserRendering();

        if (Input.GetKeyDown(KeyCode.Space) & _canShoot)
            Shoot();

            Rotate();
            Move();

    }

    private void Rotate()
    {
        transform.Rotate(0,0,_currentRotationSpeed* -_modifireRotation*Time.deltaTime);
    }

    private void LaserRendering()
    {
        _lineRenderer.SetPosition(0, _bulletSpawnPoint.position);
        
        if (Physics.Raycast(new Ray(_bulletSpawnPoint.position, transform.right), out RaycastHit hit))
        {
            _lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            _lineRenderer.SetPosition(1, _bulletSpawnPoint.position + transform.right *_laserRenderDistance);
        }       
    }
    private void SpawnButtlet()
    {
        var bullet = Instantiate(_bulletTemplate, _bulletSpawnPoint.position, _bulletSpawnPoint.rotation);
    }

    private void ShowShootEffect()
    {

        //_shootEffect.Play();
        _audioSource.Play();
        _animator.Play("Shoot");
    }

    private void ChangeRotateSpeed()
    {
        _currentRotationSpeed = Random.Range(_minRotationSpeed, _maxRotationSpeed);
        StopCoroutine(NormalizeRotationSpeed());
        StartCoroutine(NormalizeRotationSpeed());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Ground>())
        {
            _touchedOfGround = true; 
            Touch();
        }
    }
    private void ReversRotateDirection()
    {
        _modifireRotation = -_modifireRotation;        
    }
    private IEnumerator NormalizeRotationSpeed()
    {
        while (_currentRotationSpeed > _fallRotationSpeed)
        {
            _currentRotationSpeed -= _speedReducedRotation * Time.deltaTime;
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

        ChangeRotateSpeed();
        ReversRotateDirection();

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
        _rigidbody.velocity = new Vector3(_currentMoveXSpeed, _currentMoveYSpeed, 0);
    }

    private Vector3 GetRecoilDirection()
    {
        return _recoilDirectionPoint.position - transform.position;
    }

    private void Jump()
    {
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
        RaycastHit hit;
        Ray ray = new Ray(transform.position,transform.right);
        Physics.Raycast(ray, out hit,100);

        if (hit.collider != null)
        {            
            if (hit.collider.gameObject.GetComponent<SlowMotionTrigger>())            
                return true;
            if (hit.collider.gameObject.GetComponent<Multiplier>())
                return true;
        }
        return false;
    }

    private IEnumerator ShootDelayTime()
    {
        yield return new WaitForSeconds(_shootDelay);
        _canShoot = true;
    }

    private IEnumerator JumpOnGround()
    {
        _currentMoveYSpeed = 3;
        yield return new WaitForSeconds(1.5f);
        _currentMoveYSpeed = _gravityFallSpeed;
        _touchedOfGround = false;
        yield return null;
    }

    private IEnumerator ChangeSpeedAfterShoot()
    {
        StartCoroutine(_normalizeGravityAfterShoot);
        float elapsedTime = 0;
        while(elapsedTime < _recoilForceDuration)
        {
            yield return null;
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / _recoilForceDuration;

            _currentMoveXSpeed = _moveTarget.x * _recoilForceCurve.Evaluate(progress)* _recoilMoveSpeed;
            _currentMoveYSpeed = _moveTarget.y * _recoilForceCurve.Evaluate(progress) * _recoilMoveSpeed + _gravityCurve.Evaluate(progress) * _gravityFallSpeed;
        }
    }

    private IEnumerator NormalizeGravity()
    {
        while(_currentMoveYSpeed< _gravityFallSpeed | _currentMoveYSpeed > _gravityFallSpeed)
        {
            _currentMoveYSpeed = Mathf.MoveTowards(_currentMoveYSpeed, _gravityFallSpeed, _speedChangeY);
            yield return null;
        }
    }

    private IEnumerator SlowRotationToZero()
    {
        while (_currentRotationSpeed>0)
        {
            _currentRotationSpeed = Mathf.MoveTowards(_currentRotationSpeed, 0, _speedChangeRotationToZero);
            _currentMoveYSpeed = -1;
            _currentMoveXSpeed = 0;
            yield return null;
        }
            _currentMoveYSpeed = 0;
    }

    public void JumpBack()
    {
        _rigidbody.AddForce(Vector3.up * 1000);
        _rigidbody.AddForce(Vector3.back * 1000);
        _hitEffect.gameObject.SetActive(true);
        _hitEffect.Play();
    }

    public void DisableShooting()
    {
        _canShoot = false;
    }
}