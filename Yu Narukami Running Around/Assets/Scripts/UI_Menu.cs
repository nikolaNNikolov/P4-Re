using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.PostProcessing;

public class UI_Menu : MonoBehaviour
{

    public enum MenuStates
    {
        MainPanel           = 1,
        SkillPanel          = 2,
        ItemPanel           = 3,
        EquipPanel          = 4,
        PersonaPanel        = 5,
        StatusPanel         = 6,
        SocialLinkPanel     = 7,
        QuestPanel          = 8,
        SystemPanel         = 9,
        MAX                 = 10
    }

    [Header ("\tMenu Panels")]
    [SerializeField] private Image UIMenuMainPanel;
    [SerializeField] private Image UIMenuSkillPanel;
    [SerializeField] private Image UIMenuItemPanel;
    [SerializeField] private Image UIMenuEquipPanel;

    [Header ("\tScene Items")]
    [SerializeField] private SceneLogic SceneLogic;
    [SerializeField] private UI_Canvas_Controller canvasController;
	[SerializeField] private YuPlayerController playerCharacter;
	[SerializeField] private CharacterController playerCharacterController;
	[SerializeField] private Animator playerCharacterAnimator;
	[SerializeField] private GameObject firstSelectedMenuChoiceButton;
    
    private MenuStates currentActiveMenu;
    private Image UIMenuCenter;
    private Image UIMenuTV;
    private Image UIMenuYen;
    private Text UIMenuDescription;
    private string animationName;
    private bool isInMenu = false;
    private Dictionary<MenuStates, Image> panelsToImages = new Dictionary<MenuStates, Image>();

    private UI_Canvas_Controller.AvaivableUI eLastActiveUI;
    
    void Start()
    {
        UIMenuTV = this.gameObject.transform.Find("UI_TV").GetComponent<Image>();
        UIMenuYen = UIMenuMainPanel.gameObject.transform.Find("Yen_BG").GetComponent<Image>();
        UIMenuCenter = UIMenuMainPanel.gameObject.transform.Find("Menu_Center").GetComponent<Image>();
        
        UIMenuDescription = UIMenuCenter.gameObject.transform.Find("Description").GetComponent<Text>();


        panelsToImages.Add(MenuStates.MainPanel, 	UIMenuMainPanel);
        panelsToImages.Add(MenuStates.SkillPanel, 	UIMenuSkillPanel);
        panelsToImages.Add(MenuStates.ItemPanel, 	UIMenuItemPanel);
		

        
        UIMenuMainPanel.gameObject.SetActive(false);
        UIMenuTV.gameObject.SetActive(false);
        UIMenuDescription.text = "";
        currentActiveMenu = MenuStates.MainPanel;
    }


    void Update()
    {
        if(Input.GetButtonDown("Cancel") && SceneLogic.getActivateUI() != UI_Canvas_Controller.AvaivableUI.UI_Battle)
        {
            if(!UIMenuMainPanel.gameObject.activeInHierarchy && currentActiveMenu == MenuStates.MainPanel)
                showMenu();
            else
            {
                hidePanel(currentActiveMenu);
            }
        }
    }

    public void updateDescription(string newDescription)
    {
        UIMenuDescription.text = newDescription;    
    }

    public void hideGameObjectElement(GameObject toHide)
    {
        toHide.SetActive(false);
    }

    public void showMenu(GameObject toHideElement = null)
    {
        if(toHideElement != null)
            toHideElement.SetActive(false);
            
        playerCharacterController.enabled = false;
        playerCharacter.enabled = false;
        playerCharacterAnimator.SetFloat("Horizontal", 0);
        playerCharacterAnimator.SetFloat("Vertical",   0);

        if(currentActiveMenu == MenuStates.MainPanel) //check for if is in the Menu for the first time
        {
            eLastActiveUI = canvasController.eActiveUI;
            canvasController.eActiveUI = UI_Canvas_Controller.AvaivableUI.UI_Menu;
            canvasController.handleMenuIcons(true);
            Debug.Log("Entry: " + eLastActiveUI);
            
            StartCoroutine(lerpValuesForPostProcessingWeight(SceneLogic.getActivatedCamera().gameObject
                                                                        .GetComponent<PostProcessVolume>(), 0.0f, 1.0f, 0.5f));
        }

        UIMenuMainPanel.gameObject.SetActive(true);
        UIMenuCenter.gameObject.SetActive(true);
        UIMenuTV.gameObject.SetActive(true);
        UIMenuYen.gameObject.SetActive(true);
        
        activateButtons(true);
        currentActiveMenu = MenuStates.MainPanel;
    }

