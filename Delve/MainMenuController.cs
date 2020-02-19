using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public CameraAnimator CA;
    public GameObject MainMenu;
    
    public void Play()
    {

        StartCoroutine(PlayAnimation());

    }

    IEnumerator PlayAnimation()
    {

        MainMenu.SetActive(false);
        CA.PlayClip(0);
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }

    public void Exit()
    {

        Application.Quit();

    }
}
