using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class RandomizeObject : MonoBehaviour
{
    public List<GameObject> Objects;

    public void Randomize()
    {
        int RandomIndex = Random.Range(0, Objects.Count + 1);

        if(RandomIndex != 0)
        {
            GameObject Copy = (GameObject) PrefabUtility.InstantiatePrefab(Objects[RandomIndex - 1]) as GameObject;
            Copy.transform.position = this.transform.position;
            Copy.transform.rotation = this.transform.rotation;
            Copy.transform.SetParent(this.transform.parent);
            Copy.transform.localScale = this.transform.localScale;
            DestroyImmediate(this.gameObject);
        }
            //Debug.LogWarning("No Random Object to Spawn In Place!");
        
    }
}
