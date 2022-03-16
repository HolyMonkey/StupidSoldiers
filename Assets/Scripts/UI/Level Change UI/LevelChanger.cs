using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelChanger : MonoBehaviour
{
   public void OnChangeButtonOn(int id)
    {
        Application.LoadLevel(id);
    }
}
