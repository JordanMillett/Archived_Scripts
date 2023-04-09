using UnityEngine;

[System.Serializable, CreateAssetMenu(fileName = "CustomPostProcessingMaterials", menuName = "Game/CustomPostProcessingMaterials")]
public class CustomPostProcessingMaterials : UnityEngine.ScriptableObject
{
    //---Your Materials---
    public Material VisionEffect;
    
    //---Accessing the data from the Pass---
    static CustomPostProcessingMaterials _instance;

    public static CustomPostProcessingMaterials Instance
    {
        get
        {
            if (_instance != null) return _instance;
            // TODO check if application is quitting
            // and avoid loading if that is the case

            _instance = UnityEngine.Resources.Load("CustomPostProcessingMaterials") as CustomPostProcessingMaterials;
            return _instance;
        }
    }
} 