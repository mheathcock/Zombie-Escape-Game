using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    //variables 

    [SerializeField] private Image healthbarSprite;//reference to the health bar sprite

    private Camera playercam;//reference to the players camera

    void Start()
    {
        playercam = Camera.main;
    }

    /*
        This is a very simple function it will just take it the max and current health as a parameter and if called will update how much the healthbar should be filled.
    */
    public void UpdateHealthBar(float maxHealth, float currentHealth)
    {
        healthbarSprite.fillAmount = currentHealth / maxHealth;
    }

    void Update()
    {
        if (playercam != null)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - playercam.transform.position);//this will rotate the healthbar to face the player
        }
    }
}
