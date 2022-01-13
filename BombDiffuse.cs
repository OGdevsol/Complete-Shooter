using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombDiffuse : MonoBehaviour
{
    
    private int currentValue;
    private bool isDiffuse;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.bombDiffuseBar.SetActive(true);
            isDiffuse = true;
            StartCoroutine(BombDiffuseTime());
            
        }
        

       
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            currentValue = 0;
            isDiffuse = false;
            GameManager.Instance.bombSliderValue.text = "{0}%";
            GameManager.Instance.bombDiffuseBar.SetActive(false);
            GameManager.Instance.bombSlider.value = currentValue;
          
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (PlayerPrefs.GetInt("sfxvolume") == 0)
            {
                gameObject.GetComponent<AudioSource>().enabled = false;
            }
            else if (PlayerPrefs.GetInt("sfxvolume") == 1)
            {
                gameObject.GetComponent<AudioSource>().enabled = true;
            }
        }
    }


    private IEnumerator BombDiffuseTime()
    {
        if (currentValue >= 100)
        {
            GameManager.Instance.bombSliderValue.text = $"BOMB DEFUSED SUCCESSFULLY";
            isDiffuse = false;
            LevelsController.Instance.checkallbombs();
            gameObject.GetComponent<BoxCollider>().enabled = false;
            gameObject.GetComponent<AudioSource>().enabled = false;
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            yield return new WaitForSecondsRealtime(1f);
            GameManager.Instance.bombDiffuseBar.SetActive(false);
            this.gameObject.GetComponent<MapMarker>().isActive = false;
        }
        if (isDiffuse)
        {
            yield return new WaitForSeconds(0.01f);
            currentValue++;
            GameManager.Instance.bombSliderValue.text = $"{currentValue}%";
            GameManager.Instance.bombSlider.value = currentValue;
            
            if (currentValue <= 100 && isDiffuse)
            {
                StartCoroutine(BombDiffuseTime());
            }
        }
    }

}