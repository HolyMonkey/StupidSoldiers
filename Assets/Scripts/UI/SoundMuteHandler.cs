using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundMuteHandler : MonoBehaviour
{
    [SerializeField] private Texture _mute;
    [SerializeField] private Texture _unmute;
    [SerializeField] private RawImage _image;

    private bool _isSoundMute = false;

    public void SoundMuteButtonOn()
    {
        if (_isSoundMute == false)
        {
            AudioListener.pause = true;
            _isSoundMute = true;
            _image.texture = _mute;
        }
        else
        {
            AudioListener.pause = false;
            _isSoundMute = false;
            _image.texture = _unmute;
        }
    }
}
