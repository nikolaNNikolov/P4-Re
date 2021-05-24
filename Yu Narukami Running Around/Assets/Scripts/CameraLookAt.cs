using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
	public Transform targetToLookAt;
	public GameObject playerCharacter;

	private Vector3 eulerAnglesRotation;

	void Start()
	{
		eulerAnglesRotation = playerCharacter.transform.eulerAngles;
		transform.eulerAngles = eulerAnglesRotation;
		//Debug.Log("eulerAnglesRotation");
	}

	void Update()
	{
		//Debug.Log(eulerAnglesRotation);
		if(targetToLookAt != null)
		{
			transform.LookAt(targetToLookAt);
		}
	}

	public void setTarget(Transform target)
	{
		targetToLookAt = target;
		eulerAnglesRotation = playerCharacter.transform.eulerAngles;
		//eulerAnglesRotation = playerCharacter.transform.forward;
		//Debug.Log("set: " + eulerAnglesRotation);
	}

	public void resetTarget()
	{
		//Debug.Log("reset");
		transform.eulerAngles = eulerAnglesRotation;
		//Debug.Log("reset: " + eulerAnglesRotation);
		setTarget(null);
	}
}
