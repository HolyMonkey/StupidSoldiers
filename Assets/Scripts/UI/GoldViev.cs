using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GoldViev : MonoBehaviour
{
    private TMP_Text _text;
    private Wallet _wallet;

    private void Start()
    {
        _text = GetComponent<TMP_Text>();
        _wallet = FindObjectOfType<Wallet>();
        _wallet.ChangeCoinsCount += OnCoinsChanged;
        _text.text = _wallet.Coins.ToString();
    }

    private void OnDisable()
    {
        _wallet.ChangeCoinsCount -= OnCoinsChanged;
    }

    private void OnCoinsChanged(int coin)
    {
       _text.text = _wallet.Coins.ToString();
    }
}
