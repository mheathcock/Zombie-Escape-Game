using UnityEngine;
using System.Collections; 

public class WeaponController : MonoBehaviour
{
    //variables
    public bool CanAttack = true;
    public float AttackCoolDown = 1f;
    public bool isAttacking = false;
    public bool HittingEnemy = false;
    public float Damage = 20f;
     public int attackerID;
    //references
    public GameObject Axe;

    //audio source
    public AudioSource swingingSound;


    //This is the attack function it takes the held items rigidbody as a parameter. 
    public void Attack(Rigidbody CurrentRigidBody)
    {
        if (CanAttack)//if the player is allowed to attack
        {
            isAttacking = true;//then they are currently attacking
            CanAttack = false;// therefore shouldn't be able to start another attack whilst in the middle of attacking

            if (CurrentRigidBody != null)
            {
                
                Animator itemAnimator = CurrentRigidBody.GetComponent<Animator>();//get the held items animator


                if (itemAnimator != null)//if the held item does have an animator 
                {
                    itemAnimator.SetTrigger("AttackTrigger");//play attack animation
                    swingingSound.Play();//play an audio sound to reference to the player they are attacking something
                }
            }

            StartCoroutine(ResetCooldown());
        }
    }


    //This is a cooldown co routine that'll allow the attack animation to play out before the player can try attack again.
    IEnumerator ResetCooldown()
    {
        StartCoroutine(ResetAttack());
        yield return new WaitForSeconds(AttackCoolDown);//slight further delay
        CanAttack = true;//the player is allowed to attack again
    }
    
    IEnumerator ResetAttack()
    {
            yield return new WaitForSeconds(0.75f);//length of attacking animation 
            isAttacking = false;//the player is no longer attacking
    }
  

}
