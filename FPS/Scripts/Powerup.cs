using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Powerup : MonoBehaviour
{

    public Weapon W;

    public float Damage;
    public float RPM;
    public float Accuracy;
    public float RecoilAmount;
    public float Force;
    public float ReloadTime;
    public int Bullets;
    public int MagSize;

    void Start()
    {
        


    }

    void OnCollisionEnter(Collision collision)
    {
            if(collision.gameObject.GetComponent<PlayerController>() != null)
                Activate();
        
    }

    public void Activate()
    {

        W.WC.Damage += Damage;
        W.WC.RPM += RPM;
        if(W.WC.Accuracy + Accuracy < 100f)
            W.WC.Accuracy += Accuracy;
        W.WC.RecoilAmount += RecoilAmount;
        W.WC.Force += Force;
        W.WC.RecoilAmount += ReloadTime;
        W.WC.Bullets += Bullets;
        W.WC.MagSize += MagSize;

        Destroy(this.gameObject);

    }

}
