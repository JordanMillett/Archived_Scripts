using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UniversalConstants : MonoBehaviour
{
    public static WaitForSeconds FrameTime = new WaitForSeconds(1f/60f);
    public static float CullDistance = 9f;

    public static int ShipDefeatCredits = 25;
    public static int BountyShipDefeatCredits = 75;
    public static int DeliveryCreditsMultiplier = 3;
}
