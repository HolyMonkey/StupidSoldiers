using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    [SerializeField] private Transform _bulletDecalSpawnPoint;

    public float Height => _bulletDecalSpawnPoint.position.y;
}
