using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Mirror;

public class LoadingScreen : MonoBehaviour
{
    public SimpleBar LoadingBar;
    MenuManager MM;

    public static LoadingScreen LS;

    public TextMeshProUGUI Status;

    int TotalSteps = 4;
    public int Step = 0;

    void OnEnable()
    {
        Step = 0;
        LoadingBar.Current = 0f;
        Status.text = "Loading Map";
        LS = this;

        MM = GameObject.FindWithTag("Camera").GetComponent<MenuManager>();
        LoadingBar.Current = 0f;
        Invoke("ChangeScene", 0.25f);
    }
    
    void ChangeScene()
    {
        StartCoroutine(Load());
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Single);
    }

    public void NextStep(string LoadingStatus)
    {
        Status.text = LoadingStatus;
        Step++;
    }
    
    IEnumerator Load()
    {
        while(Step != TotalSteps)
        {
            yield return null;
            LoadingBar.Current = ((float)Step/(float)TotalSteps) * 100f;
        }

        Status.text = "Finished";
        
        yield return new WaitForSeconds(0.25f);

        MM.SetScreen("HUD");

        //GameObject.FindWithTag("Information").GetComponent<OnlineManager>().StartClient();
    }
}
