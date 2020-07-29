using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hands : MonoBehaviour
{
    public GameObject Bag;
    public float PlaceRange;
    public bool Full = false;
    bool BagPlaced = false;
    PickupAble Current;
    public bool Busy = false;
    float lerpSpeed = 0.05f;
    PlayerController PC;

    void Start()
    {

        PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(Full && !Busy)
            {   
                if(Current.canDrop)
                    Place();
            }
        }

        /*if(Input.GetMouseButtonDown(1))
        {
            if(!BagPlaced)
            {
                PlaceBag();
            }
        }*/
        
    }

    public bool Pickup(PickupAble PA)
    {
        if(!Full && !Busy)
        {
            Full = true;
            Current = PA;
            StartCoroutine(PickupAnimation(PA));
            return true;
        }else
        {
            return false;
        }
    }

    void Place()
    {
        Transform Cam = this.transform.parent;

        RaycastHit hit;

		if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, PlaceRange))
		{
            if(hit.normal.y > 0.8f)
            {
                Current.transform.SetParent(null);
                
                StartCoroutine(PlaceAnimation(hit.point));
            }
        }
        
    }

    public void DeleteActiveItem()
    {

        Destroy(Current.gameObject);

        Current = null;
        Full = false;
        Busy = false;

    }

    IEnumerator PickupAnimation(PickupAble PA)
    {

        Busy = true;
        float alpha = 0f;

        Vector3 StartPos = Current.transform.position;
        Vector3 StartRot = Current.transform.eulerAngles;
        Vector3 StartScale = Current.transform.localScale;

        Current.GetComponent<Collider>().enabled = false;

        while (alpha < 1f)
        {
            Current.transform.position = Vector3.Lerp(StartPos, transform.TransformPoint(this.transform.localPosition + PA.holdPos), alpha);
            Current.transform.eulerAngles = Vector3.Lerp(StartRot, this.transform.eulerAngles + PA.holdRot, alpha);
            Current.transform.localScale = Vector3.Lerp(StartScale, new Vector3(PA.holdScale, PA.holdScale, PA.holdScale), alpha);

            alpha += lerpSpeed;
            yield return null;
        }

        PC.ItemSpeedMultiplier = PA.HoldSpeedMultiplier;

        Current.transform.SetParent(this.transform);

        Current.transform.localPosition = PA.holdPos;
        Current.transform.localEulerAngles = PA.holdRot;
        Current.transform.localScale = new Vector3(PA.holdScale, PA.holdScale, PA.holdScale);

        Busy = false;

    }

    IEnumerator PlaceAnimation(Vector3 DesPos)
    {
        Busy = true;
        float alpha = 0f;

        Vector3 StartPos = Current.transform.position;
        Quaternion StartRot = Current.transform.rotation;
        Vector3 StartScale = Current.transform.localScale;

        Vector3 Direction = DesPos - new Vector3(this.transform.position.x, DesPos.y, this.transform.position.z);

        PC.ItemSpeedMultiplier = 1f;

        while (alpha < 1f)
        {   
            Current.transform.position = Vector3.Lerp(StartPos, DesPos, alpha);
            Current.transform.rotation = Quaternion.Lerp(StartRot, Quaternion.LookRotation(Direction, Vector3.up), alpha);
            Current.transform.localScale = Vector3.Lerp(StartScale, Vector3.one, alpha);

            alpha += lerpSpeed;
            yield return null;
        }

        Current.GetComponent<Collider>().enabled = true;

        Current.transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
        Current.transform.position = DesPos;
        Current.transform.localScale = Vector3.one;

        Current = null;
        Full = false;
        Busy = false;

    }

    void PlaceBag()
    {
        Transform Cam = this.transform.parent;

        RaycastHit hit;

		if(Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, PlaceRange))
		{
            if(hit.normal.y > 0.8f)
            {
                Bag.SetActive(true);
                Bag.transform.SetParent(null);
                Bag.transform.position = hit.point;
                Vector3 Direction = hit.point - new Vector3(this.transform.position.x, hit.point.y, this.transform.position.z);
                Bag.transform.rotation = Quaternion.LookRotation(Direction, Vector3.up);
                Bag.transform.localScale = Vector3.one;
                BagPlaced = true;
            }
        }
    }

    public void PickupBag()
    {
        if(BagPlaced)
        {
            Bag.transform.SetParent(this.transform);
            Bag.transform.localPosition = Vector3.zero;
            Bag.transform.localEulerAngles = Vector3.zero;
            Bag.transform.localScale = Vector3.one;

            BagPlaced = false;
            Bag.SetActive(false);
        }
    }
    
}
