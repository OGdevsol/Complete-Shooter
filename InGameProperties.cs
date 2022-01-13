using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameProperties : MonoBehaviour
{
    
    #region GlobalInstance

    public static InGameProperties localInstance;
    public static InGameProperties Instance
    {
        get
        {
            localInstance = GameObject.FindObjectOfType<InGameProperties>();
            return localInstance;
        }
    }
    
    #endregion

    #region Generic Properties
    
    public GameObject screenFade;
    
    [Space(5)] 
    [Header("--- Damage Indicator")]
    public RectTransform indicator;
    Transform otherTransform;
	
    public Transform player;
    public bool useDirection;
	
    private float angle;
    
    [Space(5)] 
    [Header("--- CrossHair-HitMarker")]
    
    public Color normelColor;
    public Color enemyPointColor;
    
    public Image[] crossHair;
    public Animator crossHairAnimator;

    public GameObject hitMarker;
    public Text currentKill;
    
    public bool isBodyShot = true;
    [HideInInspector] public int bodyShootIndex;
    
    [Space(5)] 
    [Header("--- Setting")]
    
    public GameObject autoShootOn;
    public GameObject autoShootOff;
    
    public GameObject soundOn;
    public GameObject soundOff;
    public GameObject musicOn;
    public GameObject musicOff;
    public bool SFXOFF;
    public bool SFXON;

    [Space(5)] 
    [Header("--- Scripts References")]
    public playercontroller playerController;

    #endregion
    
    #region Iniatialization

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("AutoShoot"))
        {
            PlayerPrefs.SetInt("AutoShoot",1);
            AutoShoot_Off();
        }
        else
        {
            AutoShootStatus();
        }
    }

    private void Start()
    {
        
        StartCoroutine(ScreenFadeIn());
        indicator.gameObject.SetActive (false);
        
        ChangeSfxVolume();
        ChangeMusicVolume();
        
        currentKill.text = $"{0}";
        
    }

    private IEnumerator ScreenFadeIn()
    {
        screenFade.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        screenFade.SetActive(false);
    }
    
    #endregion
    
    #region Setting
    
    private void AutoShootStatus()
    {
        if (PlayerPrefs.GetInt("AutoShoot") == 0)
        {
            AutoShoot_On();
        }
        else
        {
            AutoShoot_Off();
        }
    }
    
    public void AutoShoot_On()
    {
        PlayerPrefs.SetInt("AutoShoot",0);
        
        autoShootOn.SetActive(false);
        autoShootOff.SetActive(true);
        
    }
    public void AutoShoot_Off()
    {
        
        PlayerPrefs.SetInt("AutoShoot",1);
        autoShootOn.SetActive(true);
        autoShootOff.SetActive(false);
        
    }
    
    public void MusicVoluneOn()
    {
        PlayerPrefs.SetInt("musicvolume",1);
        musicOn.SetActive(true);
        musicOff.SetActive(false);
        SoundController.instance.musicVolumeChanged(1);
        SFXON = true;
        SFXOFF = false;
    }
    public void MusicVoluneOff() 
    {
        PlayerPrefs.SetInt("musicvolume",0);
        musicOn.SetActive(false);
        musicOff.SetActive(true);
        SoundController.instance.musicVolumeChanged(0);
        SFXOFF=true;
        SFXON = false;
    }
    
    public void SfxVolumeOn()
    {
        PlayerPrefs.SetInt("sfxvolume",1);
        soundOn.SetActive(true);
        soundOff.SetActive(false);
        SoundController.instance.sfxVolumeChanged(1);
       
       
        Debug.Log(PlayerPrefs.GetInt("sfxvolume"));
        LevelsController.Instance.bombDiffuseObject.GetComponent<AudioSource>().enabled = true;
    }
    
    public void SfxVolumeOff()
    {
        PlayerPrefs.SetInt("sfxvolume",0);
        soundOn.SetActive(false);
        soundOff.SetActive(true);
        
        SoundController.instance.sfxVolumeChanged(0);
        SoundController.instance.musicVolumeChanged(0);
        LevelsController.Instance.bombDiffuseObject.GetComponent<AudioSource>().enabled = false;
        Debug.Log(PlayerPrefs.GetInt("sfxvolume"));
    } 
    
    
    
    public void ChangeSfxVolume()
    {
        if (PlayerPrefs.GetInt("sfxvolume")  == 0)
        {
            SfxVolumeOff();
           
        }
        else
        {
            SfxVolumeOn();
            
        }
            
    }
    
    public void ChangeMusicVolume()
    {
        if (PlayerPrefs.GetInt("musicvolume")  == 0)
        {
            MusicVoluneOff();
        }
        else
        {
            MusicVoluneOn();
        }
        SoundController.instance.musicVolumeChanged(PlayerPrefs.GetInt("musicvolume"));

    }
    
    #endregion

    #region Indicator Damager

    private void Update()
    {
        if (useDirection) {
            // Angle based on the direction of the shooter relative to player
            if (otherTransform == null) {
                //
            } else {
                angle = GetHitAngle (player, otherTransform.forward);
                indicator.rotation = Quaternion.Euler (0, 0, -angle);
            }
        } else if (!useDirection) {
            // Angle taken from other objects pos and player
            if (otherTransform == null) {
                //
            } else {
                angle = GetHitAngle (player, (player.position - otherTransform.position).normalized);
                indicator.rotation = Quaternion.Euler (0, 0, -angle);
            }
        }
    }

    public float GetHitAngle(Transform player, Vector3 incomingDir)
    {
        // Flatten to plane
        var otherDir = new Vector3(-incomingDir.x, 0f, -incomingDir.z);
        var playerFwd = Vector3.ProjectOnPlane(this.player.forward, Vector3.up);

        // Direction between player fwd and incoming object
        var angle = Vector3.SignedAngle(playerFwd, otherDir, Vector3.up);

        return angle;
    }
	
    public void IndicatorArrow(Transform obj)
    {
        otherTransform = obj;
        StartCoroutine (IndicatorCoroutine ());
    }
	
    private IEnumerator IndicatorCoroutine()
    {
        if (PlayerPrefs.GetString("Mode") != "Zombie")
        {
            yield return new WaitForSeconds (0f);
            indicator.gameObject.SetActive (true);
            yield return new WaitForSeconds (2f);
            indicator.gameObject.SetActive (false);
        }
        
    }

    #endregion

    #region CrossHair

     public void CrossHairZoomOut()
        {
            crossHairAnimator.Play("ZoomOut");
        }
    
        public void CrossHairZoomIn()
        {
            crossHairAnimator.Play("ZoomIn");
        }
    
        public void CrossAssistColor()
        {
            foreach (var item in crossHair)
            {
                item.color = enemyPointColor;
            }  
            hitMarker.SetActive(true);
        }
    
        public void CrossAimColor()
        {
            foreach (var item in crossHair)
            {
                item.color = normelColor;
            }  
            hitMarker.SetActive(false);
        }
    
        public void CrosshairEnable()
        {
            foreach (var item in crossHair)
            {
                item.gameObject.SetActive(true);
            }
        }
        public void CrosshairDisable()
        {
            foreach (var item in crossHair)
            {
                item.gameObject.SetActive(false);
            }
        }

    #endregion

    public void BodyShootIndex()
    {
        bodyShootIndex++;
        currentKill.text = $"{bodyShootIndex}";
        
        if (PlayerPrefs.GetString("Mode") == "Flag")
        {
            PlayerPrefs.SetInt("TotalBodyShoot",PlayerPrefs.GetInt("TotalBodyShoot")+1);
        }
    }
    
    
    
    
}
