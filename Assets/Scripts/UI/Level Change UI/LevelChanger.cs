using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelChanger : MonoBehaviour
{
    private int _firstId =1;
    private int _secondId = 2;
    private int _thirdId = 3;
    private int _fourthId = 4;
    private int _fifeId = 5;
    private int _sixId = 6;
    private int _sevenId = 7;
    private int _eightId = 8;
    private int _nineId = 9;
    private int _tenId = 10;

    public void OnChangeButtonOn(int id)
    {
        SceneManager.LoadScene(id);
    }

    public void LoadFirstLevel()
    {
        SceneManager.LoadScene(_firstId);
    }

    public void LoadSecondLevel()
    {
        SceneManager.LoadScene(_secondId);
    }

    public void LoadThirdLevel()
    {
        SceneManager.LoadScene(_thirdId);
    }

    public void LoadFourthLevel()
    {
        SceneManager.LoadScene(_fourthId);
    }

    public void LoadFiveLevel()
    {
        SceneManager.LoadScene(_fifeId);
    }

    public void LoadSizLevel()
    {
        SceneManager.LoadScene(_sixId);
    }

    public void LoadSevenLevel()
    {
        SceneManager.LoadScene(_sevenId);
    }

    public void LoadEightLevel()
    {
        SceneManager.LoadScene(_eightId);
    }

    public void LoadNineLevel()
    {
        SceneManager.LoadScene(_nineId);
    }

    public void LoadTenLevel()
    {
        SceneManager.LoadScene(_tenId);
    }
}
