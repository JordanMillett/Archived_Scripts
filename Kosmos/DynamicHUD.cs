using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DynamicHUD : MonoBehaviour
{
    public Color Good;
    public Color Bad;
    public GameObject TargetIcon;
    public GameObject TargetEmpty;
    public GameObject Crosshair;
    public GameObject HUD;
    public GameObject Results;
    public ResultsScreen RS;
    public RectTransform ShipPoint; 
    public TextMeshProUGUI Hull;
    public TextMeshProUGUI Shields;
    public TextMeshProUGUI Energy;
    public TextMeshProUGUI LeftScore;
    public TextMeshProUGUI RightScore;

    Camera Cam;
    ShipStats SS;
    BattleMaker BM;

    int Index = 0;
    float SearchRadius = 100f;
    float CloseScale = 1f;
    float FarScale = 0.1f;
    bool Shown = false;

    List<GameObject> Markers = new List<GameObject>();

    void Start()
    {
        Cam = GetComponent<Camera>();
    }

    void OnEnable()
    {
        SS = this.transform.parent.parent.GetComponent<ShipStats>();
        BM = SS.GetComponent<BattleShipController>().BM;
        HUD.SetActive(true);
        Results.SetActive(false);
        Shown = false;
        RS.BM = BM;
    }

    void Update()
    {
        if(!SS.Dead)
        {
            if(BM.EnemyShips.Count > 0)
            {
                UpdateHUD();        //Update HUD Numbers
                ForwardIcon();      //Draw Icon that shows direction of ship
                FreeLook();         //Controls if Free Look is enabled
                MakeTargets();      //Creates Targets
                HideExtra();        //Hides Excess Targets
            }else
            {
                if(!Shown)
                    ShowResults();
            }

        }else                   //Disables GameObject
        {
            transform.GetChild(0).gameObject.SetActive(false);
            this.enabled = false;
        }
    }

    void ShowResults()
    {
        Shown = true;
        HUD.SetActive(false);
        Results.SetActive(true);

    }

    void UpdateHUD()
    {
        LeftScore.text = BM.FriendlyShips.Count.ToString();
        RightScore.text = BM.EnemyShips.Count.ToString();

        Hull.text = Mathf.Round(SS.Hull).ToString();
        Shields.text = Mathf.Round(SS.Shields).ToString();
        Energy.text = Mathf.Round(SS.Energy).ToString();
    }

    void ForwardIcon()      //Need to work on zeroing distance for target
    {
        Vector3 WorldToCam = Cam.WorldToViewportPoint(SS.transform.position + (SS.transform.forward * 25f));
        if(WorldToCam.z > 0f)
        {
            ShipPoint.gameObject.SetActive(true);
            WorldToCam = new Vector2(WorldToCam.x * 1280f, WorldToCam.y * 720f);
            WorldToCam -= new Vector3(1280f/2f, 720f/2f, 0f);
            ShipPoint.anchoredPosition = WorldToCam; 
        }else
        {
            ShipPoint.gameObject.SetActive(false);
        }
    }

    void FreeLook()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
            Crosshair.SetActive(false);
        else
            Crosshair.SetActive(true);
    }

    void MakeTargets()
    {
        Index = 0;
            Collider[] hitColliders = Physics.OverlapSphere(this.transform.position, SearchRadius);
            for(int i = 0; i < hitColliders.Length; i++)
            {
                if(hitColliders[i].gameObject.GetComponent<ShipStats>() != null)
                {
                    if(hitColliders[i].gameObject != this.transform.parent.parent.gameObject)
                    {
                        float Distance = Vector3.Distance(this.transform.position, hitColliders[i].transform.position);
                        DrawTarget(hitColliders[i].transform.position, Distance, hitColliders[i].gameObject.GetComponent<ShipStats>().Team, hitColliders[i].gameObject.GetComponent<BattleShipController>().ShipInfo.TargetMarkerScale);
                        Index++;
                    }
                }
            }
    }

    void DrawTarget(Vector3 Pos, float Dist, int Team, float ScaleMult)
    {
        if(Index > Markers.Count - 1)
        {
            Markers.Add(Instantiate(TargetIcon, Vector3.zero, Quaternion.identity));
            Markers[Index].transform.SetParent(TargetEmpty.transform);
        }

        Vector3 WorldToCam = Cam.WorldToViewportPoint(Pos);

        if(WorldToCam.z > 0f)
        {
            Markers[Index].SetActive(true);
            WorldToCam = new Vector2(WorldToCam.x * 1280f, WorldToCam.y * 720f);
            WorldToCam -= new Vector3(1280f/2f, 720f/2f, 0f);
            Markers[Index].transform.GetComponent<RectTransform>().anchoredPosition = WorldToCam;
            float Scale = Mathf.Lerp(CloseScale, FarScale, Dist/SearchRadius);
            Scale *= ScaleMult;
            Markers[Index].transform.GetComponent<RectTransform>().localScale = new Vector2(Scale, Scale);
            Markers[Index].transform.GetComponent<RectTransform>().localEulerAngles = Vector3.zero;

            if(Team == 0)
                Markers[Index].transform.GetComponent<RawImage>().color = Good;
            else
                Markers[Index].transform.GetComponent<RawImage>().color = Bad;

        }else
        {
            Markers[Index].SetActive(false);
        }

        
       
    }

    void HideExtra()
    {
        for(int i = Index; i < Markers.Count; i++)
        {
            Markers[i].SetActive(false);
        }
    }
}
