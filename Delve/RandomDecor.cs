using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomDecor : MonoBehaviour
{
    public List<GameObject> Presets;
    
    void Start()
    {
        int ranIndex = Random.Range(0, Presets.Count);
        GameObject Props = Instantiate(Presets[ranIndex], Vector3.zero, this.transform.rotation);
        Props.transform.parent = transform.GetChild(1);
        Props.transform.localPosition = Vector3.zero;
    }
}
