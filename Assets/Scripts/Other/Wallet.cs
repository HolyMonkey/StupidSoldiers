using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
    [SerializeField] private EnemyWatcher _enemyWatcher;

    public event UnityAction<int> ChangeCoinsCount;
    public int Coins { get; private set; }
    public int Increase { get; private set; }

    private void OnEnable()
    {
        _enemyWatcher.EnemyDeath += AddCoins;
    }

    private void OnDisable()
    {
        _enemyWatcher.EnemyDeath -= AddCoins;
    }

    public void AddCoins(int increase)
    {
        if (increase > 0)
        {
            Increase += increase;
            ChangeCoinsCount?.Invoke(Coins+Increase);
        }
    }

    public void SetMultiplier(int multiplier)
    {        
        Increase *= multiplier;
        Coins += Increase;
    }

    public void SetCoins(int coins)
    {
        Coins = coins;
    }
}