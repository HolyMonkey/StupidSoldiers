using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopClose : MonoBehaviour
{
    [SerializeField] private GameObject _shop;

    public void CloseButtonOn()
    {    
        Time.timeScale = 1f;
        _shop.SetActive(false);
    }
}
