using UnityEngine;
using UnityEngine.Events;

public class EnemyOptimization : MonoBehaviour
{
    public event UnityAction Visible;
    public event UnityAction Invisible;

    private void Start()
    {
        Invisible?.Invoke();
    }

    private void OnBecameVisible()
    {
        Visible?.Invoke();
    }

    private void OnBecameInvisible()
    {
        Invisible?.Invoke();
    }
}