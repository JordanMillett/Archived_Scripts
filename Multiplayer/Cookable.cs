using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cookable : MonoBehaviour
{
  
    public float CookedAmount = 0;
    public bool Dirty = false;
    public Renderer[] Rends;
    public List<Material> Mats = new List<Material>();
    Color C;

    //use renderer array in future

    void Start()
    {
        Rends = this.transform.GetChild(0).GetComponentsInChildren<Renderer>();
        foreach (Renderer R in Rends)
            Mats.Add(R.material);
        
        //Mat = this.transform.GetChild(0).GetComponent<Renderer>().material;
        //Color C = Mat.color;
        Color C = Mats[0].color;

    }

    void Update()
    {
        if(CookedAmount > 0f)
            foreach (Material M in Mats)
                M.SetColor("_BaseColor",Color.Lerp(M.color, Color.black, CookedAmount/100f));

            //Mat.SetColor("_BaseColor",Color.Lerp(Mat.color, Color.black, CookedAmount/100f));
    }
}
