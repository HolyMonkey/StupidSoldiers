using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodMaterialChanger : MonoBehaviour
{
    [SerializeField] private Material[] _bloodMaterials;

    

    private void OnEnable()
    {
        ParticleSystemRenderer particleSystemRenderer = GetComponent<ParticleSystemRenderer>();

        particleSystemRenderer.material = _bloodMaterials[Random.Range(0, _bloodMaterials.Length)];
        
    }
}
