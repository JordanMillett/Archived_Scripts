using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Discord;

public class DiscordController : MonoBehaviour 
{

	public Discord.Discord discord;

	void Start () 
    {
		discord = new Discord.Discord(790445532467298324, (System.UInt64)Discord.CreateFlags.Default);
	}
	
	void Update () 
    {
		var activityManager = discord.GetActivityManager();
		var activity = new Discord.Activity
		{
			State = "Moving Boxes",
            Details = ""
		};

		activityManager.UpdateActivity(activity, (res) =>
		{
			if (res == Discord.Result.Ok)
			{
				Debug.LogError("Everything is fine!");
			}
		});

        if(Input.GetKey("r"))
            discord.Dispose();
	}
}