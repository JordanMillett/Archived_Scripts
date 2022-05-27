using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(BlenderHairToPrefab))]
public class BlenderHairToPrefabEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        BlenderHairToPrefab BHTP = (BlenderHairToPrefab) target;

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Convert"))
        {
            List<Vector3> Pos = new List<Vector3>();
            List<Quaternion> Rot = new List<Quaternion>();

            foreach (Transform child in BHTP.transform)
            {
                Pos.Add(child.position);
                Rot.Add(child.rotation * Quaternion.Euler(90f, 0f, 0f));
            }

            List<GameObject> children = new List<GameObject>();
            for(int i = 0; i < BHTP.transform.childCount; i++)
                children.Add(BHTP.transform.GetChild(i).gameObject);

            for(int i = 0; i < Pos.Count; i++)
            {
                DestroyImmediate(children[i]);

                GameObject Copy = (GameObject) PrefabUtility.InstantiatePrefab(BHTP.Prefabs[Random.Range(0, BHTP.Prefabs.Count)]) as GameObject;
                Copy.transform.SetParent(BHTP.transform);
                Copy.transform.localScale = Vector3.one;
                Copy.transform.position = Pos[i];
                Copy.transform.rotation = Rot[i];
            }
        }

        GUILayout.EndHorizontal();

    }
}