using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Menu_Button_Controller : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] private string elementDescription;
    [SerializeField] private UI_Menu UIMenu;
    [SerializeField] private GameObject itemUIMenuComponent;
	[SerializeField] private UI_Menu.MenuStates newActiveMenu;
	private Color whiteColor = new Color(1,1,1,1);
	private Color blackColor = new Color(0,0,0,1);

	void Start()
	{
	}

	void Update()
	{		
	}

	public void OnSelect(BaseEventData eventData)
	{
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(450, 120);

        transform.Find("Text").GetComponent<Text>().color = whiteColor;
		transform.Find("Text").GetComponent<Outline>().effectColor = blackColor;
        transform.Find("Text").GetComponent<Text>().fontSize = 100;

        UIMenu.updateDescription(elementDescription);
	}

	public void OnDeselect(BaseEventData eventData)
	{
        returnToDefault();
	}

	public void showMenuElement()
	{
		returnToDefault();
		UIMenu.switchToNewPanel(newActiveMenu);
	}

	private void returnToDefault()
	{
		this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 120);
        
        transform.Find("Text").GetComponent<Text>().color = blackColor;
		transform.Find("Text").GetComponent<Outline>().effectColor = whiteColor;
        transform.Find("Text").GetComponent<Text>().fontSize = 85;
	}
}
