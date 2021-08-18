using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class Multiplier : MonoBehaviour
{
    [SerializeField] private int _multiplier;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private ParticleSystem _hitEffect;

    private bool _canDestroy = true;
    public int MultiplierValue => _multiplier;

    public event UnityAction<Multiplier> MultiplierHit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>() & _canDestroy)
        {
            MultiplierHit?.Invoke(this);
            Delete();
        }
    }

    public void Delete()
    {
        _text.enabled = false;
        Destroy(gameObject);
    }

    public void SetDestroy()
    {
        _canDestroy = false;
    }
}