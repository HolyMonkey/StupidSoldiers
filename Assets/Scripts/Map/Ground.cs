using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private ParticleSystem _bulletDecalDirty;

    private float _delay = 0.1f;
    private bool _canDrawDecal=true;


    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>()& _canDrawDecal)
        {
            _canDrawDecal = false;
            StartCoroutine(Delay());
            var decal = Instantiate(_bulletDecalDirty);
            decal.transform.position = other.transform.position;
            decal.transform.position = new Vector3(decal.transform.position.x, -2.33f,0);
        }
    }

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(_delay);
        _canDrawDecal = true;
    }
}
