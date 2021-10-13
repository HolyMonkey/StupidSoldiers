using UnityEngine;
using UnityEngine.Events;

public class SlowMotionTrigger : MonoBehaviour
{
    [SerializeField] private Enemy _enemy;
    [SerializeField] private Transform _headDecalTransform;

    public void ShowBodySplat(EnemyBodySplat splat, Vector3 position)
    {
        _enemy.ShowBodySplat(splat, _headDecalTransform.position);
    }

    public event UnityAction Hit;

    private void OnCollisionEnter(Collision other)
    {
        if(other.gameObject.GetComponent<Bullet>())        
            Hit?.Invoke();        
    }
}
