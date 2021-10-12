using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDivider : MonoBehaviour
{
    public Transform AreaEmpty;
    public int DistFromOrigin;

    public Transform GetArea(int x, int y)
    {
        for(int i = 0; i < AreaEmpty.childCount; i++)
        {
            if(AreaEmpty.GetChild(i).gameObject.name == x.ToString() + " " + y.ToString())
                return AreaEmpty.GetChild(i);
        }

        return null;
    }

    public bool AreaExists(int x, int y)
    {
        for(int i = 0; i < AreaEmpty.childCount; i++)
        {
            if(AreaEmpty.GetChild(i).gameObject.name == x.ToString() + " " + y.ToString())
                return true;
        }

        return false;
    }
}