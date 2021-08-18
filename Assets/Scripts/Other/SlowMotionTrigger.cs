using UnityEngine;
using UnityEngine.Events;

public class SlowMotionTrigger : MonoBehaviour
{
    public event UnityAction Hit;

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Bullet>())
        {
            Hit?.Invoke();
        }
    }
}
