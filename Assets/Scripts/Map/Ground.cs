using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private ParticleSystem _bulletDecalDirty;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>())
        {
            var decal = Instantiate(_bulletDecalDirty);
            decal.transform.position = other.transform.position;
            decal.transform.position = new Vector3(decal.transform.position.x, -2.33f,0);
        }
    }
}
