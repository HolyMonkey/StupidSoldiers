using UnityEngine.EventSystems;
using UnityEngine;
using System.Collections.Generic;
using Agava.YandexGames.Utility;
using UnityEngine.UI;

public class CloseOnTap : MonoBehaviour
{
    //[SerializeField] private GameObject _panelWithControlElements;
    //[SerializeField] private GameObject _tapToPlay;
    [SerializeField] private GameObject _playButton;
    [SerializeField] private Button _shopButton;
    [SerializeField] private Button _startButton;
    [SerializeField] private RawImage[] _startButtonsImage;

    private PlayerInput _playerInput;
    private bool mouseIsOver = false;

    private void Awake()
    {
        _playerInput = FindObjectOfType<PlayerInput>();
    }

    private void OnEnable()
    {
        //WebEventSystem.current.SetSelectedGameObject(gameObject);
        //_playButton.SetActive(false);
    }

    private void Update()
    {
        PointerEventData pointer = new PointerEventData(WebEventSystem.current); 

        if (Input.GetMouseButton(0) || Input.touchCount > 0)
        {
            if (WebEventSystem.current.IsPointerOverGameObject() == true)
            {

                CloseSelf();
            }
        }   
    }

    public void CloseSelf()
    {
        gameObject.SetActive(false);
        //_playButton.SetActive(true);
        _playerInput.SetPanelNotActive();
        _shopButton.enabled = true;
        _startButton.enabled = true;

        foreach (var startButtonImage in _startButtonsImage)
            startButtonImage.gameObject.SetActive(true);
        //_panelWithControlElements.SetActive(true);
        //_tapToPlay.SetActive(true);
    }

}