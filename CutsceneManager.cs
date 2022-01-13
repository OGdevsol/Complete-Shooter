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
    public Transform JetTrail1;
    public Transform JetTrail2;
    public GameObject JetFlame;
    
    private int currentLevel;

    private void Update()
    {
        
    }

    private void Start()
    
    {
        
        Environment.SetActive(true);
        
        if (cutscenebool)
        {
            currentLevel = GameManager.Instance.currentLevel-1;

            if (currentLevel == 0)
            {
                GameManager.Instance.PlayerControllerWhole.SetActive(false);
                levelscontroller.SetActive(false);
                cutscene.SetActive(true);
                GameManager.Instance.flagscollected.SetActive(false);
                InstantiateJetFlames();
               // GameManager.Instance.DiamondGO.SetActive(false);
            }
        
            else
            {
                cutscene.SetActive(false);
                levelscontroller.SetActive(true);
                GameManager.Instance.DiamondGO.SetActive(true);

            }
        }
        else
        {
            cutscene.SetActive(false);
            levelscontroller.SetActive(true);
            //GameManager.Instance.DiamondGO.SetActive(true);
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
       // Flame1.transform.SetParent(JetTrail1,false);
        GameObject Flame2= Instantiate(JetFlame, JetTrail2.position, JetTrail2.rotation) as GameObject;
        Flame2.transform.parent = JetTrail2.transform;
       // Flame2.transform.SetParent(JetTrail2,false);
        
    }
}
