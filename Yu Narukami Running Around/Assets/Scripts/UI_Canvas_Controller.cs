using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UI_Canvas_Controller : MonoBehaviour
{
	public struct BattleIcons
	{
		public CharactersAvaivableForParty character;
		private string battleSpriteLocation;
		private string dungeonSpriteLocation;
		public Sprite battleSprite;
		public Sprite dungeonSprite;
		public float healthPointsCurrent;
		public float spiritPointsCurrent;
		public float healthPointsTotal;
		public float spiritPointsTotal;
		public string firstName;
		public string lastName;
		public int level;

		public BattleIcons(CharactersAvaivableForParty _character,
			string _dungeonSpriteLocation,
			string _battleSpriteLocation,
			float _healthPointsCurrent,
			float _spiritPointsCurrent,
			float _healthPointsTotal,
			float _spiritPointsTotal,
			string _name,
			string _lastName,
			int _level)
		{
			character = _character;
			battleSpriteLocation = _battleSpriteLocation;
			dungeonSpriteLocation = _dungeonSpriteLocation;
			battleSprite = Resources.Load<Sprite>(battleSpriteLocation) as Sprite;
			dungeonSprite = Resources.Load<Sprite>(dungeonSpriteLocation)  as Sprite;
			healthPointsCurrent = _healthPointsCurrent;
			spiritPointsCurrent = _spiritPointsCurrent;
			healthPointsTotal = _healthPointsTotal;
			spiritPointsTotal = _spiritPointsTotal;
			firstName = _name;
			lastName = _lastName;
			level = _level;
		}
	}

	public enum CharactersAvaivableForParty
	{
		Yu 		= 0,
		Yosuke 	= 1,
		Chie 	= 2,
		Yukiko 	= 3,
		Kanji 	= 4,
		Teddie 	= 5,
		Naoto 	= 6,
		MAX
	}
	public enum AvaivableUI
	{
		UI_Dungeon 	= 0,
		UI_Battle 	= 1,
		UI_Daily 	= 2,
		UI_Menu		= 3,
		UI_Dialogue = 4,
		MAX
	}
	public enum TimeOfDay
	{
		Early_Morning 	= 0,
		Morning 		= 1,
		After_School 	= 2,
		Afternoon 		= 3,
		Evening 		= 4,
		MAX
	}
		
	[Header("\tUI Elements")]
	public GameObject UI_Dungeon;
	public GameObject UI_Battle;
	public GameObject UI_Daily;
	public GameObject UI_Dialogue;
	public UI_Menu UIMenu;
	public UI_Menu_Elements_Skill UIMenuSkill;
	public SceneLogic sceneLogic;
	[Range(1,4)]
	public int totalPositionsActive = 1;
	private int currentPositionsActive = 1;
	public AvaivableUI eActiveUI;
	public bool isChangedCamera;
	[Header("\tUI_Battle Positions")]
	public Image UI_Battle_positionFirst;
	public Image UI_Battle_positionSecond;
	public Image UI_Battle_positionThird;
	public Image UI_Battle_positionFourth;
	[Header("\tUI_Dungeon Positions")]
	public Image UI_Dungeon_positionFirst;
	public Image UI_Dungeon_positionSecond;
	public Image UI_Dungeon_positionThird;
	public Image UI_Dungeon_positionFourth;
	[Header("\tUI_Menu Positions")]
	public Image UI_Menu_positionFirst;
	public Image UI_Menu_positionSecond;
	public Image UI_Menu_positionThird;
	public Image UI_Menu_positionFourth;
	[Header("\tParty")]
	public CharactersAvaivableForParty[] characterInParty;


	private Dictionary<CharactersAvaivableForParty, BattleIcons> charactersToParty;
	private Dictionary<TimeOfDay, string> timeOfDayStrings;
	private BattleIcons yuIcon, yosukeIcon, chieIcon, yukikoIcon, kanjiIcon, teddieIcon, naotoIcon;
	private bool isActiveLocation = false, isExitMenu = false;
	private TimeOfDay eActiveDayTime;
	private string currentCalendarDate;
	private float secondsToCloseLocation = 3.0f;
//*********************************************************************************************
//						UI_Canvas_Controller General Methods
//*********************************************************************************************
	void Start()
	{
		charactersToParty = new Dictionary<CharactersAvaivableForParty, BattleIcons>();
		timeOfDayStrings = new Dictionary<TimeOfDay, string>();
		yuIcon = new BattleIcons(CharactersAvaivableForParty.Yu, "Materials/Textures/UI/UI Elements/P4_UI_Yu_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Yu_Battle", 100, 50, 200, 400, "Yu", "Narukami", 82);
		yosukeIcon = new BattleIcons(CharactersAvaivableForParty.Yosuke, "Materials/Textures/UI/UI Elements/P4_UI_Yosuke_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Yosuke_Battle", 30, 50, 200, 100, "Yosuke", "Hanamura", 26);
		chieIcon = new BattleIcons(CharactersAvaivableForParty.Chie, "Materials/Textures/UI/UI Elements/P4_UI_Chie_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Chie_Battle", 300, 300, 300, 300, "Chie", "Satonaka", 80);
		yukikoIcon = new BattleIcons(CharactersAvaivableForParty.Yukiko, "Materials/Textures/UI/UI Elements/P4_UI_Yukiko_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Yukiko_Battle", 40, 50, 200, 400, "Yukiko", "Amagi", 79);
		kanjiIcon = new BattleIcons(CharactersAvaivableForParty.Kanji, "Materials/Textures/UI/UI Elements/P4_UI_Kanji_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Kanji_Battle", 100, 50, 200, 400, "Kanji", "Tatsumi", 43);
		teddieIcon = new BattleIcons(CharactersAvaivableForParty.Teddie, "Materials/Textures/UI/UI Elements/P4_UI_Teddie_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Teddie_Battle", 100, 50, 200, 400, "Teddie", "", 35);
		naotoIcon = new BattleIcons(CharactersAvaivableForParty.Naoto, "Materials/Textures/UI/UI Elements/P4_UI_Naoto_Dungeon", 
			"Materials/Textures/UI/UI Elements/P4_UI_Naoto_Battle", 200, 70, 400, 300, "Naoto", "Shirogane", 81);

		charactersToParty.Add(CharactersAvaivableForParty.Yu, 		yuIcon);
		charactersToParty.Add(CharactersAvaivableForParty.Yosuke, 	yosukeIcon);
		charactersToParty.Add(CharactersAvaivableForParty.Chie, 	chieIcon);
		charactersToParty.Add(CharactersAvaivableForParty.Yukiko, 	yukikoIcon);
		charactersToParty.Add(CharactersAvaivableForParty.Kanji, 	kanjiIcon);
		charactersToParty.Add(CharactersAvaivableForParty.Teddie, 	teddieIcon);
		charactersToParty.Add(CharactersAvaivableForParty.Naoto, 	naotoIcon);

		timeOfDayStrings.Add(TimeOfDay.Early_Morning, 	"Early Morning");
		timeOfDayStrings.Add(TimeOfDay.Morning, 		"Morning");
		timeOfDayStrings.Add(TimeOfDay.After_School, 	"After School");
		timeOfDayStrings.Add(TimeOfDay.Afternoon, 		"Afternoon");
		timeOfDayStrings.Add(TimeOfDay.Evening, 		"Evening");


		hideAllBattleIcons();
		hideAllDungeonIcons();
		hideAllMenuIcons();
		currentPositionsActive = 0;
	}
		
	void Update()
	{
		if(currentPositionsActive != totalPositionsActive)
		{
			updateBattleIcons(totalPositionsActive);
			updateDungeonIcons(totalPositionsActive);
			currentPositionsActive = totalPositionsActive;
		}

		switch(eActiveUI)
		{
		case AvaivableUI.UI_Dungeon:
			updateDungeonValues();
			break;
		case AvaivableUI.UI_Battle:
			updateBattleValues();
			break;
		case AvaivableUI.UI_Menu:
			handleMenuIcons(true);
			break;
		default:
			break;
		}

	}

	private void updatePositionImage(Image givenPosition, Sprite givenSprite)
	{
		givenPosition.sprite = givenSprite;
	}

	private void updatePositionInfo(Image givenPosition, BattleIcons givenIcon)
	{
		givenPosition.GetComponent<Transform>().Find("SP").GetComponent<Image>()
					.fillAmount = (givenIcon.spiritPointsCurrent/givenIcon.spiritPointsTotal);
		givenPosition.GetComponent<Transform>().Find("HP").GetComponent<Image>()
					.fillAmount = (givenIcon.healthPointsCurrent/givenIcon.healthPointsTotal);

		if(eActiveUI == AvaivableUI.UI_Battle)
		{
			givenPosition.GetComponent<Transform>().Find("SP Level").GetComponent<Text>()
					.text = Mathf.Round(givenIcon.spiritPointsCurrent).ToString();
			givenPosition.GetComponent<Transform>().Find("HP Level").GetComponent<Text>()
					.text = Mathf.Round(givenIcon.healthPointsCurrent).ToString();
		}
	}

	private void showIcons(Image[] toShowIcons)
	{
		foreach(Image icon in toShowIcons)
			icon.GetComponent<CanvasGroup>().alpha = 1;

	}

	public void updateTotalPositionsActive()
	{
		totalPositionsActive++;
		if(totalPositionsActive >= 4) 
			totalPositionsActive = 4;
		else
			updateBattleIcons(totalPositionsActive);
	}
		
	public static float findAnimationByName(string name, Animator animator)
	{
		float timeToReturn = 0.0f;
		AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
		foreach(AnimationClip clip in clips)
		{
			if(clip.name == name)
			{
				return timeToReturn = clip.length;
			}
		}
		return timeToReturn;
	}

	public void hideUIDungeonElements()	{UI_Dungeon.SetActive(false);	}
	public void showUIDungeonElements()	{UI_Dungeon.SetActive(true);	}
	public void hideUIBattleElements()	{UI_Battle.SetActive(false);	}
	public void showUIBattleElements()	{UI_Battle.SetActive(true);		}
	public void hideUIDailyElements()	{UI_Daily.SetActive(false); 	}
	public void showUIDailyElements()	{UI_Daily.SetActive(true);		}
	public void hideAll()
	{
		hideUIDungeonElements();
		hideUIBattleElements();
		hideUIDailyElements();
	}

//*********************************************************************************************
//						UI_Battle Methods
//*********************************************************************************************

	private void updateBattleValues()
	{
		switch(totalPositionsActive)
		{
		case 1:
			updatePositionInfo(UI_Battle_positionFourth, charactersToParty[characterInParty[0]]);
			break;
		case 2:
			updatePositionInfo(UI_Battle_positionThird, charactersToParty[characterInParty[0]]);
			updatePositionInfo(UI_Battle_positionFourth, charactersToParty[characterInParty[1]]);
			break;
		case 3:
			updatePositionInfo(UI_Battle_positionSecond, charactersToParty[characterInParty[0]]);
			updatePositionInfo(UI_Battle_positionThird, charactersToParty[characterInParty[1]]);
			updatePositionInfo(UI_Battle_positionFourth, charactersToParty[characterInParty[2]]);
			break;
		case 4:
			updatePositionInfo(UI_Battle_positionFirst, charactersToParty[characterInParty[0]]);
			updatePositionInfo(UI_Battle_positionSecond, charactersToParty[characterInParty[1]]);
			updatePositionInfo(UI_Battle_positionThird, charactersToParty[characterInParty[2]]);
			updatePositionInfo(UI_Battle_positionFourth, charactersToParty[characterInParty[3]]);
			break;
		default:
			break;
		}
	}

	private void updateBattleIcons(int totalPositionsActive)
	{
		hideAllBattleIcons();
		switch(totalPositionsActive)
		{
		case 1:
			showIcons(new Image[]{UI_Battle_positionFourth});
			updatePositionImage(UI_Battle_positionFourth, charactersToParty[characterInParty[0]].battleSprite);
			break;
		case 2:
			showIcons(new Image[]{UI_Battle_positionFourth, UI_Battle_positionThird});
			updatePositionImage(UI_Battle_positionThird, charactersToParty[characterInParty[0]].battleSprite);
			updatePositionImage(UI_Battle_positionFourth, charactersToParty[characterInParty[1]].battleSprite);
			break;
		case 3:
			showIcons(new Image[]{UI_Battle_positionFourth, UI_Battle_positionThird, UI_Battle_positionSecond});
			updatePositionImage(UI_Battle_positionSecond, charactersToParty[characterInParty[0]].battleSprite);
			updatePositionImage(UI_Battle_positionThird, charactersToParty[characterInParty[1]].battleSprite);
			updatePositionImage(UI_Battle_positionFourth, charactersToParty[characterInParty[2]].battleSprite);
			break;
		case 4:
			showIcons(new Image[]{UI_Battle_positionFourth, UI_Battle_positionThird, 
									UI_Battle_positionSecond, UI_Battle_positionFirst});
			updatePositionImage(UI_Battle_positionFirst, charactersToParty[characterInParty[0]].battleSprite);
			updatePositionImage(UI_Battle_positionSecond, charactersToParty[characterInParty[1]].battleSprite);
			updatePositionImage(UI_Battle_positionThird, charactersToParty[characterInParty[2]].battleSprite);
			updatePositionImage(UI_Battle_positionFourth, charactersToParty[characterInParty[3]].battleSprite);
			break;
		default:
			break;
		}
	}

	private void hideAllBattleIcons()
	{
		UI_Battle_positionFirst.GetComponent<CanvasGroup>().alpha = 0;
		UI_Battle_positionSecond.GetComponent<CanvasGroup>().alpha = 0;
		UI_Battle_positionThird.GetComponent<CanvasGroup>().alpha = 0;
		UI_Battle_positionFourth.GetComponent<CanvasGroup>().alpha = 0;
	}

//*********************************************************************************************
//						UI_Dungeon Methods
//*********************************************************************************************

	private void updateDungeonValues()
	{
		switch(totalPositionsActive)
		{
		case 1:
			updatePositionInfo(UI_Dungeon_positionFourth, charactersToParty[characterInParty[0]]);
			break;
		case 2:
			updatePositionInfo(UI_Dungeon_positionThird, charactersToParty[characterInParty[0]]);
			updatePositionInfo(UI_Dungeon_positionFourth, charactersToParty[characterInParty[1]]);
			break;
		case 3:
			updatePositionInfo(UI_Dungeon_positionSecond, charactersToParty[characterInParty[0]]);
			updatePositionInfo(UI_Dungeon_positionThird, charactersToParty[characterInParty[1]]);
			updatePositionInfo(UI_Dungeon_positionFourth, charactersToParty[characterInParty[2]]);
			break;
		case 4:
			updatePositionInfo(UI_Dungeon_positionFirst, charactersToParty[characterInParty[0]]);
			updatePositionInfo(UI_Dungeon_positionSecond, charactersToParty[characterInParty[1]]);
			updatePositionInfo(UI_Dungeon_positionThird, charactersToParty[characterInParty[2]]);
			updatePositionInfo(UI_Dungeon_positionFourth, charactersToParty[characterInParty[3]]);
			break;
		default:
			break;
		}
	}

	private void updateDungeonIcons(int totalPositionsActive)
	{
		hideAllDungeonIcons();
		switch(totalPositionsActive)
		{
		case 1:
			showIcons(new Image[]{UI_Dungeon_positionFourth});
			updatePositionImage(UI_Dungeon_positionFourth, charactersToParty[characterInParty[0]].dungeonSprite);
			break;
		case 2:
			showIcons(new Image[]{UI_Dungeon_positionFourth, UI_Dungeon_positionThird});
			updatePositionImage(UI_Dungeon_positionThird, charactersToParty[characterInParty[0]].dungeonSprite);
			updatePositionImage(UI_Dungeon_positionFourth, charactersToParty[characterInParty[1]].dungeonSprite);
			break;
		case 3:
			showIcons(new Image[]{UI_Dungeon_positionFourth, UI_Dungeon_positionThird, UI_Dungeon_positionSecond});
			updatePositionImage(UI_Dungeon_positionSecond, charactersToParty[characterInParty[0]].dungeonSprite);
			updatePositionImage(UI_Dungeon_positionThird, charactersToParty[characterInParty[1]].dungeonSprite);
			updatePositionImage(UI_Dungeon_positionFourth, charactersToParty[characterInParty[2]].dungeonSprite);
			break;
		case 4:
			showIcons(new Image[]{UI_Dungeon_positionFourth, UI_Dungeon_positionThird, 
									UI_Dungeon_positionSecond, UI_Dungeon_positionFirst});
			updatePositionImage(UI_Dungeon_positionFirst, charactersToParty[characterInParty[0]].dungeonSprite);
			updatePositionImage(UI_Dungeon_positionSecond, charactersToParty[characterInParty[1]].dungeonSprite);
			updatePositionImage(UI_Dungeon_positionThird, charactersToParty[characterInParty[2]].dungeonSprite);
			updatePositionImage(UI_Dungeon_positionFourth, charactersToParty[characterInParty[3]].dungeonSprite);
			break;
		default:
			break;
		}
	}

	private void hideAllDungeonIcons()
	{
		UI_Dungeon_positionFirst.GetComponent<CanvasGroup>().alpha = 0;
		UI_Dungeon_positionSecond.GetComponent<CanvasGroup>().alpha = 0;
		UI_Dungeon_positionThird.GetComponent<CanvasGroup>().alpha = 0;
		UI_Dungeon_positionFourth.GetComponent<CanvasGroup>().alpha = 0;
	}

//*********************************************************************************************
//						UI_Daily Methods
//*********************************************************************************************

	public void activateUIDailyDialogueManager()
	{
		UI_Dialogue.GetComponent<Dialogue_Manager_Helper>().activateDialogueManager();
	}

	public void moveCameraToDialoguePosition(bool isInDaily, Transform lookAtObject)
	{
		sceneLogic.moveCameraToDialoguePosition(isInDaily, lookAtObject);
	}


	public void resetCameraFromDialoguePosition(bool isInDaily, Transform lookAtObject)
	{
		sceneLogic.resetCameraFromDialoguePosition(isInDaily, lookAtObject);
	}


//*********************************************************************************************
//						Multiple UI Methods
//*********************************************************************************************

	public void updateLocationText(string locationText)
	{
		//TODO: fix problems when secondsToCloseLocation < 0
		secondsToCloseLocation += 2.0f; //seconds to display text
		string animationName =  "P4_UI_Location_Show";

		UI_Dungeon.GetComponent<Transform>().Find("P4_UI_Location_Back")
			.GetComponent<Transform>().Find("P4_UI_Location_Front")
			.GetComponent<Transform>().Find("Text").GetComponent<Text>().text = locationText;


		//TODO: Change This Logic for Dungeon Only, not DAILY
		StartCoroutine(locationChange(UI_Daily.GetComponent<Transform>().Find("P4_UI_Location_Back").gameObject, locationText));
		
		if(!isActiveLocation && eActiveUI == AvaivableUI.UI_Daily)
		{
			isActiveLocation = true;
			secondsToCloseLocation = 3.0f;
			UI_Daily.GetComponent<Transform>().Find("P4_UI_Location_Back").gameObject.SetActive(true);
			StartCoroutine(deactivateLocationAfterSecondsAndAnimation(UI_Daily.GetComponent<Transform>()
				.Find("P4_UI_Location_Back").gameObject, animationName));
			
		}
	}

	IEnumerator locationChange (GameObject obj, string newLocationText)
	{
		Animator objAnimator = obj.GetComponent<Animator>();
		Text textToChange = obj.GetComponent<Transform>().Find("P4_UI_Location_Mask")
			.GetComponent<Transform>().Find("P4_UI_Location_Front")
			.GetComponent<Transform>().Find("Text").GetComponent<Text>();
		textToChange.gameObject.GetComponent<RectTransform>().anchoredPosition  = Vector3.zero;
		objAnimator.SetTrigger("ToggleHide");
		yield return new WaitForSeconds (findAnimationByName("P4_UI_Location_Toggle", objAnimator));

		textToChange.text = newLocationText;
		objAnimator.SetTrigger("ToggleShow");
		yield return new WaitForSeconds (findAnimationByName("P4_UI_Location_Toggle", objAnimator));
	}

	IEnumerator deactivateLocationAfterSecondsAndAnimation (GameObject obj, string animation)
	{
		Animator objAnimator = obj.GetComponent<Animator>();

		while (secondsToCloseLocation > 0.0f) {
			secondsToCloseLocation -= Time.deltaTime;
			yield return null;
		}

		objAnimator.SetTrigger("Hide");
		yield return new WaitForSeconds (findAnimationByName(animation, objAnimator));
		isActiveLocation = false;
		obj.SetActive(false);
	}

	public void updateTimeOfDay(TimeOfDay newTimeOfDay)
	{
		eActiveDayTime = newTimeOfDay;
		//switch(eActiveUI)
		//{
		//case AvaivableUI.UI_Dungeon:
			UI_Dungeon.GetComponent<Transform>().Find("Time_Of_Day")
				.GetComponent<Text>().text = timeOfDayStrings[newTimeOfDay];
		//	break;
		//case AvaivableUI.UI_Daily:
			UI_Daily.GetComponent<Transform>().Find("Time_Of_Day")
				.GetComponent<Text>().text = timeOfDayStrings[newTimeOfDay];
		//	break;
		//default:
		//	break;
		//}
	}

	public void updateCalendarDate(string newDate)
	{
		currentCalendarDate = newDate;
		//switch(eActiveUI)
		//{
		//case AvaivableUI.UI_Dungeon:
			UI_Dungeon.GetComponent<Transform>().Find("Calendar_Date")
				.GetComponent<Text>().text = newDate;
		//	break;
		//case AvaivableUI.UI_Daily:
			UI_Daily.GetComponent<Transform>().Find("Calendar_Date")
				.GetComponent<Text>().text = newDate;
		//	break;
		//default:
		//	break;
		//}
	}

//*********************************************************************************************
//						UI_Menu Methods
//*********************************************************************************************

private void updateMenuValues()
{
	UIMenuSkill.hideAllPosition();
	switch(totalPositionsActive)
	{
	case 1:
		updatePositionInfo(UI_Menu_positionFourth, charactersToParty[characterInParty[0]]);

		UIMenu.enableIcons(new Image[]{UIMenuSkill.getFirstPosition()});
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getFirstPosition(), charactersToParty[characterInParty[0]]);
		break;
	case 2:
		updatePositionInfo(UI_Menu_positionThird, charactersToParty[characterInParty[0]]);
		updatePositionInfo(UI_Menu_positionFourth, charactersToParty[characterInParty[1]]);

		UIMenu.enableIcons(new Image[]{UIMenuSkill.getFirstPosition(), UIMenuSkill.getSecondPosition()});
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getSecondPosition(), charactersToParty[characterInParty[1]]);
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getFirstPosition(), charactersToParty[characterInParty[0]]);
		break;
	case 3:
		updatePositionInfo(UI_Menu_positionSecond, charactersToParty[characterInParty[0]]);
		updatePositionInfo(UI_Menu_positionThird, charactersToParty[characterInParty[1]]);
		updatePositionInfo(UI_Menu_positionFourth, charactersToParty[characterInParty[2]]);

		UIMenu.enableIcons(new Image[]{UIMenuSkill.getFirstPosition(), UIMenuSkill.getSecondPosition(), 
								UIMenuSkill.getThirdPosition()});
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getThirdPosition(), charactersToParty[characterInParty[2]]);
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getSecondPosition(), charactersToParty[characterInParty[1]]);
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getFirstPosition(), charactersToParty[characterInParty[0]]);
		break;
	case 4:
		updatePositionInfo(UI_Menu_positionFirst, charactersToParty[characterInParty[0]]);
		updatePositionInfo(UI_Menu_positionSecond, charactersToParty[characterInParty[1]]);
		updatePositionInfo(UI_Menu_positionThird, charactersToParty[characterInParty[2]]);
		updatePositionInfo(UI_Menu_positionFourth, charactersToParty[characterInParty[3]]);

		UIMenu.enableIcons(new Image[]{UIMenuSkill.getFirstPosition(), UIMenuSkill.getSecondPosition(), 
								UIMenuSkill.getThirdPosition(), UIMenuSkill.getFourthPosition()});
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getFourthPosition(), charactersToParty[characterInParty[3]]);
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getThirdPosition(), charactersToParty[characterInParty[2]]);
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getSecondPosition(), charactersToParty[characterInParty[1]]);
		UIMenu.updateMenuPositionInfo(UIMenuSkill.getFirstPosition(), charactersToParty[characterInParty[0]]);
		break;
	default:
		break;
	}
}

