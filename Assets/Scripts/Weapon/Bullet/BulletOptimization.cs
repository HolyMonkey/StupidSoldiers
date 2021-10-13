using UnityEngine;

public class BulletOptimization : MonoBehaviour
{
    [SerializeField] private GameObject _bullet;

    private void OnBecameInvisible()
    {
        Destroy(_bullet);
    }
}