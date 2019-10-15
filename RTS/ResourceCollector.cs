using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceCollector : MonoBehaviour
{

    public int Money;
    public int Oil;
    public int Metal;

    public float Interval;

    //rising canvas notification above collectors

    public GameObject NotePrefab;
    public float Y_Location;
    public float Speed;
    public float Lifetime;

    ResourceController Rc;

    void Start()
    {
        Rc = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ResourceController>();
        InvokeRepeating("Collect", Interval, Interval);
    }

    void Collect()
    {
        if(Money > 0)
        {
            Rc.Change(0, Money);
            Note(0, Money);
        }

        if(Oil > 0)
        {
            Rc.Change(1, Oil);
            Note(1, Oil);
        }

        if(Metal > 0)
        {
            Rc.Change(2, Metal);
            Note(2, Metal);
        }

    }

    void Note(int Index, int Amount)
    {

        GameObject N = Instantiate(NotePrefab, this.transform.position + new Vector3(-.5f, Y_Location, 0f), Quaternion.identity);
        Rise R = N.GetComponent<Rise>();

        R.Speed = Speed;

        for(int i = 0; i < 3; i++)
            if(i != Index)
                N.transform.GetChild(i).gameObject.SetActive(false);
            else
                N.transform.GetChild(i).gameObject.SetActive(true);

        N.transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = Amount.ToString();

        StartCoroutine(Delete(N, Lifetime));

    }

    IEnumerator Delete(GameObject Gam, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        Destroy(Gam);
    

    }



}
