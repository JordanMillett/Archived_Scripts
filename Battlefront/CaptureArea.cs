using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptureArea : MonoBehaviour
{
    public CommandPost Parent;

    List<AIUnit> goodUnits = new List<AIUnit>();
    List<AIUnit> badUnits = new List<AIUnit>();

    void Refresh()
    {

        for(int i = 0; i < goodUnits.Count; i++)
        {
            if(goodUnits[i].dead)
                goodUnits.RemoveAt(i);
        }

        for(int i = 0; i < badUnits.Count; i++)
        {
            if(badUnits[i].dead)
                badUnits.RemoveAt(i);
        }

        Parent.CurrentGood = goodUnits.Count;
        Parent.CurrentBad = badUnits.Count;

    }

    void OnTriggerEnter(Collider col)
    {

        if(col.transform.gameObject.GetComponent<AIUnit>() != null)
        {
            GameObject newUnit = col.transform.gameObject;
            //Units.Add(newUnit.GetComponent<SimpleAI>());

            if(newUnit.GetComponent<AIUnit>().teamIndex == 1)
                goodUnits.Add(newUnit.GetComponent<AIUnit>());
            else
                badUnits.Add(newUnit.GetComponent<AIUnit>());

            /*if(newUnit.GetComponent<SimpleAI>().teamType)
                Parent.CurrentGood++;
            else
                Parent.CurrentBad++;*/
            Refresh();
            Parent.Refresh();
            
        }

    }

    void OnTriggerExit(Collider col)
    {

        if(col.transform.gameObject.GetComponent<AIUnit>() != null)
        {
            GameObject newUnit = col.transform.gameObject;
            //Units.Add(newUnit.GetComponent<SimpleAI>());

            if(newUnit.GetComponent<AIUnit>().teamIndex == 1)
                goodUnits.Remove(newUnit.GetComponent<AIUnit>());
            else
                badUnits.Remove(newUnit.GetComponent<AIUnit>());

            /*if(newUnit.GetComponent<SimpleAI>().teamType)
                Parent.CurrentGood--;
            else
                Parent.CurrentBad--;*/
            Refresh();
            Parent.Refresh();
            
        }

    }
}
