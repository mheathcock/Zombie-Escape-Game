using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

public class EnemyAI : MonoBehaviour
{
    //states
    public enum EnemyState
    {
        Idle,
        Patrolling,
        Chasing,
        Attacking,
        Hurt,
        Dead
    }

    private enum PatrolSubState
    {
        Walking,
        Searching
    }

    private enum AttackSubState
    {
        Normal,
        Frenzy
    }
    //state variables
    private EnemyState currentState;
    private PatrolSubState patrolSubState;
    private AttackSubState attackSubState;

    //navigation/movement variables
    private Vector3 walkPoint;
    private bool walkPointSet;
    public float walkPointRange;
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private bool isHuntingPlayer;
    [SerializeField] private bool reachedWalkPoint = false;
    [SerializeField] private Transform[] huntingWaypoints;
    [SerializeField] private Transform[] houseWaypoints;
    public float patrolSpeed = 0.2f;
    public float chaseSpeed = 0.5f;
    public float rotationSpeed = 5f; 
    private int currentWaypointIndex = 0;


    //detection variables
    public LayerMask isGround, isPlayer;
    public float hearingRange = 10f;
    public bool isAggressive = false;
    public FieldOfVision fieldofvision;



    //attacking variables
    public bool isFrenzyFirstTime = true;
    public float attackRange = 2f;
    public float attackDamageRange = 3f;
    public float damage = 5f;
    public float timeBetweenAttacks = 2f;
    [SerializeField] private bool hasDealtDamage = false;
    private float lastAttackTime;
    public static bool isPlayerBeingAttacked = false;


    //references 
    public PlayerHealth playerhealth;
    public PlayerMovement playermovement;
    private Transform player;
    private NavMeshAgent agent;
    private Animator zombieAnimator;
    private Enemy enemy;

    

    [SerializeField] private bool ProbabilityCalled = false;
    
    public static List<EnemyAI> allEnemies = new List<EnemyAI>();
    public bool testingifinState = false;
    
 

    void Start()
    {
        //get referneces to compononents(incase its not set in editor)
        player = GameObject.Find("Player").transform;
        agent = GetComponent<NavMeshAgent>();
        zombieAnimator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        playermovement = player.GetComponent<PlayerMovement>();
        playerhealth = player.GetComponent<PlayerHealth>();
        fieldofvision = GetComponent<FieldOfVision>();

        //set the initial waypoint
        currentWaypointIndex = Random.Range(0, waypoints.Length);
        //calls Probability function at the start
        if (!ProbabilityCalled)
        {
            Probability();
            ProbabilityCalled = true;//the flag being set to true means this will only be called once.
        }
        allEnemies.Add(this);

        //start with Patrolling state
        ChangeState(EnemyState.Patrolling);
        EnterPatrolState();
    }

    void Update()
    {
        //if zombie is aggressive boost its ability to run and hear further. 
        //its fov is also increased in fov script
        if (isAggressive)
        {
            chaseSpeed = 4f;
            patrolSpeed = 2f;
            hearingRange = 20f;
            attackRange = 4f;
        }

    
        
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;

            case EnemyState.Patrolling:
                UpdatePatrolState();//this is where the substate is updated
                break;

            case EnemyState.Chasing:
                Chasing();
                break;

            case EnemyState.Attacking:
                if(enemy.currentHealth < 30)//this is basically the check to see if the zombie should be in a frenzy. if its low health it'll do more damage to try surive longer(player may try avoid it)
                {
                    attackSubState = AttackSubState.Frenzy;//the substate is updated here
                    
                }
                else{
                     attackSubState = AttackSubState.Normal;//the substate is updated here
                }
                Attacking();
                break;

            case EnemyState.Hurt:
                Hurt();
                break;
        }

