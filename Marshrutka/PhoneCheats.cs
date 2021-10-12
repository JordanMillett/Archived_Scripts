using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PhoneCheats : MonoBehaviour
{
    //DATATYPES
    public enum Cheat
    {
        none,
        superspeed,
        zerog,
        bigboys,
        errorcity
    }

    //PUBLIC COMPONENTS

    //PUBLIC VARS

    //PUBLIC LISTS

    //COMPONENTS

    //VARS

    //LISTS

    public void ActivateCheat(Cheat C)
    {
        GameObject.FindWithTag("Manager").GetComponent<SaveController>().SetCheating(true);

        switch(C)
        {
            case Cheat.superspeed :
                GameObject.FindWithTag("Player").GetComponent<PlayerController>().MotorForce *= 10f;
            break;
            case Cheat.zerog :
                GameObject.FindWithTag("Player").GetComponent<Rigidbody>().useGravity = false;
            break;
            case Cheat.bigboys :
               
                List<GameObject> All = new List<GameObject>();
                Scene scene = SceneManager.GetActiveScene();
                scene.GetRootGameObjects(All);
                for(int i = 0; i < All.Count; i++)
                {
                    try
                    {
                        if(All[i].GetComponent<Passenger>() != null)
                        {
                            All[i].transform.GetChild(0).GetChild(0).transform.localScale = new Vector3
                            (
                                GameSettings.WeightLerp.y + All[i].transform.GetChild(0).GetChild(0).transform.localScale.x, 
                                All[i].transform.GetChild(0).GetChild(0).transform.localScale.y,
                                GameSettings.WeightLerp.y + All[i].transform.GetChild(0).GetChild(0).transform.localScale.z
                            );
                        }
                    }catch{}
                }
            break;
            case Cheat.errorcity :
                GameObject.FindWithTag("Player").GetComponent<Rigidbody>().useGravity = false;
                GameObject.FindWithTag("Player").transform.position = new Vector3(1000000f, 0f, 1000000f);
            break;
        }

    }
}