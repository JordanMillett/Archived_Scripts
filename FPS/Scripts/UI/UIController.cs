using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{

    public bool Paused = false;

    GameObject Hud;
    GameObject PauseMenu;

    GameObject EndScreen;

    PlayerController PC;

    public GameObject HitMarker;

    void Start()
    {
        PC = transform.parent.parent.GetComponent<PlayerController>();
        Hud = transform.GetChild(0).gameObject;
        PauseMenu = transform.GetChild(1).gameObject;

        EndScreen = transform.GetChild(2).gameObject;

        Hud.SetActive(true);
        PauseMenu.transform.GetChild(0).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(1).gameObject.SetActive(false);
        EndScreen.SetActive(false);
        //PauseMenu.SetActive(false);
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Pause();
        }
    
    }

    public void Pause()
	{
        Paused = !Paused;
        PC.enabled = !Paused;
        Hud.SetActive(!Paused);
        PauseMenu.transform.GetChild(0).gameObject.SetActive(Paused);
        PauseMenu.transform.GetChild(1).gameObject.SetActive(false);
        EndScreen.SetActive(false);

        if(Paused)
            Time.timeScale = 0f;
        else
            Time.timeScale = 1f;

        Cursor.visible = Paused;

	}

    public void Restart()
    {
        Pause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    }

	public void Quit()
	{

        Debug.Log("Quit");
		//Pause ();
		//Cursor.visible = true;
		//GameObject loaderobj = GameObject.FindWithTag ("Loader");
		//Destroy(loaderobj);
		//SceneManager.LoadScene(0);
		Application.Quit();

	}

    public void Win(int State, float Timetaken)
    {

        Hud.SetActive(false);
        PauseMenu.transform.GetChild(0).gameObject.SetActive(false);
        PauseMenu.transform.GetChild(1).gameObject.SetActive(false);

        EndScreen.SetActive(true);

        TextMeshProUGUI EndScreenText = EndScreen.transform.GetChild(1).GetComponent<TextMeshProUGUI>();

        if(State == 1)
        {

            EndScreenText.text = "You Win!";

        }else if(State == -1)
        {

            EndScreenText.text = "You Lose!";

        }

    }
}
