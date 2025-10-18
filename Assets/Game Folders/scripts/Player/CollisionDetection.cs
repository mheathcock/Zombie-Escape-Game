using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{

     //variables
    public float Damage;
    public float maxDistance = 10f; 

    private bool hasEntered = false;
    private bool hasHitEnemy = false;
    private bool isCoolingDown = false;
    private float cooldownTime = 1f; 

    //References
    public WeaponController weaponcontroller;
    public Camera PlayerCamera;
    public FlashText flashtext;
    public Enemy enemyScript;
    private Collider lastHitCollider;

   

    //these ints are used when telling the player to aim for the head to do more damage
    public int bodyHitXTimes = 0;//keeps track of how many times the body has been hit
    public int StopMessageSpamInt = 0;//keeps track of how many times the message to hit the head has been shown.



    void Start()
    {
        //find the FlashText component on the "HurtZombie" GameObject
        GameObject hurtZombieObject = GameObject.Find("HurtZombie");
        GameObject zombieObject = GameObject.Find("Zombie");

        if (hurtZombieObject != null)
        {
            flashtext = hurtZombieObject.GetComponent<FlashText>();

                if (flashtext == null)
                {
                    Debug.LogError("FlashText not found on HurtZombie");
                }
            }
            else
            {
                Debug.LogError("HurtZombie not found.");
            }

     
        
    }
    /*
    This is the collision detection function for when the weapon enters the enemy collider
    */
    private void OnTriggerEnter(Collider other)
    {
        if (weaponcontroller == null)
        {
            Debug.LogError("WeaponController is null");
            return;
        }
        /*
            /this if statement is basically checking if the player has not yet entered the collision (technically they have when this function is called but this flag is more used so the damage is only applied once 
            per collision), if the player is actually trying to attack the enemy. and if the enemy hasn't been hit yet and a cooldown isnt ongoing.
        */
        if (!hasEntered && weaponcontroller.isAttacking && !hasHitEnemy && !isCoolingDown)
        {
            Enemy enemyScript = other.GetComponentInParent<Enemy>();//reference to the enemy script
            if (enemyScript != null)
            {
                
                int enemyID = enemyScript.enemyID;  //get the enemy ID from the collidedEnemy

                if (IsRaycastHitOnEnemy())//calls the bool function to find out which hitbox on the enemy has been hit (the head collider or the body collider)
                {


                    if (lastHitCollider.CompareTag("HeadCollider"))//this is the specific hit collider for the enemys head
                    {
                        Debug.Log("HIT HEAD");

                        weaponcontroller.HittingEnemy = true;
                        Damage = 15f;//this is the amount of damage for hitting the enemies head
                        lastHitCollider.GetComponent<Animator>().SetTrigger("hit");
                        enemyScript.TakeDamage(Damage, enemyID);//calls the enemy script to tell the specific enemy (using their id) to take damage

                        if (flashtext != null)
                        {
                            flashtext.UpdateMessage("HEADSHOT!!");//this is a visual representation for the player that they have hit the enemys head. 
                        }
                    }
                    else if (lastHitCollider.CompareTag("BodyCollider"))//this is the specific hit collider for the enemys body
                    {
                        //a check to tell the player to aim for the head if theyve hit the body too many times.
                        bodyHitXTimes +=1;//the body has been hit so add 1 to the counter
                        if(bodyHitXTimes >= 3)
                        {
                            StopMessageSpamInt +=1;//increase the message has been flashed counter
                            bodyHitXTimes = 0;//reset the body has been hit counter so it can be checked if more than 3 again
                            
                            if (StopMessageSpamInt < 2)//to stop spamming the player if the message has been shown more than 2 times stop showing the message.
                            {
                                  flashtext.UpdateMessage("AIMING FOR THE HEAD DOES MORE DAMAGE!");//calls the FlashText script to display the message to aim for the head
                            }
                          
                        }
                        Debug.Log("HIT BODY");

                        weaponcontroller.HittingEnemy = true;
                        Damage = 5f;//this is the amount of damage for hitting the enemies head
                        enemyScript.TakeDamage(Damage, enemyID);//calls the enemy script to tell the specific enemy (using their id) to take damage
                    }
                }

                hasEntered = true;//the player has now collided and hit the enemy so to make sure the collision doesnt run again before the player re-attacks the flags are set 
                hasHitEnemy = true; 

                //start cooldown so cant spam hurt the enemy
                StartCoroutine(Cooldown());
            }
            else
            {
                
            }
        }
    }

    private IEnumerator Cooldown()
    {
        isCoolingDown = true;
        yield return new WaitForSeconds(cooldownTime);
        isCoolingDown = false;
    }

/*
    This is the logic for when the weapon collider leaves the enemies collider
*/
    public void OnTriggerExit(Collider other)
    {
        if (weaponcontroller == null)
        {
            Debug.LogError("WeaponController is null");
            return;
        }

        if (hasEntered)
        {
            //reset the flag when the weapon exits the collision
            hasEntered = false;
            weaponcontroller.HittingEnemy = false;

        }
        hasHitEnemy = false;
    }

    private bool IsRaycastHitOnEnemy()//bool function to see if the player is looking at the enemy(and this will help decide where the enemy has been hit by the player)
    {
        Ray ray = PlayerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0f));//shoots a raycast from where players camera looking(acts as where the crosshair is on screen)
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance, ~LayerMask.GetMask("axebladeTrigger")));//the ~LayerMask means to ignore this Layer, this is to ensure the raycast doesnt hit the axe's collider
        {
            lastHitCollider = hit.collider;  //store the last hit collider
            return true;
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        //visualize the ray in Scene view
        if (PlayerCamera != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(PlayerCamera.transform.position, PlayerCamera.transform.forward * maxDistance);
        }
    }
}
