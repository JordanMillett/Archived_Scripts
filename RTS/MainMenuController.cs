using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void Play() //take a bunch of inputs eventually
    {
    
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Quit()
    {

        Application.Quit();
        Debug.Log("CLOSED APPLICATION");

    }
}