    public void hideMenu(bool isSwitchingElement)
    {

        if(currentActiveMenu != MenuStates.MainPanel)
        {
            StartCoroutine(Dialogue_Manager.de_activateDialogueAfterAnimation(UIMenuCenter.gameObject, UIMenuMainPanel.gameObject, 
            "P4_UI_Menu_Center_Show", "Hide", false));
            StartCoroutine(Dialogue_Manager.de_activateDialogueAfterAnimation(UIMenuYen.gameObject, UIMenuYen.gameObject, 
            "P4_UI_Menu_TV_Show", "HideTV", false));
            //UIMenuMainPanel.gameObject.SetActive(false);
        }
        else
        {
            playerCharacterController.enabled = true;
            playerCharacter.enabled = true;
            StartCoroutine(lerpValuesForPostProcessingWeight(SceneLogic.getActivatedCamera().gameObject.GetComponent<PostProcessVolume>(), 1.0f, 0.0f, 0.5f));
            StartCoroutine(Dialogue_Manager.de_activateDialogueAfterAnimation(UIMenuCenter.gameObject, UIMenuCenter.gameObject, 
            "P4_UI_Menu_Center_Show", "Hide", false));
            StartCoroutine(Dialogue_Manager.de_activateDialogueAfterAnimation(UIMenuYen.gameObject, UIMenuMainPanel.gameObject, 
            "P4_UI_Menu_TV_Show", "HideTV", false));
            StartCoroutine(Dialogue_Manager.de_activateDialogueAfterAnimation(UIMenuTV.gameObject, UIMenuTV.gameObject, 
            "P4_UI_Menu_TV_Show", "HideTV", false));
            //UIMenuMainPanel.gameObject.SetActive(false);

            canvasController.setIsExitMenu(true);
            canvasController.eActiveUI = eLastActiveUI;
            Debug.Log("Exit: " + eLastActiveUI);
            canvasController.handleMenuIcons(false);
            
        }
        activateButtons(false);
        isInMenu = !isInMenu;
    }

    public void switchToNewPanel(MenuStates newActiveMenu)
    {
        MenuStates previousActiveMenu = currentActiveMenu;
        if(newActiveMenu != previousActiveMenu)
        {
            currentActiveMenu = newActiveMenu;
            hidePanel(previousActiveMenu);
            showPanel(currentActiveMenu);
        }
    }

    public void updateMenuPositionImage(Image givenPosition, Sprite givenSprite)
	{
		givenPosition.sprite = givenSprite;
	}

	public void updateMenuPositionInfo(Image givenPosition, UI_Canvas_Controller.BattleIcons givenIcon)
	{
		givenPosition.GetComponent<Transform>().Find("SP_Fill_BG")
                    .GetComponent<Transform>().Find("SP_Fill").GetComponent<Image>()
					.fillAmount = (givenIcon.spiritPointsCurrent/givenIcon.spiritPointsTotal);
		givenPosition.GetComponent<Transform>().Find("HP_Fill_BG")
                    .GetComponent<Transform>().Find("HP_Fill").GetComponent<Image>()
					.fillAmount = (givenIcon.healthPointsCurrent/givenIcon.healthPointsTotal);

        givenPosition.GetComponent<Transform>().Find("SP_Level").GetComponent<Text>()
                .text = Mathf.Round(givenIcon.spiritPointsCurrent).ToString();
        givenPosition.GetComponent<Transform>().Find("HP_Level").GetComponent<Text>()
                .text = Mathf.Round(givenIcon.healthPointsCurrent).ToString();
                
        givenPosition.GetComponent<Transform>().Find("Name").GetComponent<Text>()
                .text = givenIcon.firstName;
        givenPosition.GetComponent<Transform>().Find("Level").GetComponent<Text>()
                .text = givenIcon.level.ToString();
	}

    public void enableIcons(Image[] toShowIcons)
    {
        foreach(Image icon in toShowIcons)
			icon.gameObject.SetActive(true);
    }

    private void hidePanel(MenuStates activeMenuToHide)
    {
        if(activeMenuToHide != MenuStates.MainPanel)
        {
            panelsToImages[activeMenuToHide].gameObject.GetComponent<UI_Menu_Elements_Common>().hidePanel();
            showMenu();
        }
        else
            hideMenu(true);
    }

    private void showPanel(MenuStates activeMenuToHide)
    {
        if(activeMenuToHide != MenuStates.MainPanel)
            panelsToImages[activeMenuToHide].gameObject.SetActive(true);
        else
            showMenu();
    }

    private IEnumerator lerpValuesForPostProcessingWeight(PostProcessVolume camera, float starterValue, float targetFOV, float time)
	{
		float elapsedTime = 0;
		while (elapsedTime < time)
		{
			camera.weight = Mathf.Lerp(starterValue, targetFOV, (elapsedTime / time));
			elapsedTime += Time.deltaTime;
			yield return null;
		} 
        if(camera.weight != targetFOV )
            camera.weight = targetFOV;
 		yield return null;
	}

    private void activateButtons(bool toggleButtons)
    {
	    EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(firstSelectedMenuChoiceButton);
    }

    public bool getIsInMenu(){return isInMenu;}
    public Image getUIMenuCenter(){return UIMenuCenter;}
}
