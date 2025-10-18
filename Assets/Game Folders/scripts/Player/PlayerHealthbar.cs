using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthbar : MonoBehaviour
{
    //variables
    [SerializeField] private Image PhealthbarSprite;

    private Camera playercam;

    void Start()
    {
        playercam = Camera.main;
    }

 //these functions are the same as the enemy healthbar but this time its using the players health variables
    public void UpdatePHealthBar(float PmaxHealth, float PcurrentHealth)
    {
        PhealthbarSprite.fillAmount = PcurrentHealth / PmaxHealth;
        
        if (playercam != null)
        {
            //face the health bar towards the camera
            transform.LookAt(transform.position + playercam.transform.rotation * Vector3.forward, playercam.transform.rotation * Vector3.up);
        }
    }
}
