using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
public class Clouds : MonoBehaviour
{
    public Shader shader;
    public Transform container;
    Material material;

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {   
        if(material == null)
        {
            material = new Material(shader);
        }

        Debug.Log("Called");

        material.SetVector ("boundsMin", container.position - container.localScale / 2);
        material.SetVector ("boundsMax", container.position + container.localScale / 2);
        //material.SetTexture ("_MainTex", source);

        Graphics.Blit(source, destination, material);
    }
}
