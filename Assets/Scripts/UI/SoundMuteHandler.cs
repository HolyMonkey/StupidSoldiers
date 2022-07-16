using Agava.YandexGames.Utility;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SoundMuteHandler : MonoBehaviour
{
    [SerializeField] private Texture _mute;
    [SerializeField] private Texture _unmute;
    [SerializeField] private RawImage _image;

    private bool _isSoundMute = false;

    private void Start()
    {
        Debug.Log(_isSoundMute);
        _isSoundMute = Convert.ToBoolean(PlayerPrefs.GetInt("SoundMute"));
        Debug.Log(_isSoundMute);

        if (_isSoundMute == true)
        {
            AudioListener.pause = true;
            AudioListener.volume = 0f;
            _image.texture = _mute;
        }
        else
        {
            AudioListener.pause = false;
            AudioListener.volume = 0.5f;
            _image.texture = _unmute;

        }

    }

    public void SoundMuteButtonOn()
    {
        if (_isSoundMute == false)
        {
            AudioListener.pause = true;
            _isSoundMute = true;
            _image.texture = _mute;
            AudioListener.volume = 0f;
            PlayerPrefs.SetInt("SoundMute", Convert.ToInt32(_isSoundMute));
            Debug.Log(_isSoundMute);
        }
        else
        {
            AudioListener.pause = false;
            _isSoundMute = false;
            _image.texture = _unmute;
            PlayerPrefs.SetInt("SoundMute", Convert.ToInt32(_isSoundMute));
            AudioListener.volume = 0.5f;
            Debug.Log(_isSoundMute);
        }
    }

    private void Update()
    {
        if (_isSoundMute == true)
            return;
        AudioListener.pause = WebApplication.InBackground;
    }
}
