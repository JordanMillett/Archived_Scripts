using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Phone : MonoBehaviour
{
    public GameObject PhoneCam;
    public GameObject Flash;
    Transform Cam;

    bool PhoneOn = false;

    bool TakingPhoto = false;

    public void ToggleOn()
    {
        PhoneCam.SetActive(true);
        PhoneOn = true;
    }

    void Start()
    {
        Cam = GameObject.FindWithTag("Camera").transform;
    }

    IEnumerator TakePhoto()
    {
        TakingPhoto = true;

        yield return new WaitForSeconds(0.15f);

        Flash.SetActive(true);

        RaycastHit hit;

        if(Physics.Raycast(Cam.position, Cam.forward, out hit, 25f))
        {
            Collider[] Nearby = Physics.OverlapSphere(hit.point, 1f);
            foreach(Collider Col in Nearby)
            {
                try 
                {   
                    Package P = Col.transform.gameObject.GetComponent<Package>();

                    if(P != null)
                        P.Delivered();
                            
                }
                catch{}
            }
        }

        yield return new WaitForSeconds(1f);

        Flash.SetActive(false);
        TakingPhoto = false;
    }
    
    public void StartPhotoEffect()
    {
        StartCoroutine(PhotoEffect());
    }
    
    IEnumerator PhotoEffect()
    {
        TakingPhoto = true;
        yield return new WaitForSeconds(0.15f);

        Flash.SetActive(true);

        yield return new WaitForSeconds(1f);

        Flash.SetActive(false);
        TakingPhoto = false;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0) && PhoneOn && !TakingPhoto && !UI.Instance.Busy())
        {
            StartCoroutine(TakePhoto()); 
        }
    }
}
