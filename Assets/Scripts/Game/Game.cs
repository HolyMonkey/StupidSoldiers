using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public void Restart()
    {
        SceneManager.LoadScene(0);
        Time.timeScale = 1;
    }
}