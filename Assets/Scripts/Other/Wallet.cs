using UnityEngine;
using UnityEngine.Events;

public class Wallet : MonoBehaviour
{
    [SerializeField] private EnemyWatcher _enemyWatcher;
    [SerializeField] private DataSaver _dataSaver;

    public event UnityAction<int> ChangeCoinsCount;
    public int Coins { get; private set; }
    public int Increase { get; private set; }

    private Game _game;

    private void Awake()
    {       
        _game = FindObjectOfType<Game>();
        _dataSaver = FindObjectOfType<DataSaver>();
        _dataSaver.DownloadSave();
    }

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
            Coins += increase;
            _dataSaver.SaveData(Coins, _game.LevelNumber);
#if VK_GAMES
            GameAnalitic.GainedMoney(increase, "enemy killed", "enemy");
#endif
            ChangeCoinsCount?.Invoke(Coins);
        }
    }

    public void SetMultiplier(int multiplier)
    {        
        Increase *= multiplier;
        Coins += Increase;
#if VK_GAMES
        GameAnalitic.GainedMoney(Increase, "Multiplier hit", "mulriplier");
#endif
    }

    public void SetCoins(int coins)
    {
        Coins = coins;
    }

    public void DescreasCoins(int coins)
    {
        Coins -= coins;
        _dataSaver.SaveData(Coins, _game.LevelNumber);
        ChangeCoinsCount?.Invoke(Coins - Increase);
    }
}