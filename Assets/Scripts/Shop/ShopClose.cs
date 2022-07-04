using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopClose : MonoBehaviour
{
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _closeButton;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _closeButton.SetActive(false);
    }

    public void CloseButtonOn()
    {
        if (_playerInput == null)
        {
            throw new UnityException("Player input is not found");
        }
        else
        {
            _playerInput.SetPanelActive();
            _closeButton.SetActive(false);
            _shop.SetActive(false);
        }    
    }
}
