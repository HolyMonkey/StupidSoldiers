using Agava.YandexGames.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    public event UnityAction Touch;

    private bool _isTouched;
    private bool _isGameStarted;

    private void Awake()
    {
        _isGameStarted = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                if(_isGameStarted == false)
                {
                    if (WebEventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                }            
                Debug.Log(WebEventSystem.current.IsPointerOverGameObject());
            }
              

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

    public void StartGame()
    {
        _isGameStarted = true;
    }
}