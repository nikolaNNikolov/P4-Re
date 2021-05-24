using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Menu_Elements_Skill_Button_Controller : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    [SerializeField] Sprite spriteToSetSelected;
    [SerializeField] Sprite spriteToSetDeselected;
    [Header ("Position Elements")]
    [SerializeField] Text levelText;
    [SerializeField] Text nameText;
    [SerializeField] Text hpLevel;
    [SerializeField] Text spLevel;
    [Header ("Selected Colors")]
    [SerializeField] Color levelTextSelected;
    [SerializeField] Color nameTextSelected;
    [SerializeField] Color hpLevelSelected;
    [Header ("Deselected Colors")]
    [SerializeField] Color levelTextDeselected;
    [SerializeField] Color nameTextDeselected;
    [SerializeField] Color hpLevelDeselected;
    
    private Vector2 currPosition;

    void Start()
    {}

    void Update()
    {}

    void OnEnable()
    {

    }

    public void OnSelect(BaseEventData eventData)
    {
        this.gameObject.GetComponent<Image>().sprite = spriteToSetSelected;

        currPosition = this.gameObject.GetComponent<RectTransform>().anchoredPosition;
        Vector2 newPosition =  currPosition + new Vector2((spriteToSetSelected.rect.width-spriteToSetDeselected.rect.width)/2, 0);

        this.gameObject.GetComponent<RectTransform>().anchoredPosition = newPosition;
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteToSetSelected.rect.width, spriteToSetSelected.rect.height);
        levelText.color = levelTextSelected;
        nameText.color  = nameTextSelected;
        hpLevel.color   = hpLevelSelected;
        spLevel.color   = hpLevelSelected;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        this.gameObject.GetComponent<Image>().sprite = spriteToSetDeselected;
        this.gameObject.GetComponent<RectTransform>().anchoredPosition = currPosition;
        this.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(spriteToSetDeselected.rect.width, spriteToSetDeselected.rect.height);
        levelText.color = levelTextDeselected;
        nameText.color  = nameTextDeselected;
        hpLevel.color   = hpLevelDeselected;
        spLevel.color   = hpLevelDeselected;
    }
}
