using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using UnityEngine.UI;
using UnityEngine.AI;

public class LifeManager : MonoBehaviour
{

    public float MaxHP;
    public float Health;
    public bool isDead = false;

    public bool AI = false;
    public bool Enemy = false;
    public bool useRagdoll = false;
    public GameObject Ragdoll;

    public Simple_Bar HealthBar;

    public RawImage DamageScreen;

    //public SimpleBar HealthBar;

    public Vector3 SpawnLocation = Vector3.zero;

    Enemy En;

    //public List<Vector3> SpawnLocations;
    
    void Start()
    {
        Health = MaxHP;
        UpdateHealth();
        if(AI)
            En = GetComponent<Enemy>();
    }

    public void Damage(float Amount)
    {
        //Vector3 Location
        /*
        if(Location != Vector3.zero)
        {
            Location -= this.transform.position;



        }*/

        //if(!AI)
            //Debug.Log(Amount);

        if(AI && Enemy)
            En.LookAt(En.Player.transform.position);

        if(DamageScreen != null)
        {

            float Value = Amount/MaxHP;

            if(Value < 0.25f)
                Value = 0.25f;

            DamageScreen.material.SetFloat("_Strength", Value);
            StartCoroutine(Fade());

        }

        if(Health - Amount > 0)
        {

            Health -= Amount;
            UpdateHealth();

        }else
        {
            Kill();
        }
        

    }

    IEnumerator Fade()
    {

        float Val = DamageScreen.material.GetFloat("_Strength");
        while(Val > 0)
        {
            yield return null;
            Val -= 0.05f;
            DamageScreen.material.SetFloat("_Strength", Val);

        }

        DamageScreen.material.SetFloat("_Strength", 0f);


    }

    public void Kill()
    {
        Health = 0;
        UpdateHealth();
        isDead = true;

        if(AI)
        {
            if(!Enemy)
                GameObject.FindWithTag("Player").GetComponent<ObjectiveManager>().Lost();

            if(useRagdoll)
                ActivateRagdoll();
            else
                Destroy(this.transform.gameObject);
            
                //StartCoroutine(Delete(this.transform.gameObject, 4f));
            //this.transform.gameObject.SetActive(false);
            //Destroy(this.transform.gameObject);
        }
        else
        {
            Respawn();
        }
    }

    public void Respawn()
    {
        this.transform.position = SpawnLocation;
        Health = MaxHP;
        UpdateHealth();
        isDead = false;
    }

    void UpdateHealth()
    {
        if(!AI)
            HealthBar.Current = Health;

    }

    void ActivateRagdoll()
    {

            StartCoroutine(Delete(this.transform.gameObject, 25f));

            if(Enemy)
            {
                En.enabled = false;
                this.transform.gameObject.GetComponent<NavMeshAgent>().enabled = false;
                Destroy(this.transform.GetChild(0).gameObject);
            }
			//this.transform.GetChild(1).GetComponent<Animator>().enabled = false; //Disable Animator
			//body.SetActive(false); Hide Mesh
            this.transform.GetChild(1).gameObject.SetActive(false);
            
            this.transform.GetComponent<Rigidbody>().isKinematic = true;
			//r.isKinematic = true;  Make Kinematic
			this.gameObject.GetComponent<CapsuleCollider>().enabled = false; //Disable Collider
            
			GameObject RagdollGameObject = Instantiate(Ragdoll, this.gameObject.transform.position, this.gameObject.transform.rotation);
			RagdollGameObject.transform.SetParent(this.gameObject.transform);
			RagdollGameObject.SetActive(true);
			RagdollGameObject.name = "Ragdoll";

    
            
			GameObject Armature = this.transform.GetChild(1).gameObject;
			GameObject Armature_Ragdoll = RagdollGameObject.transform.GetChild(0).gameObject;

			//Rigidbody[] rr;
			Transform[] Armature_Position;
			Transform[] Armature_Position_Ragdoll;

			//rr = ragdoll_model.GetComponentsInChildren<Rigidbody>();
			Armature_Position = Armature.GetComponentsInChildren<Transform>();
			Armature_Position_Ragdoll = Armature_Ragdoll.GetComponentsInChildren<Transform>();

			for(int i = 0; i < Armature_Position.Length;i++)
			{
				Armature_Position_Ragdoll[i].position = Armature_Position[i].position;
				Armature_Position_Ragdoll[i].rotation = Armature_Position[i].rotation;
			}
			
			//foreach (Rigidbody rigid in rr)
			//{
				//rigid.velocity = (r.position - lastposition) * 10f;
				//rigid.isKinematic = true;
			//}
            

            this.enabled = false;

			

			

    }

    IEnumerator Delete(GameObject Gam, float Delay)
    {
        yield return new WaitForSeconds(Delay);
        try{
            Destroy(Gam);
        }catch {}

    }
}
