using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyWatcher : MonoBehaviour
{
    [SerializeField] private Enemy[] _enemies;
    [SerializeField] private int _minEnemyReward;
    [SerializeField] private int _maxEnemyReward;

    public event UnityAction<int> EnemyDeath;

    private void OnEnable()
    {
        foreach(var enemy in _enemies)
        {
            enemy.Killed += OnEnemyDeath;
        }
    }

    private void OnDisable()
    {
        foreach (var enemy in _enemies)
        {
            enemy.Killed -= OnEnemyDeath;
        }
    }

    private void OnEnemyDeath()
    {
        EnemyDeath?.Invoke(Random.Range(_minEnemyReward, _maxEnemyReward));
    }
}
