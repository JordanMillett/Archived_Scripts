using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KartPreviewer : MonoBehaviour
{
    public GameObject KartPrefab;

    GameObject ModelRef;

    Manager M;
    //Tally all parts to get lerp for highest and lowest on stat bars

    public int BodyIndex = 0;
    public int WheelIndex = 0;

    void Awake()
    {
        M = GameObject.FindWithTag("Manager").GetComponent<Manager>();

        BodyIndex = Random.Range(0, M.KartBodies.Count);
        WheelIndex = Random.Range(0, M.KartWheels.Count);

        Refresh(BodyIndex, WheelIndex);
    }

    public void Refresh(int _BodyIndex, int _WheelIndex)
    {
        BodyIndex = _BodyIndex;
        WheelIndex = _WheelIndex;

        KartConfig KC = new KartConfig();
        KC.KB = M.KartBodies[_BodyIndex];
        KC.KW = M.KartWheels[_WheelIndex];

        if(ModelRef)
            Destroy(ModelRef);

        ModelRef = Instantiate(KartPrefab, this.transform.position, this.transform.rotation);
        ModelRef.GetComponent<KartController>().Load(KC);
        ModelRef.GetComponent<KartController>().Frozen = true;
        ModelRef.GetComponent<Rigidbody>().isKinematic = true;
        ModelRef.transform.parent = this.transform;
    }

    public void Clear()
    {
        Destroy(ModelRef);
    }

}
