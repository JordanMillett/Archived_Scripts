using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public float ZoomScale = 1f;

    Transform CameraEmpty;

    OverviewHud Hud;

    GameObject CanvasHUD;

    GameObject BattleEnemy;

    void Start()
    {
        CameraEmpty = transform.GetChild(1);
        CanvasHUD = transform.GetChild(2).gameObject;
        Hud = CameraEmpty.transform.GetChild(0).GetComponent<OverviewHud>();
    }

    void OnEnable()
    {
        if(BattleEnemy != null)
            BattleFinished();
    }

    void Update()
    {
        if(Input.mouseScrollDelta.y < 0)
        {
            
            ZoomScale = ZoomScale * 1.25f;

        }else if(Input.mouseScrollDelta.y > 0)
        {
            
            ZoomScale = ZoomScale * 0.75f;

        }

        CameraEmpty.localScale = new Vector3(ZoomScale, ZoomScale, ZoomScale);
    }

    void OnTriggerEnter(Collider Col)
    {
        if(Col.GetComponent<FleetInfo>() != null)
        {
            if(!GetComponent<ShipController>().Busy)
            {
                if(Col.transform.gameObject.GetComponent<Mission>() != null)
                {
                GameObject.FindWithTag("SceneController")
                .gameObject.GetComponent<SceneController>()
                .StartBattle(this.transform.GetComponent<FleetInfo>(), Col.transform.GetComponent<FleetInfo>(), GetComponent<MissionController>().CurrentMission);
                }else
                {
                    GameObject.FindWithTag("SceneController")
                .gameObject.GetComponent<SceneController>()
                .StartBattle(this.transform.GetComponent<FleetInfo>(), Col.transform.GetComponent<FleetInfo>(), null);
                }

                BattleEnemy = Col.gameObject;
            }
        }else if (Col.GetComponent<Planet>() != null)
        {
            
            if(Col.transform.gameObject.GetComponent<Mission>() != null)
            {
                GetComponent<MissionController>().Finish();
                GetComponent<MissionController>().OA.Target.SetActive(false);
                GetComponent<MissionController>().OA.Visible = false;
            }

            GetComponent<ShipController>().GoTo(Col.transform.position); 
            CanvasHUD.SetActive(false);
            Hud.OpenShop(Col.GetComponent<Planet>());

        }
        
    }

    public void BattleFinished()
    {
        if(BattleEnemy.transform.gameObject.GetComponent<Mission>() != null)
        {
            GetComponent<MissionController>().Finish();
            GetComponent<MissionController>().OA.Target.SetActive(false);
            GetComponent<MissionController>().OA.Visible = false;

            GetComponent<MissionController>().CurrentMission = null;
        }

        GetComponent<PlayerInformation>().Credits += (BattleEnemy.transform.GetComponent<FleetInfo>().Ships.Count * UniversalConstants.ShipDefeatCredits);
        Destroy(BattleEnemy);        //Change depending on outcome of battle
        Hud.UpdateRoster();
    }

    public void ShopClosed()
    {
        CanvasHUD.SetActive(true);
    }
}
