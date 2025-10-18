using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver : MonoBehaviour
{
  
 

   public void RestartGame(){

      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);//reload the main game scene
   }
   public void BackToMainMenu(){

      SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -2);//load the main menu scene
   }
}
