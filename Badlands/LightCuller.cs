using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightCuller : MonoBehaviour
{
    Light _light;

    void Start()
    {
        _light = GetComponent<Light>();
    }
    
    void Update()
    {
        _light.shadows = Vector3.Distance(new Vector3(this.transform.position.x, 0f, this.transform.position.z), Player.P.transform.position) < 30f ? LightShadows.Soft : LightShadows.None;
    }
}
