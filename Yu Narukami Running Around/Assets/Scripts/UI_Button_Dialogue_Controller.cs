using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Button_Dialogue_Controller : MonoBehaviour, ISelectHandler, IDeselectHandler
{
	public GameObject selectedState;
	public GameObject deselectedState;
	public bool isFirst;

	private Color whiteColor = new Color(1,1,1,1);
	private Color blackColor = new Color(0,0,0,1);

	void Start()
	{
		if(!isFirst)
		{
			deselectedState.SetActive(true);
			selectedState.SetActive(false);
		}
	}

	public void OnSelect(BaseEventData eventData)
	{
		transform.Find("Text").GetComponent<Text>().color = blackColor;
		deselectedState.SetActive(false);
		selectedState.transform.Find("Arrow").GetComponent<Text>().text = "";
		selectedState.SetActive(true);
	}

	public void OnDeselect(BaseEventData eventData)
	{
		transform.Find("Text").GetComponent<Text>().color = whiteColor;
		deselectedState.SetActive(true);
		selectedState.SetActive(false);
	}
}
