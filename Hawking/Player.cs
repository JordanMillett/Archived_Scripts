using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class Player : MonoBehaviour
{
    public int uSv_Exposure = 0;
    public static int Lethal_uSv = 10000000;

    float RadAlpha = 0f;
    public float PercentRad = 0;

    public Transform Eyes;
    
    float movementForce = 80f;//60
    float maxSpeed = 6f;//5
    float yaw = 0f;
    float pitch = 0f;

    float camLimits = 80f;
    
    Rigidbody _rigidbody;

    public Transform _camera;

    public float SensMultiplier = 1f;

    FilmGrain _film;
    Vignette _vignette;
    Bloom _bloom;
    ColorAdjustments _color;

    public static Player Instance;
    
    public Weapon Equipped;

    public Transform View;
    Camera ViewCam;
    Camera Cam;
    
    public float InteractRange = 3f;

    float ADSAlpha = 0f;

    public SyncDelayed ViewSync;
    SyncDelayed HandSync;

    public GameObject Crosshair;
    public Transform Hands;


    void Awake()
    {
        Instance = this;

        Volume V = GameObject.FindWithTag("Volume").GetComponent<Volume>();

        V.profile.TryGet<FilmGrain>(out _film);
        V.profile.TryGet<Vignette>(out _vignette);
        V.profile.TryGet<Bloom>(out _bloom);
        V.profile.TryGet<ColorAdjustments>(out _color);

        ResetVolume();
    }
    
    void ResetVolume()
    {
        _film.intensity.Override(0f);
        _vignette.intensity.Override(0f);
        _bloom.intensity.Override(0f);
        _color.postExposure.Override(0f);
        _color.contrast.Override(0f);
        _color.saturation.Override(0f);
    }
    
    void UpdateVolume()
    {
        _film.intensity.Override(RadAlpha);
        _vignette.intensity.Override(RadAlpha);
        _bloom.intensity.Override(RadAlpha * 100f);
        _color.postExposure.Override(RadAlpha * -3f);
        _color.contrast.Override(RadAlpha * 50f);
        _color.saturation.Override(RadAlpha * -100f);
    }

    void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 60;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        
        _rigidbody = GetComponent<Rigidbody>();
        _camera = GameObject.FindWithTag("Camera").transform;
        
        
        
        _camera = GameObject.FindWithTag("Camera").transform;
        ViewCam = View.GetComponent<Camera>();
        Cam = _camera.GetComponent<Camera>();
        HandSync = Hands.GetComponent<SyncDelayed>();
        
    }
    
    void Update()
    {
        _camera.position = Eyes.transform.position;
        _camera.rotation = Eyes.transform.rotation;
        
        CameraControls();

        RadAlpha = ((float)uSv_Exposure / Lethal_uSv);

        PercentRad = RadAlpha * 100f;

        UpdateVolume();

        if (Equipped)
        {
            if (Equipped.Stats.FireMode == FireModes.Semi ? Input.GetMouseButtonDown(0) : Input.GetMouseButton(0))
                Equipped.Fire(_camera);
        }

        ADSAlpha += Input.GetMouseButton(1) ? 0.1f : -0.1f;
        ADSAlpha = Mathf.Clamp01(ADSAlpha);
        Cam.fieldOfView = Mathf.Lerp(90f, 50f, ADSAlpha);
        ViewCam.fieldOfView = Mathf.Lerp(30f, 20f, ADSAlpha);
        SensMultiplier = Mathf.Lerp(1f, 50f/90f, ADSAlpha);

        HandSync.Factor = Mathf.Lerp(1f, 3.5f, ADSAlpha);
        ViewSync.Factor = Mathf.Lerp(1f, 3.5f, ADSAlpha);

        Crosshair.SetActive(ADSAlpha == 0f);
        
        if(Equipped)
            View.transform.localPosition = Vector3.Lerp(Equipped.OffsetPos, Equipped.ADSPos, ADSAlpha);
        
        if(Input.GetKeyDown("f"))
        {
            Interact();
        }
        
    }
    
    void Interact()
    {
        RaycastHit hit;

		if(Physics.Raycast(Eyes.position, Eyes.forward, out hit, InteractRange))
		{
            Interactable I = hit.collider.GetComponent<Interactable>();
            if (I)
            {
                I.Activate();
            }
        }
    }
    
    void FixedUpdate()
    {
        MovementControls();
    }
    
    void MovementControls()
    {
         Vector3 MoveDirection = Vector3.zero;
        if (Input.GetKey("w") || Input.GetKey("a") || Input.GetKey("s") || Input.GetKey("d"))
        {
            if (Input.GetKey("w"))
                MoveDirection += this.transform.forward;

            if (Input.GetKey("a"))
                MoveDirection += -this.transform.right;

            if (Input.GetKey("s"))
                MoveDirection += -this.transform.forward;

            if (Input.GetKey("d"))
                MoveDirection += this.transform.right;
        }

        float lerp = Mathf.Lerp(1f, 0f, _rigidbody.velocity.magnitude / maxSpeed);
        _rigidbody.AddForce((MoveDirection * movementForce) * lerp, ForceMode.Acceleration);
    }
    
    void CameraControls()
    {
        int _mouseSensitivity = 100; //1.00

        yaw += (_mouseSensitivity/100f) * Input.GetAxis("Mouse X") * SensMultiplier;
        pitch -= (_mouseSensitivity/100f) * Input.GetAxis("Mouse Y") * SensMultiplier;
        
        if(pitch >= camLimits)
            pitch = camLimits;
            
        if(pitch <= -camLimits)
            pitch = -camLimits;

        this.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        Eyes.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }
}
