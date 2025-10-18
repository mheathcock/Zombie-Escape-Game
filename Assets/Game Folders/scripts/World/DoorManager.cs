using UnityEngine;
using System.Collections;


public class DoorManager : MonoBehaviour
{

    //variables
    public bool doorOpened;
    public bool nearDoor; 

    //reference
    public Collider doorCollider;

  
    //audio source
    public AudioSource openDoorSound;


    void Update()
    {
      
        if (Input.GetKeyDown(KeyCode.F) && IsPlayerNearDoor() && !doorOpened)//if the player is near a door that isn't open and they press the "F" keys
        {
            OpenDoor();
        }
    }

    void OpenDoor()
    {
        if (openDoorSound != null){
            openDoorSound.Play();//play an audio reference for the player to know the door is opening
        }
     

        doorOpened = true;//a flag so the door cant be opened again.

        
        StartCoroutine(RotateDoor(90f)); //rotate to 90 degrees when opening

    }

    bool IsPlayerNearDoor()
    {
        return nearDoor;//this is in reference to the collision detection below
    }

//A basic collision detection to see if the player has walked into the doors trigger collider (and is therefore near the door)
   private void OnTriggerEnter(Collider other)
{
    if (other.CompareTag("Player") && !doorOpened)
    {
        nearDoor = true;//player is near the door
     
    }
}
//this is for when the player leaves the trigger collider
private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("Player"))
    {
        nearDoor = false;//player is not near the door anymore

    }
}

    //This roates the door and takes the angle as a parameter 
    private IEnumerator RotateDoor(float targetAngle)
    {
      
        Quaternion startRotation = doorCollider.transform.rotation;  //get the current rotation of the door

    
        Quaternion targetRotation = Quaternion.Euler(0f, targetAngle, 0f) * startRotation;//calculate the target rotation

        float elapsedTime = 0f;
        float totalRotationTime = 1f; 
    //So the door doesn't just instantly open and looks more realistic
        while (elapsedTime < totalRotationTime)
        {
            doorCollider.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / totalRotationTime);//smoothly rotate the door
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        doorCollider.transform.rotation = targetRotation;//just a final line to ensure the doors rotation matches the target rotation
    }
}
