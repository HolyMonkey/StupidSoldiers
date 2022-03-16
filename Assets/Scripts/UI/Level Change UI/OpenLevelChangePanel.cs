using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenLevelChangePanel : MonoBehaviour
{
    [SerializeField] private GameObject _levelPanel;

    public void OnBuutonOn()
    {
        _levelPanel.SetActive(true);
    }
}
