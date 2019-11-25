using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{

    LifeManager LM;
    bool Running = false;
    public float DownTime = 2f;
    public float DownAngle = 30f;
    float currentAngle = 0;
    float alpha = 0;

    void Start()
    {
        LM = GetComponent<LifeManager>();
        
        //LM.SpawnLocation = this.transform.position;
    }

    void Update()
    {
        if(LM.Health != LM.MaxHP && !Running)
        {
            LM.Health = LM.MaxHP;
            StartCoroutine(Fall());

        }
    }

    IEnumerator Fall()
    {

        Running = true;


        while (alpha < 1f)
        {

            currentAngle = Mathf.Lerp(currentAngle, DownAngle, alpha);
            
            //this.transform.parent.transform.localEulerAngles = Vector3.Lerp(this.transform.parent.transform.localEulerAngles, new Vector3(DownAngle,0f,0f), alpha);
            this.transform.parent.transform.localEulerAngles = new Vector3(currentAngle, this.transform.parent.transform.localEulerAngles.y, this.transform.parent.transform.localEulerAngles.z);
            alpha += 0.1f;
            yield return null;

        }

        yield return new WaitForSeconds(DownTime);
    
        while (alpha > 0f)
        {

            currentAngle = Mathf.Lerp(currentAngle, 0f, 1 - alpha);
            //this.transform.parent.transform.localEulerAngles = Vector3.Lerp(this.transform.parent.transform.localEulerAngles, new Vector3(90f,0f,0f), 1 - alpha);
            this.transform.parent.transform.localEulerAngles = new Vector3(currentAngle, this.transform.parent.transform.localEulerAngles.y, this.transform.parent.transform.localEulerAngles.z);
            alpha -= 0.03f;
            yield return null;

        }

        
        Running = false;
        LM.Health = LM.MaxHP;

    }


}
