using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EyesShader : MonoBehaviour
{
    MeshRenderer _meshRenderer;
    
    public Transform Eyes;
    
    Transform Cam;
    
    void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        Cam = GameObject.FindWithTag("Camera").GetComponent<Transform>();
    }

    void Update()
    {
        Vector3 Dir = (Eyes.transform.position - Cam.transform.position).normalized;

        float angle = Vector3.SignedAngle(Eyes.transform.forward, Dir, Eyes.transform.right);

        _meshRenderer.materials[3].SetFloat("_Look", angle/40f);
    }
}
