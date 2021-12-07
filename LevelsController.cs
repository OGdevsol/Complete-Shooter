using System;
using System.Collections;
using System.Collections.Generic;
using TacticalAI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class LevelsController : MonoBehaviour
{
    
    #region Global Instance
    
    public static LevelsController localinstance;
    public static LevelsController Instance 
    {
        get
        {
            if (localinstance == null)
                localinstance = GameObject.FindObjectOfType<LevelsController>();
            return localinstance;
        }
    }
    
    #endregion
    public GameObject[] enemyPrefab;
    public Enemy[] levelData;
    [HideInInspector]public int currentLevel;
    public int[] levelCash;
    public GameObject Controls;
    public GameObject bombDiffuseObject;
    public GameObject pausebutton;
    [HideInInspector]
    public bool allenemydead = false;
    [HideInInspector]
    public bool allbombdefused = false;

    public int LevelReward;
    public int RewardIncrement;
    public int TotalCash;
    public int currentCashValue;
    
    private void Start()
    {
        currentLevel = GameManager.Instance.currentLevel-1;
        if (PlayerPrefs.GetString("Mode")=="Flag" || PlayerPrefs.GetString("Mode")=="BombDiffuse")
        {
            for (int i = 0; i <  levelData[currentLevel].enemiesType.Count; i++)
            {
                GameObject g =Instantiate(enemyPrefab[CheckEnemyType(i)],levelData[currentLevel].enemiesPosition[i].transform.position,levelData[currentLevel].enemiesPosition[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform); 
            }

            for (int i = 0; i < levelData[currentLevel].enemiesWave; i++)
            {
                levelData[currentLevel].totalEnemies[i].gameObject.SetActive(true);
            }

            if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                BombInstantiate();
                
            }

        }
    }

    public void BombInstantiate()
    {
        for (int i = 0; i < levelData[currentLevel].bombposition.Count; i++)
        {
           GameObject bomb =  Instantiate(bombDiffuseObject,levelData[currentLevel].bombposition[i].transform.position,Quaternion.Euler(90,0,0));
           bomb.SetActive(true);
      
        }
    }
    
    private int defused = 0;
    public void checkallbombs()
    {
        defused++;
        GameManager.Instance.BombsDefused.text = defused.ToString();
        if (levelData[currentLevel].bombposition.Count == defused)
        {
            allbombdefused = true;
            bombdiffusecheck();
        }

    }
    
    public void NextEnemy()
    {
        if (levelData[currentLevel].totalEnemies.Count > 0)
        {
            levelData[currentLevel].totalEnemies[0].gameObject.SetActive(true);
        }
        else
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                pausebutton.GetComponent<Button>().interactable = false;
                StartCoroutine(waitmethod(3.5f));
                
            }

            if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                StartCoroutine(slowmo());
                allenemydead = true;     
                bombdiffusecheck();
                
            }
        }

    }

    IEnumerator slowmo()
    {
        Time.timeScale = .25f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1;  
        
    }
    IEnumerator waitmethod(float x)
    {
        Time.timeScale = .25f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(x);
        Controls.SetActive(false);
        GameManager.Instance.GameComplete();
    }

    
    
    
    public void bombdiffusecheck()
    {

        if (allbombdefused && allenemydead )
        {
            Controls.SetActive(false);
            GameManager.Instance.GameComplete();
            
        }
        

    }
    private int x;
    private int CheckEnemyType(int i )
    {
        if (levelData[currentLevel].enemiesType[i] == EnemyType.A)
        {
                x= 0;
                return x;
        }
        if (levelData[currentLevel].enemiesType[i] == EnemyType.B)
        {
                x= 1;
                return x;
        }
        return x;
    }
}

[Serializable]
 public class Enemy
 {
     [Space(5)]
     [Space(2)]public Transform playerSpwanPosition;
     [Space(2)]public int enemiesWave;

     [Space(2)]public List<EnemyType> enemiesType;
     
     [Space(2)]public List<Transform> enemiesPosition;
     [Space(2)]public List<Transform> bombposition;
     [Space(2)]public List<Transform> totalEnemies;

     [Space(2)] public List<GameObject> bombDiffusePoint;

 }
 
[Serializable]
public enum EnemyType
{
    A,B
}








