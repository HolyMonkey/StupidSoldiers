using Lean.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EgChicken : Item
{
    [SerializeField] private Material _eg;

    private string _color;

    public override void Bye()
    {
        if(IsBuy== false)
        {
            if (Player.Wallet.Coins >= Price)
            {
                _eg.color = Scene;
                Player.Wallet.DescreasCoins(Price);
                IsBuy = true;
                ByedText.gameObject.SetActive(false);
                UseText.gameObject.SetActive(true);
                ByeIcon.SetActive(false);
                PlayerPrefs.SetInt(KeyName, Convert.ToInt32(IsBuy));
                _color = JsonUtility.ToJson(Scene);
                PlayerPrefs.SetString("LastByedEg", _color);
                PlayerPrefs.SetInt("UsedTextId", TextId);
                var translationUsed = LeanLocalization.GetTranslationText("UsedText");
                UseText.text = translationUsed;
            }
        }
        else
        {
            _eg.color = Scene;
        }
    }
}
