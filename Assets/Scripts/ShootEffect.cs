using UnityEngine;

public class ShootEffect : MonoBehaviour
{
    [SerializeField] private float _lifeTime;

    private void OnEnable()
    {
        Destroy(this.gameObject, _lifeTime);   
    }
}
