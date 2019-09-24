using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sync_Bone : MonoBehaviour 
{

	Transform Bone;
	GameObject Player;

	void Start () //find bone through body then rig instead of public
	{
		Player = GameObject.FindWithTag("Player");
		 
		Transform[] children = Player.transform.GetChild(1).transform.GetChild(1).GetComponentsInChildren<Transform>();

 		foreach (Transform child in children)
      		if (child.name == this.gameObject.name)
		 		Bone = child;
	}
	
	void LateUpdate() 
	{
		this.transform.position = Bone.position;
		this.transform.rotation = Bone.rotation;
	}
}
