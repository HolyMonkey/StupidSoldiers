using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class Item : MonoBehaviour
{
    [SerializeField] protected int Price;
    [SerializeField] protected Color Scene;
    [SerializeField] protected Player Player;
    [SerializeField] protected Sprite Icon;
    [SerializeField] protected TMP_Text PriceText;
    [SerializeField] protected Sprite IconViev;
    [SerializeField] protected TMP_Text BuyText;
    [SerializeField] protected Button Button;
    [SerializeField] protected string SaveKeyName;
    [SerializeField] protected TMP_Text ByedText;
    [SerializeField] protected TMP_Text UseText;
    [SerializeField] protected GameObject ByeIcon;
    [SerializeField] protected int TextId;

    protected bool IsBuy;

    public bool IsByed => IsBuy;
    public string KeyName => SaveKeyName;

    private void Awake()
    {
        PriceText.text = Price.ToString();
        IconViev = Icon;
        int intIsByed = PlayerPrefs.GetInt(SaveKeyName);
        IsBuy = Convert.ToBoolean(intIsByed);
        if(IsBuy == false)
        {
            ByedText.gameObject.SetActive(true);
            UseText.gameObject.SetActive(false);
            ByeIcon.SetActive(true);
        }
        else
        {
            ByedText.gameObject.SetActive(false);
            UseText.gameObject.SetActive(true);
            ByeIcon.SetActive(false);
        }
    }

    private void Update()
    {
        if (IsByed == false)
        {
            if (Player.Wallet.Coins <= Price)
                Button.enabled = false;
            else
                Button.enabled = true;
        }       
    }

    public virtual void Bye()
    {
        if(IsBuy == false)
        {
            if (Player.Wallet.Coins >= Price)
            {
                Player.ChangeScene(Scene, Price, TextId);
                IsBuy = true;
                ByedText.gameObject.SetActive(false);
                UseText.gameObject.SetActive(true);
                ByeIcon.SetActive(false);
                PlayerPrefs.SetInt(KeyName, Convert.ToInt32(IsBuy));
                UseText.text = "Используется";
            }       
        }
        else
        {
            Player.ChangeScene(Scene);
        }   
            
    }
}
