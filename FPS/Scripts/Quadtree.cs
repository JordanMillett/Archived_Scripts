using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quadtree : MonoBehaviour
{
    
    public int SubdivisionLevel = 2;
    public int CurrentLevel = 0;
    public GameObject Root;
    //bool DontDestroy = false;

    void Start()
    {   
        /*
        if(CurrentLevel > 0)
            this.transform.SetParent(Root.transform);
        else
        {
            DontDestroy = true;
            Root = this.transform.gameObject;
        }*/
        //this.transform.rotation = Empty.transform.rotation;
    }

    void Update()
    {
        
    }

    public void Split(Vector3 Pos)
    {
        
        if(CurrentLevel <= SubdivisionLevel)
        {
            /* 
            if(CurrentLevel == 0)
            {
                Destroy(GetComponent<BoxCollider>());
                Destroy(GetComponent<MeshFilter>());
                Destroy(GetComponent<Renderer>());
                Destroy(this);
                Debug.Log("Did");
            }*/

            CurrentLevel++;

            GameObject Cube1 = Instantiate(this.transform.gameObject, this.transform.position, this.transform.rotation);

            GameObject Cube2 = Instantiate(this.transform.gameObject, this.transform.position, this.transform.rotation);

            GameObject Cube3 = Instantiate(this.transform.gameObject, this.transform.position, this.transform.rotation);

            GameObject Cube4 = Instantiate(this.transform.gameObject, this.transform.position, this.transform.rotation);
            

            Cube1.transform.Translate(Vector3.right * -this.transform.localScale.x/4f, Space.Self);
            Cube1.transform.Translate(Vector3.up * this.transform.localScale.y/4f, Space.Self);

            Cube2.transform.Translate(Vector3.right * this.transform.localScale.x/4f, Space.Self);
            Cube2.transform.Translate(Vector3.up * this.transform.localScale.y/4f, Space.Self);

            Cube3.transform.Translate(Vector3.right * -this.transform.localScale.x/4f, Space.Self);
            Cube3.transform.Translate(Vector3.up * -this.transform.localScale.y/4f, Space.Self);

            Cube4.transform.Translate(Vector3.right * this.transform.localScale.x/4f, Space.Self);
            Cube4.transform.Translate(Vector3.up * -this.transform.localScale.y/4f, Space.Self);


            //Cube1.transform.localPosition += new Vector3(-this.transform.localScale.x/4f, this.transform.localScale.y/4f, 0f) + -Cube1.transform.right;
            //Cube2.transform.localPosition += new Vector3(this.transform.localScale.x/4f, this.transform.localScale.y/4f, 0f) + -Cube2.transform.right;
            //Cube3.transform.localPosition += new Vector3(-this.transform.localScale.x/4f, -this.transform.localScale.y/4f, 0f) + -Cube3.transform.right;
            //Cube4.transform.localPosition += new Vector3(this.transform.localScale.x/4f, -this.transform.localScale.y/4f, 0f) + -Cube4.transform.right;

         
            Cube1.transform.localScale = new Vector3(this.transform.localScale.x/2f,
            this.transform.localScale.y/2f,this.transform.localScale.z);

            Cube2.transform.localScale = new Vector3(this.transform.localScale.x/2f,
            this.transform.localScale.y/2f,this.transform.localScale.z);

            Cube3.transform.localScale = new Vector3(this.transform.localScale.x/2f,
            this.transform.localScale.y/2f,this.transform.localScale.z);

            Cube4.transform.localScale = new Vector3(this.transform.localScale.x/2f,
            this.transform.localScale.y/2f,this.transform.localScale.z);
         

            //Cube1.transform.SetParent(Root.transform);
            //Cube2.transform.SetParent(Root.transform);
            //Cube3.transform.SetParent(Root.transform);
            //Cube4.transform.SetParent(Root.transform);

    

            Cube1.name = "Cube1";
            Cube2.name = "Cube2";
            Cube3.name = "Cube3";
            Cube4.name = "Cube4";

            /* 
            Destroy(GetComponent<BoxCollider>());
            Destroy(GetComponent<MeshFilter>());
            Destroy(GetComponent<Renderer>());
            Destroy(this);
            */

            

            Vector3 Dif = Pos - this.transform.position;

            if(Dif.x < 0f && Dif.y > 0f)
                Cube1.GetComponent<Quadtree>().Split(Pos);
            if(Dif.x > 0f && Dif.y > 0f)
                Cube2.GetComponent<Quadtree>().Split(Pos);
            if(Dif.x < 0f && Dif.y < 0f)
                Cube3.GetComponent<Quadtree>().Split(Pos);
            if(Dif.x > 0f && Dif.y < 0f)
                Cube4.GetComponent<Quadtree>().Split(Pos);
            
            Destroy(this.transform.gameObject);

            //if(CurrentLevel != 0)
            /* 
            if(!DontDestroy)
                Destroy(this.transform.gameObject);
            else
            {
                Destroy(GetComponent<BoxCollider>());
                Destroy(GetComponent<MeshFilter>());
                Destroy(GetComponent<Renderer>());
                Destroy(this);
            }*/

        }else
        {
            Destroy(this.transform.gameObject);
        }


       

    }
}
