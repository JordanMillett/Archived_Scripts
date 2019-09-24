using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	GameObject body;
    Camera cam;
    Rigidbody r;
    float yaw = 0.0f;
    float pitch = 0.0f;
    float speedMult = 1f;
    float speed = 0f;
    public float Maxspeed = 7;
	public float MouseSens = 1f;
    public float camLimits = 60f;

    float DefaultFov = 80f;   // <---- DONT FORGET
    public float ZoomedFov = 30f;

    GameObject EquipSlot;
    bool Holding = false;


    void Start()
    {
        InitComponents();
        Cursor.visible = false;
    }

    void Update()
    {

        CameraControls();

        if(Input.GetKey(KeyCode.LeftShift))
            speedMult = 1.5f;
        else
            speedMult = 1f;

        if(Input.GetKey("q"))
            Drop();

        if(Input.GetMouseButton(1))
            cam.fieldOfView = ZoomedFov;
        else
            cam.fieldOfView = DefaultFov;

    }

    void FixedUpdate()
	{
        
		Movement();

        /*
        
        Item with cooked percent
        heating element/area, if contacting or if in area
        knob and button interaction system, mouse wheel for knob
        pickup and putdown system, from first project
        mesh deform some how
        
        */

	}

    void InitComponents()
    {

		cam = this.gameObject.transform.GetChild(0).gameObject.GetComponent<Camera>();
        EquipSlot = cam.transform.GetChild(1).gameObject;
        body = this.gameObject.transform.GetChild(1).gameObject;
        r = this.gameObject.GetComponent<Rigidbody>();
        
    }

    public void Pickup(GameObject Item)
    {
        if(!Holding)
        {
            Holding = true;
            //Item.GetComponent<Rigidbody>().isKinematic = true;
            Rigidbody[] Rs = Item.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody Rigids in Rs)
                Rigids.isKinematic = true;

            //Item.layer = 8;
            Transform[] Trs = Item.GetComponentsInChildren<Transform>();
            foreach (Transform T in Trs)
                T.gameObject.layer = 8;

            Item.transform.SetParent(EquipSlot.transform);
            Item.transform.localPosition = Vector3.zero;
            Item.transform.localEulerAngles = Vector3.zero;
        }

    }

    void Drop()
    {
        if(Holding)
        {
            Holding = false;
            GameObject Item = EquipSlot.transform.GetChild(0).gameObject;
            Item.transform.SetParent(null);
            
            Vector3 DropPoint = Vector3.zero;

            RaycastHit hit;
		    if(Physics.Raycast(cam.transform.position,cam.transform.forward, out hit,10f))
		    {
                DropPoint = hit.point;
            }else
            {
                DropPoint = cam.transform.position + cam.transform.forward * 10f;
            }

            //Item.GetComponent<Rigidbody>().isKinematic = false;
            Rigidbody[] Rs = Item.GetComponentsInChildren<Rigidbody>();
            foreach (Rigidbody Rigids in Rs)
                Rigids.isKinematic = false;

            //Item.layer = 0;
            Transform[] Trs = Item.GetComponentsInChildren<Transform>();
            foreach (Transform T in Trs)
                T.gameObject.layer = 0;

            Item.transform.eulerAngles = new Vector3(0f,cam.transform.eulerAngles.y,0f);
            Item.transform.position = DropPoint;
            
        }
    }

    void Movement()
    {

        float heading = Mathf.Atan2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));

		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if(speed < Maxspeed)
                speed += 0.4f;

			body.transform.localEulerAngles = new Vector3(0f,heading*Mathf.Rad2Deg,0f);

			if (Input.GetKey("w"))
                r.MovePosition(r.position + (transform.forward * speed * speedMult) * Time.fixedDeltaTime);

			if (Input.GetKey("a")) 
                r.MovePosition(r.position + (-transform.right * speed * speedMult) * Time.fixedDeltaTime);

			if (Input.GetKey("s")) 
                r.MovePosition(r.position + (-transform.forward * speed * speedMult) * Time.fixedDeltaTime);

			if (Input.GetKey("d")) 
                r.MovePosition(r.position + (transform.right * speed * speedMult) * Time.fixedDeltaTime);


		}else
        {
            speed = 0f;
        }

    }

    void CameraControls()  
    {

		yaw += MouseSens * Input.GetAxis ("Mouse X");
        transform.localEulerAngles = new Vector3(0.0f, yaw, 0.0f);

        pitch -= MouseSens * Input.GetAxis ("Mouse Y");
        
		if(pitch >= camLimits)
			pitch = camLimits;
        
		
		if(pitch <= -camLimits)
			pitch = -camLimits;

        float y = cam.transform.localEulerAngles.y;
        float z = cam.transform.localEulerAngles.z;

        cam.transform.localEulerAngles = new Vector3(pitch, y, z);

    }

}
