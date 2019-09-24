using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blaster : MonoBehaviour 
{
    GameObject cam;

    public GameObject Projectile;
    public float speed = 10000f;
    public float Offset = 0f;

    Rigidbody r;

    void Start()
    {

        cam = GameObject.FindWithTag("Player").transform.GetChild(0).gameObject;

    }

	void Update () 
    {
		if(Input.GetMouseButtonDown(0) && this.gameObject.layer == 10)
            Fire();

        //this.transform.eulerAngles = cam.transform.eulerAngles;

	}

    void Fire()
    {

        Vector3 OffsetVector = new Vector3(Random.Range(-15f,15f),Random.Range(-15f,15f),Random.Range(-15f,15f));
        OffsetVector *= Offset / 100f;

        GameObject laser = Instantiate(Projectile, this.gameObject.transform.position, this.gameObject.transform.rotation * Quaternion.Euler(cam.transform.eulerAngles.x, 0f,0f) * Quaternion.Euler(OffsetVector));
        //laser.transform.eulerAngles = new Vector3(cam.transform.eulerAngles.x * 5f, 0f,0f);
        //laser.transform.Rotate(90f,0f,0f);
        laser.transform.position += transform.forward * 1.2f;
        try{

            r = laser.gameObject.GetComponent<Rigidbody>();

            if(r == null)
            { 
                r = laser.gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
                r.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                Arrow a = laser.gameObject.AddComponent(typeof(Arrow)) as Arrow;
            }
        }catch{}

        r.AddForce(laser.transform.forward * speed);
        StartCoroutine(Delete(laser));

    }

    IEnumerator Delete(GameObject Gam)
    {
        yield return new WaitForSeconds(5f);
        try{
            Destroy(Gam);
        }catch {}

    }
}
