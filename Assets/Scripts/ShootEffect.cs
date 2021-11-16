using UnityEngine;

public class ShootEffect : MonoBehaviour
{
    [SerializeField] private float _lifeTime;

    private void OnEnable()
    {
        Destroy(gameObject, _lifeTime);   
    }
}
