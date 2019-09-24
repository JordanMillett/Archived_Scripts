using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Customer : MonoBehaviour
{
    public Transform Seat_Location;
    public float Speed;
    public Animator an;

    public Renderer Shirt;
    public Gradient ShirtColors;

    public Renderer Pants;
    public Gradient PantsColors;

    public Renderer Skin;
    public Gradient SkinColors;

    public Gradient HairColors;

    public GameObject[] Hair;
    public GameObject[] FaceAccessory;

    void Start()
    {
        GetColors();
        GetMisc();
        StartCoroutine(Sit());

    }

    void GetColors()
    {

        Shirt.material.SetColor("_BaseColor",ShirtColors.Evaluate(Random.Range(0f,1f)));
        Pants.material.SetColor("_BaseColor",PantsColors.Evaluate(Random.Range(0f,1f)));
        Skin.material.SetColor("_BaseColor",SkinColors.Evaluate(Random.Range(0f,1f)));

    }

    void GetMisc()
    {

        //int Index = Random.Range(0,Hair.Length);
        //instantiate Hair 

    }

    IEnumerator Sit()
    {

        float startTime = Time.time;
        float journeyLength = Vector3.Distance(this.transform.position, Seat_Location.position);
        float fracJourney = 0f;
        float distCovered = 0f;

        while(fracJourney < 1f)
        {
            distCovered = (Time.time - startTime) * Speed;
            fracJourney = distCovered / journeyLength;
            this.transform.position = Vector3.Lerp(this.transform.position,Seat_Location.position,fracJourney);
            this.transform.eulerAngles = Vector3.Lerp(this.transform.eulerAngles,Seat_Location.eulerAngles,fracJourney);
            yield return null;

            if(fracJourney > .5f)
                an.SetBool("Sitting",true);

        }

        


    }
}
