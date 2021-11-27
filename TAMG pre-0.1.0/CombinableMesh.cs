using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CombinableMesh : MonoBehaviour
{
    public bool Generated = false;
    public GameObject CombinedPrefab;
    public GameObject CombinedObject;

    public List<Vector3> CombinedPositions;
    public List<Quaternion> CombinedRotations;

    public int TotalVerts = 0;
    public int TotalObjects = 0;

    public void Generate()
    {
        //----------------- INITIALIZE VARIABLES/LISTS
        CombinableMeshObject[] CMOS = GetComponentsInChildren<CombinableMeshObject>();
    
        if(CMOS.Length == 0)
        {
            throw new Exception("Object has no children");
        }else if(CMOS.Length == 1)
        {
            throw new Exception("Object has only one child");
        }

        TotalObjects = CMOS.Length;

        TotalVerts = CMOS[0].MeshLODS[0].sharedMesh.vertices.Length * CMOS.Length;

        if(TotalVerts > 60000)
        {
            throw new Exception("Object has too many vertices");
        }

        CombinedPositions = new List<Vector3>();
        CombinedRotations = new List<Quaternion>();

        foreach (CombinableMeshObject CMO in CMOS)
        {
            CombinedPositions.Add(CMO.transform.position);
            CombinedRotations.Add(CMO.transform.rotation);
        }

        CombinedPrefab = PrefabUtility.GetCorrespondingObjectFromSource(CMOS[0].gameObject);

        //----------------- MAKE PARENT OBJECTS/COMPONENTS
        CombinedObject = new GameObject();
        CombinedObject.name = "Combined Object";
        CombinedObject.transform.SetParent(this.transform);
        MeshCollider CombinedObjectCollider = CombinedObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        LODGroup CombinedObjectLODGroup = CombinedObject.AddComponent(typeof(LODGroup)) as LODGroup;

        GameObject CombinedMapModel = new GameObject();
        CombinedMapModel.name = "Combined Map Model";
        CombinedMapModel.layer = 10;
        CombinedMapModel.transform.SetParent(CombinedObject.transform);
        Material MapMaterial = CMOS[0].MapMesh.GetComponent<MeshRenderer>().sharedMaterial;
        MeshRenderer MapMeshRenderer = CombinedMapModel.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
        MeshFilter MapMeshFilter = CombinedMapModel.AddComponent(typeof(MeshFilter)) as MeshFilter;
        MapMeshRenderer.sharedMaterial = MapMaterial;
        MapMeshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        GameObject CombinedModel = new GameObject();
        CombinedModel.name = "Combined Model";
        CombinedModel.transform.SetParent(CombinedObject.transform);

        //----------------- MAKE LOD OBJECTS AND MERGE MESHES

        int LODCount = CMOS[0].MeshLODS.Count;
        MeshRenderer[] Rends = new MeshRenderer[LODCount];

        for(int i = 0; i < CMOS.Length; i++)
        {
            PrefabUtility.UnpackPrefabInstance(CMOS[i].gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        }

        for(int i = 0; i < LODCount; i++)
        {
            GameObject CombinedLOD = new GameObject();
            CombinedLOD.name = "Model_LOD" + i;
            CombinedLOD.transform.SetParent(CombinedModel.transform);

            Material ModelMaterial = CMOS[0].MeshLODS[i].GetComponent<MeshRenderer>().sharedMaterial;
            MeshFilter ModelMeshFilter = CombinedLOD.AddComponent(typeof(MeshFilter)) as MeshFilter;
        
            MeshFilter[] modelMeshFilters = new MeshFilter[CMOS.Length];
            CombineInstance[] modelCombine = new CombineInstance[CMOS.Length];

            for(int x = 0; x < CMOS.Length; x++)
            {
                modelMeshFilters[x] = CMOS[x].MeshLODS[i];
            }

            for(int x = 0; x < CMOS.Length; x++)
            {
                modelCombine[x].mesh = modelMeshFilters[x].sharedMesh;
                modelCombine[x].transform = modelMeshFilters[x].transform.localToWorldMatrix;
            }

            ModelMeshFilter.sharedMesh = new Mesh();
            ModelMeshFilter.sharedMesh.CombineMeshes(modelCombine);

            MeshRenderer ModelMeshRenderer = CombinedLOD.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
            ModelMeshRenderer.sharedMaterial = ModelMaterial;
            ModelMeshRenderer.shadowCastingMode = CMOS[0].MeshLODS[i].GetComponent<MeshRenderer>().shadowCastingMode;
            Rends[i] = ModelMeshRenderer;
        }  

        //----------------- GENERATE MAP MESH
        CombineInstance[] mapCombine = new CombineInstance[CMOS.Length];
        MeshFilter[] mapMeshFilters = new MeshFilter[CMOS.Length];

        for(int i = 0; i < CMOS.Length; i++)
        {
            mapMeshFilters[i] = CMOS[i].MapMesh;
        }

        for(int i = 0; i < CMOS.Length; i++)
        {
            mapCombine[i].mesh = mapMeshFilters[i].sharedMesh;
            mapCombine[i].transform = mapMeshFilters[i].transform.localToWorldMatrix;
        }

        MapMeshFilter.sharedMesh = new Mesh();
        MapMeshFilter.sharedMesh.CombineMeshes(mapCombine);

        CombinedObjectCollider.sharedMesh = Rends[CMOS[0].ColliderLODIndex].GetComponent<MeshFilter>().sharedMesh;

        //Debug.Log(Rends[0].GetComponent<MeshFilter>().sharedMesh.vertices.Length);

        MeshRenderer[,] LODRenderers = new MeshRenderer[LODCount, 1];  //only 1 mesh per lod group

        for(int i = 0; i < LODCount; i++)
        {
            LODRenderers[i, 0] = Rends[i];
        }

        //----------------- MAKE LODS WITH EACH RENDERER
        LOD[] CombinedMeshLODS = new LOD[LODCount];
        for(int i = 0; i < LODCount; i++)
        {   
            MeshRenderer[] MeshRenArray = new MeshRenderer[]{LODRenderers[i,0]};

            if(i < LODCount - 1)
            {
                CombinedMeshLODS[i] = new LOD(0.85f / (i + 1f), MeshRenArray);
            }else
            {
                CombinedMeshLODS[i] = new LOD(0.005f, MeshRenArray);
            }
        }
        CombinedObjectLODGroup.SetLODs(CombinedMeshLODS);

        //----------------- FINALIZE
        foreach (CombinableMeshObject CMO in CMOS)
        {
            DestroyImmediate(CMO.gameObject);
        }

        CombinedObjectLODGroup.RecalculateBounds();

        AddTag();

        Generated = true;
    }

    public void Split()
    {
        for (int i = 0; i < CombinedPositions.Count; i++)
        {
            GameObject Copy = (GameObject) PrefabUtility.InstantiatePrefab(CombinedPrefab) as GameObject;
            Copy.transform.SetParent(this.transform);
            Copy.transform.position = CombinedPositions[i];
            Copy.transform.rotation = CombinedRotations[i];
            Copy.transform.localScale = new Vector3(1f, 1f, 1f);
        }

        DestroyImmediate(CombinedObject);
        CombinedPrefab = null;
        CombinedPositions.Clear();
        CombinedRotations.Clear();
        Generated = false;
    }

    public void Regenerate(bool _split)
    {
        if(Generated)
        {
            if(_split)
            {
                Split();
            }else
            {
                Split();
                Generate();
            }
        }
    }

    public void RegenerateAll(bool _split)
    {
        GameObject[] CMS = GameObject.FindGameObjectsWithTag("Combinable");

        for (int i = 0; i < CMS.Length; i++)
        {
            CMS[i].GetComponent<CombinableMesh>().Regenerate(_split);
        }
    }

    public void AddTag()
    {
        this.gameObject.tag = "Combinable";
    }
    
    public void RemoveTag()
    {
        this.gameObject.tag = "Untagged";
    }
}
