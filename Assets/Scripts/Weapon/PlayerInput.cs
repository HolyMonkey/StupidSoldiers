using Agava.YandexGames.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerInput : MonoBehaviour
{
    private bool _isTouched;
    private bool _isGameStarted;
    private bool _isPanelOpen;

    public bool IsPanelOpen => _isPanelOpen;

    public event UnityAction Touch;

    private void Awake()
    {
        _isGameStarted = false;
        _isPanelOpen = false;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) || Input.touchCount > 0 || Input.GetMouseButtonDown(0))
        {
            if (Input.GetMouseButton(0) || Input.touchCount > 0)
            {
                if (_isGameStarted == false || _isPanelOpen == true)
                {
                    if (WebEventSystem.current.IsPointerOverGameObject())
                    {
                        return;
                    }
                }
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

    public void SetPanelActive()
    {
        _isPanelOpen = true;
    }

    public void SetPanelNotActive()
    {
        _isPanelOpen = false;
    }
}