using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaderbordCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject _leadebordPanel;
    [SerializeField] private GameObject _closeButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private RawImage[] _startButtonsImage;


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
            _shopButton.enabled = true;
            _startButton.enabled = true;

            foreach (var startButtonImage in _startButtonsImage)
                startButtonImage.gameObject.SetActive(true);

            _playerInput.SetPanelNotActive();
        }        
    }
}
