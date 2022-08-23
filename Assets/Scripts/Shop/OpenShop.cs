using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject _shop;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private Button _leaderbordButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private RawImage[] _startButtonsImage;
    [SerializeField] private Item[] _items;
    [SerializeField] private Button _inviteFriendsButton;
    [SerializeField] private Button _educationButton;

    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
        _closeButton.SetActive(false);
    }

    public void ShopButonOn()
    {
        if (_playerInput == null)
        {
            throw new UnityException("Player input is not found");
        }
        else
        {
            _playerInput.SetPanelActive();
            _closeButton.SetActive(true);
            _leaderbordButton.enabled = false;
            _startButton.enabled = false;
            _inviteFriendsButton.enabled = false;
            _educationButton.enabled = false;

            _shop.SetActive(true);

            foreach (var startButtonImage in _startButtonsImage)
                startButtonImage.gameObject.SetActive(false);

            foreach (var item in _items)
                item.ChangeTextOnOpenShop();

         
        }
    }
}
