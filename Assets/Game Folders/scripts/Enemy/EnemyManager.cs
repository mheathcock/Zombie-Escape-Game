using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject zombiePrefab;//reference to the zombie enemy game object
     public int numberOfZombies = 10;
     public int RandomSpawn;

    // Start is called before the first frame update
    void Start()
    {
       /* inside this for loop is what will spawn the zombies. 
       It actually does 1 less than the number to spawn as there is the original zombie game object which counts as that extra one 
       */
         for (int i = 1; i < numberOfZombies; i++)
         {
           int randomSpawn = Random.Range(1, 3);//this is a random number thats either 1 or 2 and it'll decide if the enemy being spawned shall be spawned near the house or the woods.
           Debug.Log("rspawn "+ randomSpawn);
              Vector3 spawnPosition;

            //spawn near house
            if (randomSpawn == 1)
            {
                  spawnPosition = new Vector3(Random.Range(-10f, 5f), 0f, Random.Range(-10f, 10f));//these random numbers are x and z co-ordinates that correspond to around the house or further into the woods
            }
            //spawn near woods
            else{
             spawnPosition = new Vector3(Random.Range(-91f, -22f), 0f, Random.Range(-16f, 2f));

            }

             
            Quaternion spawnRotation = Quaternion.identity;

            GameObject zombie = Instantiate(zombiePrefab, spawnPosition, spawnRotation);//this is the line that'll "spawn" the enemy with its rotation and position. 
            Enemy enemyScript = zombie.GetComponent<Enemy>();//reference to the zombies enemy script

            enemyScript.enemyID = i;//this sets the ID of the zombie to the current for loop iteration. example: an enemy spawned on iteration 4 will have an ID of 4.
         }

        
    }

   }
