using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ClickToSpawn : MonoBehaviour
{

    public GameObject Prefab;
    public bool SpawnOn = false;
  
    public void Spawn(Vector3 Position)
    {

        GameObject Copy = (GameObject) PrefabUtility.InstantiatePrefab(Prefab) as GameObject;
        Copy.transform.position = Position;
        Copy.transform.rotation = this.transform.rotation;
        Copy.transform.SetParent(this.transform);
        Copy.transform.localScale = this.transform.localScale;

    }
}
