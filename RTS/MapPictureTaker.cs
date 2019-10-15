using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPictureTaker : MonoBehaviour
{

    void Start()
    {
        
    }

    void Update()
    {
        
    }
    /* 
    public void TakePhoto()
    {
        Texture2D texture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, false);
        //Read the pixels in the Rect starting at 0,0 and ending at the screen's width and height
        texture.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0, false);
        texture.Apply();
        //Check that the display field has been assigned in the Inspector
        if (m_Display != null)
            //Give your GameObject with the renderer this texture
            m_Display.material.mainTexture = texture;
            //Reset the grab state
        }
    }
    */
}
