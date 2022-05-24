using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderbordCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject _leadebordPanel;

    public void CloseButtonOn()
    {
        _leadebordPanel.SetActive(false);
    }
}
