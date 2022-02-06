using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class flagcollect : MonoBehaviour
{
    public static flagcollect instance;
   

    private void Awake()
    {
        instance = this;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && PlayerPrefs.GetString("Mode")=="Flag")
        {
            
            Destroy(gameObject);
            GameManager.Instance.flagcollectcounter += 1;
            PlayerPrefs.SetInt("TotalFlags", PlayerPrefs.GetInt("TotalFlags")+1);
            GameManager.Instance.flagscollectedtext.text = GameManager.Instance.flagcollectcounter.ToString();
           // FlagCollectionFunctionality();
        }
    }

    public void FlagCollectionFunctionality()
    {
        if (LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].enemiesType.Count==0 && LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].EnemiesOnHold.Count==0 )
        {
            Debug.Log("Check Called");
            GameManager.Instance.GameComplete();
        }
    }
}
