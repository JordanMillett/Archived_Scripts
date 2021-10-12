using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DashObject : MonoBehaviour
{
    public UnityEvent OnPickup;
    public UnityEvent OnPutDown;

    public float DistanceOffset = 1f;
    public bool PickedUp = false;
    public bool Locked = false;
    Vector3 ReturnPosition;
    Transform ReturnParent;
    Quaternion ReturnRotation;
    PlayerController PC;
    Transform Cam;

    public AudioClip PickupSound;
    public AudioClip PutdownSound;

    public AudioSourceController ASC;

    void Start()
    {
        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        Cam = GameObject.FindWithTag("Camera").transform;

        ReturnParent = this.transform.parent;
        ReturnPosition = this.transform.localPosition;
        ReturnRotation = this.transform.localRotation;
    }

    public void Pickup()
    {
        if(!Locked && !PickedUp)
        {
            GetComponent<Collider>().isTrigger = true;

            this.transform.SetParent(Cam);
            this.transform.localPosition = new Vector3(0f, 0f, DistanceOffset);
            this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);

            this.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Unlit", 1f);

            PC.NoLook = true;
            PickedUp = true;
            
            ASC.Sound = PickupSound;
            ASC.SetVolume(1f);
            ASC.Play();

            OnPickup.Invoke();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
                    

            StartCoroutine(PutDown());
        }
    }

    IEnumerator PutDown()
    {
        while(PickedUp)
        {
            yield return null;

            if(Input.GetMouseButtonDown(1))
            {
                RemoveFromHands();
            }
        }
    }

    public void RemoveFromHands()
    {
        this.transform.SetParent(ReturnParent);
        this.transform.localPosition = ReturnPosition;
        this.transform.localRotation = ReturnRotation;

        this.transform.GetChild(0).GetComponent<MeshRenderer>().material.SetFloat("_Unlit", 0f);
                
        PC.NoLook = false;
        PickedUp = false;

        ASC.Sound = PutdownSound;
        ASC.SetVolume(1f);
        ASC.Play();

        OnPutDown.Invoke();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Delete()
    {
        PC.NoLook = false;
        Destroy(this.gameObject);
    }
}
