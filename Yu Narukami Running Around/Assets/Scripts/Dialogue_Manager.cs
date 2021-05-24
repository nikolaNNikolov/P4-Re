using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using LitJson;

public class Dialogue_Manager : MonoBehaviour
{
	public Dialogue_Manager_Helper UIDailyScript;
	public Image dialogueWindow;
	public Image dialogueChoicesWindow;
	public Image dialogueCharacterPortrait;
	public Image dialoguePlayerCharacterPortrait;
	public Image dialoguePhoneUI;
	public Sprite emptyImage;
	public Text textDisplay;
	public Text speakerText;
	public YuPlayerController playerCharacter;
	public CharacterController playerCharacterController;
	public Animator playerCharacterAnimator;
	public GameObject firstSelectedDialogueChoiceButton;
	public Interactable_Controller interController;

	public bool isTextLoading;

	public Transform lookAtObject;
	public Sprite characterSprite;
	private Image dialogueCharacterPortraitChildren;
	private Coroutine currentDialogueCoroutine;
	private Text currentTextElement;
	private string currentTextToWrite;

	private JsonData dialogue;
	private JsonData currentLayer;
	private int index;
	[SerializeField] private float writerTimeFloat = 0.1f;
	private float writerTimerCountdown;
	private Transform conversationLookAt;
	//private float textDisplayDelay = 0.01f;
	private bool isInDialogue;
	private Color activeTextColor, inactiveTextColor;
	private string speakerObjectName;

	private int randomNumToSendMood;

	void Start()
	{
		activeTextColor = new Color(255, 255, 255);
		inactiveTextColor  = new Color(20,20,20);
		isTextLoading = false;
	}

	void Update()
	{
		randomNumToSendMood =  Random.Range(0, 7);
	}

	public bool printLine()
	{
		if(isInDialogue)
		{
			JsonData line = currentLayer[index];
			string speaker = "";

			foreach(JsonData key in line.Keys)
				speaker = key.ToString();

			switch(speaker)
			{
				case "CalendarDate":
					JsonData calendarDateOptions = line[0];
					for(int optionsNumber = 0; optionsNumber < calendarDateOptions.Count; optionsNumber++)
					{
						JsonData choice = calendarDateOptions[optionsNumber];
						if(choice[0][0].ToString() == UIDailyScript.getCalendarDateString())
						{
							currentLayer = choice[0];
							index = 1;
							printLine();
							return true;
						}
					}
					textDisplay.text = "";
					speakerText.text = "";
					isInDialogue = false;
					closeDialogueWindow();
					return false;
				case "TimeOfDay":
					JsonData timesOfDayOptions = line[0];
					for(int optionsNumber = 0; optionsNumber < timesOfDayOptions.Count; optionsNumber++)
					{
						JsonData choice = timesOfDayOptions[optionsNumber];
						if(choice[0][0].ToString() == UIDailyScript.getTimeOfDayString())
						{
							currentLayer = choice[0];
							index = 1;
							printLine();
							return true;
						}
					}
					return true;
				case "?":
					JsonData options = line[0];

					textDisplay.color = inactiveTextColor;
					for(int optionsNumber = 0; optionsNumber < options.Count; optionsNumber++)
					{
						JsonData choice = options[optionsNumber];
						dialogueChoicesWindow.transform.GetChild(optionsNumber).gameObject.SetActive(true);
						dialogueChoicesWindow.transform.GetChild(optionsNumber).GetComponent<Transform>().Find("Text")
							.GetComponent<Text>().text= choice[0][0].ToString();
						dialogueChoicesWindow.transform.GetChild(optionsNumber).GetComponent<Button>().onClick.
						AddListener(delegate {toDoOnSelect (choice);});
					}
					openDialogueChoicesWindow();
					controlChoices();
					return true;
				case "LayerToReturnTo":
					UIDailyScript.set_dialogueLayerToReturnTo(currentLayer);
					UIDailyScript.set_dialogueIntToReturnTo(index);
					index++;
					printLine();
					return true;
				case "GoBack":
					currentLayer = UIDailyScript.get_dialogueLayerToReturnTo();
					index = UIDailyScript.get_dialogueIntToReturnTo() + 2;
					printLine();
					return true;
				case "EOD":
					isInDialogue = false;
					closeDialogueWindow();
					return false;
				default:
					dialogueWindow.GetComponent<Animator>().SetTrigger("ButtonActivate");
					textDisplay.color = activeTextColor;
					speakerText.text = speakerObjectName;
					currentDialogueCoroutine = StartCoroutine(writeText(textDisplay, line[0].ToString(), writerTimeFloat));
					//textDisplay.text = line[0].ToString();
					index++;
					return true;
			}
		}
		else{return false;}
	}

