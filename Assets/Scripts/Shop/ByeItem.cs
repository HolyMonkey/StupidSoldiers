using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ByeItem : MonoBehaviour
{
    [SerializeField] private Item _byedItem;

    public void ByeItemOn()
    {
        _byedItem.Bye();
    }
}
