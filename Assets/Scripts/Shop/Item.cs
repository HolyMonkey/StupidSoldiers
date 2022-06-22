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

    protected bool IsBuy;

    private void Awake()
    {
        PriceText.text = Price.ToString();
        IconViev = Icon;
        IsBuy = false;
    }

    private void Update()
    {
        if (Player.Wallet.Coins <= Price)
            Button.enabled = false;
        else
            Button.enabled = true;
    }

    public virtual void Bye()
    {
        if (Player.Wallet.Coins >= Price)
        {
            Player.ChangeScene(Scene, Price);
            IsBuy = true;
        }

        if (IsBuy == true)
        {
            PriceText.text = "Куплено";
            Button.enabled = false;
        }
         
            
    }
}
