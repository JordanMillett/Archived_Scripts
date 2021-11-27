using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EQ_Scanner : MonoBehaviour
{
    public AudioClip SoundEffect;
    AudioSource AS;
    bool isOn = false;
    GameObject Cam;
    public TextMeshProUGUI Contents;

    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    public void ToggleOn()
    {
        isOn = true;
        Cam = GameObject.FindWithTag("Camera").gameObject;
    }

    void Update()
    {
        if(isOn)
        {
            RaycastHit hit;

            if(Physics.Raycast(Cam.transform.position,Cam.transform.forward, out hit, 10f))
            {
                try 
                {
                            
                    Package P = hit.collider.transform.gameObject.GetComponent<Package>();

                    if(P != null)
                    {
                        if(Input.GetMouseButtonDown(0))
                        {
                            P.Scanned();
                            AS.volume = (.5f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f);
                            AS.clip = SoundEffect;
                            AS.Play();
                        }
                        /*
                        if(Input.GetMouseButtonDown(0))
                        {
                            //GameObject.FindWithTag("Target").GetComponent<Target>().SetLocation(P.Destination);
                            //GameObject.FindWithTag("Compass").GetComponent<Compass>().MarkerLocation = P.Destination;
                            //P.Scanned();
                        }*/

                        Contents.text = P.Contents;
                        
                    }else
                    {
                        Contents.text = "NA";
                    }
                    
                }
                catch{}
        }else
        {
            Contents.text = "NA";
        }

            
        }
    }
}

/*
public TextMeshProUGUI ID;
    public TextMeshProUGUI Contents;
    public TextMeshProUGUI Durability;
    public GameObject Cam;
    float InteractRange = 10f;

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera").gameObject;
    }

    void Update()
    {
        RaycastHit hit;

		if(Physics.Raycast(Cam.transform.position,Cam.transform.forward, out hit, InteractRange))
		{
			try 
			{
						
				Package P = hit.collider.transform.gameObject.GetComponent<Package>();

				if(P != null)
				{
                    if(Input.GetMouseButtonDown(0))
                    {
                        GameObject.FindWithTag("Target").GetComponent<Target>().SetLocation(P.Destination);
                        //GameObject.FindWithTag("Compass").GetComponent<Compass>().MarkerLocation = P.Destination;
                        P.Scanned();
                    }

                    ID.text = P.ID.ToString();
                    Contents.text = P.Contents;
                    Durability.text = P.Durability.ToString() + "%";

                    
                }else
                {
                    ID.text = "NA";
                    Contents.text = "NA";
                    Durability.text = "NA";
                }
				
			}
			catch{}
        }else
        {
            ID.text = "NA";
            Contents.text = "NA";
            Durability.text = "NA";
        }
    }
*/
