using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Agava.YandexGames;

public class RewardedVideoHandler : MonoBehaviour
{
    [SerializeField] private GameObject _sucsesResult;
    [SerializeField] private Wallet _wallet;

    public void RevardedVedeoButtonOn()
    {
        VideoAd.Show();
        StartCoroutine(StartCooldown(7));
        _sucsesResult.SetActive(true);
        _wallet.AddCoins(100);
        StartCoroutine(StartCooldown(3));
        _sucsesResult.SetActive(false);
    }

    private IEnumerator StartCooldown(int cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        StopCoroutine(StartCooldown(cooldown));
    }
}
