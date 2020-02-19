using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAnimator : MonoBehaviour
{
    public GameObject SceneCamera;

    PlayerController PC;
    GameObject Cam;

    public bool usePlayer = true;

    public List<ClipData> Scenes; 

    void Start()
    {
        SceneCamera.SetActive(false);
        if(usePlayer)
        {
            PC = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            Cam = PC.transform.GetChild(0).transform.gameObject;
        }else
        {

            Cam = GameObject.FindWithTag("MainCamera").gameObject;

        }
    }

    public void PlayClip(int Index)
    {
        if(usePlayer)
            PC.Frozen = true;
        Cam.SetActive(false);
        SceneCamera.SetActive(true);
        

        StartCoroutine(Clip(Index));

    }

    public void NewFrame(int Index, int Frame)
    {

        

        if(Index >= Scenes[Index].Positions.Count - 1)
        {
            Scenes[Index].Positions.Add(SceneCamera.transform.position);
            Scenes[Index].Rotations.Add(SceneCamera.transform.rotation);
        }
        else
        {
            Scenes[Index].Positions.Insert(Frame + 1,SceneCamera.transform.position);
            Scenes[Index].Rotations.Insert(Frame + 1,SceneCamera.transform.rotation);
        }

    }

    public void DeleteClip(int Index)
    {

        Scenes.RemoveAt(Index);

    }

    public void DeleteFrame(int Index, int Frame)
    {

        Scenes[Index].Positions.RemoveAt(Frame);
        Scenes[Index].Rotations.RemoveAt(Frame);

    }

    public void SetName(int Index, string Name)
    {

        Scenes[Index].Name = Name;

    }

    public void NewClip(int Index)
    {

        ClipData StartData = ScriptableObject.CreateInstance<ClipData>();

        List<Vector3> Pos = new List<Vector3>();
        List<Quaternion> Rot = new List<Quaternion>();

        StartData.Name = "Untitled";
        StartData.Positions = Pos;
        StartData.Rotations = Rot;

        if(Index >= Scenes.Count - 1)
            Scenes.Add(StartData);
        else
            Scenes.Insert(Index + 1, StartData);

    }

    IEnumerator Clip(int Index) //Lerp Animation, Use file and gizmo to record
    {

        //yield return new WaitForSeconds(2f);

        SceneCamera.transform.position = Scenes[Index].Positions[0];
        SceneCamera.transform.rotation = Scenes[Index].Rotations[0];

        for(int i = 1; i < Scenes[Index].Positions.Count; i++)
        {
            float alpha = 0f;

            while(alpha < 1f)
            {
                //yield return new WaitForSeconds(2f);
                yield return null;
                //SceneCamera.transform.position = Scenes[Index].Positions[i];
                SceneCamera.transform.position = Vector3.Lerp(Scenes[Index].Positions[i - 1], Scenes[Index].Positions[i], alpha);
                //SceneCamera.transform.eulerAngles = Vector3.Lerp(Scenes[Index].Rotations[i - 1], Scenes[Index].Rotations[i], alpha);
                SceneCamera.transform.rotation = Quaternion.Lerp(Scenes[Index].Rotations[i - 1], Scenes[Index].Rotations[i], alpha);
                

                alpha += 0.01f;
                //SceneCamera.transform.eulerAngles = Scenes[Index].Rotations[i];
            }

        }

        //yield return new WaitForSeconds(0f);
        yield return null;
        SceneCamera.SetActive(false);
        Cam.SetActive(true);
        if(usePlayer)
            PC.Frozen = false;

    }

}