	public void stopCurrentDialogueCoroutine()
	{
		if(currentDialogueCoroutine != null)
		{
			dialogueWindow.GetComponent<Animator>().SetTrigger("ButtonSkip");
			isTextLoading = false;
			StopCoroutine(currentDialogueCoroutine);
			currentTextElement.text = currentTextToWrite;
			
			if(interController.getContainsCharacterPortrait())
				interController.stopAnimation();
			//Debug.Log("Stop the coroutine!" + currentDialogueCoroutine);
		}
	}

	public bool loadDialogue(string path, string speakerName)
	{
		if(!isInDialogue)
		{
			openDialogueWindow();
			index = 0;
			var jsonTextFile = Resources.Load<TextAsset>("Dialogues/" + path);
			dialogue = JsonMapper.ToObject(jsonTextFile.text);
			speakerObjectName = speakerName;
			currentLayer = dialogue;
			isInDialogue = true;
			return true;
		}
		return false;
	}

	private void controlChoices()
	{
		EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(firstSelectedDialogueChoiceButton);
	}

	private void toDoOnSelect(JsonData choice)
	{
		currentLayer = choice[0];
		index = 1;
		closeDialogueChoicesWindow();
	}

	private void closeDialogueWindow()
	{
		string animationName_Back_2 = "P4_UI_Dialogue_Back_2_Show";
		playerCharacterController.enabled = true;
		playerCharacter.enabled = true;
		StartCoroutine(de_activateDialogueAfterAnimation(dialogueWindow.gameObject, dialogueWindow.gameObject, 
			animationName_Back_2, "Hide", false));
		
		UIDailyScript.resetDialogueManager();
		UIDailyScript.resetCameraFromDialoguePosition(interController.getIsDialogueInDaily(), conversationLookAt);
	}

	private void openDialogueWindow()
	{
		dialoguePlayerCharacterPortrait.gameObject.SetActive(false);
		dialogueCharacterPortrait.gameObject.SetActive(false);
		dialoguePhoneUI.gameObject.SetActive(false);

		conversationLookAt = playerCharacter.GetComponent<Transform>().Find("ConversationLookAt").GetComponent<Transform>();
		UIDailyScript.moveCameraToDialoguePosition(interController.getIsDialogueInDaily(), conversationLookAt);
		
		characterSprite = interController.getCharacterPortrait();

		if(interController.getContainsCharacterPortrait())
		{
			if(interController.getIsDialogueInPhone())
			{
				dialogueCharacterPortrait.gameObject.SetActive(false);
				dialoguePhoneUI.gameObject.SetActive(true);

				Image dialogueCharacterPortraitPhone = dialoguePhoneUI.gameObject.transform.Find("Phone_Background")
																.GetComponent<Transform>().Find("Character_Image_Mask")
																.GetComponent<Transform>().Find("Portrait_Back_2").GetComponent<Image>();
				dialoguePhoneUI.gameObject.transform.Find("Phone_Background")
																.GetComponent<Transform>().Find("Character_Image_Mask")
																.GetComponent<Transform>().Find("Portrait_BG").GetComponent<Image>()
																.sprite = interController.getCharacterPortraitBG();

				dialogueCharacterPortraitPhone.sprite = characterSprite;
				dialogueCharacterPortraitChildren = dialogueCharacterPortraitPhone.gameObject.transform.Find("Portrait_Back_1").GetComponent<Image>();
				dialogueCharacterPortraitChildren.sprite = characterSprite;
				dialogueCharacterPortraitChildren = dialogueCharacterPortraitChildren.gameObject.transform.Find("Portrait").GetComponent<Image>();
				dialogueCharacterPortraitChildren.sprite = characterSprite;
			}
			else
			{
				dialogueCharacterPortrait.gameObject.SetActive(true);
				dialoguePhoneUI.gameObject.SetActive(false);

				dialogueCharacterPortrait.sprite = characterSprite;
				dialogueCharacterPortraitChildren = dialogueCharacterPortrait.gameObject.transform.Find("Portrait_Back_1").GetComponent<Image>();
				dialogueCharacterPortraitChildren.sprite = characterSprite;
				dialogueCharacterPortraitChildren = dialogueCharacterPortraitChildren.gameObject.transform.Find("Portrait").GetComponent<Image>();
				dialogueCharacterPortraitChildren.sprite = characterSprite;

				//dialogueCharacterPortraitChildren = dialogueCharacterPortrait;
			}

			
		}
		else
		{
			dialogueCharacterPortrait.gameObject.SetActive(false);
			dialogueCharacterPortrait.GetComponentInChildren<Image>().sprite = emptyImage;
		}

		playerCharacterController.enabled = false;
		playerCharacter.enabled = false;
		playerCharacterAnimator.SetFloat("Horizontal", 0);
		playerCharacterAnimator.SetFloat("Vertical",   0);
		dialogueWindow.gameObject.SetActive(true);
	}



