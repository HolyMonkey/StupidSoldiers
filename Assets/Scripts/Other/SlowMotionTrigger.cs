using UnityEngine;
using UnityEngine.Events;

public class SlowMotionTrigger : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Transform _decalPosition;

    public Transform GetModelTransform()
    {
        return _enemy.GetModelTransform();
    }

    public void SetCollidePosition()
    {
        _enemy.SetCollidePosition(_decalPosition.position);
    }

    public Rigidbody GetRigidbody()
    {
        return _enemy.GetModelRigidbody();
    }

    public event UnityAction Hit;

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<Bullet>())
        {
            Hit?.Invoke();
        }
    }
}
