using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopClose : MonoBehaviour
{
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private Button _leaderbordButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private RawImage[] _startButtonsImage;
    [SerializeField] private Button _inviteFriendsButton;
    [SerializeField] private Button _educationButton;

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
            _playerInput.SetPanelNotActive();
            _closeButton.SetActive(false);
            _leaderbordButton.enabled = true;
            _startButton.enabled = true;
            _inviteFriendsButton.enabled = true;
            _educationButton.enabled = true;

            foreach (var startButtonImage in _startButtonsImage)
                startButtonImage.gameObject.SetActive(true);

            _shop.SetActive(false);
        }    
    }
}
