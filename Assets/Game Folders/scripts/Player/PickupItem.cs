using UnityEngine;

public class PickupItem : MonoBehaviour
{
    //variables

    public float PickupRange;
    public bool startingBallThrown;
    public bool BallPickedUpOnce = false;
    public float InterpolationSpeed = 50f;   
    private bool HoldingItem = false; 
    public bool FoundBall = false;   
   
    
    //audio sources
    public AudioSource ThrowingSound;
    public AudioSource PickupSound;
    public AudioSource PickupBallSound;
    public AudioSource BackgroundMusic;
    public AudioSource TenseMusic;


     //variables for throwing
    public float AirResistance = 0.2f;      
    public float Gravity = 9.8f;             
    public float ThrowForce;                
    public float ThrowUpwardForce;             
    public bool StartingBallThrown = false;    
    private bool ThrowMessageShown = false;    

    //referneces 
    public ShowObjective showobjective;
    public GameObject fakeBall;               
    public FlashText flashText;    
    public Camera PlayerCamera;          
    public Rigidbody ItemRigidBody;
    public Collider ItemCollider;
    public Transform WeaponContainer;
    public LayerMask PickupLayer;
    public EnemyAI enemyAI;
    
            

    void Start(){
        if (BackgroundMusic != null)
        {
            BackgroundMusic.Play();
        }

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))//check if the "E" key is pressed.
        {
            
            Ray Pickupray = new Ray(PlayerCamera.transform.position, PlayerCamera.transform.forward);//create a forward raycast starting from the players camera position.

          
            if (Physics.Raycast(Pickupray, out RaycastHit hitInfo, PickupRange, PickupLayer))  //check if the ray hits an object within the specified range and on the PickupLayer.
            {
                if (PickupSound != null)
                {
                    PickupSound.Play();//play an audio reference to the player that they have picked up an item
                }
              
                if (ItemRigidBody)  //check if the player is already holding an item.
                {
                    //if the player is already holding an item let go of the currently held item 
                    ItemRigidBody.isKinematic = false;
                    ItemCollider.isTrigger = false;
                    ItemRigidBody.transform.SetParent(null);
                    
                    HoldingItem = false;

                    /* now we can pick up this new item by updating the info of the ItemRigidBody and ItemCollider
                    It sets the ItemRigidBody and ItemCollider to the RigidBody and Collider of the gameObject that the raycast hit
                    */
                    ItemRigidBody = hitInfo.rigidbody;
                    ItemCollider = hitInfo.collider;

                //pick up the new item. 
                ItemRigidBody.isKinematic = true;
                ItemCollider.isTrigger = true;
                HoldingItem = true;
                }

                else//if the player isn't already holding an item then just pick up this new item.
                {
                    
                    ItemRigidBody = hitInfo.rigidbody;
                    ItemCollider = hitInfo.collider;

                   
                    ItemRigidBody.isKinematic = true;
                    ItemCollider.isTrigger = true;
                    HoldingItem = true;
                }

               
            }
        }

       //This is now the code for when the player is holding an item.
        if (ItemRigidBody)
        {
            HoldingItem = true;
           
        ItemRigidBody.position = WeaponContainer.position;//this moves the held item to the weaponContainers position which is basically where a hand would be.

        
        ItemRigidBody.transform.parent = WeaponContainer;//its then set as a child of the weapon container so it moves with it


           
            if (ItemCollider != null && ItemCollider.CompareTag("Ball"))//check if the held item has the tag "Ball".
            {
              
                if (!BallPickedUpOnce) //This is just a flag so this set of code will only happen once
                {

                    BallPickedUpOnce = true;//because the flag is set to true here once this code has run it shouldn't run again.
                      showobjective.GetMessage();//this calls the objective script and itll just update the players objective in the top left corner.
                      
                    if (PickupBallSound != null)//it will also play an audio reference to the player to tell them they have completed their past objective (finding the ball)
                    {
                        PickupBallSound.Play();
                    }
                    /*
                    This will update the background music to something more tense and scary. This is a way of telling the player they could be in danger 
                    and it'll allow the player to also understand why the zombies are suddenly more difficult. more aggressive and faster
                    */
                    if (TenseMusic != null)
                    {
                        BackgroundMusic.Stop();
                        TenseMusic.Play();
                    }
                    //This is updating the enemies AI to set a bool that'll make them more aggressive.
                   foreach (EnemyAI enemyAIInstance in EnemyAI.allEnemies)
            {
                enemyAIInstance.isAggressive = true;
            }
                }
          
            }
          
            else if (ItemCollider.CompareTag("FakeBall") && !ThrowMessageShown)  //check if the held item has the tag "FakeBall" and the throw message has not been shown. 
            {
             
               
               if (flashText != null)
                {
                flashText.UpdateMessage("PRESS Q TO THROW YOUR BALL"); //display a message to press Q to throw the ball. Allows the player to understand the controls.
                }
                ThrowMessageShown = true; //set the flag indicating the throw message has been shown.
            }
        }

     //This code will check if the player is trying to attack
        if (Input.GetMouseButtonDown(0) && HoldingItem)  //check for mouse click whilst holding an item
        {
            WeaponController weaponController = WeaponContainer.GetComponent<WeaponController>();//reference to the weapon controller script

            if (weaponController != null)
            {
             
                weaponController.Attack(ItemRigidBody);   //call the Attack function with the held item.
            }
        }
        
    //This code will check if the player is trying to throw an item
        
        if (Input.GetKeyDown(KeyCode.Q) && HoldingItem) //check if the "Q" button is pressed whilst holding an item
        {
            ThrowHeldObject();//call a function to throw the object
        }
    }

    //Function to throw objects
    void ThrowHeldObject()
    {
        if (ThrowingSound != null)
        {
            ThrowingSound.Play();//play an audio reference so the player knows they are throwing an item
        }
        if (!StartingBallThrown)//this is a flag as i want the players first throw with the ball to be very powerful so they have to go find it
        {
            //The force of the throw is increased 
            ThrowForce = 30f;
            ThrowUpwardForce = 20f;
            StartingBallThrown = true;
            if (fakeBall != null)
            {
                Destroy(fakeBall, 3f);//this will destroy the starting ball so the player doesnt confuse it with the actual ball they will have to find. 
            }
        }
        else//If the player is throwing something after the start of the game has happened
        {
            //the throwing forces are a lot more realistic 
            ThrowForce = 5f;
            ThrowUpwardForce = 5f;
        }
        //This is where the physicss are re enabled for the item that has been thrown 

        if (ItemRigidBody) //check if the player is holding an item.
        {
          
            ItemRigidBody.isKinematic = false;

            ItemCollider.isTrigger = false;

            
            ItemRigidBody.transform.SetParent(null);//unparents the thrown item from the weapon container so it no longer follows the player around.

           

           
            Vector3 airResistanceForce = (PlayerCamera.transform.forward * ThrowForce * (1 - AirResistance)) + (Vector3.up * ThrowUpwardForce);//finds the direction of the throw (where the player is facing) as well as the force of their throw with air resistance taken into account.
            ItemRigidBody.AddForce(airResistanceForce, ForceMode.Impulse); //apply the throwing force with air resistance.

         
            ItemRigidBody.AddForce(Vector3.down * Gravity * ItemRigidBody.mass, ForceMode.Acceleration);   //apply gravity as a downward force.

            // Reset variables indicating the player is no longer holding an item.
            ItemRigidBody = null;
            ItemCollider = null;
            HoldingItem = false;
            StartingBallThrown = true;
             
        }
    }
}