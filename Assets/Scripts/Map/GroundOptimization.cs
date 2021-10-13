using UnityEngine;

public class GroundOptimization : MonoBehaviour
{
    [SerializeField] private GameObject _ground;

    private void OnBecameVisible()
    {
        _ground.SetActive(true);   
    }

    private void OnBecameInvisible()
    {
        _ground.SetActive(false);
    }
}