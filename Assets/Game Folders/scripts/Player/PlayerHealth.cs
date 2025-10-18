using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerHealth : MonoBehaviour
{
    //variables 
    public float PmaxHealth = 100f;
    public float PcurrentHealth;

    //references
   [SerializeField] private PlayerHealthbar playerhealthbar;
     public GameOver gameoverScreen;
    public FlashScreen flashscreen;

    //audio source
    public AudioSource playerHurtSound;
   
  
    void Start()
    {
        PcurrentHealth = PmaxHealth;//sets players current health to the max health when the game starts
         playerhealthbar.UpdatePHealthBar(PmaxHealth, PcurrentHealth);//updates the players health bar to match
       
    }
    /*
    Function for the player taking damage. It takes damage as a parameter 
    */
    public void TakeDamage(float damage)
    {
        if (flashscreen != null) 
        {
            flashscreen.FlashEffect();//this will flash the screen red when the player takes damage. a visual reference for them to know
        }
        if (playerHurtSound != null)
        {
            playerHurtSound.Play();//it will also play an audio reference for the player 
        }
        
        if (PcurrentHealth > 0)//if the player hasn't died then they will take damage
        {
              PcurrentHealth -= damage;
         playerhealthbar.UpdatePHealthBar(PmaxHealth, PcurrentHealth);//update healthbar to match current health
        }
        else{//if the player has less than 0 hp then they have died and lost the game
             SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);//change the scene to the game over scene
        }
      
    }
}
