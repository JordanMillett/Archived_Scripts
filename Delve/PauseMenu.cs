using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public PlayerController PC;
    public GameObject Menu;
    public Inventory I;

    public bool isEnabled = false;

    void Update()
    {

        if(Input.GetKeyDown(KeyCode.Escape))
        {

            Toggle();

            

        }

    }

    public void Toggle()
    {
        if(I.isOpen)
        {

            I.Toggle();

        }

        isEnabled = !isEnabled;
        PC.Frozen = isEnabled;
        Menu.SetActive(isEnabled);
        
        Cursor.visible = isEnabled;

        if(!isEnabled)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;

        

    }

    public void Restart()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }
    
    public void Quit()
    {

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);

    }
}
