using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
	[SerializeField] private Rigidbody _rigidBody;
    [SerializeField] private BoxCollider _collider;
    [SerializeField] private ParticleSystem _partical;
    [SerializeField] private Transform _bulletPoint;
    [SerializeField] private GameObject _trailTemplate;

    [SerializeField] private Transform _shellPoint;
    [SerializeField] private GameObject _shellTemplate;
    [SerializeField] private GameObject _bloodTemplate;

    [SerializeField] private StressReceiver _shaker;
    [SerializeField] private AudioShootPlayer _audioPlayer;

    [SerializeField] private float _modSpeed = 1;
    private float _rotationSpeed = 170;
    private float _rotationMin = 170;
    private float _rotationMax = 210;
    private float _rotationChangeChance = 20;

    private int _shotForce = 80;
    private Transform _target;

    private void Start()
    {
        _rigidBody.AddForce(-transform.up * _shotForce);
    }


    private void Update()
    {
        if (_target)
        {
            transform.position = Vector3.Slerp(transform.position, _target.position, 10*Time.deltaTime);
            transform.rotation = Quaternion.Slerp(transform.rotation, _target.rotation, 10 * Time.deltaTime);
        } 
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                Shoot();
            }

            transform.Rotate(Vector3.right * _rotationSpeed * _modSpeed * Time.deltaTime);
        }
        
    }

    private void Shoot()
    {
        var trail = Instantiate(_trailTemplate, _bulletPoint.transform.position, Quaternion.identity);
        trail.transform.forward = transform.up;
        trail.GetComponent<Rigidbody>().AddForce(trail.transform.forward * 2000);

        var shell = Instantiate(_shellTemplate, _shellPoint.position, Quaternion.identity);       

        _rigidBody.AddForce(-transform.up * _shotForce * 1.5f);
        StartCoroutine(DashWeapon());

        _partical.Play();

        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.up);
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform.TryGetComponent(out Aim aim))
            {
                aim.Hit();
            }
            if (hit.transform.GetComponent<Player>())
            {
                var blood = Instantiate(_bloodTemplate, hit.point, Quaternion.identity);
                blood.transform.forward = transform.up;
                hit.transform.GetComponent<Emojies>().PlaySad();
            }
        }
        _rotationSpeed = Random.Range(_rotationMin, _rotationMax);
        if (Random.Range(0, 100) < _rotationChangeChance)
        {
            _rotationSpeed *= -1;
        }

        _shaker.InduceStress(0.1f);
        _audioPlayer.PlaySound();
    }

    private IEnumerator DashWeapon()
    {
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            _rigidBody.AddForce(transform.up * _shotForce / 3 * 0.5f);
        }
    }

    public void SetTargetPoint(Transform target)
    {
        _target = target;
    }


    public void SetKinematic()
    {
        _rigidBody.isKinematic = true;        
    }

    public void SetNonKinematic()
    {
        _rigidBody.isKinematic = false;
    }


    public void Throw(Vector3 force, float angularSpeed)
    {
        _target = null;
        _rigidBody.AddForce(force);
        StartCoroutine(DisactivateColliderAtTime());
    }

    private IEnumerator DisactivateColliderAtTime()
    {
        _collider.enabled = false;
        yield return new WaitForSeconds(0.9f);
        _collider.enabled = true;
    }
}

