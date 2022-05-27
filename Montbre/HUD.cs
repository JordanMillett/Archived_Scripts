using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public Player P; //root

    public GameObject Defense_Tab;

    public TextMeshProUGUI Defense_Friendlies;
    public TextMeshProUGUI Defense_Enemies;
    public TextMeshProUGUI Defense_Money;
    public TextMeshProUGUI Defense_Timer;
    public SimpleBar Defense_AirBattle;

    public GameObject Conquest_Tab;

    public TextMeshProUGUI Conquest_Friendlies;
    public TextMeshProUGUI Conquest_Enemies;
    public SimpleBar Conquest_Tickets;

    public GameObject CrossHair;
    public SelectionWheel Wheel;

    public RawImage Primary;
    public RawImage Secondary;
    public TextMeshProUGUI Ammo;

    public GameObject TankView;
    public GameObject HitMarker;

    public AudioClip HitSound;
    public AudioClip CriticalSound;
    public AudioClip KillSound;

    public int Hit = 0;

    bool Chosen = false;

    Color AwayColor = new Color(170f/255f, 170f/255f, 170f/255f, 200f/255f);
    Color OutColor = new Color(255f/255f, 255f/255f, 255f/255f, 200f/255f);

    AudioSource AS;

    void Start()
    {
        AS = GetComponent<AudioSource>();
    }

    public void Choose(GameModes GM)
    {
        Defense_Tab.SetActive(GM == GameModes.Defense);
        Conquest_Tab.SetActive(GM == GameModes.Conquest || GM == GameModes.Hill);

        Chosen = true;
    }

    void Update()
    {
        if(Chosen)
        {
            if(Hit != 0)
            {
                HitMarker.SetActive(true);

                if(Hit > 0)
                {
                    AS.PlayOneShot(HitSound, (1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f));
                }
                if(Hit < 0)
                {
                    AS.PlayOneShot(CriticalSound, (1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f));
                }

                if(Mathf.Abs(Hit) == 2)
                {
                    AS.PlayOneShot(KillSound, (1f * (Settings._sfxVolume/100f)) * (Settings._masterVolume/100f));
                }
                
                Hit = 0;
            }else
            {
                HitMarker.SetActive(false);
            }

            //DYNAMIC CHANGING HUD
            if(Player.Controlled.Type == Unit.Types.Infantry)
            {
                TankView.SetActive(false);
                CrossHair.SetActive(!Input.GetMouseButton(1));

                if(Player.Controlled.inf.Equipped)
                {
                    Ammo.text = Player.Controlled.inf.Equipped.CurrentMagazine + "/" + Player.Controlled.inf.Equipped.Info.MagazineSize;
                    Primary.color = Player.Controlled.inf.Equipped == Player.Controlled.inf.Primary ? OutColor : AwayColor;
                    Secondary.color = Player.Controlled.inf.Equipped == Player.Controlled.inf.Secondary ? OutColor : AwayColor;
                }else
                {
                    Ammo.text = "";
                    Primary.color = AwayColor;
                    Secondary.color = AwayColor;
                }
                
            }else if(Player.Controlled.Type == Unit.Types.Tank)
            {
                Primary.color = Player.Controlled.tan.PlayerUsingPrimary ? OutColor : AwayColor;
                Secondary.color = !Player.Controlled.tan.PlayerUsingPrimary ? OutColor : AwayColor;

                //P.CurrentFOV/40f;
                TankView.GetComponent<RectTransform>().transform.localScale = new Vector3(70f/P.CurrentFOV, 70f/P.CurrentFOV, 70f/P.CurrentFOV);
                TankView.SetActive(true);
                CrossHair.SetActive(false);
                Ammo.text = "";
            }else if(Player.Controlled.Type == Unit.Types.Plane)
            {
                Primary.color = Player.Controlled.pla.PlayerUsingPrimary ? OutColor : AwayColor;
                Secondary.color = !Player.Controlled.pla.PlayerUsingPrimary ? OutColor : AwayColor;

                TankView.SetActive(false);
                CrossHair.SetActive(false);
                Ammo.text = "";
            }

            //GAMEMODE HUD
            if(Game.GameMode == GameModes.Defense)
            {
                Defense_Friendlies.text = Game.Defense_Allies.ToString();
                Defense_Enemies.text = Game.Defense_Enemies.ToString();

                if(Game.Defense_AllyPlanes + Game.Defense_EnemyPlanes == 0)
                {   
                    Defense_AirBattle.BG.color = Game.NeutralColor;
                    Defense_AirBattle.Current = 0f;
                }else
                {
                    Defense_AirBattle.BG.color = Game.EnemyColor;
                    Defense_AirBattle.Current = (float)Game.Defense_AllyPlanes/((float)Game.Defense_EnemyPlanes + Game.Defense_AllyPlanes);
                }

                Defense_Money.text = Game.Defense_Money.ToString();

                if(Game.Defense_EndTime == 0 && Game.Defense_StartTime != 0)
                    Defense_Timer.text = Game.GetTimeFormatted(Time.time - Game.Defense_StartTime);
                else
                    Defense_Timer.text = Game.GetTimeFormatted(Game.Defense_EndTime);
            }else
            {
                Conquest_Friendlies.text = Game.Conquest_TeamOneTickets.ToString();
                Conquest_Enemies.text = Game.Conquest_TeamTwoTickets.ToString();

                Conquest_Tickets.BG.color = Game.EnemyColor;
                Conquest_Tickets.Current = (float)Game.Conquest_TeamOneTickets/((float)Game.Conquest_TeamOneTickets + Game.Conquest_TeamTwoTickets);
            }
        }
    }
}
