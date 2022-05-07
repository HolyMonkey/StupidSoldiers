using UnityEngine;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Transform _finishPoint;
    [SerializeField] private int _deltaBar;

    public float Progress { get; private set; }

    private void Update()
    {
        Progress = (_weapon.transform.position.x / _finishPoint.transform.position.x) * _deltaBar;
    }
}