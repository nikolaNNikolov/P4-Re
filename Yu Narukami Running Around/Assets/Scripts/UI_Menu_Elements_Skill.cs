using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_Menu_Elements_Skill : MonoBehaviour
{

    [SerializeField] private Image firstPosition;
    [SerializeField] private Image secondPosition;
    [SerializeField] private Image thirdPosition;
	[SerializeField] private Image fourtPosition;
	[SerializeField] private GameObject firstSelectedMenuChoiceButton;

    void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
		EventSystem.current.SetSelectedGameObject(firstSelectedMenuChoiceButton);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void hideAllPosition()
    {
        firstPosition.gameObject.SetActive(false);
        secondPosition.gameObject.SetActive(false);
        thirdPosition.gameObject.SetActive(false);
        fourtPosition.gameObject.SetActive(false);
    }

    public Image getFirstPosition()     {return firstPosition;  }
    public Image getSecondPosition()    {return secondPosition; }
    public Image getThirdPosition()     {return thirdPosition;  }
    public Image getFourthPosition()    {return fourtPosition;  }
}
