using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Won : MonoBehaviour
{
    // Start is called before the first frame update
  public void MainMenu(){

     SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -3);//load the main menu screen
  }
}