        if (enemy.isDead)
        {
            KillZombie();
        }
    }

    void Idle()
    {
      
        agent.isStopped = true;  // stop moving
        zombieAnimator.SetBool("zIdle", true);//play idle animation
        StartCoroutine(IdleRoutine());
    }

    IEnumerator IdleRoutine()
    {
        yield return new WaitForSeconds(4f); //this is the duration of the idle animation.
        ChangeState(EnemyState.Patrolling);//state is set to patrolling
        EnterPatrolState();//but this function will decide which substate of patrolling (searching or normal)
    }

    /*this function is the probability function which will determine where the zombie should start patrolling. (should it patrol the house or the woods)

    the way it works is it generates a random number then checks if another random number is smaller than that. This was an attempt to create complete random chance
    instead of having it check against a predetermined value (example < 50)  
    */
    void Probability()
    {
        float guardWhereProb = Random.Range(1, 101);//this creates a random number between 1,100

        if (Random.Range(1, 101) <= guardWhereProb)//this creates another random number and checks if its smaller than guardWhereProb
        {
            isHuntingPlayer = false; //patrol House
        }
        else
        {
            isHuntingPlayer = true; //patrol woods (closer to player)
        }
    }

    //This is the function thatll decide which patrolling state to be in.
    void EnterPatrolState()
    {
        if (Random.Range(1,101) <= 20){//20% chance the zombie is given the searching state
             patrolSubState = PatrolSubState.Searching;
        }
        else{//80% chance the zombie is given the normal patrolling state
               patrolSubState = PatrolSubState.Walking;
        }
     
        
    }
    //updates the Patrol SubState to match 
    void UpdatePatrolState()
    {
        switch (patrolSubState)
        {
            case PatrolSubState.Walking:
                NormalPatrolling();
                break;
            case PatrolSubState.Searching:
                SearchingForPlayer();
                break;
           
        }
    }
   
   /*This is the basic patrolling SubState

    This patrolling uses predetermined waypoints placed around the map.
    They are either waypoints for hunting the player(just waypoints in the woods and closer to where the player spawns) or way points around the house
   */
    void NormalPatrolling()
    {
        
        //The field of vision is set here. when Normal patrolling the enemies have lower FOV as they arent really hunting for the player right now.
    fieldofvision.sightRange = 30f;
    fieldofvision.FieldOfVisionAngle = 60f;
    // patrolling speed
    agent.speed = patrolSpeed;
    agent.isStopped = false;
    //play walking animation
    zombieAnimator.SetBool("walk", true);
    zombieAnimator.SetBool("hit", false);
    zombieAnimator.SetBool("hurt", false);
    zombieAnimator.SetBool("dead", false);
    zombieAnimator.SetBool("zIdle", false);

    if (testingifinState)
    {
        
        Debug.Log("Starting to search");
        patrolSubState = PatrolSubState.Searching;
        ChangeState(EnemyState.Patrolling);
        
        return;
    }
    //this checks if the player is in the hearing range and running. if so the zombie can hear the player and should chase them
    if ((Vector3.Distance(transform.position, player.position) < hearingRange) && player.GetComponent<PlayerMovement>().isRunning)
    {
        //transition to chasing state
        ChangeState(EnemyState.Chasing);
    } 
    // if the player is not in hearing range or is trying to sneakily walk past the zombie  
     if (fieldofvision.IsPlayerInVision(player))//if the player is in the zombies fov
    {
   
        ChangeState(EnemyState.Chasing);
    }

    if (isHuntingPlayer)//if the enemy is hunting the player they should be using the huntingWaypoints
    {
        waypoints = huntingWaypoints;//these are the hunting waypoints(closer to the players spawnpoint)
    }
    else
    {
        waypoints = houseWaypoints;//these are waypoints nearer to the house
    }
    /*
        This is the waypoint logic
    */
   
    if (!agent.pathPending && agent.remainingDistance < 0.5f) //check if the enemy has reached the current waypoint
    {
        //set the next waypoint
        currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;//this will allow the enemy to loop through its way points. if it gets to the end of the array itll go back to the first element.
        agent.SetDestination(waypoints[currentWaypointIndex].position);//this just tells the enemy where next to walk to (its next waypoint)
    }

    }

    /*
    This is the second slightly more advanced patrolling SubState in the sense that it wont use predetermined waypoints ,
    but instead will find a random valid waypoint and move to that instead. its an attempt to simulate more random "searching for the player" behaviour
    the enemies are not patrolling a post but instead looking around.

    */
   void SearchingForPlayer()
{
     agent.speed = patrolSpeed;
    agent.isStopped = false;
    //play walking animation
    zombieAnimator.SetBool("walk", true);
    zombieAnimator.SetBool("hit", false);
    zombieAnimator.SetBool("hurt", false);
    zombieAnimator.SetBool("dead", false);
    zombieAnimator.SetBool("zIdle", false);
    testingifinState = true;

    //the fov is set here and this uses higher fov to again simulate the enemies actively looking for the player
    fieldofvision.sightRange = 40f;
    fieldofvision.FieldOfVisionAngle = 80f;

    /*
    this is the random movement code 
    i used https://docs.unity3d.com/ScriptReference/AI.NavMesh.SamplePosition.html to help find a random position
    */
    if (!agent.pathPending && agent.remainingDistance < 0.5f){//check if the enemy has reached the current waypoint
        Vector3 randomDirection = transform.position + Random.insideUnitSphere * 10;//this will generate a random direction within a sphere 
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, 10, 1 << NavMesh.GetAreaFromName("Walkable")))//will try to find a valid area on the navmesh that is within a distance of 10 from the randomDirection that was just made
        {
            agent.SetDestination(hit.position);
            
        }
    }
   

     //this checks if the player is in the hearing range and running. if so the zombie can hear the player and should chase them
    if ((Vector3.Distance(transform.position, player.position) < hearingRange) && player.GetComponent<PlayerMovement>().isRunning)
    {
        // Transition to chasing state
        ChangeState(EnemyState.Chasing);
    } 
    // if the player is not in hearing range or is trying to sneakily walk past the zombie  
     if (fieldofvision.IsPlayerInVision(player))//if the player is in the zombies fov
    {

        ChangeState(EnemyState.Chasing);
    }
}

         
/*
Chasing Function
*/
    void Chasing()
    {
        agent.speed = chaseSpeed;//set chasing speed
        agent.destination = player.position;//makes it so the enemy will move to where the player is effectively "chasing" the player

        //check if the player is in attack range
        if (Vector3.Distance(transform.position, player.position) < attackRange)
        {
            //transition to attacking state
            ChangeState(EnemyState.Attacking);
        }
        else
        {
            //this if statement is checking if the player is NOT in hearing range       OR  if they are in hearing range but NOT running (so are therefore silent to the zombie)
            if (!(Vector3.Distance(transform.position, player.position) < hearingRange) || (Vector3.Distance(transform.position, player.position) < hearingRange && !(player.GetComponent<PlayerMovement>().isRunning)))
            {
                if (!fieldofvision.IsPlayerInVision(player))//if player is no longer in fov
                {
                    //if the enemys patrol state already isnt "searching" then it has a greater chance of setting it to "searching" as a way of simulating the enemy looking for the player its just lost sight of.
                    if ((patrolSubState != PatrolSubState.Searching) && Random.Range(1, 101) < 40){
                                patrolSubState = PatrolSubState.Searching;
                    }
                    ChangeState(EnemyState.Patrolling);
                    EnterPatrolState();
                }
                else
                {
                   
                }
            }
        }
    }
