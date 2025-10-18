using UnityEngine;

public class FieldOfVision : MonoBehaviour
{
    public float sightRange = 15f;
    public float FieldOfVisionAngle = 45f;
    public LayerMask Player;

    public EnemyAI enemyai;
    private Vector3 directionToPlayer; 
    void Start()
    {
        enemyai = GetComponent<EnemyAI>();
    }

   

    public void Update(){
        //if zombie is aggressive its "senses" are increased 
        if (enemyai.isAggressive)
        {
            sightRange = 30f;
            FieldOfVisionAngle = 60f;
        }
    }

    public bool IsPlayerInVision(Transform player)
    {
       
        directionToPlayer = player.position - transform.position; //calculate the direction to the player

        //check if the angle between the object's forward direction and the direction to the player is within the FieldOfVisionAngle
        float angleToPlayer = Vector3.Angle(transform.forward, directionToPlayer);
        if (angleToPlayer < FieldOfVisionAngle * 0.5f)
        {

             //debug information to visualize the field of vision
            Vector3 forwardLine = Quaternion.Euler(0, FieldOfVisionAngle * 0.5f, 0) * transform.forward;
            Vector3 leftBoundary = Quaternion.Euler(0, -FieldOfVisionAngle * 0.5f, 0) * transform.forward;
            Vector3 rightBoundary = Quaternion.Euler(0, FieldOfVisionAngle * 0.5f, 0) * transform.forward;

            Debug.DrawRay(transform.position, forwardLine * sightRange, Color.yellow);
            Debug.DrawRay(transform.position, leftBoundary * sightRange, Color.yellow);
            Debug.DrawRay(transform.position, rightBoundary * sightRange, Color.yellow);
            
            //check if there are no obstacles between the object and the player
            RaycastHit hit;
            if (Physics.Raycast(transform.position, directionToPlayer.normalized, out hit, sightRange, Player))
            {
                if (hit.collider.CompareTag("Player"))
                {
                 
                   
                    return true;
                }
            }
        }
        

      
        return false;
    }

    void OnDrawGizmos()
    {
        //draw the field of vision in the scene view
        Gizmos.color = Color.yellow;

        //draw the forward line of the field of vision
        Vector3 forwardLine = Quaternion.Euler(0, FieldOfVisionAngle * 0.5f, 0) * transform.forward;
        Gizmos.DrawRay(transform.position, forwardLine * sightRange);

        //draw the left and right boundaries of the field of vision
        Vector3 leftBoundary = Quaternion.Euler(0, -FieldOfVisionAngle * 0.5f, 0) * transform.forward;
        Vector3 rightBoundary = Quaternion.Euler(0, FieldOfVisionAngle * 0.5f, 0) * transform.forward;

        Gizmos.DrawRay(transform.position, leftBoundary * sightRange);
        Gizmos.DrawRay(transform.position, rightBoundary * sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position, directionToPlayer.normalized * sightRange);
    }
}
