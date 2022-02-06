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

    #region Variables Declaration

    public GameObject[] enemyPrefab;
    public Enemy[] levelData;
    [HideInInspector] public int currentLevel;
    public int[] levelCash;
    public GameObject Controls;
    public GameObject bombDiffuseObject;
    public GameObject pausebutton;
    [HideInInspector] public bool allenemydead = false;
    [HideInInspector] public bool allbombdefused = false;
    public GameObject LastEnemyRemaining;
    public bool lastEnemy;
    public bool deactivateCurrentWeapon;
    public bool WaveForLevel;

    #endregion

    private void Awake()
    {
        #region Instantiate enemies/bombs according to their fixed count.

        currentLevel = GameManager.Instance.currentLevel - 1;
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            for (int i = 0; i < levelData[currentLevel].enemiesType.Count; i++)
            {
                GameObject g = Instantiate(enemyPrefab[CheckEnemyType(i)],
                    levelData[currentLevel].enemiesPositionFlag[i].transform.position,
                    levelData[currentLevel].enemiesPositionFlag[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform);
                CheckTotalEnemiesForWave();
            }
        }

        if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            BombInstantiate();
            for (int i = 0; i < levelData[currentLevel].enemiesType.Count; i++)

            {
                GameObject g = Instantiate(enemyPrefab[CheckEnemyType(i)],
                    levelData[currentLevel].enemiesPositionBomb[i].transform.position,
                    levelData[currentLevel].enemiesPositionBomb[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform);
                CheckTotalEnemiesForWave();
            }
        }


        if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            for (int i = 0; i < levelData[currentLevel].enemiesTypeBombPlant.Count; i++)
            {
                GameObject g = Instantiate(enemyPrefab[CheckEnemyType(i)],
                    levelData[currentLevel].enemiesPositionBombPlant[i].transform.position,
                    levelData[currentLevel].enemiesPositionBombPlant[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform);
                CheckTotalEnemiesForWave();
            }
        }

        if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            for (int i = 0; i < levelData[currentLevel].enemiesTypeHostageRescue.Count; i++)
            {
               

                GameObject g = Instantiate(enemyPrefab[CheckEnemyType(i)],
                    levelData[currentLevel].enemiesPositionHostageRescue[i].transform.position,
                    levelData[currentLevel].enemiesPositionHostageRescue[i].transform.rotation);
                levelData[currentLevel].totalEnemies.Add(g.transform);
                CheckTotalEnemiesForWave();
            }
        }

        for (int i = 0; i < levelData[currentLevel].enemiesWave; i++)
        {
            levelData[currentLevel].totalEnemies[i].gameObject.SetActive(true);
        }
        #endregion
    }
    #region Enemy Wave

    public void CheckTotalEnemiesForWave()
    {
        if (levelData[currentLevel].totalEnemies.Count > 3)
        {
            for (int j = 2;
                 j < levelData[currentLevel].totalEnemies.Count;
                 j++) // J == 2 so there are always 3 enemies in the first wave;
            {
                levelData[currentLevel].totalEnemies[j].gameObject.SetActive(false);
                GameObject k = levelData[currentLevel].totalEnemies[j].gameObject;
                levelData[currentLevel].EnemiesOnHold.Add(k.transform);
                levelData[currentLevel].totalEnemies.Remove(k.transform);
            }
        }
    }

    public void NextEnemy()
    {
        /*if (levelData[currentLevel].totalEnemies.Count < 0)
        {
            levelData[currentLevel].totalEnemies[0].gameObject.SetActive(true);
        }*/

        if (levelData[currentLevel].totalEnemies.Count == 1 && levelData[currentLevel].EnemiesOnHold.Count == 0)
        {
            LastEnemyRemaining.SetActive(true);
            lastEnemy = true;
        }
        else
        {
            if (PlayerPrefs.GetString("Mode") == "Flag" && levelData[currentLevel].totalEnemies.Count == 0 &&
                levelData[currentLevel].EnemiesOnHold.Count == 0)
            {
                LastEnemyRemaining.SetActive(false);
                lastEnemy = false;
                StartCoroutine(waitmethod(3.5f));
                GameManager.Instance.GameComplete(); //Level Completed Here
            }

            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse" &&
                     levelData[currentLevel].totalEnemies.Count == 0 &&
                     levelData[currentLevel].EnemiesOnHold.Count == 0)
            {
                LastEnemyRemaining.SetActive(false);
                lastEnemy = false;
                StartCoroutine(slowmo());
                allenemydead = true;
                bombdiffusecheck(); // Level Completed Here
            }
            else if (PlayerPrefs.GetString("Mode") == "BombPlant" && levelData[currentLevel].totalEnemies.Count == 0 &&
                     levelData[currentLevel].EnemiesOnHold.Count == 0)
            {
                BombPlantInstantiate();
                LastEnemyRemaining.SetActive(false);
                lastEnemy = false;
                StartCoroutine(slowmo());
                allenemydead = true;

                StartCoroutine(
                    HandsActivationTimer()); //Level Completed in bombplantscript>>BombPlantTimer()>>WaitBeforeExplosion()
            }
            else if (PlayerPrefs.GetString("Mode") == "HostageRescue" &&
                     levelData[currentLevel].totalEnemies.Count == 0 &&
                     levelData[currentLevel].EnemiesOnHold.Count == 0)
            {
                StartCoroutine(slowmo());
                allenemydead = true;
                GameManager.Instance.GameComplete();
            }
        }
    }

    #endregion

    #region BombPlant/BombDefuse

    public IEnumerator HandsActivationTimer()
    {
        yield return new WaitForSecondsRealtime(1);
        GameManager.Instance.activateBombPlantHands();
    }

    public void BombInstantiate()
    {
        for (int i = 0; i < levelData[currentLevel].bombposition.Count; i++)
        {
            GameObject bomb = Instantiate(bombDiffuseObject, levelData[currentLevel].bombposition[i].transform.position,
                levelData[currentLevel].bombposition[i].transform.rotation);
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
            levelData[currentLevel].indicators.Add(b.gameObject);
        }

        PlayerPrefs.SetInt("BombCountForLevel", levelData[currentLevel].bombPlantQuantity.Length);
        GameManager.Instance.throwGrenadeButton.gameObject.SetActive(false);
    }

    public void bombdiffusecheck()
    {
        if (allbombdefused && allenemydead)
        {
            Controls.SetActive(false);
            GameManager.Instance.GameComplete();
        }
    }

    private int defused = 0;

    public void checkallbombs()
    {
        defused++;
        GameManager.Instance.BombsDefused.text = defused.ToString();
        PlayerPrefs.SetInt("TotalBombs", defused + PlayerPrefs.GetInt("TotalBombs"));
        if (levelData[currentLevel].bombposition.Count == defused)
        {
            allbombdefused = true;
            bombdiffusecheck();
        }
    }

    #endregion

    #region SlowMotion

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
    }

    #endregion


    private int x;

    private int CheckEnemyType(int i)
    {
        if (PlayerPrefs.GetString("Mode") == "Flag" || PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            if (levelData[currentLevel].enemiesType[i] == EnemyType.A)
            {
                x = 0;
                return x;
            }

            if (levelData[currentLevel].enemiesType[i] == EnemyType.B)
            {
                x = 1;
                return x;
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            if (levelData[currentLevel].enemiesTypeBombPlant[i] == EnemyType.A)
            {
                x = 0;
                return x;
            }

            if (levelData[currentLevel].enemiesTypeBombPlant[i] == EnemyType.B)
            {
                x = 1;
                return x;
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            if (levelData[currentLevel].enemiesTypeHostageRescue[i] == EnemyType.A)
            {
                x = 0;
                return x;
            }

            if (levelData[currentLevel].enemiesTypeHostageRescue[i] == EnemyType.B)
            {
                x = 1;
                return x;
            }
        }

        return x;
    }
}

#region Serializable Enemy Class

[Serializable]
public class Enemy

{
    [Space(5)] [Header("PlayerPositions-------")] [Space(5)] [Space(2)]
    public Transform playerSpwanPosition;

    [Space(5)] [Header("Flag-Diamond/BombDiffuse------- (Mode1/2)")] [Space(2)]
    public int enemiesWave;

    [Space(2)] public List<EnemyType> enemiesType;
    [Space(2)] public List<Transform> enemiesPositionFlag;
    [Space(2)] public List<Transform> enemiesPositionBomb;
    [Space(2)] public List<Transform> bombposition;

    [Space(5)] [Header("BombPlant (Mode3)--------")] [Space(2)]
    public List<EnemyType> enemiesTypeBombPlant;

    [Space(2)] public List<Transform> enemiesPositionBombPlant;
    [Space(2)] public List<Transform> BombPlantPosition;
    [Space(2)] public List<GameObject> indicators;
    [Space(2)] public List<Transform> PlantedBombsInScene;
    public int[] bombPlantQuantity;

    [Space(5)] [Header("Hostage Rescue (Mode4)------")] [Space(2)]
    public List<EnemyType> enemiesTypeHostageRescue;

    [Space(2)] public List<Transform> enemiesPositionHostageRescue;


    [Space(5)] [Header("----Total Enemies/Enemies on hold")] [Space(2)]
    public List<Transform> totalEnemies;

    [Space(2)] public List<Transform> EnemiesOnHold;


    
}

#endregion


[Serializable]
public enum EnemyType
{
    A,
    B
}