using UnityEngine;

public class StartingGameBoundary : MonoBehaviour
{
    //reference to the PickupItem script 
    public PickupItem pickupItem;

    private void Start()
    {
        //checkthat the pickupItem variable is assigned in the Inspector
        if (pickupItem == null)
        {
            Debug.LogError("PickupItem script reference not assigned");
        }
    }

//A very simple function that will just remove the starting game boundaries when the player throws the ball for the first time.
    private void Update()
    {
        

        if (pickupItem.StartingBallThrown)//get the bool from the pickupItem script
    
        {//if true then remove the boundaries and let the player explore
            
            Destroy(gameObject);

            //disable this script to prevent continuous updates
            enabled = false;
        }
    }
}
