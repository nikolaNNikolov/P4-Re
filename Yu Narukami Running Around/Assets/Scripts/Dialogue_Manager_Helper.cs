using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class Dialogue_Manager_Helper : MonoBehaviour
{
    [SerializeField] private UI_Canvas_Controller UICanvasController;
    [SerializeField] public GameObject UIDaily;
    [SerializeField] private Dialogue_Manager dialogueManager;

	private JsonData dialogueLayerToReturnTo;
	private int dialogueIntToReturnTo;



//*********************************************************************************************
//						Dialogue Manager Helper Methods
//*********************************************************************************************
	public void activateDialogueManager()
	{
		if(!dialogueManager.isActiveAndEnabled)
		{
			Debug.Log(dialogueManager.isActiveAndEnabled);
			dialogueManager.enabled = true;
		}
	}

	public void resetDialogueManager()
	{
		StartCoroutine(deactivateAndActivateDialogueManager(dialogueManager));
	}

	IEnumerator deactivateAndActivateDialogueManager(Dialogue_Manager DialMan)
	{
		DialMan.enabled = false;
		yield return new WaitForSeconds(2);
		DialMan.enabled = true;
	}

	public void toggleCheckPrompt(bool toShow, bool isInDialogue)
	{
		if(isInDialogue)
			UIDaily.GetComponent<Transform>().Find("P4_UI_Check_Prompt").gameObject.GetComponent<Text>().text = "'E' - Pick up phone";
		else
			UIDaily.GetComponent<Transform>().Find("P4_UI_Check_Prompt").gameObject.GetComponent<Text>().text = "'E' - Check";

		if(toShow)
			UIDaily.GetComponent<Transform>().Find("P4_UI_Check_Prompt").gameObject.SetActive(true);
		else
			UIDaily.GetComponent<Transform>().Find("P4_UI_Check_Prompt").gameObject.SetActive(false);
	}

	public void moveCameraToDialoguePosition(bool isInDaily, Transform lookAtObject)
	{
		UICanvasController.moveCameraToDialoguePosition(isInDaily, lookAtObject);
	}

	public void resetCameraFromDialoguePosition(bool isInDaily, Transform lookAtObject)
	{
		UICanvasController.resetCameraFromDialoguePosition(isInDaily, lookAtObject);
	}

//*********************************************************************************************
//						Get and Set Methods
//*********************************************************************************************
    
	public JsonData get_dialogueLayerToReturnTo()			{return dialogueLayerToReturnTo;					}
	public int get_dialogueIntToReturnTo()					{return dialogueIntToReturnTo;						}
	public void set_dialogueIntToReturnTo(int data)			{dialogueIntToReturnTo = data;						}
	public void set_dialogueLayerToReturnTo(JsonData data)	{dialogueLayerToReturnTo = data;					}
	public string getTimeOfDayString()						{return UICanvasController.getTimeOfDayString();	}
	public string getCalendarDateString()					{return UICanvasController.getCalendarDateString();	}
}
