using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleMaker : MonoBehaviour
{
    public GameObject Camera;

    public List<ShipStats> FriendlyShips;
    public List<ShipStats> EnemyShips;

    public List<Ship> OriginalFriendlyShips;
    public List<Ship> OriginalEnemyShips;

    FleetInfo PlayerFleet;

    public Mission CurrentMission;

    public void MakeBattle(FleetInfo Player, FleetInfo Enemy, Mission M)
    {
        OriginalFriendlyShips = Player.Ships;
        OriginalEnemyShips = Enemy.Ships;
        CurrentMission = M;

        FriendlyShips = new List<ShipStats>();
        EnemyShips = new List<ShipStats>();

        PlayerFleet = Player;

        float xPos = 0;
        for(int i = 0; i < Player.Ships.Count; i++)
        {
            GameObject P = Instantiate(Player.Ships[i].ShipPrefab, new Vector3(5f * xPos, 5f * Mathf.Floor(i/10f), -50f), Quaternion.identity);
            P.transform.SetParent(this.transform);
            P.GetComponent<ShipStats>().Team = 0;
            P.GetComponent<BattleShipController>().BM = this;
            FriendlyShips.Add(P.GetComponent<ShipStats>());

            if(i == 0)
            {
                P.GetComponent<BattleShipController>().PlayerControlled = true;
                Camera.transform.SetParent(P.transform);
                Camera.transform.localPosition = Vector3.zero;
                Camera.transform.GetChild(0).transform.localPosition = Player.Ships[0].CameraPosition;
                Camera.SetActive(true);
            }

            xPos++;
            if(xPos >= 10)
                xPos = 0;
        }

        xPos = 0;
        for(int i = 0; i < Enemy.Ships.Count; i++)
        {


            GameObject E = Instantiate(Enemy.Ships[i].ShipPrefab, new Vector3(-5f * xPos, 5f * Mathf.Floor(i/10f), 50f), Quaternion.identity);
            E.transform.SetParent(this.transform);
            E.GetComponent<ShipStats>().Team = 1;
            E.GetComponent<BattleShipController>().BM = this;
            EnemyShips.Add(E.GetComponent<ShipStats>());
            xPos++;
            if(xPos >= 10)
                xPos = 0;
        }

        
    }

    void Update()
    {
        CheckWin();

        if(EnemyShips.Count == 0 && Input.GetKeyDown("f"))
        {
            List<Ship> Ships = new List<Ship>();
            for(int i = 0; i < FriendlyShips.Count; i++)
            {
                Ships.Add(FriendlyShips[i].GetComponent<BattleShipController>().ShipInfo);
            }
            PlayerFleet.Ships = Ships;


            Camera.transform.SetParent(this.transform);
            Camera.SetActive(false);
            
            foreach(Transform child in transform)
            {
                if(child.gameObject != Camera)
                {
                    Destroy(child.gameObject);
                }
            }

            GameObject.FindWithTag("SceneController").gameObject.GetComponent<SceneController>().GoBack();
        }
    }

    void CheckWin()
    {
        for(int i = 0; i < FriendlyShips.Count; i++)
        {
            if(FriendlyShips[i] == null)
                FriendlyShips.RemoveAt(i);
        }

        for(int i = 0; i < EnemyShips.Count; i++)
        {
            if(EnemyShips[i] == null)
                EnemyShips.RemoveAt(i);
        }

        
    }
}
