using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int PlayerIndex = -1;

    public Transform toPitch;
    public Camera _camera;
    Rigidbody _rigidbody;

    float maxSpeed = 5f;
    float yaw = 0f;
    float pitch = 0f;
    float camLimits = 85f;

    bool AIMoving = false;

    void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

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
            CameraControls();
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
                Vector3 moveDirection = transform.forward;
                moveDirection = Quaternion.AngleAxis(toPitch.transform.localRotation.eulerAngles.y, this.transform.up) * moveDirection;
                _rigidbody.MovePosition(this.transform.position + moveDirection * Time.deltaTime * maxSpeed);
            }
        }

        Gravity();

    }

    IEnumerator AIBehavior()
    {
        AIMoving = false;
        yield return new WaitForSeconds(Random.Range(1f, 10f));

        while(true)
        {
            AIMoving = true;
            yaw = Random.Range(0f, 360f);
            toPitch.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
            yield return new WaitForSeconds(Random.Range(1f, 6f));

            AIMoving = false;
            yield return new WaitForSeconds(Random.Range(1f, 3f));

            float Ran = Random.value;

            if(Ran < 0.33f)
            {
                AIMoving = false;
                yield return new WaitForSeconds(Random.Range(3f, 6f));
            }else if(Ran > 0.66f)
            {
                yaw = Random.Range(0f, 360f);
                toPitch.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
                AIMoving = true;
                yield return new WaitForSeconds(Random.Range(1f, 2f));
            }else
            {
                yaw += Random.Range(0f, 40f);
                toPitch.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
                AIMoving = true;
                yield return new WaitForSeconds(Random.Range(1f, 4f));
            }

            if(Random.value > 0.75f)
            {
                yaw += Random.Range(0f, 40f);
                toPitch.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
                AIMoving = true;
                yield return new WaitForSeconds(Random.Range(1f, 15f));
            }
        }
    }

    void Movement()
    {
		if(Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {

            Vector3 moveDirection = Vector3.zero;

            if (Input.GetKey("w"))
                moveDirection += transform.forward;

            if (Input.GetKey("a")) 
                moveDirection += -transform.right * 0.75f;

            if (Input.GetKey("s")) 
                moveDirection += -transform.forward;

            if (Input.GetKey("d")) 
                moveDirection += transform.right * 0.75f;

            moveDirection = Quaternion.AngleAxis(toPitch.transform.localRotation.eulerAngles.y, this.transform.up) * moveDirection;
            _rigidbody.MovePosition(this.transform.position + moveDirection * Time.deltaTime * maxSpeed);
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

        toPitch.transform.localRotation = Quaternion.Euler(pitch, yaw, 0f);
    }

    void Gravity()
    {
        Vector3 ForceDirection = Vector3.Normalize(this.transform.position);
        _rigidbody.AddForce(ForceDirection * -10f);

        Quaternion Upright = Quaternion.FromToRotation(this.transform.up, ForceDirection) * this.transform.rotation;
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, Upright, Time.fixedDeltaTime * 10f);
    }
}
