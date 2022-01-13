using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombplantscript : MonoBehaviour
{
    public static bombplantscript instance;

    public bool PlayerInRange;

    public bool bombPlanted;

    public int bombsCount;
    
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        PlayerPrefs.SetInt("BombPlantedCount",0);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInRange = true;
            if (PlayerInRange)
            {
                Invoke(nameof(PlantBombTimer),7);
                GameManager.Instance.LoadingBarBombPlant.SetActive(true);
                //GameManager.Instance.LoadingBarBombPlantText.text = "PLANTING BOMB";
                Debug.Log("PlayerAround");
                bombPlanted = false;
            }
           
            
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInRange = false;
            Debug.Log("PlayerNotAround");
            bombPlanted = false;
            GameManager.Instance.LoadingBarBombPlant.SetActive(false);

            CancelInvoke(nameof(PlantBombTimer));

        }
    }

    public void PlantBombTimer()
    {
        
       // yield return new WaitForSecondsRealtime(7.5f);
        GameManager.Instance.LoadingBarBombPlant.SetActive(false);
        if (PlayerInRange && !bombPlanted)
        {
            GameObject g = Instantiate(GameManager.Instance.Bomb, gameObject.transform.position,
                    transform.rotation);  
                g.SetActive(true);
                gameObject.GetComponent<Collider>().enabled = false;
                bombPlanted = true;
                //GameManager.Instance.LoadingBarBombPlantText.text = "BOMB Planted Successfully";
                gameObject.GetComponent<MapMarker>().isActive = false;
                PlayerPrefs.SetInt("BombPlantedCount",PlayerPrefs.GetInt("BombPlantedCount")+1);
                if (PlayerPrefs.GetInt("BombPlantedCount")==PlayerPrefs.GetInt("BombCountForLevel"))
                {
                    GameManager.Instance.BombPlantHands.transform.GetChild(8).gameObject.SetActive(false);
                    StartCoroutine(waitbeforeExplosion());
                    //Destroy(gameObject);
                }
        }
        IEnumerator waitbeforeExplosion()
        {
            yield return new WaitForSeconds(2);
            GameManager.Instance.ExplosionCam.gameObject.SetActive(true);
            yield return new WaitForSecondsRealtime(2);
            for (int i = 0; i < LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].BombPlantPosition.Count; i++)
            {
                Instantiate(GameManager.Instance.Explosion,
                    LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].BombPlantPosition[i]
                        .transform.position,
                    LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].BombPlantPosition[i]
                        .transform.rotation);
            }
            yield return new WaitForSeconds(2);
                GameManager.Instance.GameComplete();
                
               
            
        }
       
        
    }
}
