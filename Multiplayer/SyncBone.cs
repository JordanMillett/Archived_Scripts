using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncBone : MonoBehaviour
{
    Transform Bone;
    public GameObject Root;

    void Start ()
	{
		 
		Transform[] children = Root.transform.GetChild(0).transform.GetChild(0).GetComponentsInChildren<Transform>();

 		foreach (Transform child in children)
      		if (child.name == this.gameObject.name)
		 		Bone = child;
	}
	
	void FixedUpdate() 
	{
		this.transform.position = Bone.position;
		this.transform.rotation = Bone.rotation;
	}
}
