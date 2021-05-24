using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YuPlayerController : MonoBehaviour
{
	public GameObject sceneLogic;

	private float speedConstant;
	private float gravityScaleConstant = 1f;
	private float rotateSpeed = 3f;
	private Animator yuAnimator;
	private CharacterController playerController;
	private Vector3 movementDirection;

	private bool flipMe = false;

    void Start()
    {
		playerController = GetComponent<CharacterController>();
		yuAnimator = GetComponent<Animator>();
    }

    void Update()
    {
		float horizontalAxis = Input.GetAxis("Horizontal");
		float verticalAxis = System.Math.Abs(Input.GetAxis("Vertical"));
		float RotateMe = Input.GetAxis("RotateMe");
		float walkingAndRunningModifier = 1.0f;

		if(Input.GetAxis("Walk") > 0)
			walkingAndRunningModifier = 0.4f; //used to determine walk speed
			
		yuAnimator.SetFloat("Horizontal", Input.GetAxis("Horizontal") * walkingAndRunningModifier);
		yuAnimator.SetFloat("Vertical",   Input.GetAxis("Vertical") * walkingAndRunningModifier);
		speedConstant = System.Math.Abs(yuAnimator.GetFloat("Vertical")) * 20.0f + System.Math.Abs(yuAnimator.GetFloat("Horizontal"));

		if(RotateMe > 0)
		{
			if(!flipMe)
			{
				StartCoroutine(turnAroundSmoothly(this.transform, 0.5f));
				flipMe = true;
			}
		}
		else
		{
			flipMe = false;
		}

		movementDirection = (transform.forward * verticalAxis);
		if(verticalAxis != 0){movementDirection += (transform.right * horizontalAxis);}
		else        {movementDirection += Vector3.zero;}
		this.transform.Rotate(0, horizontalAxis * rotateSpeed, 0);
	 
		movementDirection = movementDirection.normalized * speedConstant;
		if(playerController.isGrounded)
			movementDirection.y =0.0f;
		else
			movementDirection.y += (Physics.gravity.y * gravityScaleConstant);

		playerController.Move(movementDirection * Time.deltaTime);
   	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag == "Location")
		{
			sceneLogic.GetComponent<SceneLogic>().updateLocationName(other.gameObject.name);
		}
	}


	IEnumerator turnAroundSmoothly(Transform objectToMove, float duration)
	{
		Quaternion currentRot = objectToMove.rotation;
		Quaternion newRot = objectToMove.rotation * Quaternion.Euler(0,180f,0);

		float counter = 0;
		while(counter < duration)
		{
			counter += Time.deltaTime;
			objectToMove.rotation = Quaternion.Lerp(currentRot, newRot, counter/duration);
			yield return null;
		}
	}
}
