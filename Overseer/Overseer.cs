using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Overseer : MonoBehaviour
{   
    //Eye changes with zoom, pupil

    public int PlayerIndex = -1;

    public Transform toPitch;
    public Camera _camera;

    float maxSpeed = 20f;
    float yaw = 0f;
    float pitch = 0f;
    float camLimits = 20f;
    
    bool AIMoving = false;
    bool AILooking = false;

    Vector3 lookDirection = Vector3.zero;
    Vector3 moveDirection = Vector3.zero;

    bool Attacking = false;
    public GameObject LaserTemp;

    public Light ZoomLight;
    float DefaultFOV = 60f;
    float ZoomFOV = 20f;
    float ZoomAlpha = 0f;
    float DefaultLightAngle = 170f;
    float ZoomLightAngle = 30f;
    public Color DefaultLightColor;
    public Color ZoomLightColor;

    void Start()
    {
        if(PlayerIndex > -1)
        {
            _camera.gameObject.SetActive(true);
        }else
        {
            StartCoroutine(AIBehavior());
        }
    }

    void Update()
    {
        if(PlayerIndex > -1)
        {
            CameraControls();
            AttackControls();
        }
    }

    void FixedUpdate()
    {
        if(PlayerIndex > -1)
        {
            Movement();
        }
        else
        {
            if(AIMoving)
            {
                this.transform.localRotation *= Quaternion.Euler(moveDirection * maxSpeed * Time.deltaTime);
            }

            if(AILooking)
            {
                toPitch.transform.localRotation = Quaternion.Lerp(toPitch.transform.localRotation, Quaternion.Euler(pitch, 0f, yaw), Time.fixedDeltaTime * 5f);
            }
        }
    }

    IEnumerator AIBehavior()
    {
        AIMoving = false;
        yield return new WaitForSeconds(Random.Range(1f, 5f));

        while(true)
        {
            yaw = Random.Range(-camLimits, camLimits);
            pitch = Random.Range(-camLimits, camLimits);
            AILooking = true;

            if(Random.value > 0.5f)
                ToggleAttack();
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            if(Attacking)
                ToggleAttack();

            AIMoving = true;
            moveDirection.x = Random.value > 0.5f ? 1f : 0f;
            moveDirection.z = Random.value > 0.5f ? 1f : 0f;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            AILooking = false;

            if(Random.value > 0.5f)
                ToggleAttack();
            yield return new WaitForSeconds(Random.Range(1f, 2f));
            if(Attacking)
                ToggleAttack();

            AIMoving = true;
            moveDirection.x = Random.value > 0.5f ? 1f : 0f;
            moveDirection.z = Random.value > 0.5f ? 1f : 0f;
            yield return new WaitForSeconds(Random.Range(1f, 3f));
            AIMoving = false;

            if(Random.value > 0.5f)
            {
                yaw = Random.Range(-camLimits, camLimits);
                pitch = Random.Range(-camLimits, camLimits);
                AILooking = true;
            }else
            {
                AILooking = false;
            }

            AIMoving = false;
            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    void AttackControls()
    {
        if(Input.GetMouseButtonDown(0))
        {
            ToggleAttack();
        }
    }

    void ToggleAttack()
    {
        Attacking = !Attacking;

        LaserTemp.SetActive(Attacking);
    }

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {

            moveDirection = Vector3.zero;

            if (Input.GetKey("w"))
                moveDirection += new Vector3(1f, 0f, 0f);

            if (Input.GetKey("a")) 
                moveDirection += new Vector3(0f, 0f, 1f);

            if (Input.GetKey("s")) 
                moveDirection += new Vector3(-1f, 0f, 0f);

            if (Input.GetKey("d")) 
                moveDirection += new Vector3(0f, 0f, -1f);

            this.transform.localRotation *= Quaternion.Euler(moveDirection * maxSpeed * Time.deltaTime);
        }
    }

    void CameraControls()
    {
        yaw += (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse X");
        pitch -= (Settings._mouseSensitivity/100f) * Input.GetAxis("Mouse Y");
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        if(yaw >= camLimits)
            yaw = camLimits;
            
        if(yaw <= -camLimits)
            yaw = -camLimits;

        toPitch.transform.localRotation = Quaternion.Euler(pitch, 0f, yaw);

        //Zoom controls here
        if(Input.GetMouseButton(1))
        {
            if(ZoomAlpha < 1f)
                ZoomAlpha += 0.1f;
        }
        else
        {
            if(ZoomAlpha > 0f)
                ZoomAlpha -= 0.1f;
        }

        _camera.fieldOfView = Mathf.Lerp(DefaultFOV, ZoomFOV, ZoomAlpha);
        ZoomLight.spotAngle = Mathf.Lerp(DefaultLightAngle, ZoomLightAngle, ZoomAlpha);
        ZoomLight.color = Color.Lerp(DefaultLightColor, ZoomLightColor, ZoomAlpha);
    }
}
