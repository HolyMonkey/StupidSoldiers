using UnityEngine;
using UnityEngine.Events;

public class Barrier : MonoBehaviour
{
    [SerializeField] private GameObject _model;
    [SerializeField] private ParticleSystem _hitEffect;

    public event UnityAction Hit;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Bullet>())
        {
            GetComponent<BoxCollider>().enabled = false;
            _model.SetActive(false);
            _hitEffect.gameObject.SetActive(true);
            _hitEffect.Play();
            Hit?.Invoke();
        }
    }
}
