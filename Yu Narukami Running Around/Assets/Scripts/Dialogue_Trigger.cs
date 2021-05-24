using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dialogue_Trigger : MonoBehaviour
{
	public LayerMask layerMaskForRay;
	public Camera dialogueRayCamera;
	public float distanceToDetectRaycastHit;
	public GameObject playerCharacter;
	public UI_Canvas_Controller uiCanvasController;

	public Dialogue_Manager dialogueManager;

	private bool isTriggeredRaycastDialogue = false;
	private bool dialogueLoaded = false;
	private GameObject lookAtObject;
	private Interactable_Controller interController;

    void Start()
    {
		//if(dialoguePath == null)
		//	dialoguePath = "TestScene/JSON/DIalogueTest";
    }
		
    void Update()
    {
		if(uiCanvasController.eActiveUI != UI_Canvas_Controller.AvaivableUI.UI_Battle)
		{
			detectRaycastHit();
			runDialogue(Input.GetButtonDown("Interact"));
		}
    }	

	private void runDialogue(bool keyPressedTrigger)
	{
		if(dialogueManager.isActiveAndEnabled && isTriggeredRaycastDialogue)
				dialogueManager.displayCheckPrompt(interController.getIsDialogueInPhone());
			else
				dialogueManager.hideCheckPrompt();
			
		if(isTriggeredRaycastDialogue &&
			dialogueManager.isActiveAndEnabled && 
			keyPressedTrigger)
		{
			dialogueManager.lookAtObject = lookAtObject.transform;
			dialogueManager.interController = interController;

			if(!dialogueLoaded)
				dialogueLoaded = dialogueManager.loadDialogue(interController.getDialoguePath(), interController.getCharacterName());

			if(dialogueLoaded)
			{
				if(dialogueManager.isTextLoading)
					dialogueManager.stopCurrentDialogueCoroutine();
				else
					dialogueLoaded = dialogueManager.printLine();
			}
		}else
		{/*do nothing*/}
		
	}


	private void detectRaycastHit()
	{
		Ray raycast = dialogueRayCamera.ViewportPointToRay(Vector3.one / 2.0f);
		RaycastHit raycastHitInfo;
		//float angleToDetectHit = 85.0f;

		Debug.DrawRay(raycast.origin, raycast.direction * distanceToDetectRaycastHit, Color.red);

		if(Physics.Raycast(raycast, out raycastHitInfo, distanceToDetectRaycastHit, layerMaskForRay))
		{
			var detectedHitObject = raycastHitInfo.collider.GetComponent<Interactable_Controller>();
			lookAtObject = raycastHitInfo.collider.GetComponent<Transform>().Find("LookAtObject").gameObject;
			if(detectedHitObject == null)
			{
				isTriggeredRaycastDialogue = false;
			}
			else
			{
				//TODO: fix angle algorythm. Temporary will disable it
				/*float angle = Vector3.Angle((dialogueRayCamera.gameObject.transform.position - this.gameObject.transform.position), 
					this.gameObject.transform.forward);
				if(angle < angleToDetectHit)
				{
					isTriggeredRaycastDialogue = true; 
				}
				else
				{
					Debug.Log("not hit angle");
				}*/
				//Debug.Log(detectedHitObject);
				interController = detectedHitObject;
				RotateMe(dialogueRayCamera.gameObject.transform, detectedHitObject.GetComponent<Transform>());
				isTriggeredRaycastDialogue = true; 
				
			}
			//TODO 2: rotate head to 'look at' player character camera object		
		}
		else
		{
			isTriggeredRaycastDialogue = false;
		}
	}

	private void RotateMe(Transform rotationToSet, Transform objectToRotate)
	{
		Vector3 eulerAnglesRotation = rotationToSet.eulerAngles;
		Vector3 forwardRotation = rotationToSet.forward;

		objectToRotate.eulerAngles = eulerAnglesRotation;
		objectToRotate.forward = rotationToSet.forward;
		objectToRotate.RotateAround(objectToRotate.transform.position, objectToRotate.transform.up, 180f);
	}

}
