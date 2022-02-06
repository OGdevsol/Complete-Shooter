using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class MenuManager : MonoBehaviour
{
    #region Global Reference

    public static MenuManager localInstance;

    public static MenuManager Instance
    {
        get
        {
            if (localInstance == null)
                localInstance = GameObject.FindObjectOfType<MenuManager>();
            return localInstance;
        }
    }

    #endregion

    #region Properties

    [Space(5)] [Header("--- UI Panels")] [Space(3)]
    public GameObject[] reference;

    [Space(2)] private int currentActivePanel;

    public Text cashText;

    [Space(6)] public Text loadingProgress;
    private AsyncOperation async;

    [Space(5)] [Header("--- Levels Flag")] [Space(3)]
    public GameObject[] flagLevelsBtn;

    [Space(5)] [Header("--- Levels Bomb-Diffuse")] [Space(3)]
    public GameObject[] bombLevelsBtn;

    [Space(5)] [Header("--- Levels BombPlant")] [Space(3)]
    public GameObject[] bombPlantLevelsBtn;

    [Space(5)] [Header("--- Levels HostageRescue")] [Space(3)]
    public GameObject[] hostageRescueLevelsBtn;

    public GameObject[] checkMark;

    private int index;
    public bool isWeaponClick;

    [Header("Weapons Panel//3DModel n Stuff" )] public Weapon_Manager weaponManager;

    public Text selectWeaponText;
    public Text DiamondCount;
    public GameObject Model;

    #endregion

    #region Initilization

    private void Start()
    {
      
       // PlayerPrefs.SetInt("Cash", 1000000);
        Time.timeScale = 1;
        ControlFreak2.CFCursor.lockState = CursorLockMode.None;
        currentActivePanel = 0;
        EnablePanel(currentActivePanel);
        CashShown();
        ////////////////////////////////////// Levels Locking
        UnlockFlagLevels();
        UnlockBombLevels();
        UnlockBombPlantLevels();
        UnlockHostageRescueLevels();
        SelectedModeStatus();
        Weapon_Manager.Instance.CheckUpgradeStatus();
        AdsController.Instance.gameplay = false;
        isWeaponClick = false;
        ////////////////////////////////////// Ads Calling
        if (AdsController.Instance)
        {
            AdsController.Instance.ShowBanner();
            AdsController.Instance.HideLargeBanner();
            
        }
       

    }

    public void CashShown()
    {
        cashText.text = PlayerPrefs.GetInt("Cash").ToString();
        DiamondCount.text = PlayerPrefs.GetInt("DiamondCount").ToString();
    }

    #endregion

    #region Generic

    public void Exit()
    {
        EnablePanel(3);
    }

    public void EnablePanel(int x)
    {
       
        if (reference[0].activeInHierarchy && x == 2 || x == 5 && reference[2].activeInHierarchy)
        {
            isWeaponClick = true;
            selectWeaponText.text = "Select";
            Weapon_Manager.Instance.upgradeButtonMain.gameObject.SetActive(false);
        }
        else
        {
            selectWeaponText.text = "Next";
            isWeaponClick = false;
            Weapon_Manager.Instance.upgradeButtonMain.gameObject.SetActive(true);
            
        }

        
        
        for (int i = 0; i < reference.Length; i++)
        {
            reference[i].SetActive(false);
        }
        reference[x].SetActive(true);
        if (reference[0].gameObject.activeInHierarchy)
        {
            Model.SetActive(true);
        }
        else if(!reference[0].gameObject.activeInHierarchy)
        {
            Model.SetActive(false);
        }
    }

    public void GameQuit()
    {
        Application.Quit();
    }

    public void WeaponsClickBack()
    {
        
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                EnablePanel(6);
            }
            if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                EnablePanel(7);
            }
            if (PlayerPrefs.GetString("Mode") == "BombPlant")
            {
                EnablePanel(8);
            }
            if (PlayerPrefs.GetString("Mode") == "HostageRescue")
            {
                EnablePanel(9);
            }
            /*else
            {
                PlayerPrefs.SetString("Mode","Flag");
            }*/
            else if (!PlayerPrefs.HasKey("Mode"))
            {
                PlayerPrefs.SetString("Mode","Flag");
            }
            
                
            
        
    }

   

    #endregion

    #region Loading

    public void LoadLevel(int sceneIndex)
    {
        StartCoroutine(LoadAsynchronously(sceneIndex));
    }

    private IEnumerator LoadAsynchronously(int sceneIndex)
    {
        EnablePanel(5);
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(sceneIndex);
    }

    #endregion

    #region Flag Mode Levels

    public void LevelIndexFlag(int z)
    {
        PlayerPrefs.SetInt("FlagLevel", z);
        EnablePanel(2);

        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
    }

    private void UnlockFlagLevels()
    {
        int currentlevel = PlayerPrefs.GetInt("FlagLevel");
        for (int t = 0; t < flagLevelsBtn.Length; t++)
        {
            if (t < PlayerPrefs.GetInt("UnlockFlag"))
            {
                flagLevelsBtn[t].GetComponent<Button>().interactable = true;
                flagLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(true);
                flagLevelsBtn[t].transform.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                flagLevelsBtn[t].GetComponent<Button>().interactable = false;
                flagLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(false);
                flagLevelsBtn[t].transform.GetChild(0).gameObject.SetActive(false);
            }
        }


        flagLevelsBtn[PlayerPrefs.GetInt("SelectedFlagLevel")].transform.GetChild(0).gameObject.SetActive(true);
        flagLevelsBtn[PlayerPrefs.GetInt("SelectedFlagLevel")].transform.GetChild(1).gameObject.SetActive(true);
        flagLevelsBtn[PlayerPrefs.GetInt("SelectedFlagLevel")].GetComponent<Button>().interactable = true;
//      
//        
    }

    public void FlagCollection()
    {
        PlayerPrefs.SetInt("FlagLevel", PlayerPrefs.GetInt("SelectedFlagLevel") + 1);
        EnablePanel(2);
        weaponManager.OpenParent();
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
        isWeaponClick = false;
        Analyticsmanager.instance.CustomEvent("Flag");
    }


    public void BuyFlagLevels()
    {
        for (int y = 0; y < 15; y++)
        {
            flagLevelsBtn[y].GetComponent<Button>().interactable = true;
            flagLevelsBtn[y].transform.GetChild(1).gameObject.SetActive(true);
            PlayerPrefs.SetInt("UnlockFlag", 15);
        }

        PlayerPrefs.SetString("FlagLevelPurchased", "Done");
    }

    #endregion

    #region Bomb Diffuse Mode Levels

    public void LevelIndexBomb(int z)
    {
        PlayerPrefs.SetInt("BombLevel", z);
        EnablePanel(2);
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
    }

    private void UnlockBombLevels()
    {
        int currentlevel = PlayerPrefs.GetInt("BombLevel");
        for (int t = 0; t < bombLevelsBtn.Length; t++)
        {
            if (t < PlayerPrefs.GetInt("UnlockBomb"))
            {
                bombLevelsBtn[t].GetComponent<Button>().interactable = true;
                bombLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                bombLevelsBtn[t].GetComponent<Button>().interactable = false;
                bombLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(false);
            }
        }


        bombLevelsBtn[PlayerPrefs.GetInt("SelectedBombLevel")].transform.GetChild(0).gameObject.SetActive(true);
        bombLevelsBtn[PlayerPrefs.GetInt("SelectedBombLevel")].transform.GetChild(1).gameObject.SetActive(true);
        bombLevelsBtn[PlayerPrefs.GetInt("SelectedBombLevel")].GetComponent<Button>().interactable = true;
    }

    public void BombDiffuse()
    {
        PlayerPrefs.SetInt("BombLevel", PlayerPrefs.GetInt("SelectedBombLevel") + 1);
        EnablePanel(2);
        weaponManager.OpenParent();
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
        Analyticsmanager.instance.CustomEvent("BombDiffuse");
    }

    public void BuyBombLevels()
    {
        for (int y = 0; y < 15; y++)
        {
            bombLevelsBtn[y].GetComponent<Button>().interactable = true;
            bombLevelsBtn[y].transform.GetChild(1).gameObject.SetActive(true);
            PlayerPrefs.SetInt("UnlockBomb", 15);
        }

        PlayerPrefs.SetString("BombLevelPurchased", "Done");
    }

    #endregion

    #region BombPlantModeLevels

    public void LevelIndexBombPlant(int z)
    {
        PlayerPrefs.SetInt("BombPlantLevel", z);
        EnablePanel(2);
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
    }

    private void UnlockBombPlantLevels()
    {
        int currentlevel = PlayerPrefs.GetInt("BombPlantLevel");
        for (int t = 0; t < bombPlantLevelsBtn.Length; t++)
        {
            if (t < PlayerPrefs.GetInt("UnlockBombPlant"))
            {
                bombPlantLevelsBtn[t].GetComponent<Button>().interactable = true;
                bombPlantLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                bombPlantLevelsBtn[t].GetComponent<Button>().interactable = false;
                bombPlantLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(false);
            }
        }


        bombPlantLevelsBtn[PlayerPrefs.GetInt("SelectedBombPlantLevel")].transform.GetChild(0).gameObject
            .SetActive(true);
        bombPlantLevelsBtn[PlayerPrefs.GetInt("SelectedBombPlantLevel")].transform.GetChild(1).gameObject
            .SetActive(true);
        bombPlantLevelsBtn[PlayerPrefs.GetInt("SelectedBombPlantLevel")].GetComponent<Button>().interactable = true;
    }

    public void BombPlant()
    {
        PlayerPrefs.SetInt("BombPlantLevel", PlayerPrefs.GetInt("SelectedBombPlantLevel") + 1);
        EnablePanel(2);
        weaponManager.OpenParent();
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
        Analyticsmanager.instance.CustomEvent("BombPlant");
    }

    public void BuyBombPlantLevels()
    {
        for (int y = 0; y < 15; y++)
        {
            bombPlantLevelsBtn[y].GetComponent<Button>().interactable = true;
            bombPlantLevelsBtn[y].transform.GetChild(1).gameObject.SetActive(true);
            PlayerPrefs.SetInt("UnlockBombPlant", 15);
        }

        PlayerPrefs.SetString("BombPlantLevelPurchased", "Done");
    }

    #endregion

    #region HostageRescueModeLevels

    public void LevelIndexHostageRescue(int z)
    {
        PlayerPrefs.SetInt("HostageRescueLevel", z);
        EnablePanel(2);
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
    }
    private void UnlockHostageRescueLevels()
    {
        int currentlevel = PlayerPrefs.GetInt("HostageRescueLevel");
        for (int t = 0; t < hostageRescueLevelsBtn.Length; t++)
        {
            if (t < PlayerPrefs.GetInt("UnlockHostageRescue"))
            {
                hostageRescueLevelsBtn[t].GetComponent<Button>().interactable = true;
                hostageRescueLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(true);
            }
            else
            {
                hostageRescueLevelsBtn[t].GetComponent<Button>().interactable = false;
                hostageRescueLevelsBtn[t].transform.GetChild(1).gameObject.SetActive(false);
            }
        }


        hostageRescueLevelsBtn[PlayerPrefs.GetInt("SelectedHostageRescueLevel")].transform.GetChild(0).gameObject
            .SetActive(true);
        hostageRescueLevelsBtn[PlayerPrefs.GetInt("SelectedHostageRescueLevel")].transform.GetChild(1).gameObject
            .SetActive(true);
        hostageRescueLevelsBtn[PlayerPrefs.GetInt("SelectedHostageRescueLevel")].GetComponent<Button>().interactable = true;
    }

    public void HostageRescue()
    {
        PlayerPrefs.SetInt("HostageRescueLevel", PlayerPrefs.GetInt("HostageRescueLevel") + 1);
        EnablePanel(2);
        weaponManager.OpenParent();
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
        Analyticsmanager.instance.CustomEvent("HostageRescue");
    }

    public void BuyHostageRescueLevels()
    {
        for (int y = 0; y < 15; y++)
        {
            hostageRescueLevelsBtn[y].GetComponent<Button>().interactable = true;
            hostageRescueLevelsBtn[y].transform.GetChild(1).gameObject.SetActive(true);
            PlayerPrefs.SetInt("UnlockHostageRescue", 15);
        }

        PlayerPrefs.SetString("HostageRescueLevelPurchased", "Done");
    }

    #endregion

    #region Mode Selection

    public void WeaponsClick()
    {
        EnablePanel(2);
        weaponManager.OpenParent();
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
        weaponManager.CustomSelect(PlayerPrefs.GetInt("Weapon"));
    }

    #region Modes

    public void SelectMode()
    {
        PlayerPrefs.SetString("Mode", EventSystem.current.currentSelectedGameObject.name);
        SelectedModeStatus();
    }

    public void Next_Selection()
    {
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            EnablePanel(6);
        }
        else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
        {
            EnablePanel(7);
        }
    }

    #endregion

    private void CurrentSelectedMode(int clicked)
    {
        for (int i = 0; i < checkMark.Length; i++)
        {
            checkMark[i].SetActive(false);
        }

        checkMark[clicked].SetActive(true);
    }

    private void SelectedModeStatus()
    {
        if (!PlayerPrefs.HasKey("Mode"))
        {
            PlayerPrefs.SetString("Mode", "Flag");
            CurrentSelectedMode(0);
            Analyticsmanager.instance.CustomEvent("Flag");
            Debug.Log("Flag Initialization");
        }
        else
        {
            if (PlayerPrefs.GetString("Mode") == "Flag")
            {
                CurrentSelectedMode(0);
                Debug.Log("Flag");
            }
            else if (PlayerPrefs.GetString("Mode") == "BombDiffuse")
            {
                CurrentSelectedMode(1);
                Debug.Log("BombDiffuse");
            }
            else if (PlayerPrefs.GetString("Mode") == "BombPlant")
            {
                CurrentSelectedMode(2);
                Debug.Log("BombPlant");
            }
            else if (PlayerPrefs.GetString("Mode") == "HostageRescue")
            {
                CurrentSelectedMode(3);
                Debug.Log("HostageRescue");
            }
        }
    }

    #endregion

    private void LateUpdate()
    {
        if (reference[2].activeInHierarchy)
        {
            if (weaponManager.notEnoughCash.activeInHierarchy)
            {
                weaponManager.CloseParent();
            }
            else
            {
                weaponManager.OpenParent();
            }
        }
        else
        {
            weaponManager.CloseParent();
        }
    }

   

    public void Ad_MenuInterstitial()
    {
        AdsController.Instance.Show_Admob_InterstitialMain();
    }

    public void CheckApiLevel()
    {
        PlayerPrefs.SetString("APILevel27", "Done");
    }
}


/*async = SceneManager.LoadSceneAsync(sceneIndex);
        while (!async.isDone)
        {
            float progress = Mathf.Clamp01(async.progress / .9f);
            loadingProgress.text = (progress * 100f).ToString("0") + "%";
            yield return null;
        }*/