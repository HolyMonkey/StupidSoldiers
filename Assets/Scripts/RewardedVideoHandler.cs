using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agava.YandexGames;
using System;

public class RewardedVideoHandler : MonoBehaviour
{
    [SerializeField] private GameObject _sucsesResult;
    [SerializeField] private Wallet _wallet;

    private Action _adOpened;
    private Action _adRewarded;
    private Action _adClosed;
    private Action<string> _adErrorOccured;

    private void OnEnable()
    {
        _adOpened += OnAdOpened;
        _adRewarded += OnAdRewarded;
        _adClosed += OnAdClosed;
        _adErrorOccured += OnAdErrorOccured;
    }

    private void OnDisable()
    {
        _adOpened -= OnAdOpened;
        _adRewarded -= OnAdRewarded;
        _adClosed -= OnAdClosed;
        _adErrorOccured -= OnAdErrorOccured;
    }

    public void RevardedVedeoButtonOn()
    {
#if YANDEX_GAMES
        VideoAd.Show(_adOpened, _adRewarded, _adClosed, _adErrorOccured);
#endif
#if VK_GAMES
        Agava.VKGames.VideoAd.Show(_adRewarded);
#endif
    }

    private IEnumerator StartCooldown(int cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        StopCoroutine(StartCooldown(cooldown));
    }

    private void OnAdErrorOccured(string obj)
    {
        Debug.Log("Video is not played");
        AudioListener.pause = false;
        AudioListener.volume = 0f;
    }

    private void OnAdClosed()
    {
        Debug.Log("VideoClose");
        AudioListener.pause = false;
        AudioListener.volume = 0.5f;
    }

    private void OnAdRewarded()
    {
        _sucsesResult.SetActive(true);
        _wallet.AddCoins(100);
        StartCoroutine(StartCooldown(3));
        _sucsesResult.SetActive(false);
    }

    private void OnAdOpened()
    {
        AudioListener.pause = true;
        AudioListener.volume = 0f;
    }
}
