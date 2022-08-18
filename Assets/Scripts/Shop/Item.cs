using Lean.Localization;
using System;
using System.Collections.Generic;
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
    [SerializeField] protected string IsUsedSaver;
    [SerializeField] protected List<Item> Items;
    [SerializeField] protected string _title;

    protected bool IsBuy;
    protected bool IsUsed;

    public bool IsByed => IsBuy;
    public string KeyName => SaveKeyName;

    private void Awake()
    {
        PriceText.text = Price.ToString();
        IconViev = Icon;
        int intIsByed = PlayerPrefs.GetInt(SaveKeyName);
        IsBuy = Convert.ToBoolean(intIsByed);
        IsUsed = Convert.ToBoolean(PlayerPrefs.GetInt(IsUsedSaver));

        if (IsUsed)
        {
            ByedText.gameObject.SetActive(false);
            UseText.gameObject.SetActive(true);
            UseText.text = LeanLocalization.GetTranslationText("UsedText");
            ByeIcon.SetActive(false);
        }
        else
        {
            if (IsBuy == false)
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
      
    }

    private void Update()
    {
        if (IsByed == false)
        {
            if (Player.Wallet.Coins < Price)
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
                var translationUsed = LeanLocalization.GetTranslationText("UsedText");
                UseText.text = translationUsed;
                IsUsed = true;
                PlayerPrefs.SetInt(IsUsedSaver, Convert.ToInt32(IsUsed));
                PlayerPrefs.Save();

#if VK_GAMES
                GameAnalitic.SpentMoney(Price, _title);
#endif

                foreach (var item in Items)
                    item.ChangeText();
            }       
        }
        else
        {
            Player.ChangeScene(Scene);
            var translationUsed = LeanLocalization.GetTranslationText("UsedText");
            UseText.text = translationUsed;
            IsUsed = true;
            PlayerPrefs.SetInt(IsUsedSaver, Convert.ToInt32(IsUsed));
            PlayerPrefs.Save();

            foreach (var item in Items)
            {
                item.ChangeText();

                if (item.IsUsed == true)
                    item.ChangeIsUsed();
            }             
        }           
    }

    public void ChangeText()
    {  
            if (IsBuy)
            {
                ByedText.gameObject.SetActive(false);
                UseText.gameObject.SetActive(true);
                UseText.text = LeanLocalization.GetTranslationText("Used");
                ByeIcon.SetActive(false);

            }
            else
            {
                ByedText.gameObject.SetActive(true);
                UseText.gameObject.SetActive(false);
                ByeIcon.SetActive(true);
            }
        
    }

    public void ChangeIsUsed()
    {
        IsUsed = false;
        PlayerPrefs.SetInt(IsUsedSaver, Convert.ToInt32(IsUsed));
        PlayerPrefs.Save();
    }

    public void ChangeTextOnOpenShop()
    {
        if (IsUsed)
        {
            ByedText.gameObject.SetActive(false);
            UseText.gameObject.SetActive(true);
            UseText.text = LeanLocalization.GetTranslationText("UsedText");
            ByeIcon.SetActive(false);
        }
        else
        {
            if (IsBuy == false)
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
    }
}
