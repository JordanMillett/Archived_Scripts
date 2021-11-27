using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConstantObject : MonoBehaviour
{
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        //Debug.Log(this.gameObject.name);
        if(SceneManager.GetActiveScene().buildIndex == 0)
            Destroy(this.gameObject);
    }
}
