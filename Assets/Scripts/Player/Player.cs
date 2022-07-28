using Lean.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _currentScene;
    [SerializeField] private Material _eg;
    [SerializeField] private TMP_Text[] _usedTexts;

    private string _color;
    private Wallet _wallet;
    private int _idOfUsedText;

    public Wallet Wallet => _wallet;

    private void Awake()
    {
        _wallet = GetComponent<Wallet>();
        _color = PlayerPrefs.GetString("LastByedChicken");

        if (_color != "")
        {
            Color color = JsonUtility.FromJson<Color>(_color);
            _currentScene.sharedMaterials.FirstOrDefault().color = color;
        }

        _color = PlayerPrefs.GetString("LastByedEg");

        if (_color != "")
        {
            Color color = JsonUtility.FromJson<Color>(_color);
            _eg.color = color;
        }
    }

    public void ChangeScene(Color scene, int coins, int textId)
    {
        _currentScene.sharedMaterials.FirstOrDefault().color = scene;
        _wallet.DescreasCoins(coins);
        _color = JsonUtility.ToJson(scene);
        PlayerPrefs.SetString("LastByedChicken", _color);
        PlayerPrefs.SetInt("UsedTextId", textId);
    }

    public void ChangeScene(Color scene)
    {
        _currentScene.sharedMaterials.FirstOrDefault().color = scene;
    }
}
