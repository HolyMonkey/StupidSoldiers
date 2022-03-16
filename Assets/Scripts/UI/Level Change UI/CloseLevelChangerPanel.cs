using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloseLevelChangerPanel : MonoBehaviour
{
    [SerializeField] private GameObject _levelPanel;

    public void OnCloseButtonOn()
    {
        _levelPanel.SetActive(false);
    }
}
