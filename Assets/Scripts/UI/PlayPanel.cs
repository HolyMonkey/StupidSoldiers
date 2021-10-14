using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayPanel : MonoBehaviour
{
    [SerializeField] private Slider _progress;
    [SerializeField] private TMP_Text _coins;
    [SerializeField] private Wallet _plaerWallet;

    private ProgressBar _progressBar;


    private void OnEnable()
    {
        
    }

    private void OnDisable()
    {
        
    }

    public void ShowPanel()
    {

    }

    public void ClosePanel()
    {

    }


}