using UnityEngine;
using UnityEngine.Events;

public class PlayerInput : MonoBehaviour
{
    public event UnityAction Touch;

    private bool _isTouched;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if (!_isTouched)
            {
                _isTouched = true;
                Touch?.Invoke();
            }
        }
        else
        {
            _isTouched = false;
        }
    }
}