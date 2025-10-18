using TMPro;
using UnityEngine;

public class ShowObjective : MonoBehaviour
{
   //variables
    public PickupItem pickupitem;
    public TextMeshProUGUI ShowText; 
    public string ObjectiveMessage = "";//empty string to start off with

    // Update is called once per frame
    void Update()
    {
        ObjectiveMessage = GetMessage();//this goes and grabs the objective and assigns it as the message to display

        if (ShowText != null)
        {
          
            ShowText.text = ObjectiveMessage;//the text is made to be whatever the objective message is
            ShowText.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("Text component not assigned.");//Debug Purpose
        }
    }
/*
A function to decide what the players current objective should be based on world factors.
*/
    public string GetMessage()
    {
         if (pickupitem.BallPickedUpOnce)//this is in reference to the variable in the pickupItem script.
        {
            
            return "ESCAPE BACK TO THE FENCE";//if the player has picked up their ball (completed their first main objective) they need to be told to escape and where to. 
        }
        if (pickupitem.StartingBallThrown)//if player has thrown the fakeball that kickstarts the main game and they are given their first main objective
        {
            return "FIND YOUR BALL AT THE HOUSE";
        }
       
        
        else
        {
            return "PLAY IN THE WOODS";/*This is a more loose objective given at the very start to create some immersion for the game & 
            let the player explore, not outwardly tell them what to do.
            */
        }
    }
}