private void updateMenuIcons(int totalPositionsActive)
{
	hideAllDungeonIcons();
	hideAllMenuIcons();
	setIsExitMenu(false);
	switch(totalPositionsActive)
	{
	case 1:
		showIcons(new Image[]{UI_Menu_positionFourth});
		updatePositionImage(UI_Menu_positionFourth, charactersToParty[characterInParty[0]].dungeonSprite);
		break;
	case 2:
		showIcons(new Image[]{UI_Menu_positionFourth, UI_Menu_positionThird});
		updatePositionImage(UI_Menu_positionThird, charactersToParty[characterInParty[0]].dungeonSprite);
		updatePositionImage(UI_Menu_positionFourth, charactersToParty[characterInParty[1]].dungeonSprite);
		break;
	case 3:
		showIcons(new Image[]{UI_Menu_positionFourth, UI_Menu_positionThird, UI_Menu_positionSecond});
		updatePositionImage(UI_Menu_positionSecond, charactersToParty[characterInParty[0]].dungeonSprite);
		updatePositionImage(UI_Menu_positionThird, charactersToParty[characterInParty[1]].dungeonSprite);
		updatePositionImage(UI_Menu_positionFourth, charactersToParty[characterInParty[2]].dungeonSprite);
		break;
	case 4:
		showIcons(new Image[]{UI_Menu_positionFourth, UI_Menu_positionThird, 
								UI_Menu_positionSecond, UI_Menu_positionFirst});
		updatePositionImage(UI_Menu_positionFirst, charactersToParty[characterInParty[0]].dungeonSprite);
		updatePositionImage(UI_Menu_positionSecond, charactersToParty[characterInParty[1]].dungeonSprite);
		updatePositionImage(UI_Menu_positionThird, charactersToParty[characterInParty[2]].dungeonSprite);
		updatePositionImage(UI_Menu_positionFourth, charactersToParty[characterInParty[3]].dungeonSprite);
		break;
	default:
		break;
	}
}

private void hideAllMenuIcons()
{
	UI_Menu_positionFirst.GetComponent<CanvasGroup>().alpha = 0;
	UI_Menu_positionSecond.GetComponent<CanvasGroup>().alpha = 0;
	UI_Menu_positionThird.GetComponent<CanvasGroup>().alpha = 0;
	UI_Menu_positionFourth.GetComponent<CanvasGroup>().alpha = 0;
}

public void handleMenuIcons(bool isShow)
{
	if(isShow)
	{
		updateMenuIcons(totalPositionsActive);
		updateMenuValues();
	}else
	{
		updateDungeonIcons(totalPositionsActive);
		updateDungeonValues();
	}
}


//*********************************************************************************************
//						Get and Set Methods
//*********************************************************************************************

	public string getTimeOfDayString()		{return timeOfDayStrings[eActiveDayTime];	}
	public string getCalendarDateString()	{return currentCalendarDate;				}
	public void setIsExitMenu(bool value)	{isExitMenu = value;						}
}