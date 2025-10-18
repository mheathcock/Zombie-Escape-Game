using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FlashScreen : MonoBehaviour
{
    //variables
    public Image flashImage;
    
    public float flashImageDuration = 0.5f;


    void Start(){
        flashImage.enabled = false;//the screen should only turn red when called.
    }
    public void FlashEffect()//when this script is called the screen will flash red to indicate the player has taken damage
    {
        StartCoroutine(FlashRED());
    }

    public IEnumerator FlashRED()
    {
      
        flashImage.enabled = true;
       
        yield return new WaitForSeconds(flashImageDuration);//after x time hide the red screen
        flashImage.enabled = false;
       
    }
}
