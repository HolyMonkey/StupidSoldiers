using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyOnLoadHandler : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
