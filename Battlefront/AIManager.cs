using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    public List<CommandPost> CommandPosts = new List<CommandPost>();

    public Transform goodRespawn;
    public Transform badRespawn;

    public List<AIClass> Classes;

    void Start()
    {
        GameObject[] CommandPosts_Array = GameObject.FindGameObjectsWithTag("CommandPost");
        for(int i = 0; i < CommandPosts_Array.Length; i++)
        {
            CommandPosts.Add(CommandPosts_Array[i].GetComponent<CommandPost>());
        }
    }

    public AIClass GetClass()
    {

        int Index = Random.Range(0, Classes.Count);

        return Classes[Index];

    }

    public bool FoundUncapturedPosts(int teamIndex)
    {

        foreach(CommandPost CP in CommandPosts)
        {

            if(CP.State != teamIndex)
            {

                return true;

            }

        }

        return false;

    }

    public Vector3 GetSpawnPoint(int teamIndex)
    {

        List<int> FreePoints = new List<int>();

        for(int i = 0; i < CommandPosts.Count; i++)
        {

            if(CommandPosts[i].State == teamIndex)
            {

                FreePoints.Add(i);

            }

        }

        if(FreePoints.Count > 0)
        {

            int RanIndex = Random.Range(0, FreePoints.Count);

            float radius = 5f;

            Vector3 RespawnPosition = CommandPosts[FreePoints[RanIndex]].transform.position + new Vector3(Random.Range(-radius, radius), 0f, Random.Range(-radius, radius));

            return RespawnPosition;
        }else
        {

            
            if(teamIndex == 1)
                return goodRespawn.position;
            else
                return badRespawn.position;
        
        }

    }

    public Vector3 GetCommandPost(int teamIndex, int Preference)
    {
        List<int> Attackables = new List<int>();

        for(int i = 0; i < CommandPosts.Count; i++)
        {

            if(CommandPosts[i].State != teamIndex)
            {
                
                Attackables.Add(i);

                //return CP.transform.position;

            }

        }

        if(Attackables.Count > 0)
        {

            int leftover = Preference;

            if(leftover <= Attackables.Count)
                return CommandPosts[Attackables[leftover - 1]].transform.position;

            while(leftover > Attackables.Count)
            {
                leftover -= Attackables.Count;
            }

            return CommandPosts[Attackables[leftover - 1]].transform.position;

        }else
        {
            Debug.LogWarning("No Free Command Posts for Team : " + teamIndex);
            return Vector3.zero;
        }

    }
}
