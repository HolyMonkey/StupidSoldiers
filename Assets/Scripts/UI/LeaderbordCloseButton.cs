using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderbordCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject _leadebordPanel;
    [SerializeField] private GameObject _closeButton;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
    }

    public void CloseButtonOn()
    {
        if (_playerInput == null)
        {
            throw new UnityException("Player input is not found");
        }
        else
        {
            _leadebordPanel.SetActive(false);
            _closeButton.SetActive(false);
            _playerInput.SetPanelNotActive();
        }        
    }
}
