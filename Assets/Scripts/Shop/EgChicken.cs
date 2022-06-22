using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EgChicken : Item
{
    [SerializeField] private Material _eg;

    public override void Bye()
    {
        if (Player.Wallet.Coins >= Price)
        {
            _eg.color = Scene;
            Player.Wallet.DescreasCoins(Price);
            IsBuy = true;
        }

        if (IsBuy == true)
        {
            PriceText.text = "Куплено";
            Button.enabled = false;
        }
    }
}
