using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOverview : MonoBehaviour
{
    GameObject MapObject;
    Camera MapOverviewCamera;

    void OnEnable()
    {
        if(MapObject == null)
        {
            MapObject = GameObject.FindWithTag("MapOverviewCam").gameObject;
            MapOverviewCamera = MapObject.GetComponent<Camera>();
            MapOverviewCamera.enabled = true;
        }else
        {
            MapOverviewCamera.enabled = true;
        }
    }

    void OnDisable()
    {
        if(MapObject != null)
            MapOverviewCamera.enabled = false;
    }
}
