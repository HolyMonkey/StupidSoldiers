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

    public void AddCoins(int coin)
    {
        if (coin > 0)
        {
            Coins += coin;
            Increase += coin;
            ChangeCoinsCount?.Invoke(Coins);
        }
    }

    public void SetMultiplier(int multiplier)
    {
        Increase *= multiplier;
    }

    // заменить на событие
    public void SetCoins(int coins)
    {
        Coins = coins;
    }

}