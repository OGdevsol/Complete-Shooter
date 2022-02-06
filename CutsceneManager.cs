using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;
    
    public GameObject cutscene;
    public GameObject Environment;
    public GameObject levelscontroller;
    public static bool cutscenebool = true;
    public Transform JetTrail1;
    public Transform JetTrail2;
    public GameObject JetFlame;
    private int currentLevel;
    public bool hostageRescuecutScene=false;

    public GameObject HostageRescueCutscene;
    

    private void Awake()
    {
       Environment.SetActive(true);
       
    }

    private void Start()
    

    {
      
        
        currentLevel = GameManager.Instance.currentLevel-1;
        if (PlayerPrefs.GetString("Mode")=="HostageRescue" &&  currentLevel == 0 )
        {
            GameManager.Instance.PlayerControllerWhole.SetActive(false);
            levelscontroller.SetActive(false);
            HostageRescueCutscene.SetActive(true);
            
        }
        
        else
        {
            HostageRescueCutscene.SetActive(false);
            levelscontroller.SetActive(true);
               

        }
        
       
        
        if (cutscenebool)
        {
            currentLevel = GameManager.Instance.currentLevel-1;

            if (currentLevel == 0 && PlayerPrefs.GetString("Mode")=="Flag" || currentLevel == 0 && PlayerPrefs.GetString("Mode")=="BombDiffuse")
            {
                GameManager.Instance.PlayerControllerWhole.SetActive(false);
                levelscontroller.SetActive(false);
                cutscene.SetActive(true);
                InstantiateJetFlames();
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

    public void InstantiateJetFlames()
    {
        GameObject Flame1 =  Instantiate(JetFlame, JetTrail1.position, JetTrail1.rotation) as GameObject;
        Flame1.transform.parent = JetTrail1.transform;
        GameObject Flame2= Instantiate(JetFlame, JetTrail2.position, JetTrail2.rotation) as GameObject;
        Flame2.transform.parent = JetTrail2.transform;
       
        
    }
    
}

