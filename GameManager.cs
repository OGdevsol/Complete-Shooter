using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    #region GlobalInstance

    public static GameManager localInstance;

    public static GameManager Instance
    {
        get
        {
            localInstance = GameObject.FindObjectOfType<GameManager>();
            return localInstance;
        }
    }

    #endregion

    #region UserInterface

    [Space(5)] [Header("---- UI References")]
    public GameObject[] reference;
    public GameObject rankUpPanel;
    public Text rankUp;
    [Space(3)]
    public GameObject victoryEffect;
    public GameObject defeatEffect;
    public Text[] rankUpdTexts;
    [HideInInspector] public int currentLevel = 0;
    private int levelCount;
    private int selectedLevel;
    
    [Space(10)]
    public Text levelNumber;
    public Text totalKill;
    public Text BombsDefused;
    public Text totalbombs;
    public Text HeadshotsValue;

    
    [Space(5)] [Header("---- Scripts Reference")]
    public playercontroller playerController;
    private static int _levelCountStored;
    public GameObject AudioController;

    [Space(5)] [Header("---- Flag Collect")]
    public GameObject flagscollected;
    public Text flagscollectedtext;
    public GameObject FlagsGO; //Flags main game object that is going to be activated in flag mode to show total flags collected texts.
    [HideInInspector]
    public int flagcollectcounter = 0;
    

    [Space(5)] [Header("---- Bomb Diffuse")]
    public Text bombSliderValue;
    public Slider bombSlider;
    public GameObject bombDiffuseBar;

    public GameObject bombDiffuseStatus;
    public GameObject flagModeStatus;

   
    
    [Space(5)] [Header("---- Effects/Stats")]
    public GameObject HeadshotEffect;
    public GameObject blood;
    public GameObject Explosion;
    public Text TotalKills;
    public Text AccuracyValue;
    public Text FlagsCollectedVal;
    public GameObject FlagsCollectedtoDisableInbombDefuse;
    public string NA = "N/A";
    public GameObject FlagsCollectedMainText;
    public GameObject BombsDefusedGO;
    public Text TotalBombsDefusedVal;
    public GameObject Controls;
    public GameObject marker;
    public bool GameCompleteBool;
    public GameObject HeadShotText;
    public GameObject DiamondGO;
    public Text diamondCount;
    public GameObject PlusOneAnim;
    public Transform[] patrolNodesAssign;//Run time assignment in BaseScript.cs
    public GameObject[] weaponsToDeactivate;
    public GameObject BombPlantHands;
    public GameObject bombPlantPositionIndicator;
    public GameObject Bomb;
    public GameObject[] TemporaryDeactivations;
    public Camera ExplosionCam;
    public GameObject PlayerControllerWhole;
    public GameObject LoadingBarBombPlant;
    public Text LoadingBarBombPlantText;
    
    





    #endregion

    
    
    #region Initlization

    private void Awake()
    {
        
        AudioController.SetActive(true);
        DiamondGO.SetActive(true); 
        diamondCount.text = PlayerPrefs.GetInt("DiamondCount").ToString();

        Time.timeScale = 1;

        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            currentLevel = PlayerPrefs.GetInt("FlagLevel");
            flagModeStatus.SetActive(true);
            flagscollected.gameObject.SetActive(true);
            bombDiffuseStatus.SetActive(false);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            currentLevel = PlayerPrefs.GetInt("BombLevel");
            bombDiffuseStatus.SetActive(true);
            flagscollected.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            currentLevel = PlayerPrefs.GetInt("BombPlantLevel");
           
        }
        else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            currentLevel = PlayerPrefs.GetInt("HostageRescueLevel");
            bombDiffuseStatus.SetActive(true);
            flagscollected.gameObject.SetActive(false);
            
        }
        
        print("Current Level "+currentLevel);

        if (PlayerPrefs.GetString("Mode")=="Flag" && currentLevel<=30 || PlayerPrefs.GetString("Mode")=="BombDiffuse" && currentLevel<=30 ) 
        {
            playerController.transform.position =
                LevelsController.Instance.levelData[currentLevel - 1].playerSpwanPosition.position;
            playerrotate.Instance.rotationX = LevelsController.Instance.levelData[currentLevel - 1]
                .playerSpwanPosition.eulerAngles.y;
            weaponselector.Instance.grenade = 100;

            totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesType.Count.ToString();
            totalbombs.text = LevelsController.Instance.levelData[currentLevel - 1].bombposition.Count.ToString();
        
            EnableReference(0);
            levelNumber.text = currentLevel.ToString();
            PlayerPrefs.SetInt("TotalFlags", PlayerPrefs.GetInt("TotalFlags"));
        }
        else if(PlayerPrefs.GetString("Mode")=="Flag" && currentLevel>=30 || PlayerPrefs.GetString("Mode")=="BombDiffuse" && currentLevel>=30)
        {
            
            currentLevel = 30;
            playerController.transform.position =
                LevelsController.Instance.levelData[currentLevel - 1].playerSpwanPosition.position;
            playerrotate.Instance.rotationX = LevelsController.Instance.levelData[currentLevel - 1]
                .playerSpwanPosition.eulerAngles.y;
            weaponselector.Instance.grenade = 100;

            totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesType.Count.ToString();
            totalbombs.text = LevelsController.Instance.levelData[currentLevel - 1].bombposition.Count.ToString();
        
            EnableReference(0);
            levelNumber.text = currentLevel.ToString();
            PlayerPrefs.SetInt("TotalFlags", PlayerPrefs.GetInt("TotalFlags"));
            
        }
        else if(PlayerPrefs.GetString("Mode")=="BombPlant" || PlayerPrefs.GetString("Mode")=="HostageRescue")
        {
           
            playerController.transform.position =
                LevelsController.Instance.levelData[currentLevel - 1].playerSpwanPosition.position;
            playerrotate.Instance.rotationX = LevelsController.Instance.levelData[currentLevel - 1]
                .playerSpwanPosition.eulerAngles.y;
            weaponselector.Instance.grenade = 100;
            totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesType.Count.ToString();
            totalbombs.text = LevelsController.Instance.levelData[currentLevel - 1].bombPlantQuantity.Length.ToString();
            levelNumber.text = currentLevel.ToString();
            EnableReference(0);
            levelNumber.text = currentLevel.ToString();
        }
        
 #if !UNITY_EDITOR

        if(Analyticsmanager.instance)
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                Analyticsmanager.instance.LevelStartEvent("FML", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                Analyticsmanager.instance.LevelStartEvent("BML", currentLevel);
            }
        }


        if (!AdsController.Instance) return;
        
        AdsController.Instance.gameplay = true;
        AdsController.Instance.HideBanner();
        AdsController.Instance.HideLargeBanner();
         #endif
        
}

    private void EnableReference(int index)
    {
        for (int i = 0; i < reference.Length; i++)
        {
            reference[i].SetActive(false);
        }
        reference[index].SetActive(true);
        
    }
    
    #endregion
    
    #region State Methods

    public void Restart()
    {
        SceneManager.LoadScene("03 GamePlay");
    }

    public void Pause()
    {
        EnableReference(1);
        Time.timeScale = 0;
        LevelsController.Instance.LastEnemyRemaining.SetActive(false);
        AdsController.Instance.Unity_InterstitialGame();
    }
    
    public void Resume()
    {
        Time.timeScale = 1;
        EnableReference(0);
        if (LevelsController.Instance.lastEnemy)
        {
            LevelsController.Instance.LastEnemyRemaining.SetActive(true);
        }
    }
    
    public void Home()
    {
        StartCoroutine(LoadAsynchronously(1));
    }
    
    public void Next()
    {
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            if (PlayerPrefs.GetInt("FlagLevel") >= 30)
            {
                PlayerPrefs.SetInt("FlagLevel",1); 
            }
            else
            {
                PlayerPrefs.SetInt("FlagLevel",currentLevel+1); 
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            if (PlayerPrefs.GetInt("BombLevel") >= 30)
            {
                PlayerPrefs.SetInt("BombLevel",1); 
            }
            else
            {
                PlayerPrefs.SetInt("BombLevel",currentLevel+1);
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            if (PlayerPrefs.GetInt("BombPlantLevel") >= 15)
            {
                PlayerPrefs.SetInt("BombPlantLevel",1); 
            }
            else
            {
                PlayerPrefs.SetInt("BombPlantLevel",currentLevel+1);
            }
        }
        StartCoroutine(LoadAsynchronously(2));
    }
    public void Reset()
    {
        InGameProperties.Instance.SfxVolumeOn();
        InGameProperties.Instance.MusicVoluneOn();
        InGameProperties.Instance.AutoShoot_Off();
    }
    
    #endregion
    
    #region Loading
    
    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        EnableReference(4);
        yield return new WaitForSecondsRealtime(2.5f);
        SceneManager.LoadScene(sceneIndex);
    }

    #endregion
    
    #region Victory / Defeat!
    
    private  void FlagStats()
    {
        if (PlayerPrefs.HasKey("FlagLevelPurchased"))
        {
            PlayerPrefs.SetInt("SelectedFlagLevel", 29);
            return;
        }
        int curr = PlayerPrefs.GetInt("UnlockFlag");
      
        if (currentLevel > curr && currentLevel<=30 )
        {
            PlayerPrefs.SetInt("UnlockFlag", (curr + 1));
        }
        selectedLevel = PlayerPrefs.GetInt("UnlockFlag");
        if (currentLevel < 30)
        {
            PlayerPrefs.SetInt("SelectedFlagLevel",selectedLevel);
        }

    }
    public void activateBombPlantHands()
    {
        for (int i = 0; i < weaponsToDeactivate.Length; i++)
        {
            weaponsToDeactivate[i].SetActive(false);
            BombPlantHands.SetActive(true);
        }
    }
 
    
    private  void BombDiffuseStats()
    {
        if (PlayerPrefs.HasKey("BombLevelPurchased"))
        {
            PlayerPrefs.SetInt("SelectedBombLevel",29);
            return;
        }
        int curr = PlayerPrefs.GetInt("UnlockBomb");
        Debug.Log("int curr ==  "+ curr);
        Debug.Log("PlayerprefUnlockBomb==  "+ PlayerPrefs.GetInt("UnlockBomb"));
        Debug.Log("Current Level ===  "+ currentLevel);
        if (currentLevel > curr && currentLevel<=30)
            
        {
            PlayerPrefs.SetInt("UnlockBomb", (curr + 1));
        }
        selectedLevel = PlayerPrefs.GetInt("UnlockBomb");
        if (currentLevel < 30)
        {
            PlayerPrefs.SetInt("SelectedBombLevel",selectedLevel);
        }

    }
    public void GameComplete()
    {
        Controls.SetActive(false);
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            FlagStats();
            FlagsCollectedMainText.SetActive(true);
                
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            BombDiffuseStats();
            FlagsCollectedMainText.SetActive(false);


        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            for (int i = 0; i < TemporaryDeactivations.Length; i++)
            {
                TemporaryDeactivations[i].SetActive(false);
            }


        }
        StartCoroutine(VictoryPanel());
        PlayerPrefs.SetInt("Cash",PlayerPrefs.GetInt("Cash")+LevelsController.Instance.levelCash[currentLevel-1]);
        
    }
    public  IEnumerator VictoryPanel()
    {
        yield return new WaitForSecondsRealtime(0.25f);
        AccuracyCheck();
        victoryEffect.SetActive(true);
        SoundController.instance.playFromPool(AudioType.LevelComplete);
        ModeStats();
        SoundController.instance.audioMusic.enabled = false;
        yield return new WaitForSeconds(3f);
        victoryEffect.SetActive(false);
        SoundController.instance.playFromPool(AudioType.LevelComplete);
        RankUpSystem();
        if(Analyticsmanager.instance)
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                Analyticsmanager.instance.LevelCompleteEvent("FML", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                Analyticsmanager.instance.LevelCompleteEvent("BML", currentLevel);
            }
        }
        
        if(AdsController.Instance)
        {
            AdsController.Instance.Unity_InterstitialGame();
        }

    }

    public void AccuracyCheck()
    {
        if (PlayerPrefs.GetString("Mode")=="Flag" && PlayerPrefs.GetFloat("AccuracyFlag")>60.0 || PlayerPrefs.GetString("Mode")=="BombDiffuse" && PlayerPrefs.GetFloat("AccuracyBomb")>60.0)
        {
            AccuracyValue.color=Color.green;
            
        }
        else if(PlayerPrefs.GetString("Mode")=="Flag" && PlayerPrefs.GetFloat("AccuracyFlag")<30 || PlayerPrefs.GetString("Mode")=="BombDiffuse" && PlayerPrefs.GetFloat("AccuracyBomb")<30)
        {
            AccuracyValue.color=Color.red;
        }  
    }

    public void ModeStats()
    {
        if (PlayerPrefs.GetString("Mode")=="Flag")
        {
            PlayerPrefs.SetFloat("AccuracyFlag", PlayerPrefs.GetFloat("EnemyHitFlag") / PlayerPrefs.GetFloat("ShotsFiredFlag")*100);
            HeadshotsValue.text = PlayerPrefs.GetFloat("HeadshotCountFlag").ToString();
            TotalKills.text = PlayerPrefs.GetFloat("EnemiesKilledFlag").ToString();
            AccuracyValue.text = PlayerPrefs.GetFloat("AccuracyFlag").ToString();
            BombsDefusedGO.SetActive(false); 
        }
        else if (PlayerPrefs.GetString("Mode")=="BombDiffuse")
        {
            PlayerPrefs.SetFloat("AccuracyBomb", PlayerPrefs.GetFloat("EnemyHitBomb") / PlayerPrefs.GetFloat("ShotsFiredBomb")*100);
            HeadshotsValue.text = PlayerPrefs.GetFloat("HeadshotCountBomb").ToString();
            TotalKills.text = PlayerPrefs.GetFloat("EnemiesKilledBomb").ToString();
            AccuracyValue.text = PlayerPrefs.GetFloat("AccuracyBomb").ToString();
            FlagsCollectedtoDisableInbombDefuse.SetActive(false);
            TotalBombsDefusedVal.text = PlayerPrefs.GetInt("TotalBombs").ToString();
        }
        else if (PlayerPrefs.GetString("Mode")=="BombPlant")
        {
            HeadshotsValue.text = 0.ToString(); 
            TotalKills.text = 0.ToString(); 
            AccuracyValue.text = 0.ToString(); 
            FlagsCollectedtoDisableInbombDefuse.SetActive(false);
            TotalBombsDefusedVal.text = 0.ToString();
            BombsDefusedGO.SetActive(false); 
        }
        
    }
    public void GameFail()
    {
        StartCoroutine(DefeatPanel());
    }
    
    private IEnumerator DefeatPanel()
    {
        yield return new WaitForSeconds(0.2f);
        SoundController.instance.playFromPool(AudioType.LevelFail);
        LevelsController.Instance.LastEnemyRemaining.SetActive(false);
        LevelsController.Instance.lastEnemy = false;
        defeatEffect.SetActive(true);
        yield return new WaitForSeconds(3f);
        defeatEffect.SetActive(false);
        EnableReference(3);
        Time.timeScale = 0;
        if(Analyticsmanager.instance)
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                Analyticsmanager.instance.LevelFailedEvent("FML", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                Analyticsmanager.instance.LevelFailedEvent("BML", currentLevel);
            }
        }
        
        if(AdsController.Instance)
        {
            AdsController.Instance.Unity_InterstitialGame();
        }
    }

    #endregion

    public IEnumerator PlusOneAnimation() //HeadHitbox Event >> Hitbox.cs script, for +1 animation of diamond
    {
        PlusOneAnim.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        PlusOneAnim.SetActive(false);
    }
    public IEnumerator HeadshotEffectText()
    {
        HeadShotText.SetActive(true);
        yield return new WaitForSecondsRealtime(1.5f);
        HeadShotText.SetActive(false);
    }

    #region Rankup

    private int rankUpValue;
    private void RankUpSystem()
    {
        if (PlayerPrefs.GetInt("TotalBodyShoot") % 5 == 0 && PlayerPrefs.GetString("Mode") == "Flag")
        {
            PlayerPrefs.SetInt("RankUp", PlayerPrefs.GetInt("RankUp") + 1);
            rankUpValue = PlayerPrefs.GetInt("RankUp");
            SoundController.instance.playFromPool(AudioType.Realod);
            rankUpPanel.SetActive(true);
            rankUp.text = $"{rankUpValue}";
            RankUpdTags();
            StartCoroutine(RankUpDeactivate());
           
        }
        else
        {
            EnableReference(2);
            Time.timeScale = 0;
        }
    }

    public IEnumerator RankUpDeactivate()
    {
        yield return new WaitForSeconds(2f);
        rankUpPanel.SetActive(false);
        EnableReference(2);
        Time.timeScale = 0;
    }

    private void RankUpdTags()
    {
        switch (rankUpValue)
        {
            case 1:
                rankUpdTexts[0].text = rankUpdTexts[1].text = $"BRONZE";
                break;
            case 2:
                rankUpdTexts[0].text = rankUpdTexts[1].text = $"SILVER RANK";
                break;
            case 3:
                rankUpdTexts[0].text = rankUpdTexts[1].text = $"GOLD RANK";
                break;
            case 4:
                rankUpdTexts[0].text = rankUpdTexts[1].text = $"PLATINUM RANK";
                break;
            default:
                rankUpdTexts[0].text = rankUpdTexts[1].text = $"DIAMOND RANK";
                break;
        }
    }
    
    

    #endregion

   
}
/*var async = SceneManager.LoadSceneAsync(sceneIndex);
       while (!async.isDone)
       {
           float progress = Mathf.Clamp01(async.progress / .9f);
           // LoadingBar.fillAmount = progress;
           // LoadingProgress.text = (progress * 100f).ToString("0") + "%";
           yield return null;
       }*/