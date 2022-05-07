using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected int Price;
    [SerializeField] protected Color Scene;
    [SerializeField] protected Player Player;
    [SerializeField] protected Sprite Icon;
    [SerializeField] protected TMP_Text PriceText;
    [SerializeField] protected Sprite IconViev;
    [SerializeField] protected TMP_Text BuyText;

    protected bool IsBuy;

    private void Awake()
    {
        PriceText.text = Price.ToString();
        IconViev = Icon;
    }

    private void Update()
    {
        if (Player.Wallet.Coins >= Price)
            BuyText.text = "Купить";
        else
            BuyText.text = "Недостаточно денег";
    }

    public void Bye()
    {
        if (Player.Wallet.Coins >= Price)
            Player.ChangeScene(Scene, Price);
    }
}
