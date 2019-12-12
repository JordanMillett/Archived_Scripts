using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    GameObject Player;
    GameObject Camera;

    Vector3 Yaw_Dir;
    Vector3 Pitch_Dir;

    Rigidbody R;

    public float MoveSpeed;
    public float Delay;
    public float StopDistance;

    float CurrentDistance;

    void Start()
    {
        Player = GameObject.FindWithTag("Player");
        Camera = this.transform.GetChild(2).gameObject;
        R = GetComponent<Rigidbody>();
    }

    void Update()
    {
        CurrentDistance = Vector3.Distance(Camera.transform.position, Player.transform.GetChild(0).transform.position);
        LookAt(Player.transform.GetChild(0).transform.position);

        if(CurrentDistance > StopDistance)
            StartCoroutine(MoveTowards());
    }

    IEnumerator MoveTowards()
    {
        yield return new WaitForSeconds(Delay);
        if(CurrentDistance > StopDistance)
            R.MovePosition(R.position + (transform.forward * MoveSpeed) * Time.fixedDeltaTime);
    }

    public void LookAt(Vector3 Pos)
    {
        Yaw_Dir = this.transform.position - Pos;
        Yaw_Dir = new Vector3(Yaw_Dir.x, 0f, Yaw_Dir.z);

        Pitch_Dir = Camera.transform.position - Pos;
        Pitch_Dir = new Vector3(0f , Pitch_Dir.y, Vector3.Distance(Camera.transform.position, Pos));
        
        this.transform.eulerAngles = new Vector3(0f, -Vector3.SignedAngle(Yaw_Dir, Vector3.forward, Vector3.up) + 180f,0f);

        Camera.transform.localEulerAngles = new Vector3(Vector3.SignedAngle(Pitch_Dir, Vector3.forward, Vector3.right), 0f, 0f);
    }
}
