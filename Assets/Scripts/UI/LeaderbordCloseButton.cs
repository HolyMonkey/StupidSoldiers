using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaderbordCloseButton : MonoBehaviour
{
    [SerializeField] private GameObject _leadebordPanel;
    [SerializeField] private GameObject _closeButton;

    public void CloseButtonOn()
    {
        _leadebordPanel.SetActive(false);
        _closeButton.SetActive(false);
    }
}
