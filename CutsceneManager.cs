using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    
    public GameObject cutscene;
    public GameObject Environment;
    public GameObject levelscontroller;
    public static bool cutscenebool = true;
    
    private int currentLevel;
    
    private void Start()
    
    {
        
        Environment.SetActive(true);
        
        if (cutscenebool)
        {
            currentLevel = GameManager.Instance.currentLevel-1;

            if (currentLevel == 0 && PlayerPrefs.GetString("Mode")=="Flag" || currentLevel== 0 && PlayerPrefs.GetString("Mode")=="BombDiffuse")
            {
                levelscontroller.SetActive(false);
                cutscene.SetActive(true);
            }
        
            else
            {
                cutscene.SetActive(false);
                levelscontroller.SetActive(true);

            }
        }
        else
        {
            cutscene.SetActive(false);
            levelscontroller.SetActive(true);
            cutscenebool = true;
        }
        
        
    }

    public void skipbutton()
    {
        cutscenebool = false;
        SceneManager.LoadScene(2);
    }
}
