using UnityEngine;

public class PlayerMovement : MonoBehaviour
{

    //variables
    private Vector3 playerMovementInput;
    private Vector2 playerMouseInput;

    private float xRotation;   

    public Rigidbody playerBody;

    public float moveSpeed = 3;
    public float sensitivity;
    public bool isRunning = true;
    private bool stillMoving;

    //jumping variables
    public float gravityMultiplier;
    public float jumpForce;
    public bool canJump;

   


    //references

    public FlashText flashtext;
    public Transform playerCamera;

    //audio sounds
     public AudioSource walkingSound;


    
  

    void Start()
    {

        //initialize compononents 
        GameObject flashTextObject = GameObject.Find("HurtLegTrigger"); 
        flashtext = flashTextObject.GetComponent<FlashText>();
        
    }

  

    private void Update()
    {
        isRunning = !Input.GetKey(KeyCode.LeftShift);//check if the player is NOT holding the left shift button (basically check if they are not walking)
      
        playerMovementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));  //get the movement inputs
        playerMouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));//also get the mouse movement updates so we know where the player is looking.

        if (canJump && Input.GetKeyDown(KeyCode.Space))//if statement checking if the player is trying to jump.
        {
            Jump();
        }

        MovePlayer();
        MovePlayerCamera();
    }

    private void MovePlayer()
    {
        bool isWalking = Input.GetKey(KeyCode.LeftShift);
        bool isMoving = playerBody.velocity.magnitude > 0.1f;

        if (!isRunning)//if the player is walking then their moveSpeed is lower 
        {
            moveSpeed = 3;

            if (stillMoving)
            {
                StopWalkingSound();
            }
        }
        else//if the player is running then their moveSpeed should be higher.
        {
            moveSpeed = 6;

            if (!isWalking && isMoving && !stillMoving)
            {
                PlayWalkingSound();
            }
            else if (isWalking || !isMoving && stillMoving)
            {
                StopWalkingSound();
            }
        }

        stillMoving = isMoving;


        Vector3 moveVector = transform.TransformDirection(playerMovementInput) * moveSpeed;//calculate move vector 
        playerBody.velocity = new Vector3(moveVector.x, playerBody.velocity.y, moveVector.z);//calculate player velocity.

        playerBody.AddForce(Vector3.up * Physics.gravity.y * gravityMultiplier, ForceMode.Acceleration);
    }

    private void PlayWalkingSound()
    {
        if (walkingSound != null && !walkingSound.isPlaying && isRunning)
        {
            walkingSound.Play();
          
        }
    }

    private void StopWalkingSound()
    {
        if (walkingSound != null && walkingSound.isPlaying)
        {
            walkingSound.Stop();
          
        }
    }

    private void MovePlayerCamera()
    {
        transform.Rotate(0, playerMouseInput.x * sensitivity, 0); //rotate the player based on mouse input 

        xRotation -= playerMouseInput.y * sensitivity;  //adjusts the vertical rotation of the camera 
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);//clamp the vertical rotation to prevent flipping
        playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
    }

    //Jump function
    private void Jump()
    {
        if(flashtext.legInjured)//This is important. At the start of the game the player can jump quite high however when they try jump over a gate they will injure their leg and not be able to jump as high. 
        {//this is after their leg is injured.
            jumpForce = 6;
        }
        else{//this is before the leg is injured 
            jumpForce = 10;
        }
        playerBody.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);//add upwards force
        canJump = false;//whilst in mid air they shouldn't be able to jump again (there is no ground to jump on)
    }
    //This is a basic collision detection script for detecting if the player is touching the ground
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("isGround"))//the ground has a tag notifying it isGround
        {
            canJump = true;//if the player is touching the ground they are allowed to jump again
        }
    }
}