	private void openDialogueChoicesWindow()
	{
		string animationName = "P4_UI_Dialogue_Box_Show";
		string animationNamePortrait = "P4_UI_Dialogue_Yu_Portrait_Show";

		dialogueWindow.GetComponent<Animator>().SetBool("toggleConversation", true);
			
		StartCoroutine(de_activateDialogueAfterAnimation(dialogueWindow.gameObject, dialogueChoicesWindow.gameObject,
			animationName, "HideChoices", true));
		StartCoroutine(de_activateDialogueAfterAnimation(dialogueWindow.gameObject, dialoguePlayerCharacterPortrait.gameObject,
			animationNamePortrait, "HideChoicesPortrait", true));
	}

	private void closeDialogueChoicesWindow()
	{
		string animationName = "P4_UI_Dialogue_Box_Show";
		string animationNamePortrait = "P4_UI_Dialogue_Yu_Portrait_Show";
		deactivateAllButtonsInDialogue();

		dialogueWindow.GetComponent<Animator>().SetBool("toggleConversation", false);

		StartCoroutine(de_activateDialogueAfterAnimation(dialogueWindow.gameObject, dialogueChoicesWindow.gameObject,
			animationName, "HideChoices", false));
		StartCoroutine(de_activateDialogueAfterAnimation(dialogueWindow.gameObject, dialoguePlayerCharacterPortrait.gameObject,
			animationName, "HideChoicesPortrait", false));
	}

	private void deactivateAllButtonsInDialogue()
	{
		dialogueChoicesWindow.GetComponentInChildren<Text>().text = "";

		for(int i = 0; i < dialogueChoicesWindow.transform.childCount; i++)
		{
			dialogueChoicesWindow.transform.GetChild(i).gameObject.SetActive(false);
			dialogueChoicesWindow.transform.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
		}
	}

	private IEnumerator writeText(Text textElement, string textToWrite, float writeTime)
	{
		int characterIndex = 0;
		currentTextElement = textElement;
		currentTextToWrite = textToWrite;
		
		if(interController.getContainsCharacterPortrait())
			interController.startAnimation((Interactable_Controller.characterExpressions)randomNumToSendMood);

		while(characterIndex < textToWrite.Length)
		{
			isTextLoading = true;
			writerTimerCountdown -= Time.deltaTime;
			while(writerTimerCountdown <= 0f)
			{
				writerTimerCountdown += writeTime;
				characterIndex++;
				textElement.text = textToWrite.Substring(0, characterIndex);
				yield return new WaitForSeconds (writerTimerCountdown);
			}
		}
		if(interController.getContainsCharacterPortrait())
			interController.stopAnimation();

		isTextLoading = false;
		Debug.Log("Exit");
		yield return null;
	}


	public static IEnumerator de_activateDialogueAfterAnimation(GameObject animatorGameObject, GameObject objectToHide,
		 string animation, string triggerName, bool trigger)
	{
		if(trigger == true)
			objectToHide.SetActive(trigger);

		Animator objAnimator = animatorGameObject.GetComponent<Animator>();
		objAnimator.SetTrigger(triggerName);
		yield return new WaitForSeconds (UI_Canvas_Controller.findAnimationByName(animation, objAnimator) - 0.1f);
		
		if(trigger == false)
			objectToHide.SetActive(trigger);
	}


	public void displayCheckPrompt(bool isInDialogue)
	{
		UIDailyScript.toggleCheckPrompt(true, isInDialogue);	
	}
	public void hideCheckPrompt()	
	{
		UIDailyScript.toggleCheckPrompt(false, false);
	}

}
