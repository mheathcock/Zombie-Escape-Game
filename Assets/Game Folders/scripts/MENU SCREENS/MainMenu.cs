using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
   public void Play(){

    SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex +1);//load the main game scene
   }

   public void QuitGame()
   {
    Debug.Log("Game has quit");
    Application.Quit();
   }
}
