using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Compass : MonoBehaviour
{
    public RectTransform CompassOne;
    public RectTransform CompassTwo;
    public RectTransform CompassThree;

    float yPos = -8f;
    float xOffset = 0f;
    float xLimit = 1280f;
    float zeroing = 640f;

    float CameraHeading = 180f;
    Transform Cam;
    Transform Player;

    public RectTransform Marker;
    public Vector3 MarkerLocation;

    Vector3 Direction;

    void OnEnable()
    {
        //Debug.Log("LOAD");
        MarkerLocation = Vector3.zero;
        Cam = GameObject.FindWithTag("Camera").transform;
        Player = GameObject.FindWithTag("Player").transform;
        CameraHeading = Cam.transform.eulerAngles.y;

        CompassOne.anchoredPosition = new Vector2(zeroing - xLimit, yPos);
        CompassTwo.anchoredPosition = new Vector2(zeroing, yPos);
        CompassThree.anchoredPosition = new Vector2(zeroing + xLimit, yPos);
    }

    void Update()
    {
        //Debug.Log(CameraHeading);
        CameraHeading = Cam.transform.eulerAngles.y;

        xOffset = (xLimit/-360f) * CameraHeading;
        
        CompassOne.anchoredPosition = new Vector2(zeroing - xLimit + xOffset, yPos);
        CompassTwo.anchoredPosition = new Vector2(zeroing + xOffset, yPos);
        CompassThree.anchoredPosition = new Vector2(zeroing + xLimit + xOffset, yPos);

        if(MarkerLocation == Vector3.zero)
        {
            Marker.gameObject.SetActive(false);
        }else
        {
            Marker.gameObject.SetActive(true);

            Direction = Player.position - new Vector3(MarkerLocation.x, Player.position.y, MarkerLocation.z);
            Direction = Vector3.Normalize(Direction);

            float angle = -Vector3.SignedAngle(Player.forward, Direction, Vector3.up) + 180f;

            xOffset = (xLimit/-360f) * angle;

            if(xOffset < -640f)
                xOffset = xOffset + 1280f;

            Marker.anchoredPosition = new Vector2(xOffset, 0f);

            float Distance = Vector3.Distance(Player.position, new Vector3(MarkerLocation.x, Player.position.y, MarkerLocation.z));
            if(Distance > 25f)
                Distance = 25f;

            Distance /= 25f;
            
            float size = Mathf.Lerp(1f, 0.5f, Distance);
            Marker.localScale = new Vector2(size, size);
        }
    }
}
