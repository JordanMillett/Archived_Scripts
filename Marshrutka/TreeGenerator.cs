using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGenerator : MonoBehaviour
{
    public int ForestGarbageCount = 1000;
    public int TreeCount = 1000;
    public GameObject TreePrefab;
    public GameObject ForestGarbagePrefab;
    public float HeightOffset;
    public string SpawnableTag;
    public float LandSize;
    public float RaycastHeight;
    public Vector2 ScaleRange;
    public Vector2 HeightRange;
    public Vector2 ThicknessRange;

    public bool isWorking = false;

    public IEnumerator Generate()
    {
        int BreatheInterval = 250;

        //Debug.Break();

        isWorking = true;

        for(int i = 0; i < ForestGarbageCount; i++)
        {
            if(i % BreatheInterval == 0) //pause every 250 trees
                yield return null;

            GenerateObject(false);
        }

        for(int i = 0; i < TreeCount; i++)
        {
            if(i % BreatheInterval == 0) //pause every 250 trees
                yield return null;

            GenerateObject(true);
        }

        isWorking = false;
    }

    void GenerateObject(bool Tree)
    {
        Vector3 RayStart = new Vector3(Random.Range(-LandSize, LandSize), RaycastHeight, Random.Range(-LandSize, LandSize));

        RaycastHit hit;
        if(Physics.Raycast(RayStart, -Vector3.up, out hit, RaycastHeight * 2f))
        {
            if(hit.collider.gameObject.CompareTag(SpawnableTag))
            {
                if(Tree)
                {
                    GameObject NewTree = GameObject.Instantiate(TreePrefab, hit.point + new Vector3(0f, HeightOffset, 0f), Quaternion.identity);
                    NewTree.transform.SetParent(transform);
                    NewTree.transform.localEulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
                    float OverallScale = Random.Range(ScaleRange.x, ScaleRange.y);
                    float Thickness = Random.Range(ThicknessRange.x, ThicknessRange.y);
                    NewTree.transform.localScale = new Vector3(Thickness * OverallScale, Random.Range(HeightRange.x, HeightRange.y) * OverallScale, Thickness * OverallScale);
                }else
                {
                    GameObject NewGarbage = GameObject.Instantiate(ForestGarbagePrefab, hit.point + new Vector3(0f, 0f, 0f), Quaternion.identity);
                    NewGarbage.transform.SetParent(transform);
                    NewGarbage.transform.rotation = Quaternion.LookRotation(hit.normal, Vector3.up);
                    NewGarbage.transform.RotateAround(NewGarbage.transform.position, NewGarbage.transform.forward, Random.Range(0f, 360f));
                }
            }
        }
    }
}