void Attacking()
{
    float dstToPlayer = Vector3.Distance(transform.position, player.position);//gets the current distance to the players position

    Vector3 lookDir = player.position - transform.position;//tells the enemy what direction the player is in and therefore where it should turn to face
    lookDir.y = 0;
    Quaternion targetRotation = Quaternion.LookRotation(lookDir);

   
    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed); //smoothly rotate towards the player using slerp

   
    //play attack animation
    zombieAnimator.SetBool("walk", false);
    zombieAnimator.SetBool("hit", true);
    zombieAnimator.SetBool("hurt", false);
    zombieAnimator.SetBool("dead", false);
    zombieAnimator.SetBool("zIdle", false);

   if (attackSubState == AttackSubState.Frenzy) //if in frenzy SubState
        {
            
            damage = 10;//damage is increased in frenzy state to simulate the enemy being desperate and trying to force the player to back off 
        }
        else{
            damage = 5;
        }

    /*
    This section was to try make it so if the player gets swarmed by enemies it doesnt deplete their health really quickly
    it does this by checking if theres any enemies in the surrounding area and if so itll do less damage to the player when attacking
    https://docs.unity3d.com/ScriptReference/Physics.OverlapSphere.html
    */
    Collider[] hitColliders = Physics.OverlapSphere(transform.position, 3.5f);
    foreach (Collider col in hitColliders)
    {
        if (col.CompareTag("Enemy") && col.gameObject != gameObject)//check if the collider has the "Enemy" tag but also checks that its not checking itself

        {
          
            damage -= 3;
            break;
        }
    }

    if (!isPlayerBeingAttacked && Time.time - lastAttackTime > timeBetweenAttacks)
    {
        lastAttackTime = Time.time;

        StartCoroutine(DelayedDamage());
    }
}

IEnumerator DelayedDamage()
{
    yield return new WaitForSeconds(0.5f);
    
    if (Vector3.Distance(transform.position, player.position) < attackRange)//check if the player is still within attack range
    {
        

      
        transform.LookAt(player); //set the rotation to continuously look at the player

        if (playerhealth != null)
        {
            playerhealth.TakeDamage(damage);//calls the playerhealth script to do damage to the player
        }
    }

    hasDealtDamage = false;


    agent.isStopped = false;
    ChangeState(EnemyState.Chasing);  //transition back to chasing state
}

    void Hurt()
    {
        //play hurt animation
        zombieAnimator.SetBool("walk", false);
        zombieAnimator.SetBool("hit", false);
        zombieAnimator.SetBool("hurt", true);  
        zombieAnimator.SetBool("dead", false);
        zombieAnimator.SetBool("zIdle", false);
        
    

        StartCoroutine(ResetHurt());
    }

    private IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(1.4f);
        Destroy(gameObject);
    }

    IEnumerator ResetHurt()
    {
        yield return new WaitForSeconds(0.75f); //length of hurt animation
        zombieAnimator.SetBool("hurt", false);

       
        ChangeState(EnemyState.Chasing);
    }

    void ChangeState(EnemyState newState)
    {
        //set the new state
        currentState = newState;
    }

    void KillZombie()
    {
        zombieAnimator.SetBool("walk", false);
        zombieAnimator.SetBool("hit", false);
        zombieAnimator.SetBool("hurt", false);
        zombieAnimator.SetBool("dead", true);
        zombieAnimator.SetBool("zIdle", false);
        StartCoroutine(WaitForAnim());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, attackDamageRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, hearingRange);
    }
}




