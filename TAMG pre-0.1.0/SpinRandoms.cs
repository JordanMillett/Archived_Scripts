using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.EditorTools;

[EditorTool("Spin Randoms")]
public class SpinRandoms : EditorTool
{
    public void OnEnable()
    {
            GameObject[] AllObjects = GameObject.FindGameObjectsWithTag("RandomSpinnable");

            foreach (GameObject G in AllObjects)
            {
                G.transform.localEulerAngles = new Vector3(0f, Mathf.Round((Random.Range(0f, 360f)/90f))*90f, 0f);
            }
        
        Debug.Log("isdone");
    }
}
