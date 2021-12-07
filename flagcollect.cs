using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class flagcollect : MonoBehaviour
{
 
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            
            GameManager.Instance.flagcollectcounter += 1;
            GameManager.Instance.flagscollectedtext.text = GameManager.Instance.flagcollectcounter.ToString();
            Destroy(gameObject);
        }
    }
}
