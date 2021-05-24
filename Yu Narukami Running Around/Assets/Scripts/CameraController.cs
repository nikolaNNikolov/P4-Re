using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform targetToLookAt;
	public Transform targetToFollow;
	public GameObject SceneLogic;
	private Vector3 offset;
	private float rotateSpeed = 5.0f;

    void Start()
    {
    }

    void Update()
	{
		offset = SceneLogic.GetComponent<SceneLogic>().getOffset();	//get initial offset from the begging of the scene	
		transform.position = targetToFollow.position - offset; //set offset, in case of camera deactivation
		//Get the X position of the mouse, rotate target
		float horizontal = Input.GetAxis("Mouse X") * rotateSpeed;
		targetToFollow.Rotate(0, horizontal, 0);

		//Move the camera to rotation of target & offset
		float desiredYAngle = targetToFollow.eulerAngles.y;
		Quaternion rotation = Quaternion.Euler(0, desiredYAngle, 0);
		transform.position = targetToFollow.position - (rotation * offset);
		transform.LookAt(targetToLookAt);
    }
}
