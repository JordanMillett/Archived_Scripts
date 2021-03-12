using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombinableMeshObject : MonoBehaviour
{
    public List<MeshFilter> MeshLODS;
    public MeshFilter MapMesh;
    public int ColliderLODIndex = 0;
}
