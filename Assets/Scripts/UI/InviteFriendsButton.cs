using Agava.VKGames;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InviteFriendsButton : MonoBehaviour
{
#if YANDEX_GAMES
  private void Awake()
    {
        gameObject.SetActive(false);
    }
#endif

    [SerializeField] private Wallet _money;

    public void OnInviteButtonClick()
    {
        SocialInteraction.InviteFriends(OnRewardedCallback);
    }


    private void OnRewardedCallback()
    {
        _money.AddCoins(50);
    }
}
