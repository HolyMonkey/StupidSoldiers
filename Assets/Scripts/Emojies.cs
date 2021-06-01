using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emojies : MonoBehaviour
{
	[SerializeField] private ParticleSystem effectSad;
    [SerializeField] private ParticleSystem effectSmile;

    private float _lastEmojiTime = 0;

    public void PlaySmile()
    {
        if (_lastEmojiTime <= 0)
        {
            effectSmile.Play();
            _lastEmojiTime = 2;
        }
    }

    public void PlaySad()
    {
        if (_lastEmojiTime <= 0)
        {
            effectSad.Play();
            _lastEmojiTime = 2;
        }
    }


    private void Update()
    {
        _lastEmojiTime -= Time.deltaTime;
    }
}

