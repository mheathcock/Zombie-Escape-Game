using UnityEngine;
using System.Collections; 

public class Enemy : MonoBehaviour
{
    //Variables
    public int enemyID;
    public bool zombieHurt = false;
    private bool hasTakenDamageThisAttack = false;
    public float maxHealth = 100f;
    public float currentHealth;
    public bool isDead = false;

    //references
    [SerializeField] private WeaponController weaponcontroller;
    [SerializeField] private Healthbar healthbar;
    [SerializeField]private BloodSplatter bloodSplatter;
    [SerializeField] private Animator zombieAnimator;
    //audio variables
    public AudioSource hitSound;
    public AudioSource EnemyHurtSound;
    public AudioSource EnemyDiesSound;

   
   

    void Start()
    {
        bloodSplatter = GetComponent<BloodSplatter>();
        currentHealth = maxHealth;
        zombieAnimator = GetComponent<Animator>();
        healthbar.UpdateHealthBar(maxHealth, currentHealth);
    }
    /*
    Function that'll allow the enemy to lose health. it's called in other scripts that detect if the enemy has been hit. 
    It takes the damage and the enemyID as parameters so the correct amount of health is deducted from the correct enemy.
    */
    public void TakeDamage(float damage, int enemyID)
    {
        if (!isDead)
        {
            Debug.Log("Taking damage" + enemyID);
            currentHealth -= damage;//do damage to the enemy
              healthbar.UpdateHealthBar(maxHealth, currentHealth);//updates the visual representation of the enemy health for the player
            //play sound effects this will give the player an audio representation they have done damage to the enemy
            if (hitSound != null)
            {
                hitSound.Play();
            }
            if (EnemyHurtSound != null)
            {
                EnemyHurtSound.Play();
            }
            
            zombieHurt = true;

            //check if the bloodSplatter is not null
            if (bloodSplatter != null)
            {
                bloodSplatter.PlayBloodSplatter();//this is a particle system thatll just simulate blood shooting out from the enemy so the player has a visual representation they have done damage to the enemy
            }

            
            StartCoroutine(ResetHurtFlag());
        }

        
        if (currentHealth <= 0) //check if the enemy is dead
        {
            
            if (bloodSplatter != null)
            {
                bloodSplatter.PlayBloodSplatter();
            }
            
            isDead = true;
            
            if (EnemyDiesSound != null)//audio representation to the player that the zombie has died.
            {
                EnemyDiesSound.Play();
            }

            Die(enemyID);//the enemyID is passed so the correct enemy dies not all enemies
        }
    }

    private IEnumerator ResetHurtFlag()
    {
        yield return new WaitForSeconds(0.75f);
        zombieHurt = false;
        zombieAnimator.SetBool("hurt", false);
    }

    private IEnumerator WaitForAnim()
    {
        yield return new WaitForSeconds(1.4f);//this is the time that the death animation lasts for
    }

    void Die(int enemyID)
    {
         isDead = true;
        StartCoroutine(WaitForAnim());
    }
}
