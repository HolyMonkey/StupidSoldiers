using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private SkinnedMeshRenderer _currentScene;
    [SerializeField] private SkinnedMeshRenderer _test;

    private Wallet _wallet;

    public Wallet Wallet => _wallet;

    private void Awake()
    {
        //_currentScene = _test;
        _wallet = GetComponent<Wallet>();
    }

    public void ChangeScene(Color scene, int coins)
    {
        _currentScene.sharedMaterials.FirstOrDefault().color = scene;
        _wallet.DescreasCoins(coins);
    }
}
