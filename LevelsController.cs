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
    public GameObject LastEnemyRemaining;
    public bool lastEnemy;
    public bool deactivateCurrentWeapon;
    private void Start()
    {
       
        currentLevel = GameManager.Instance.currentLevel-1;
        if (PlayerPrefs.GetString("Mode")=="Flag" || PlayerPrefs.GetString("Mode") == "BombDiffuse" )
        {
            for (int i = 0; i <  levelData[currentLevel].enemiesType.Count; i++)
            {
                if (PlayerPrefs.GetString("Mode")=="Flag")
                {
               
                    GameObject g =Instantiate(enemyPrefab[CheckEnemyType(i)],levelData[currentLevel].enemiesPositionFlag[i].transform.position,levelData[currentLevel].enemiesPositionFlag[i].transform.rotation);
                    levelData[currentLevel].totalEnemies.Add(g.transform);
               
                }
                else if(PlayerPrefs.GetString("Mode")=="BombDiffuse")
                {
               
                    GameObject g =Instantiate(enemyPrefab[CheckEnemyType(i)],levelData[currentLevel].enemiesPositionBomb[i].transform.position,levelData[currentLevel].enemiesPositionBomb[i].transform.rotation);
                    levelData[currentLevel].totalEnemies.Add(g.transform); 
                }
            
            }
            
        }
        else if (PlayerPrefs.GetString("Mode")=="BombPlant")
        {
            for (int i = 0; i <  levelData[currentLevel].enemiesTypeBombPlant.Count; i++)
            {
                GameObject g =Instantiate(enemyPrefab[CheckEnemyType(i)],levelData[currentLevel].enemiesPositionBombPlant[i].transform.position,levelData[currentLevel].enemiesPositionBombPlant[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform);
            
            }
        }
        /*for (int i = 0; i <  levelData[currentLevel].enemiesTypeHostageRescue.Count; i++)
        {
            if (PlayerPrefs.GetString("Mode")=="HostageRescue")
            {
               
                GameObject g =Instantiate(enemyPrefab[CheckEnemyType(i)],levelData[currentLevel].enemiesPositionHostageRescue[i].transform.position,levelData[currentLevel].enemiesPositionHostageRescue[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform);
               
            }
        }*/

        for (int i = 0; i < levelData[currentLevel].enemiesWave; i++)
        {
            levelData[currentLevel].totalEnemies[i].gameObject.SetActive(true);
        }

        if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            BombInstantiate();
        }
    }

    public void BombInstantiate()
    {
        for (int i = 0; i < levelData[currentLevel].bombposition.Count; i++)
        {
           GameObject bomb =  Instantiate(bombDiffuseObject,levelData[currentLevel].bombposition[i].transform.position, levelData[currentLevel].bombposition[i].transform.rotation);
           bomb.SetActive(true);
      
        }

    }

    public void BombPlantInstantiate()
    {
        deactivateCurrentWeapon = true;
        for (int i = 0; i < levelData[currentLevel].BombPlantPosition.Count; i++)
        {
            GameObject b = Instantiate(GameManager.Instance.bombPlantPositionIndicator,
                levelData[currentLevel].BombPlantPosition[i].transform.position,
                levelData[currentLevel].BombPlantPosition[i].transform.rotation);
                b.SetActive(true);
        }
        PlayerPrefs.SetInt("BombCountForLevel", levelData[currentLevel].bombPlantQuantity.Length);
    }
    
    private int defused = 0;
    public void checkallbombs()
    {
        defused++;
        GameManager.Instance.BombsDefused.text = defused.ToString();
        PlayerPrefs.SetInt("TotalBombs",defused+PlayerPrefs.GetInt("TotalBombs"));
        Debug.Log("TotalBombsDefused  " + PlayerPrefs.GetInt("TotalBombs"));
        if (levelData[currentLevel].bombposition.Count == defused)
        {
            allbombdefused = true;
            bombdiffusecheck();
        }

    }
    
    public void NextEnemy()
    {
        if (levelData[currentLevel].totalEnemies.Count < 0)
        {
            levelData[currentLevel].totalEnemies[0].gameObject.SetActive(true);
        }
        if (levelData[currentLevel].totalEnemies.Count == 1)
        {
          LastEnemyRemaining.SetActive(true);
          lastEnemy=true;
        }
        else
        {
            if (PlayerPrefs.GetString("Mode") == "Flag" && levelData[currentLevel].totalEnemies.Count == 0 )
            {
               
                LastEnemyRemaining.SetActive(false);
                lastEnemy = false;
                StartCoroutine(waitmethod(3.5f));
            }

            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse"  && levelData[currentLevel].totalEnemies.Count == 0)
            {
                LastEnemyRemaining.SetActive(false);
                lastEnemy = false;
                StartCoroutine(slowmo());
                allenemydead = true;     
                bombdiffusecheck();
                
            }
            else if (PlayerPrefs.GetString("Mode") == "BombPlant"  && levelData[currentLevel].totalEnemies.Count == 0)
            {
                BombPlantInstantiate();
                LastEnemyRemaining.SetActive(false);
                lastEnemy = false;
                StartCoroutine(slowmo());
                allenemydead = true;
                GameManager.Instance.activateBombPlantHands();
            }
        }

    }

    IEnumerator slowmo()
    {
        Time.timeScale = .20f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1;  
        
    }
    IEnumerator waitmethod(float x)
    {
        Time.timeScale = .20f;
        yield return new WaitForSecondsRealtime(1.5f);
        Time.timeScale = 1;
        yield return new WaitForSecondsRealtime(x);
        //Flag mode level will be completed when all flags have been collected. Check flagcollect.cs>FlagCollectionFunctionality
    }
    public void bombdiffusecheck()
    {

        if (allbombdefused && allenemydead)
        {
            Controls.SetActive(false);
            GameManager.Instance.GameComplete();
            
        }
    }
    private int x;
    private int CheckEnemyType(int i )
    {
        if (PlayerPrefs.GetString("Mode")=="Flag" || PlayerPrefs.GetString("Mode")=="BombDiffuse")
        {
            if (levelData[currentLevel].enemiesType[i] == EnemyType.A || levelData[currentLevel].enemiesTypeBombPlant[i]==EnemyType.A)
            {
                x= 0;
                return x;
                
            }
            if (levelData[currentLevel].enemiesType[i] == EnemyType.B || levelData[currentLevel].enemiesTypeBombPlant[i]==EnemyType.B)
            {
                x= 1;
                return x;
            }
        }
        else if (PlayerPrefs.GetString("Mode")=="BombPlant" || PlayerPrefs.GetString("Mode")=="HostageRescue")
        {
            if (levelData[currentLevel].enemiesTypeBombPlant[i]==EnemyType.A)
            {
                x= 0;
                return x;
                
            }
            if (levelData[currentLevel].enemiesTypeBombPlant[i]==EnemyType.B)
            {
                x= 1;
                return x;
            }
            
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
     [Space(2)]public List<EnemyType> enemiesTypeBombPlant;
     [Space(2)]public List<Transform> enemiesPositionBombPlant;
     [Space(2)]public List<Transform> BombPlantPosition;
     public int [] bombPlantQuantity;
     [Space(2)]public List<EnemyType> enemiesTypeHostageRescue;
     [Space(2)]public List<Transform> enemiesPositionFlag;
     [Space(2)]public List<Transform> enemiesPositionBomb;
     [Space(2)]public List<Transform> enemiesPositionHostageRescue;
     [Space(2)]public List<Transform> bombposition;
     [Space(2)]public List<Transform> totalEnemies;
     [Space(2)] public List<GameObject> bombDiffusePoint;
     [Space(2)] public List<Transform> RespawnPosition;
    

 }



 
[Serializable]
public enum EnemyType
{
    A,B
}



