using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    [SerializeField] private GameObject _shop;

    public void ShopButonOn()
    {
        _shop.SetActive(true);
    }
}
