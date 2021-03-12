using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Scanner : MonoBehaviour
{
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
}
