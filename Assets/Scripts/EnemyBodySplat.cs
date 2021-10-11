using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBodySplat : MonoBehaviour
{
    [SerializeField] private CharacterJoint _joint;
    [SerializeField] private float _heitghtDuration;
    [SerializeField] private float _wigthDuration;
    [SerializeField] private float _heitghtSpeed;
    [SerializeField] private float _wigthSpeed;
    [SerializeField] private GameObject[] _rays;
    [SerializeField] private GameObject _main;

    [SerializeField] private AnimationCurve _wigthCurve;
    [SerializeField] private AnimationCurve _heightCurve;



    private Transform _spawnPoint;

    public void StartFollow(Transform spawnPoint,Rigidbody enemy)
    {
        _spawnPoint = spawnPoint;
        _joint.connectedBody = enemy;
    }

    private void OnEnable()
    {
        StartCoroutine(Weight());
        StartCoroutine(Gravity());
        StartCoroutine(Height());

    }

    private IEnumerator Height()
    {
        float elapsedTime = 0;
        while (elapsedTime < _heitghtDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = _heitghtDuration / elapsedTime;

            foreach (var ray in _rays)
            {
                ray.transform.localScale = new Vector3(ray.transform.localScale.x+ _heightCurve.Evaluate(progress) * Time.deltaTime * _heitghtSpeed/2, ray.transform.localScale.y + _heightCurve.Evaluate(progress) * Time.deltaTime * _heitghtSpeed, ray.transform.localScale.z);
            }
            yield return null;
        }
    }

    private IEnumerator Weight()
    {
        float elapsedTime = 0;
        while (elapsedTime < _wigthDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = _wigthDuration / elapsedTime;
            _main.transform.localScale =Vector3.Lerp(_main.transform.localScale , new Vector3(_wigthCurve.Evaluate(progress)* _wigthSpeed , _wigthCurve.Evaluate(progress) * _wigthSpeed, _wigthCurve.Evaluate(progress) * _wigthSpeed),0.02f);
           
            yield return null;
        }
    }

    private IEnumerator Gravity()
    {
        while (true)
        {
            foreach (var ray in _rays)
            {
                ray.GetComponent<Rigidbody>().velocity = new Vector3(ray.GetComponent<Rigidbody>().velocity.x, -1, ray.GetComponent<Rigidbody>().velocity.z);
            }

            yield return null;
        }
    }

    public void SetGravity(Vector3 direction, float duration)
    {

    }

}
