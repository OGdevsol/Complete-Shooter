using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaRestriction : MonoBehaviour
{
    public static AreaRestriction instance; 
    public GameObject[] FlagLvLBarriers;
    public GameObject[] BombLvLBarriers;
    public GameObject LeavingAreaPanel;
    public GameObject player;
    public Transform[] OriginalSpawnPos;
   

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
       ActivateBarriers();
    }
    
    public void ActivateBarriers()
    {
        if (PlayerPrefs.GetString("Mode")=="Flag" && PlayerPrefs.GetInt("FlagLevel")<=30)
        {
            FlagLvLBarriers[PlayerPrefs.GetInt("FlagLevel")-1].SetActive(true);
        }
        else if(PlayerPrefs.GetString("Mode")=="BombDiffuse" && PlayerPrefs.GetInt("BombLevel")<=30)
        {
            BombLvLBarriers[PlayerPrefs.GetInt("BombLevel")-1].SetActive(true);
            
        }
    }
    
    
}
