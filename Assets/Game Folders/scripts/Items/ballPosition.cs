using UnityEngine;

public class BallPosition : MonoBehaviour
{
    public GameObject Ball;

    void Start()
    {
        SpawnBall();
    }

    void SpawnBall()
    {
        Vector3 randomPosition = GetRandomSpawnPosition();

        
        if (IsPositionClear(randomPosition))//check if the spawnPosition is ok
        {
            Instantiate(Ball, randomPosition, Quaternion.identity);//spawn the ball
        }
        else
        {
            //if the position is occupied try spawning again 
            SpawnBall();
        }
    }
/*
    Gets a random set of co ordinates in the house
*/
    Vector3 GetRandomSpawnPosition()
    {
        //the x and z are the dimensions of the house
        float xPos = Random.Range(8, 19);
        float zPos = Random.Range(-12, 5);
        float yPos = (Random.Range(0, 2) == 0) ? 2 : 7;//these are the values of slightly above floor 1 and floor 2. it chooses a random one of these two. basically either first or second floor

        return new Vector3(xPos, yPos, zPos);
    }
/*
A bool to check if the spawn Position is not in a wall or an object
*/
    bool IsPositionClear(Vector3 position)
    {
        
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right, Vector3.up, Vector3.down };

        
        float raycastDistance = 1f;

        foreach (var direction in directions)
        {
            Ray ray = new Ray(position, direction);//creates a raycast using the directions specified 

          
            if (Physics.Raycast(ray, raycastDistance))  //perform the raycast
            {
             
                return false;//if the ray hits something the position is occupied
            }
        }

        return true;//if none of the raycasts hit anything the position is clear

    }
}
