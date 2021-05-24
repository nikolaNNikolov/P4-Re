using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class UI_Battle : MonoBehaviour
{
	public enum WheelOptions
	{
		Analysis 	= 0,
		Tactics 	= 1,
		Guard 		= 2,
		Attack 		= 3,
		Skill 		= 4,
		Persona 	= 5,
		Item 		= 6,
		Escape 		= 7,
		MAX
	};

	public Image UI_TV;
	public Image UI_Wheel;
	public Image UI_Bottom_Description;
	public GameObject firstSelectedCombatChoiceButton;
	public YuPlayerController playerCharacter;
	public CharacterController playerCharacterController;
	public Animator playerCharacterAnimator;

	public bool isChildElementActive = false;

	private Dictionary<WheelOptions, GameObject> wheelToTextOnScreen = new Dictionary<WheelOptions, GameObject>();
	private RectTransform currentPositionOfWheelFrontMask, currentPositionOfWheelFront;
	private WheelOptions currentItemOnWheelFrontMask, previousItemOnWheelFrontMask;
	private bool hasWheelMaskMoved = false;
	private Color currentItemOnMenuColor, notSelectedItemOnMenuColor;
	private GameObject bottomDescriptionText;
	private Vector2 WheelFrontMovement =  new Vector2(0.0f, 50.0f);


    void OnDisable()
    {
		if(playerCharacter != null && playerCharacterController != null)
		{
			playerCharacterController.enabled = true;
			playerCharacter.enabled = true;
		}
    }

    void OnEnable()
    {
		playerCharacterController.enabled = false;
		playerCharacter.enabled = false;
		playerCharacterAnimator.SetFloat("Horizontal", 0);
		playerCharacterAnimator.SetFloat("Vertical",   0);    
	}

    void Start()
    {
		toggleTV(true);
		currentItemOnMenuColor = new Color(255, 242, 0);	//#FFF200 - Yellow
		notSelectedItemOnMenuColor  = new Color(0,0,0);		//#000000 - Black

		bottomDescriptionText = UI_Bottom_Description.GetComponent<Transform>().Find("Text").gameObject;
		currentPositionOfWheelFrontMask = UI_Wheel.GetComponent<Transform>().Find("P4_UI_WheelFront_Mask")
												.GetComponent<RectTransform>();
		currentPositionOfWheelFront = UI_Wheel.GetComponent<Transform>().Find("P4_UI_WheelFront_Mask")
												.GetComponent<Transform>().Find("P4_UI_WheelFront")
												.GetComponent<RectTransform>();
		
		wheelToTextOnScreen.Add(WheelOptions.Analysis, 	UI_Wheel.GetComponent<Transform>().Find("Analysis").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Tactics, 	UI_Wheel.GetComponent<Transform>().Find("Tactics").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Guard, 	UI_Wheel.GetComponent<Transform>().Find("Guard").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Attack, 	UI_Wheel.GetComponent<Transform>().Find("Attack").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Skill, 	UI_Wheel.GetComponent<Transform>().Find("Skill").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Persona, 	UI_Wheel.GetComponent<Transform>().Find("Persona").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Item, 		UI_Wheel.GetComponent<Transform>().Find("Item").gameObject);
		wheelToTextOnScreen.Add(WheelOptions.Escape, 	UI_Wheel.GetComponent<Transform>().Find("Escape").gameObject);

		currentItemOnWheelFrontMask = findCurrentItemOnMask();
		previousItemOnWheelFrontMask = currentItemOnWheelFrontMask;
		bottomDescriptionText.GetComponent<Text>().text = wheelToTextOnScreen[currentItemOnWheelFrontMask]
															.GetComponent<GetAndSetItemDescription>()
															.retrieveItemDescription();
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(wheelToTextOnScreen[currentItemOnWheelFrontMask]);
	}

    void Update()
    {
		if(UI_Wheel.gameObject.activeInHierarchy)
		{
			if(Input.GetButtonDown("ButtonsDown"))
			{
				if(!(currentPositionOfWheelFrontMask.anchoredPosition.y - 50.0f < -182.0f))
				{
					currentPositionOfWheelFrontMask.anchoredPosition -= WheelFrontMovement;
					currentPositionOfWheelFront.anchoredPosition += WheelFrontMovement;
					hasWheelMaskMoved = true;
				}
			}
			else if(Input.GetButtonDown("ButtonsUp"))
			{
				if(!(currentPositionOfWheelFrontMask.anchoredPosition.y + 50.0f > 168.0f))
				{
					currentPositionOfWheelFrontMask.anchoredPosition += WheelFrontMovement;
					currentPositionOfWheelFront.anchoredPosition -= WheelFrontMovement;
					hasWheelMaskMoved = true;
				}
			}
			else if(hasWheelMaskMoved)
			{
				currentItemOnWheelFrontMask = findCurrentItemOnMask();
				EventSystem.current.SetSelectedGameObject(null);
				EventSystem.current.SetSelectedGameObject(wheelToTextOnScreen[currentItemOnWheelFrontMask]);
				if(previousItemOnWheelFrontMask != currentItemOnWheelFrontMask)
				{
					wheelToTextOnScreen[previousItemOnWheelFrontMask].GetComponent<Transform>().Find("Text").
							GetComponent<Text>().color = notSelectedItemOnMenuColor;
					wheelToTextOnScreen[currentItemOnWheelFrontMask].GetComponent<Transform>().Find("Text").
							GetComponent<Text>().color = currentItemOnMenuColor;
					previousItemOnWheelFrontMask = currentItemOnWheelFrontMask;
				}
				bottomDescriptionText.GetComponent<Text>().text = wheelToTextOnScreen[currentItemOnWheelFrontMask]
					.GetComponent<GetAndSetItemDescription>()
					.retrieveItemDescription();
				hasWheelMaskMoved = false;
			}
			else if(Input.GetButtonDown("Interact") && wheelToTextOnScreen[currentItemOnWheelFrontMask].activeInHierarchy)
			{
				wheelSelectedButtonItem(currentItemOnWheelFrontMask);
				toggleTV(false);
				
			}
			else
			{
				//do nothing
			}
		}
		else if(!UI_Wheel.gameObject.activeInHierarchy && !isChildElementActive)
		{
			activateUIWheel();
		}
    }

	public void toggleTV(bool toShow)
	{
		if(toShow)
			UI_TV.gameObject.SetActive(toShow);
		else
			deactivateWheelElementAfterAnimation(UI_TV.gameObject, "P4_UI_TV_Hide");
	}

	private void wheelSelectedButtonItem(WheelOptions selectedItem)
	{
		this.gameObject.GetComponent<Transform>().Find(wheelToTextOnScreen[selectedItem]
															.GetComponent<GetAndSetItemDescription>()
															.retrieveItemUIComponentName()).gameObject.SetActive(true);
		deactivateWheelElementAfterAnimation(UI_Wheel.gameObject, "P4_UI_Wheel_Show");
	}
		
	private WheelOptions findCurrentItemOnMask()
	{
		foreach(KeyValuePair<WheelOptions, GameObject> menuItem in wheelToTextOnScreen)
		{
			if(currentPositionOfWheelFrontMask.anchoredPosition.y == menuItem.Value.GetComponent<RectTransform>().anchoredPosition.y)
				return menuItem.Key;
		}
		return WheelOptions.MAX;
	}

	public void deactivateWheelElementAfterAnimation(GameObject element, string animationName)
	{
		StartCoroutine(deactivateAfterAnimation(element, animationName));
	}

	public void activateUIWheel()
	{
		UI_Wheel.gameObject.SetActive(true);
	}

	IEnumerator deactivateAfterAnimation(GameObject obj, string animation)
	{
		Animator objAnimator = obj.GetComponent<Animator>();
		objAnimator.SetTrigger("Hide");
		yield return new WaitForSeconds (UI_Canvas_Controller.findAnimationByName(animation, objAnimator) - 0.1f);
		obj.SetActive(false);
	}
}
