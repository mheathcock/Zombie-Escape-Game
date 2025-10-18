using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class FlashText : MonoBehaviour
{

    //variables
    public string Message = "";
    public float MessageDuration;
    private bool InBoundary = false;
    public bool legInjured = false;
    private bool isDisplayingMessage = false;

    //references
    public PickupItem pickupitem;
    public TextMeshProUGUI ShowText;
    public FlashScreen flashscreen; 
    public PlayerHealth playerhealth;
    public CameraShake cameraShake;

    //audio sources
    public AudioSource hurtSound;
    public AudioSource ItemOfInterestSound;

    void Start()
{

    //Initialize references 

    if (flashscreen != null)
    {
        flashscreen = GetComponentInChildren<FlashScreen>();//get the flashScreen Component
    }


    if (cameraShake != null)
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();//get the camera shake Component
    }

   
    GameObject player = GameObject.FindWithTag("Player"); //find the player GameObject 
    if (player != null)
    {
        playerhealth = player.GetComponent<PlayerHealth>();//get the player health Component 
    }
    
}

    //A basic trigger collider to see if the player has entered a trigger line
    private void OnTriggerEnter(Collider other)
    {
          if (gameObject.CompareTag("FinishLineTrigger"))//if the player has entered the finish line trigger
        {   
                FinishTriggered();
        }
        
        if (other.CompareTag("Player") && !InBoundary)//if the player has entered any of the other triggers
        {
            InBoundary = true;//the player is in the triggers boundary
            Message = GetMessage();//grab the specific message based on what boundary the player is in
            StartCoroutine(DisplayMessage());//display the specific message
        }

    }

/*
This is the function that'll be called to get the specific message
*/
    private string GetMessage()
    {
        //These if statements will basically check which trigger the player has entered and then return the specific message to be displayed
       
        if (gameObject.CompareTag("DoorTrigger"))
        {
            if (ItemOfInterestSound != null){
                ItemOfInterestSound.Play();//play an audio reference for the player
            }
            return "PRESS F TO OPEN DOOR";
            
        }
        else if (gameObject.CompareTag("PickupAxeTrigger"))
        {
             if (ItemOfInterestSound != null){
                ItemOfInterestSound.Play();//play an audio reference for the player
            }
            return "AN AXE MIGHT BE USEFUL HERE...";
             
        }
        else if (gameObject.CompareTag("SpawnTextTrigger"))
        {
            return "PRESS E TO PICK UP YOUR BALL";
        }
        else if (gameObject.CompareTag("JumpTextTrigger"))
        {
            return "PRESS SPACE TO JUMP";
        }
         else if (gameObject.CompareTag("HurtLegTrigger"))//This is for when the player "hurts" their leg at the start of the game
        {
            legInjured = true;//this will be used in the playerscripts to determine how high they can jump
            if(playerhealth != null)
            {
                
                playerhealth.TakeDamage(5);//the player has hurt themselves so they lose a bit of health
            }

            hurtSound.Play();//play an audio reference for the player

            //trigger camera shake to show a visual representation of the player being hurt. I decided to only use it here and not when hurt by enemies so it wouldn't be too distracting.
            if (cameraShake != null)
            {
                cameraShake.Shake();
            }

            return "OUCH! YOU FEEL A SHARP PAIN IN YOUR ANKLE";
        }
        else if (gameObject.CompareTag("FoundZombieTrigger"))
        {
            return "A DEAD ZOMBIE?! TRY JUMP OVER IT";
        }


       
        return ""; //return an empty string as a default message if no specific condition is met
    }
/*
A coroutine to display the specific message
*/
    public IEnumerator DisplayMessage()
    {
        if (ShowText != null)
        {
            isDisplayingMessage = true; //set the flag to indicate that a message is being displayed
            ShowText.text = Message;
            ShowText.gameObject.SetActive(true);

            yield return new WaitForSeconds(MessageDuration);//keep message on screen for a certain duration

            ShowText.gameObject.SetActive(false);
            isDisplayingMessage = false; //reset the flag after the message is finished

           
            yield return new WaitForSeconds(0.1f); //add a small delay before checking for the next message

        }
        else//if text is null for some reason
        {
            Debug.LogError("Text component null");
        }
    }
/*
    This is for messages that arent shown due to a trigger the player walks into. they may be shown when a certain action is done.
*/
    public void UpdateMessage(string newMessage)
    {
        //check if a message is currently being displayed, if not, start displaying the new message
        if (!isDisplayingMessage)
        {
            Message = newMessage;
            StartCoroutine(DisplayMessage());
        }
        
    }

/*
A seperate function for when the player has entered the finish line trigger
*/
    public void FinishTriggered(){
         
            if (pickupitem.ItemCollider != null){
               if (pickupitem.ItemCollider.CompareTag("Ball"))//check if the player is holding the ball (they have successfully found it and made it back to the safe area without dying)
            {
                Debug.Log("PLAYER WON");
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +2);//go to the win scene. The player has completed the game

                } 
            }
            
    }
}
