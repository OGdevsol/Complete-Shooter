using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombplantscript : MonoBehaviour
{
    public static bombplantscript instance;

    public bool PlayerInRange;

    public bool bombPlanted;

    public int bombsCount = 0;

    public AudioSource ExplosionSound;


    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        PlayerPrefs.SetInt("BombPlantCountMiniMap", 0);
        PlayerPrefs.SetInt("BombPlantedCount", 0);
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
                Invoke(nameof(PlantBombTimer), 2);
                GameManager.Instance.LoadingBarBombPlant.SetActive(true);
                //GameManager.Instance.LoadingBarBombPlantText.text = "PLANTING BOMB";
                bombPlanted = false;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerInRange = false;
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
            //________________________________Place Bombs_____________________________________________

            GameObject g = Instantiate(GameManager.Instance.Bomb, gameObject.transform.position,
                transform.rotation);
            g.SetActive(true);
            LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].PlantedBombsInScene
                .Add(g.transform);
            gameObject.GetComponent<Collider>().enabled = false;
            bombPlanted = true;
            gameObject.GetComponent<MapMarker>().isActive = false;
            //____________________________Playerprefs for BombPlant related stuff__________________________________

            PlayerPrefs.SetInt("BombPlantCountMiniMap", PlayerPrefs.GetInt("BombPlantCountMiniMap") + 1);
            GameManager.Instance.BombsPlantedInLevel.text = PlayerPrefs.GetInt("BombPlantCountMiniMap").ToString();
            PlayerPrefs.SetInt("BombPlantedCount", PlayerPrefs.GetInt("BombPlantedCount") + 1);
            PlayerPrefs.SetInt("TotalBombsPlantedVal", PlayerPrefs.GetInt("TotalBombsPlantedVal") + 1);
            //____________________________Deactivate bomb from hands once all bombs have been placed_________________

            if (PlayerPrefs.GetInt("BombPlantedCount") == PlayerPrefs.GetInt("BombCountForLevel"))
            {
                GameManager.Instance.BombPlantHands.transform.GetChild(8).gameObject.SetActive(false);
                StartCoroutine(waitbeforeExplosion());
            }
        }

        IEnumerator waitbeforeExplosion()
        {
            GameManager.Instance.pauseButton.SetActive(false);
            //_____________________________Activate explosion cams___________________________________

            yield return new WaitForSecondsRealtime(1);
            GameManager.Instance.ExplosionCam.gameObject.SetActive(true);
            GameManager.Instance.Controls.SetActive(false);
            GameManager.Instance.ExplosionCam.transform.position = GameManager.Instance
                .ExplosionCamsTransforms[LevelsController.Instance.currentLevel].transform.position;
            GameManager.Instance.ExplosionCam.transform.rotation = GameManager.Instance
                .ExplosionCamsTransforms[LevelsController.Instance.currentLevel].transform.rotation;
            GameManager.Instance.BombPlantHands.SetActive(false);
            yield return new WaitForSecondsRealtime(2);
            //________________________________Explosion Effect__________________________________________________
            for (int i = 0;
                 i < LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].PlantedBombsInScene
                     .Count;
                 i++)
            {
                LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].indicators[i]
                    .transform.gameObject.transform.GetChild(1).gameObject.SetActive(true);
            }

            ExplosionSound.Play();
            for (int i = 0;
                 i < LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].PlantedBombsInScene
                     .Count;
                 i++)
            {
                LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].PlantedBombsInScene[i]
                    .gameObject.SetActive(false);
            }

            yield return new WaitForSecondsRealtime(0.65f);

            //__________________________________Level Complete____________________________________________

            GameManager.Instance.GameComplete();
            disableIndicator();
        }
    }

    public void disableIndicator()
    {
        if (bombPlanted)
        {
            for (int i = 0;
                 i < LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].indicators.Count;
                 i++)
            {
                LevelsController.Instance.levelData[LevelsController.Instance.currentLevel].indicators[i]
                    .SetActive(false);
            }
        }
    }
}