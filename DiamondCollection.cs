using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondCollection : MonoBehaviour
{
    public AudioSource CollectSound;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(effect());
            PlayerPrefs.SetInt("DiamondCount",PlayerPrefs.GetInt("DiamondCount")+1);
            GameManager.Instance.diamondCount.text = PlayerPrefs.GetInt("DiamondCount").ToString();
        }
    }

    private IEnumerator effect()
    {
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
        gameObject.transform.GetChild(2).gameObject.SetActive(true);
        CollectSound.Play();
        yield return new WaitForSecondsRealtime(0.15f);
        gameObject.SetActive(false);
        
    }
}
