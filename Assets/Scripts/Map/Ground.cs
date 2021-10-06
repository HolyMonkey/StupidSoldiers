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
    }

}
