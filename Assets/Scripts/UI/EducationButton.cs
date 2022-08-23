using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EducationButton : MonoBehaviour
{
    [SerializeField] private Button[] _disableButtons;
    [SerializeField] private GameObject _panel;
    [SerializeField] private RawImage[] _startButtonsImage;

    private PlayerInput _playerInput;

#if YANDEX_GAMES
  private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
    }

    public void OnEducationButtonOn()
    {
        foreach (var disableButton in _disableButtons)
            disableButton.enabled = false;

        foreach (var startButtonImage in _startButtonsImage)
            startButtonImage.gameObject.SetActive(false);

        _playerInput.SetPanelActive();

        _panel.SetActive(true);
    }
}
