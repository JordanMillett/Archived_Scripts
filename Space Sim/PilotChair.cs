using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PilotChair : MonoBehaviour
{

    Vector3 ExitPos;
    Ship S;
    GameObject Player;

    public bool Enabled;

    void Start()
    {
        S = GameObject.FindWithTag("Ship").GetComponent<Ship>();
        Player = GameObject.FindWithTag("Player");
    }

    public void Sit()
    {
        
        Enabled = !Enabled;

        if(Enabled)
        {
            Player.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
            Player.GetComponent<Rigidbody>().isKinematic = true;
            Player.GetComponent<PlayerController>().ResetRots();
            Player.GetComponent<PlayerController>().enabled = false;
            ExitPos = Player.transform.position;
            Player.transform.position = this.transform.position;
            StartCoroutine(Sitting());
        }else
        {
            Player.GetComponent<Rigidbody>().isKinematic = false;
            Player.GetComponent<Rigidbody>().collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Player.GetComponent<PlayerController>().enabled = true;
            Player.transform.position = ExitPos;
        }
    }

    IEnumerator Sitting()
    {

        yield return new WaitForSeconds(0.25f);

        bool Sat = true;

        while(Sat)
        {
            if(Input.GetKeyDown(KeyCode.F))
                Sat = false;

            yield return null;

        }

        Sit();

    }
}
