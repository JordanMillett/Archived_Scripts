using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToParent : MonoBehaviour 
{
	public GameObject parent;

	void OnDisable()
	{

		this.transform.SetParent(parent.transform);

	}
}
