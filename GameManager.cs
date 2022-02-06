using System.Collections;
using System.Collections.Generic;
using GoogleMobileAds.Api;
/*using UnityEditor.PackageManager;*/
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
    [Space(3)] public GameObject victoryEffect;
    public GameObject defeatEffect;
    public Text[] rankUpdTexts;
    [HideInInspector] public int currentLevel = 0;
    private int levelCount;
    private int selectedLevel;

    [Space(10)] public Text levelNumber;
    public Text totalKill;
    public Text BombsDefused;
    public Text totalbombs;
    public Text HeadshotsValue;
    public GameObject YOULOSEpanel;
    public GameObject YOUWINpanel;
    public GameObject YOULOSEeffect;
    public GameObject YOUWINEffect;

    public GameObject pausePanel;
    public GameObject pauseButton;


    [Space(5)] [Header("---- Scripts Reference")]
    public playercontroller playerController;

    private static int _levelCountStored;
    public GameObject AudioController;

    [Space(5)] [Header("---- Flag Collect")]
    public Text flagscollectedtext;

    public GameObject
        FlagsGO; //Flags main game object that is going to be activated in flag mode to show total flags collected texts.

    [HideInInspector] public int flagcollectcounter = 0;
    public GameObject DiamondGameObject;


    [Space(5)] [Header("---- Bomb Diffuse")]
    public Text bombSliderValue;

    public Slider bombSlider;
    public GameObject bombDiffuseBar;
    public GameObject bombDiffuseStatus;
    public GameObject flagModeStatus;

    [Space(5)] [Header("---- Bomb Plant")] public GameObject BombPlantHands;
    public GameObject bombPlantPositionIndicator;
    public GameObject Bomb;
    public Text LoadingBarBombPlantText;
    public Text BombsToBePlanted;
    public Text BombsPlantedInLevel;

    [Space(5)] [Header("---- Hostage Rescue")]
    public Text TotalEnemies;


    [Space(5)] [Header("---- Stats")] public GameObject TotalDiamondsCollected;
    public GameObject FlagsCollectedMainText;
    public GameObject BombsDefusedGO;
    public string NA = "N/A";
    public GameObject HeadShotText;
    public GameObject DiamondGO;
    public GameObject bombPlantedStats;
    public Text TotalBombsPlantedVal;
    public Text totalDiamondsCollectedVal;
    public Text TotalKills;
    public Text AccuracyValue;
    public Text TotalBombsDefusedVal;
    public Text diamondCount;


    [Space(5)] [Header("---- Effects")] public GameObject HeadshotEffect;
    public GameObject blood;
    public GameObject Explosion;
    public GameObject FlagsCollectedtoDisableInbombDefuse;
    public GameObject PlusOneAnim;
    public Camera ExplosionCam;
    public Button throwGrenadeButton;
    public Transform[] ExplosionCamsTransforms;
    public Text grenadeCount;

    [Space(5)] [Header("---- Misc")] public GameObject Controls;
    public Transform[] patrolNodesAssign; //Runtime assignment in BaseScript.cs
    public GameObject[] weaponsToDeactivate;
    public GameObject PlayerControllerWhole;
    public GameObject LoadingBarBombPlant;
    public GameObject miniMapBombsPlanted;
    public GameObject HitMarker;
    public GameObject resumeButton;

    #endregion

    #region Initlization

    private void Awake()
    {
        Time.timeScale = 1;
        GeneralObjectsActivation();
        ActivateModesUI();
        PlayerPosAndUIAssign();

        #region Ads/Analytics

        if (Analyticsmanager.instance)
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                Analyticsmanager.instance.LevelStartEvent("M1L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                Analyticsmanager.instance.LevelStartEvent("M2L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombPlant")
            {
                Analyticsmanager.instance.LevelStartEvent("M3L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
            {
                Analyticsmanager.instance.LevelStartEvent("M4L", currentLevel);
            }
        }

        if (!AdsController.Instance) return;

        AdsController.Instance.gameplay = true;
        AdsController.Instance.HideBanner();
        AdsController.Instance.HideLargeBanner();

        #endregion
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

   private IEnumerator wait()
    {
        resumeButton.SetActive(false);
        yield return new WaitForSecondsRealtime(0.5f);
        AdsController.Instance.Unity_InterstitialGame();
        yield return new WaitForSecondsRealtime(0.5f);
        resumeButton.SetActive(true); // Pause panel resume button to appear after 1 second wait to deter monkey testing
    }

    #region Modes' Minimap/UI Activations/Deactivations

    private void ActivateModesUI()
    {
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            currentLevel = PlayerPrefs.GetInt("FlagLevel");
            bombDiffuseStatus.SetActive(false);
            bombPlantedStats.SetActive(false);
            miniMapBombsPlanted.SetActive(false);
            flagModeStatus.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            currentLevel = PlayerPrefs.GetInt("BombLevel");
            bombPlantedStats.SetActive(false);
            miniMapBombsPlanted.SetActive(false);
            bombDiffuseStatus.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            currentLevel = PlayerPrefs.GetInt("BombPlantLevel");
            bombDiffuseStatus.SetActive(false);
            bombPlantedStats.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            currentLevel = PlayerPrefs.GetInt("HostageRescueLevel");
            bombDiffuseStatus.SetActive(false);
            bombPlantedStats.SetActive(false);
            miniMapBombsPlanted.SetActive(false);
        }
    }

    #endregion

    #region Modes' Player Positions/UI Assignment

    public void PlayerPosAndUIAssign()
    {
        if (PlayerPrefs.GetString("Mode") == "Flag" && currentLevel <= 15 ||
            PlayerPrefs.GetString("Mode") == "BombDiffuse" && currentLevel <= 15 ||
            PlayerPrefs.GetString("Mode") == "BombPlant" && currentLevel <= 15 ||
            PlayerPrefs.GetString("Mode") == "HostageRescue" && currentLevel <= 15)
        {
            //if (playerController!=null && LevelsController.Instance)
            // {
            playerController.transform.position =
                LevelsController.Instance.levelData[currentLevel - 1].playerSpwanPosition.position;
            playerrotate.Instance.rotationX = LevelsController.Instance.levelData[currentLevel - 1]
                .playerSpwanPosition.eulerAngles.y;

            // }

            PlayerPrefs.GetInt("GrenadeCount");
            if (PlayerPrefs.GetString("Mode") == "BombDiffuse" || PlayerPrefs.GetString("Mode") == "Flag")
            {
                totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesType.Count.ToString();
                totalbombs.text = LevelsController.Instance.levelData[currentLevel - 1].bombposition.Count.ToString();
            }

            if (PlayerPrefs.GetString("Mode") == "BombPlant")
            {
                totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesTypeBombPlant.Count
                    .ToString();
                BombsToBePlanted.text = LevelsController.Instance.levelData[currentLevel - 1].bombPlantQuantity.Length
                    .ToString();
            }

            if (PlayerPrefs.GetString("Mode") == "HostageRescue")
            {
                totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesTypeHostageRescue.Count
                    .ToString();
                miniMapBombsPlanted.SetActive(false);
            }

            /*totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesType.Count.ToString();
            totalbombs.text = LevelsController.Instance.levelData[currentLevel - 1].bombposition.Count.ToString();*/

            EnableReference(0);
            levelNumber.text = currentLevel.ToString();
            PlayerPrefs.SetInt("TotalFlags", PlayerPrefs.GetInt("TotalFlags"));
        }
        else if (PlayerPrefs.GetString("Mode") == "Flag" && currentLevel >= 15 ||
                 PlayerPrefs.GetString("Mode") == "BombDiffuse" && currentLevel >= 15 ||
                 PlayerPrefs.GetString("Mode") == "Flag" && currentLevel <= 0 ||
                 PlayerPrefs.GetString("Mode") == "BombDiffuse" && currentLevel <= 0)
        {
            currentLevel = 1;
            playerController.transform.position =
                LevelsController.Instance.levelData[currentLevel - 1].playerSpwanPosition.position;
            playerrotate.Instance.rotationX = LevelsController.Instance.levelData[currentLevel - 1]
                .playerSpwanPosition.eulerAngles.y;


            totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesType.Count.ToString();
            totalbombs.text = LevelsController.Instance.levelData[currentLevel - 1].bombposition.Count.ToString();

            EnableReference(0);
            levelNumber.text = currentLevel.ToString();
            PlayerPrefs.SetInt("TotalFlags", PlayerPrefs.GetInt("TotalFlags"));
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant" && currentLevel >= 15 ||
                 PlayerPrefs.GetString("Mode") == "HostageRescue" && currentLevel >= 15 ||
                 PlayerPrefs.GetString("Mode") == "BombPlant" && currentLevel <= 0 ||
                 PlayerPrefs.GetString("Mode") == "HostageRescue" && currentLevel <= 0)
        {
            currentLevel = 1;
            playerController.transform.position =
                LevelsController.Instance.levelData[currentLevel - 1].playerSpwanPosition.position;
            playerrotate.Instance.rotationX = LevelsController.Instance.levelData[currentLevel - 1]
                .playerSpwanPosition.eulerAngles.y;
            weaponselector.Instance.grenade = PlayerPrefs.GetInt("GrenadeCount");
            levelNumber.text = currentLevel.ToString();
            totalKill.text = LevelsController.Instance.levelData[currentLevel - 1].enemiesTypeBombPlant.Count
                .ToString();
            BombsToBePlanted.text = LevelsController.Instance.levelData[currentLevel - 1].bombPlantQuantity.Length
                .ToString();
            EnableReference(0);
        }
    }

    #endregion

    // Grenade Count PlayerPref is being set here 

    #region Enable general objects/objects that were disabled

    private void GeneralObjectsActivation()
    {
        AudioController.SetActive(true);
        DiamondGO.SetActive(true);
        diamondCount.text = PlayerPrefs.GetInt("DiamondCount").ToString();
        PlayerPrefs.SetInt("GrenadeCount", 10);
        weaponselector.Instance.grenade = PlayerPrefs.GetInt("GrenadeCount");
        grenadeCount.text = PlayerPrefs.GetInt("GrenadeCount").ToString();
        Time.timeScale = 1;
    }

    #endregion

    #region Game States

    public void Restart()
    {
        SceneManager.LoadScene("03 GamePlay");
        // SceneManager.LoadSceneAsync("03 GamePlay");
    }

    public void Pause()
    {
        EnableReference(1);
        if (YOUWINpanel.activeInHierarchy || YOUWINEffect.activeInHierarchy)
        {
            pausePanel.SetActive(false);
        }
        else
        {
            pausePanel.SetActive(true);
            Time.timeScale = 0;
            LevelsController.Instance.LastEnemyRemaining.SetActive(false);
            StartCoroutine(wait());
            if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                if (BombDiffuse.instance.IsInRange)
                {
                    BombDiffuse.instance.timer.Stop();
                }
            }
        }
    }

    public void Resume()
    {
        Time.timeScale = 1;
        EnableReference(0);
        if (LevelsController.Instance.lastEnemy)
        {
            LevelsController.Instance.LastEnemyRemaining.SetActive(true);
        }

        if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            if (BombDiffuse.instance.IsInRange)
            {
                BombDiffuse.instance.timer.Play();
            }
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
                PlayerPrefs.SetInt("FlagLevel", 1);
            }
            else
            {
                PlayerPrefs.SetInt("FlagLevel", currentLevel + 1);
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            if (PlayerPrefs.GetInt("BombLevel") >= 30)
            {
                PlayerPrefs.SetInt("BombLevel", 1);
            }
            else
            {
                PlayerPrefs.SetInt("BombLevel", currentLevel + 1);
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            if (PlayerPrefs.GetInt("BombPlantLevel") >= 15)
            {
                PlayerPrefs.SetInt("BombPlantLevel", 1);
            }
            else
            {
                PlayerPrefs.SetInt("BombPlantLevel", currentLevel + 1);
            }
        }
        else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            if (PlayerPrefs.GetInt("HostageRescueLevel") >= 15)
            {
                PlayerPrefs.SetInt("HostageRescueLevel", 1);
            }
            else
            {
                PlayerPrefs.SetInt("HostageRescueLevel", currentLevel + 1);
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
        yield return new WaitForSecondsRealtime(1.5f);
        SceneManager.LoadScene(sceneIndex);
    }

    #endregion

    #region BombPlantHands Activation

    public void activateBombPlantHands()
    {
        for (int i = 0; i < weaponsToDeactivate.Length; i++)
        {
            weaponsToDeactivate[i].SetActive(false);
            BombPlantHands.SetActive(true);
        }
    }

    #endregion

    #region Victory / Defeat!

    private void FlagStats()
    {
        if (PlayerPrefs.HasKey("FlagLevelPurchased"))
        {
            PlayerPrefs.SetInt("SelectedFlagLevel", 14);
            return;
        }

        int curr = PlayerPrefs.GetInt("UnlockFlag");

        if (currentLevel > curr && currentLevel <= 15)
        {
            PlayerPrefs.SetInt("UnlockFlag", (curr + 1));
        }

        selectedLevel = PlayerPrefs.GetInt("UnlockFlag");
        if (currentLevel < 15)
        {
            PlayerPrefs.SetInt("SelectedFlagLevel", selectedLevel);
        }
    }

    private void BombDiffuseStats()
    {
        if (PlayerPrefs.HasKey("BombLevelPurchased"))
        {
            PlayerPrefs.SetInt("SelectedBombLevel", 14);
            return;
        }

        int curr = PlayerPrefs.GetInt("UnlockBomb");
        if (currentLevel > curr && currentLevel <= 15)
        {
            PlayerPrefs.SetInt("UnlockBomb", (curr + 1));
        }

        selectedLevel = PlayerPrefs.GetInt("UnlockBomb");
        if (currentLevel < 15)
        {
            PlayerPrefs.SetInt("SelectedBombLevel", selectedLevel);
        }
    }

    private void BombPlantStats()
    {
        if (PlayerPrefs.HasKey("BombPlantLevelPurchased"))
        {
            PlayerPrefs.SetInt("SelectedBombPlantLevel", 14);
            return;
        }

        int curr = PlayerPrefs.GetInt("UnlockBombPlant");
        if (currentLevel > curr && currentLevel <= 15)
        {
            PlayerPrefs.SetInt("UnlockBombPlant", (curr + 1));
        }

        selectedLevel = PlayerPrefs.GetInt("UnlockBombPlant");
        if (currentLevel < 15)
        {
            PlayerPrefs.SetInt("SelectedBombPlantLevel", selectedLevel);
        }
    }

    private void HostageRescueStats()
    {
        if (PlayerPrefs.HasKey("HostageRescueLevelPurchased"))
        {
            PlayerPrefs.SetInt("SelectedHostageRescueLevel", 14);
            return;
        }

        int curr = PlayerPrefs.GetInt("UnlockHostageRescue");
        if (currentLevel > curr && currentLevel <= 15)
        {
            PlayerPrefs.SetInt("UnlockHostageRescue", (curr + 1));
        }

        selectedLevel = PlayerPrefs.GetInt("UnlockHostageRescue");
        if (currentLevel < 15)
        {
            PlayerPrefs.SetInt("SelectedHostageRescueLevel", selectedLevel);
        }
    }

    public void GameComplete()
    {
        isComplete=true;
        Controls.SetActive(false);
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            FlagStats();
            FlagsCollectedMainText.SetActive(false);
            TotalDiamondsCollected.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            BombDiffuseStats();
            FlagsCollectedMainText.SetActive(false);
            BombsDefusedGO.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            BombPlantStats();
            FlagsCollectedMainText.SetActive(false);
            bombPlantedStats.SetActive(true);
        }
        else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            HostageRescueStats();
            FlagsCollectedMainText.SetActive(false);
        }

        StartCoroutine(VictoryPanel());
        PlayerPrefs.SetInt("Cash", PlayerPrefs.GetInt("Cash") + LevelsController.Instance.levelCash[currentLevel - 1]);
    }

    private IEnumerator VictoryPanel()
    {
        
         Controls.SetActive(false);
       // PlayerControllerWhole.SetActive(false);
         yield return new WaitForSecondsRealtime(0.30f);
        
            victoryEffect.SetActive(true);
            LevelsController.Instance.LastEnemyRemaining.SetActive(false);
            SoundController.instance.playFromPool(AudioType.LevelComplete);
            ModeStats();
            SoundController.instance.audioMusic.enabled = false;
            yield return new WaitForSeconds(3f);
            victoryEffect.SetActive(false);
            SoundController.instance.playFromPool(AudioType.LevelComplete);
            RankUpSystem();
        
        
       

        #region Ads/Analytics

        if (Analyticsmanager.instance)
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                Analyticsmanager.instance.LevelCompleteEvent("M1L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                Analyticsmanager.instance.LevelCompleteEvent("M2L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombPlant")
            {
                Analyticsmanager.instance.LevelCompleteEvent("M3L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
            {
                Analyticsmanager.instance.LevelCompleteEvent("M4L", currentLevel);
            }
        }

        if (AdsController.Instance)
        {
            AdsController.Instance.Unity_InterstitialGame();
        }

        #endregion
    }

    private bool isComplete;
    public void GameFail()
    {

        if (!isComplete)
        {
            StartCoroutine(DefeatPanel());
        }
       
    }

    private IEnumerator DefeatPanel()
    {
        playercontroller.instance.playerDead = true;
        yield return new WaitForSeconds(0.55f);
       
            
            SoundController.instance.playFromPool(AudioType.LevelFail);
            defeatEffect.SetActive(true);
            yield return new WaitForSeconds(3f);
            defeatEffect.SetActive(false);
            EnableReference(3);
            Time.timeScale = 0;
        
       
        LevelsController.Instance.LastEnemyRemaining.SetActive(false);
        LevelsController.Instance.lastEnemy = false;


        #region Ads/Analytics

        if (Analyticsmanager.instance)
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                Analyticsmanager.instance.LevelFailedEvent("M1L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                Analyticsmanager.instance.LevelFailedEvent("M2L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "BombPlant")
            {
                Analyticsmanager.instance.LevelFailedEvent("M3L", currentLevel);
            }
            else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
            {
                Analyticsmanager.instance.LevelFailedEvent("M4L", currentLevel);
            }
        }

        if (AdsController.Instance)
        {
            AdsController.Instance.Unity_InterstitialGame();
        }

        #endregion
    }

    #endregion

    #region Modes' Player Statistics

    public void ModeStats()
    {
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            PlayerPrefs.SetFloat("AccuracyFlag",
                PlayerPrefs.GetFloat("EnemyHitFlag") / PlayerPrefs.GetFloat("ShotsFiredFlag") * 100);
            HeadshotsValue.text = PlayerPrefs.GetFloat("HeadshotCountFlag").ToString();
            TotalKills.text = PlayerPrefs.GetFloat("EnemiesKilledFlag").ToString();
            AccuracyValue.text = PlayerPrefs.GetFloat("AccuracyFlag").ToString();
            AccuracyCheck();
            totalDiamondsCollectedVal.text = PlayerPrefs.GetInt("DiamondCount").ToString();
            BombsDefusedGO.SetActive(false);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            PlayerPrefs.SetFloat("AccuracyBomb",
                PlayerPrefs.GetFloat("EnemyHitBomb") / PlayerPrefs.GetFloat("ShotsFiredBomb") * 100);
            HeadshotsValue.text = PlayerPrefs.GetFloat("HeadshotCountBomb").ToString();
            TotalKills.text = PlayerPrefs.GetFloat("EnemiesKilledBomb").ToString();
            AccuracyValue.text = PlayerPrefs.GetFloat("AccuracyBomb").ToString();
            AccuracyCheck();
            FlagsCollectedtoDisableInbombDefuse.SetActive(false);
            TotalBombsDefusedVal.text = PlayerPrefs.GetInt("TotalBombs").ToString();
            bombPlantedStats.SetActive(false);
            TotalDiamondsCollected.SetActive(false);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombPlant")
        {
            PlayerPrefs.SetFloat("AccuracyBombPlant",
                PlayerPrefs.GetFloat("EnemyHitBombPlant") / PlayerPrefs.GetFloat("ShotsFiredBombPlant") * 100);
            HeadshotsValue.text = PlayerPrefs.GetFloat("HeadshotCountBombPlant").ToString();
            TotalKills.text = PlayerPrefs.GetFloat("EnemiesKilledBombPlant").ToString();
            AccuracyValue.text = PlayerPrefs.GetFloat("AccuracyBombPlant").ToString();
            AccuracyCheck();
            FlagsCollectedtoDisableInbombDefuse.SetActive(false);
            BombsDefusedGO.SetActive(false);
            TotalBombsPlantedVal.text = PlayerPrefs.GetInt("TotalBombsPlantedVal").ToString();
            TotalDiamondsCollected.SetActive(false);
        }
        else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
        {
            PlayerPrefs.SetFloat("AccuracyHostageRescue",
                PlayerPrefs.GetFloat("EnemyHitHostageRescue") / PlayerPrefs.GetFloat("ShotsFiredHostageRescue") * 100);
            HeadshotsValue.text = PlayerPrefs.GetFloat("HeadshotCountHostageRescue").ToString();
            TotalKills.text = PlayerPrefs.GetFloat("EnemiesKilledHostageRescue").ToString();
            AccuracyValue.text = PlayerPrefs.GetFloat("AccuracyHostageRescue").ToString();
            AccuracyCheck();
            FlagsCollectedtoDisableInbombDefuse.SetActive(false);
            BombsDefusedGO.SetActive(false);
        }
    }

    private void AccuracyCheck()
    {
        if (PlayerPrefs.GetString("Mode") == "Flag" && PlayerPrefs.GetFloat("AccuracyFlag") > 60.0 ||
            PlayerPrefs.GetString("Mode") == "BombDiffuse" && PlayerPrefs.GetFloat("AccuracyBomb") > 60.0 ||
            PlayerPrefs.GetString("Mode") == "BombPlant" && PlayerPrefs.GetFloat("AccuracyBombPlant") > 60.0 ||
            PlayerPrefs.GetString("Mode") == "HostageRescue" && PlayerPrefs.GetFloat("AccuracyHostageRescue") > 60.0)
        {
            AccuracyValue.color = Color.green;
        }
        else if (PlayerPrefs.GetString("Mode") == "Flag" && PlayerPrefs.GetFloat("AccuracyFlag") < 30 ||
                 PlayerPrefs.GetString("Mode") == "BombDiffuse" && PlayerPrefs.GetFloat("AccuracyBomb") < 30 ||
                 PlayerPrefs.GetString("Mode") == "BombPlant" && PlayerPrefs.GetFloat("AccuracyBombPlant") < 30 ||
                 PlayerPrefs.GetString("Mode") == "HostageRescue" && PlayerPrefs.GetFloat("AccuracyHostageRescue") < 30)
        {
            AccuracyValue.color = Color.red;
        }
    }

    #endregion

    #region Headshot Effect (On Canvas)

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

    #endregion

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

    #region Drop Diamond On Headshot

    public void DropDiamond(Transform location)
    {
        Vector3 instPos = new Vector3(location.position.x, location.position.y - 0.65f, location.position.z + 0.4f);
        GameObject obj = Instantiate(DiamondGameObject, instPos, new Quaternion(-0, -90, 0, 0));
        obj.transform.GetChild(0).gameObject.SetActive(true);
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